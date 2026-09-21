using Photon;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class ControllerManager : Photon.MonoBehaviour
{
	private Transform PlayerTransform;

	[NonSerialized]
	public PlayerInput PlayerInput;

	[NonSerialized]
	public PlayerSkin PlayerSkin;

	private int falsePositives;

	private void Awake()
	{
		base.name = base.photonView.owner.name;
		if (base.photonView.isMine)
		{
			PlayerTransform = Utils.AddChild(GameSettings.instance.PlayerController, base.transform).transform;
			PlayerInput = PlayerTransform.GetComponent<PlayerInput>();
			EventManager.AddListener("Fire", FireWeapon);
		}
		else
		{
			PlayerTransform = Utils.AddChild(GameSettings.instance.PlayerSkin, base.transform).transform;
			PlayerSkin = PlayerTransform.GetComponent<PlayerSkin>();
		}
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (base.photonView.isMine)
		{
			bool activeSelf = PlayerInput.gameObject.activeSelf;
			Team playerTeam = PlayerInput.PlayerTeam;
			int num = SaveLoadManager.GetPlayerSkinSelected();
			if (playerTeam == Team.Red && PlayerInput.Zombie)
			{
				num = 99;
			}
			int num2 = PlayerInput.PlayerWeapon.GetSelectedWeaponData().WeaponID;
			int weaponSkinSelected = SaveLoadManager.GetWeaponSkinSelected(num2);
			base.photonView.RPC("PhotonConnected", playerConnect, activeSelf, num, (int)playerTeam, num2, weaponSkinSelected);
		}
	}

	[PunRPC]
	private void PhotonConnected(bool activePlayer, int skin, int team, int weapon, int skinID)
	{
		PlayerSkin.PlayerTeam = (Team)team;
		PlayerSkin.PlayerSkinID = skin;
		if (activePlayer)
		{
			PhotonActivePlayer();
			PlayerSkin.SetWeapon(WeaponManager.GetWeaponType(weapon), skinID);
		}
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(PlayerTransform.position);
			stream.SendNext(PlayerTransform.rotation);
			stream.SendNext(PlayerInput.MoveVector);
			stream.SendNext(PlayerInput.mCharacterController.isGrounded);
			stream.SendNext(PlayerInput.RotateCamera);
		}
		else
		{
			PlayerSkin.PhotonPosition = (Vector3)stream.ReceiveNext();
			PlayerSkin.PhotonRotation = (Quaternion)stream.ReceiveNext();
			PlayerSkin.SetMove((Vector2)stream.ReceiveNext());
			PlayerSkin.SetGrounded((bool)stream.ReceiveNext());
			PlayerSkin.SetRotate((float)stream.ReceiveNext());
		}
	}

	public void SetWeapon(int weaponID)
	{
		int weaponSkinSelected = SaveLoadManager.GetWeaponSkinSelected(weaponID);
		base.photonView.RPC("PhotonSetWeapon", PhotonTargets.Others, weaponID, weaponSkinSelected);
	}

	[PunRPC]
	private void PhotonSetWeapon(int weaponID, int skinID)
	{
		PlayerSkin.SetWeapon(WeaponManager.GetWeaponType(weaponID), skinID);
	}

	public void FireWeapon()
	{
		base.photonView.RPC("PhotonFireWeapon", PhotonTargets.Others);
	}

	[PunRPC]
	private void PhotonFireWeapon()
	{
		PlayerSkin.Fire();
	}

	public void ActivePlayer()
	{
		if (base.photonView.isMine)
		{
			PlayerInput.FPController.Activate();
			PhotonNetwork.player.SetDead(dead: false);
			PlayerInput.Dead = false;
		}
		base.photonView.RPC("PhotonActivePlayer", PhotonTargets.Others);
	}

	[PunRPC]
	private void PhotonActivePlayer()
	{
		if (!base.photonView.isMine)
		{
			PlayerTransform.gameObject.SetActive(value: true);
			PlayerSkin.PlayerRagdoll.Deactive();
			PlayerSkin.Dead = false;
		}
	}

	public void DeactivePlayer([Optional] Vector3 force, bool headshot = false)
	{
		if (base.photonView.isMine)
		{
			PlayerInput.FPController.Deactivate();
			PhotonNetwork.player.SetDead(dead: true);
			PlayerInput.Dead = true;
		}
		base.photonView.RPC("PhotonDeactivePlayer", PhotonTargets.Others, force, headshot);
	}

	[PunRPC]
	private void PhotonDeactivePlayer(Vector3 force, bool headshot)
	{
		if (!base.photonView.isMine)
		{
			PlayerSkin.PlayerRagdoll.Active(force, headshot);
			PlayerSkin.Dead = true;
		}
	}

	public void SetPosition(Vector3 position)
	{
		base.photonView.RPC("PhotonSetPosition", PhotonTargets.All, position);
	}

	[PunRPC]
	private void PhotonSetPosition(Vector3 position)
	{
		if (base.photonView.isMine)
		{
			PlayerInput.FPController.Stop();
			PlayerInput.FPController.SetPosition(position);
		}
		else
		{
			PlayerSkin.SetPosition(position);
		}
	}

	public void Damage(DamageInfo damageInfo)
	{
		base.photonView.RPC("PhotonDamage", base.photonView.owner, DamageInfo.Deserialize(damageInfo));
	}

	[PunRPC]
	private void PhotonDamage(string json, PhotonMessageInfo info)
	{
		if (info.timestamp + 0.60000002384185791 > PhotonNetwork.time)
		{
			PlayerInput.Damage(DamageInfo.Serialize(json));
			if (falsePositives >= 0)
			{
				falsePositives--;
			}
		}
		else
		{
			falsePositives++;
			if (falsePositives >= 10)
			{
				UIGameManager.instance.OnExitServer();
			}
		}
	}

	public void SetTeam(Team team)
	{
		base.photonView.owner.SetTeam(team);
		int num = SaveLoadManager.GetPlayerSkinSelected();
		if (team == Team.Red && PlayerInput.Zombie)
		{
			num = 99;
		}
		base.photonView.RPC("PhotonSetTeam", PhotonTargets.All, (int)team, num);
	}

	[PunRPC]
	private void PhotonSetTeam(int team, int skin)
	{
		if (base.photonView.isMine)
		{
			PlayerInput.PlayerTeam = (Team)team;
			return;
		}
		PlayerSkin.PlayerTeam = (Team)team;
		PlayerSkin.PlayerSkinID = skin;
		PlayerSkin.UpdateSkin();
	}

	public void UpdateFoosteps()
	{
		base.photonView.RPC("PhotonUpdateFoosteps", PhotonTargets.All);
	}

	[PunRPC]
	private void PhotonUpdateFoosteps()
	{
		if (base.photonView.isMine)
		{
			PlayerInput.UpdateFoosteps();
		}
		else
		{
			PlayerSkin.UpdateFoosteps();
		}
	}
}
