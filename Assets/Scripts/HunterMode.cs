using Photon;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HunterMode : Photon.MonoBehaviour
{
	private void Awake()
	{
		if (PhotonNetwork.room.GetGameMode() != GameMode.Hunter)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		UIGameManager.SetActiveScore(active: true, 20);
		GameManager.SetStartDamageTime(1f);
		UIPanelManager.ShowPanel("Display");
		GameManager.SetMode(GameMode.Hunter);
		GameManager.MaxScore = 20;
		CameraManager.ActiveStaticCamera();
		vp_Timer.In(0.5f, delegate
		{
			if (PhotonNetwork.isMasterClient)
			{
				ActivationWaitPlayer();
			}
			else if (GameManager.GetController().PlayerInput.Dead)
			{
				CameraManager.ActiveSpectateCamera();
			}
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
	}

	private void ActivationWaitPlayer()
	{
		EventManager.Dispatch("WaitPlayer");
		GameManager.UpdateRoundState(RoundState.WaitPlayer);
		GameManager.OnSelectTeam(Team.Blue);
		OnWaitPlayer();
		OnCreatePlayer();
	}

	private void OnWaitPlayer()
	{
		GameManager.OnStatus(Localization.Get("Waiting for other players"), local: true, string.Empty);
		vp_Timer.In(4f, delegate
		{
			if (GameManager.GetRoundState() == RoundState.WaitPlayer)
			{
				if (PhotonNetwork.playerList.Length <= 1)
				{
					OnWaitPlayer();
				}
				else
				{
					vp_Timer.In(4f, delegate
					{
						OnStartRound();
					});
				}
			}
		});
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.UpdateScore(playerConnect);
			GameManager.UpdateRoundState(playerConnect);
			if (GameManager.GetRoundState() != 0)
			{
				CheckPlayers();
			}
		}
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer playerDisconnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			CheckPlayers();
		}
	}

	private void OnStartRound()
	{
		if (PhotonNetwork.playerList.Length <= 1)
		{
			ActivationWaitPlayer();
		}
		else if (PhotonNetwork.isMasterClient)
		{
			List<PhotonPlayer> list = PhotonNetwork.playerList.ToList();
			int num = OnSelectMaxDeaths(list.Count);
			string text = string.Empty;
			for (int i = 0; i < num; i++)
			{
				int index = Random.Range(0, list.Count);
				text = text + list[index].ID + "#";
				list.RemoveAt(index);
			}
			GameManager.UpdateRoundState(RoundState.PlayRound);
			base.photonView.RPC("OnSendKillerInfo", PhotonTargets.All, text);
		}
	}

	[PunRPC]
	private void OnSendKillerInfo(string text, PhotonMessageInfo info)
	{
		string[] array = text.Split("#"[0]);
		bool flag = false;
		for (int i = 0; i < array.Length - 1; i++)
		{
			if (PhotonNetwork.player.ID == int.Parse(array[i]))
			{
				flag = true;
				break;
			}
		}
		if (flag)
		{
			GameManager.OnSelectTeam(Team.Red);
		}
		else
		{
			GameManager.OnSelectTeam(Team.Blue);
		}
		EventManager.Dispatch("StartRound");
		OnCreatePlayer();
		vp_Timer.In(3f, delegate
		{
			if (PhotonNetwork.isMasterClient)
			{
				CheckPlayers();
			}
		});
	}

	private void OnCreatePlayer()
	{
		PlayerInput playerInput = GameManager.GetController().PlayerInput;
		playerInput.SetHealth(100);
		CameraManager.DeactiveAll();
		GameManager.GetController().ActivePlayer();
		OnSpawnPlayer();
		if (playerInput.PlayerTeam == Team.Blue)
		{
			WeaponManager.SetPistolType(6);
			WeaponManager.SetRifleType(0);
			playerInput.PlayerWeapon.UpdateWeaponAll(WeaponTypeList.Pistol);
			vp_Timer.In(0.5f, delegate
			{
				playerInput.PlayerWeapon.GetWeaponData(WeaponTypeList.Pistol).AmmoMax = 0;
				playerInput.PlayerWeapon.GetWeaponData(WeaponTypeList.Pistol).Ammo = 1;
				UIGameManager.SetAmmo(playerInput.PlayerWeapon.GetSelectedWeaponData().Ammo, playerInput.PlayerWeapon.GetSelectedWeaponData().AmmoMax);
			});
		}
		else
		{
			WeaponManager.SetPistolType(SaveLoadManager.GetWeaponSelected(WeaponTypeList.Pistol));
			WeaponManager.SetRifleType(SaveLoadManager.GetWeaponSelected(WeaponTypeList.Rifle));
			playerInput.PlayerWeapon.UpdateWeaponAll(WeaponTypeList.Rifle);
		}
	}

	private void OnSpawnPlayer()
	{
		ControllerManager controller = GameManager.GetController();
		controller.SetPosition(GameManager.GetTeamSpawn().GetSpawnPosition());
		controller.PlayerInput.FPCamera.SetRotation(GameManager.GetTeamSpawn().m_Transform.eulerAngles);
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		SaveLoadManager.SetDeaths1();
		GameManager.OnStatus(Utils.KillerStatus(damageInfo), local: false, string.Empty);
		Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.GetController().PlayerInput.PlayerTransform.position, damageInfo.AttackPosition);
		CameraManager.ActiveDeadCamera(GameManager.GetController().PlayerInput.FPCamera.Transform.position, GameManager.GetController().PlayerInput.FPCamera.Transform.eulerAngles, ragdollForce * 100f);
		GameManager.GetController().DeactivePlayer(ragdollForce, damageInfo.HeadShot);
		base.photonView.RPC("OnKilledPlayer", PhotonPlayer.Find(damageInfo.PlayerID), DamageInfo.Deserialize(damageInfo));
		base.photonView.RPC("CheckPlayers", PhotonTargets.MasterClient);
		vp_Timer.In(3f, delegate
		{
			if (GameManager.GetController().PlayerInput.Dead)
			{
				CameraManager.ActiveSpectateCamera();
			}
		});
		vp_Timer.In(1.5f, delegate
		{
			AdsManager.ShowInterstitial();
		});
	}

	[PunRPC]
	private void OnKilledPlayer(string info)
	{
		PhotonNetwork.player.SetKills1();
		SaveLoadManager.SetKills1();
		if (AdsManager.isLoadedAds)
		{
			DamageInfo damageInfo = DamageInfo.Serialize(info);
			if (damageInfo.HeadShot)
			{
				PlayerLevelManager.UpdatePlayerXP(12);
				SaveLoadManager.SetMoney1(10);
				SaveLoadManager.SetHeadshot1();
			}
			else
			{
				PlayerLevelManager.UpdatePlayerXP(6);
				SaveLoadManager.SetMoney1(5);
			}
		}
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessageInfo info)
	{
		GameManager.UpdateRoundState(RoundState.EndRound);
		if (GameManager.CheckScore())
		{
			GameManager.LoadNextLevel(GameMode.Hunter);
			return;
		}
		float delay = 8f - (float)(PhotonNetwork.time - info.timestamp);
		vp_Timer.In(delay, delegate
		{
			OnStartRound();
		});
	}

	[PunRPC]
	private void CheckPlayers()
	{
		if (!PhotonNetwork.isMasterClient || GameManager.GetRoundState() == RoundState.EndRound)
		{
			return;
		}
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue && !playerList[i].GetDead())
			{
				flag = true;
				break;
			}
		}
		for (int j = 0; j < playerList.Length; j++)
		{
			if (playerList[j].GetTeam() == Team.Red && !playerList[j].GetDead())
			{
				flag2 = true;
				break;
			}
		}
		if (!flag)
		{
			GameManager.RedScore = ++GameManager.RedScore;
			GameManager.UpdateScore();
			GameManager.OnMainStatus("@", local: false, 5f, "Red Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
		else if (!flag2)
		{
			GameManager.BlueScore = ++GameManager.BlueScore;
			GameManager.UpdateScore();
			GameManager.OnMainStatus("@", local: false, 5f, "Blue Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	private int OnSelectMaxDeaths(int maxPlayers)
	{
		if (maxPlayers >= 10)
		{
			return 4;
		}
		if (maxPlayers >= 7)
		{
			return 3;
		}
		if (maxPlayers >= 4)
		{
			return 2;
		}
		return 1;
	}
}
