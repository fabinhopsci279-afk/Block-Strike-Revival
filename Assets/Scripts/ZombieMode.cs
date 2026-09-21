using Photon;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ZombieMode : Photon.MonoBehaviour
{
	private vp_Timer.Handle StartZombieTimer = new vp_Timer.Handle();

	private void Awake()
	{
		if (PhotonNetwork.room.GetGameMode() != GameMode.ZombieSurvival)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		UIGameManager.SetActiveScore(active: true, 20);
		GameManager.SetStartDamageTime(1f);
		UIPanelManager.ShowPanel("Display");
		GameManager.SetMode(GameMode.ZombieSurvival);
		GameManager.MaxScore = 20;
		CameraManager.ActiveStaticCamera();
		vp_Timer.In(0.5f, delegate
		{
			if (PhotonNetwork.isMasterClient)
			{
				ActivationWaitPlayer();
			}
			else if (GameManager.GetRoundState() == RoundState.WaitPlayer)
			{
				OnCreatePlayer();
			}
			else
			{
				OnCreateZombie();
			}
		});
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
	}

	private void ActivationWaitPlayer()
	{
		EventManager.Dispatch("WaitPlayer");
		GameManager.UpdateRoundState(RoundState.WaitPlayer);
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
					GameManager.UpdateRoundState(RoundState.StartRound);
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
		if (!PhotonNetwork.isMasterClient)
		{
			return;
		}
		GameManager.UpdateScore(playerConnect);
		GameManager.UpdateRoundState(playerConnect);
		if (GameManager.GetRoundState() != 0)
		{
			CheckPlayers();
			if (UIGameManager.instance.isScoreTimer)
			{
				base.photonView.RPC("UpdateTimer", playerConnect, UIGameManager.instance.ScoreTimer - Time.time);
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
			GameManager.UpdateRoundState(RoundState.StartRound);
			base.photonView.RPC("StartTimer", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void StartTimer(PhotonMessageInfo info)
	{
		OnCreatePlayer();
		UIToast.Show(Localization.Get("Infestation will start in 20 seconds"));
		float num = 20f;
		num -= (float)(PhotonNetwork.time - info.timestamp);
		vp_Timer.In(num, delegate
		{
			if (PhotonNetwork.isMasterClient)
			{
				List<PhotonPlayer> list = PhotonNetwork.playerList.ToList();
				int num2 = OnSelectMaxDeaths(list.Count);
				string text = string.Empty;
				for (int i = 0; i < num2; i++)
				{
					int index = Random.Range(0, list.Count);
					text = text + list[index].ID + "#";
					list.RemoveAt(index);
				}
				GameManager.UpdateRoundState(RoundState.PlayRound);
				base.photonView.RPC("OnSendKillerInfo", PhotonTargets.All, text);
			}
		}, StartZombieTimer);
	}

	[PunRPC]
	private void UpdateTimer(float time, PhotonMessageInfo info)
	{
		vp_Timer.In(1.5f, delegate
		{
			time -= (float)(PhotonNetwork.time - info.timestamp);
			UIGameManager.StartScoreTimer(time, StopTimer);
		});
	}

	private void StopTimer()
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.BlueScore = ++GameManager.BlueScore;
			GameManager.UpdateScore();
			GameManager.OnMainStatus("@", local: false, 5f, "Survivors Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
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
		UIToast.Show(Localization.Get("Infestation started"));
		float num = 300f;
		num -= (float)(PhotonNetwork.time - info.timestamp);
		UIGameManager.StartScoreTimer(num, StopTimer);
		if (flag)
		{
			OnCreateZombie();
		}
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
		playerInput.Zombie = false;
		GameManager.OnSelectTeam(Team.Blue);
		playerInput.SetHealth(100);
		CameraManager.DeactiveAll();
		GameManager.GetController().ActivePlayer();
		OnSpawnPlayer();
		playerInput.UpdatePlayerSpeed(0.18f);
		playerInput.FPCamera.RenderingFieldOfView = 60f;
		WeaponManager.SetKnifeType(SaveLoadManager.GetWeaponSelected(WeaponTypeList.Knife));
		WeaponManager.SetPistolType(SaveLoadManager.GetWeaponSelected(WeaponTypeList.Pistol));
		WeaponManager.SetRifleType(SaveLoadManager.GetWeaponSelected(WeaponTypeList.Rifle));
		playerInput.PlayerWeapon.UpdateWeaponAll(WeaponTypeList.Rifle);
		vp_Timer.In(0.5f, delegate
		{
			PlayerWeapons.WeaponData weaponData = playerInput.PlayerWeapon.GetWeaponData(WeaponTypeList.Pistol);
			PlayerWeapons.WeaponData weaponData2 = weaponData;
			weaponData2.AmmoMax = (int)weaponData2.AmmoMax * 3;
			PlayerWeapons.WeaponData weaponData3 = playerInput.PlayerWeapon.GetWeaponData(WeaponTypeList.Rifle);
			PlayerWeapons.WeaponData weaponData4 = weaponData3;
			weaponData4.AmmoMax = (int)weaponData4.AmmoMax * 3;
			UIGameManager.SetAmmo(playerInput.PlayerWeapon.GetSelectedWeaponData().Ammo, playerInput.PlayerWeapon.GetSelectedWeaponData().AmmoMax);
		});
	}

	private void OnCreateZombie()
	{
		PlayerInput playerInput = GameManager.GetController().PlayerInput;
		playerInput.Zombie = true;
		GameManager.OnSelectTeam(Team.Red);
		playerInput.SetHealth(1000);
		CameraManager.DeactiveAll();
		GameManager.GetController().ActivePlayer();
		OnSpawnPlayer();
		playerInput.UpdatePlayerSpeed(0.2f);
		playerInput.FPCamera.RenderingFieldOfView = 100f;
		WeaponManager.SetKnifeType(17);
		WeaponManager.SetPistolType(0);
		WeaponManager.SetRifleType(0);
		playerInput.PlayerWeapon.UpdateWeaponAll(WeaponTypeList.Knife);
	}

	private void OnSpawnPlayer()
	{
		ControllerManager controller = GameManager.GetController();
		controller.SetPosition(GameManager.GetTeamSpawn().GetSpawnPosition());
		controller.PlayerInput.FPCamera.SetRotation(GameManager.GetTeamSpawn().m_Transform.eulerAngles);
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		if (GameManager.GetRoundState() == RoundState.PlayRound)
		{
			PhotonNetwork.player.SetDeaths1();
			SaveLoadManager.SetDeaths1();
		}
		if (damageInfo.PlayerID != -1)
		{
			GameManager.OnStatus(Utils.KillerStatus(damageInfo), local: false, string.Empty);
		}
		else if (GameManager.GetRoundState() == RoundState.PlayRound)
		{
			string text = Utils.GetTeamHexColor(PhotonNetwork.player) + " @";
			GameManager.OnStatus(text, local: false, "died");
		}
		if (GameManager.GetRoundState() == RoundState.PlayRound)
		{
			Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.GetController().PlayerInput.PlayerTransform.position, damageInfo.AttackPosition);
			CameraManager.ActiveDeadCamera(GameManager.GetController().PlayerInput.FPCamera.Transform.position, GameManager.GetController().PlayerInput.FPCamera.Transform.eulerAngles, ragdollForce * 100f);
			GameManager.GetController().DeactivePlayer(ragdollForce, damageInfo.HeadShot);
		}
		else
		{
			OnCreatePlayer();
		}
		if (damageInfo.PlayerID != -1)
		{
			base.photonView.RPC("OnKilledPlayer", PhotonPlayer.Find(damageInfo.PlayerID), DamageInfo.Deserialize(damageInfo));
		}
		int num = (damageInfo.PlayerID == -1) ? 1 : 3;
		if (GameManager.GetRoundState() == RoundState.PlayRound)
		{
			base.photonView.RPC("CheckPlayers", PhotonTargets.MasterClient);
			vp_Timer.In(num, delegate
			{
				if (GameManager.GetController().PlayerInput.Dead)
				{
					OnCreateZombie();
				}
			});
			vp_Timer.In(1.5f, delegate
			{
				AdsManager.ShowInterstitial();
			});
		}
	}

	[PunRPC]
	private void OnKilledPlayer(string info)
	{
		PhotonNetwork.player.SetKills1();
		SaveLoadManager.SetKills1();
		if (!AdsManager.isLoadedAds)
		{
			return;
		}
		DamageInfo damageInfo = DamageInfo.Serialize(info);
		if (damageInfo.HeadShot)
		{
			if (damageInfo.AttackerTeam != Team.Red)
			{
				PlayerLevelManager.UpdatePlayerXP(12);
				SaveLoadManager.SetMoney1(10);
			}
			else
			{
				PlayerLevelManager.UpdatePlayerXP(20);
				SaveLoadManager.SetMoney1(20);
			}
			SaveLoadManager.SetHeadshot1();
		}
		else if (damageInfo.AttackerTeam != Team.Red)
		{
			PlayerLevelManager.UpdatePlayerXP(12);
			SaveLoadManager.SetMoney1(10);
		}
		else
		{
			PlayerLevelManager.UpdatePlayerXP(17);
			SaveLoadManager.SetMoney1(15);
		}
	}

	[PunRPC]
	private void OnFinishRound(PhotonMessageInfo info)
	{
		StartZombieTimer.Cancel();
		UIGameManager.StopScoreTimer();
		GameManager.UpdateRoundState(RoundState.EndRound);
		if (GameManager.CheckScore())
		{
			GameManager.LoadNextLevel(GameMode.ZombieSurvival);
			return;
		}
		float delay = 6f - (float)(PhotonNetwork.time - info.timestamp);
		vp_Timer.In(delay, delegate
		{
			OnStartRound();
		});
	}

	[PunRPC]
	private void OnLoadNextMap(PhotonMessageInfo info)
	{
		float num = 5f - (float)(PhotonNetwork.time - info.timestamp);
		if (num < 0f)
		{
			num = 0.1f;
		}
		vp_Timer.In(1f, delegate
		{
			PhotonNetwork.player.ClearProperties();
		});
		vp_Timer.In(num, delegate
		{
			PhotonNetwork.RemoveRPCs(PhotonNetwork.player);
			PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
			PhotonNetwork.LoadLevel(LevelManager.GetNextScene(GameMode.ZombieSurvival));
		});
	}

	[PunRPC]
	private void CheckPlayers()
	{
		if (!PhotonNetwork.isMasterClient || GameManager.GetRoundState() != RoundState.PlayRound)
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
			if (playerList[j].GetTeam() == Team.Red)
			{
				flag2 = true;
				break;
			}
		}
		if (!flag)
		{
			GameManager.RedScore = ++GameManager.RedScore;
			GameManager.UpdateScore();
			GameManager.OnMainStatus("@", local: false, 5f, "Zombie Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
		else if (!flag2)
		{
			GameManager.BlueScore = ++GameManager.BlueScore;
			GameManager.UpdateScore();
			GameManager.OnMainStatus("@", local: false, 5f, "Survivors Win");
			base.photonView.RPC("OnFinishRound", PhotonTargets.All);
		}
	}

	private int OnSelectMaxDeaths(int maxPlayers)
	{
		if (maxPlayers >= 8)
		{
			return 2;
		}
		return 1;
	}
}
