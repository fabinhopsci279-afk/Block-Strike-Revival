using CodeStage.AntiCheat.ObscuredTypes;
using Photon;
using UnityEngine;

public class TDMMode : Photon.MonoBehaviour
{
	public ObscuredInt MaxScore = 100;

	private int AdsShow = 5;

	private int RevivalPlayer;

	private void Awake()
	{
		if (PhotonNetwork.room.GetGameMode() != 0)
		{
			UnityEngine.Object.Destroy(this);
		}
	}

	private void Start()
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.UpdateRoundState(RoundState.PlayRound);
		}
		CameraManager.ActiveStaticCamera();
		UIGameManager.SetActiveScore(active: true, MaxScore);
		UISelectTeam.OnStart();
		GameManager.SetMode(GameMode.TeamDeathmatch);
		GameManager.MaxScore = MaxScore;
		EventManager.AddListener<Team>("SelectTeam", OnSelectTeam);
		EventManager.AddListener<DamageInfo>("DeadPlayer", OnDeadPlayer);
	}

	private void OnSelectTeam(Team team)
	{
		UIPanelManager.ShowPanel("Display");
		OnRevivalPlayer();
	}

	private void OnSpawnPlayer()
	{
		ControllerManager controller = GameManager.GetController();
		controller.PlayerInput.FPController.Stop();
		controller.PlayerInput.FPController.SetPosition(GameManager.GetTeamSpawn().GetSpawnPosition());
		controller.PlayerInput.FPCamera.SetRotation(GameManager.GetTeamSpawn().m_Transform.eulerAngles);
	}

	private void OnRevivalPlayer()
	{
		PlayerInput playerInput = GameManager.GetController().PlayerInput;
		playerInput.SetHealth(100);
		CameraManager.DeactiveAll();
		GameManager.GetController().ActivePlayer();
		OnSpawnPlayer();
		playerInput.PlayerWeapon.UpdateWeaponAll(WeaponTypeList.Rifle);
	}

	private void OnDeadPlayer(DamageInfo damageInfo)
	{
		PhotonNetwork.player.SetDeaths1();
		SaveLoadManager.SetDeaths1();
		if (damageInfo.PlayerID != -1)
		{
			GameManager.OnStatus(Utils.KillerStatus(damageInfo), local: false, string.Empty);
			OnScore(damageInfo.AttackerTeam);
		}
		Vector3 ragdollForce = Utils.GetRagdollForce(GameManager.GetController().PlayerInput.PlayerTransform.position, damageInfo.AttackPosition);
		CameraManager.ActiveDeadCamera(GameManager.GetController().PlayerInput.FPCamera.Transform.position, GameManager.GetController().PlayerInput.FPCamera.Transform.eulerAngles, ragdollForce * 100f);
		GameManager.GetController().DeactivePlayer(ragdollForce, damageInfo.HeadShot);
		base.photonView.RPC("OnKilledPlayer", PhotonPlayer.Find(damageInfo.PlayerID), DamageInfo.Deserialize(damageInfo));
		GameManager.BalanceTeam();
		RevivalPlayer++;
		if (RevivalPlayer >= AdsShow)
		{
			RevivalPlayer = 0;
			AdsShow = Random.Range(4, 6);
			vp_Timer.In(0.5f, delegate
			{
				AdsManager.ShowInterstitial();
			});
		}
		vp_Timer.In(3f, delegate
		{
			OnRevivalPlayer();
		});
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		if (PhotonNetwork.isMasterClient)
		{
			GameManager.UpdateScore(playerConnect);
			GameManager.UpdateRoundState(playerConnect);
		}
	}

	[PunRPC]
	private void OnKilledPlayer(string info)
	{
		PhotonNetwork.player.SetKills1();
		SaveLoadManager.SetKills1();
		DamageInfo damageInfo = DamageInfo.Serialize(info);
		AchievementsManager.UpdateKills(damageInfo);
		if (AdsManager.isLoadedAds)
		{
			if (damageInfo.HeadShot)
			{
				PlayerLevelManager.UpdatePlayerXP(10);
				SaveLoadManager.SetMoney1(7);
				SaveLoadManager.SetHeadshot1();
			}
			else
			{
				PlayerLevelManager.UpdatePlayerXP(5);
				SaveLoadManager.SetMoney1(3);
			}
		}
	}

	public void OnScore(Team team)
	{
		base.photonView.RPC("PhotonOnScore", PhotonTargets.MasterClient, (int)team);
	}

	[PunRPC]
	private void PhotonOnScore(int intTeam)
	{
		switch (intTeam)
		{
		case 1:
			GameManager.BlueScore = ++GameManager.BlueScore;
			break;
		case 2:
			GameManager.RedScore = ++GameManager.RedScore;
			break;
		}
		GameManager.UpdateScore();
		if (GameManager.CheckScore())
		{
			GameManager.UpdateRoundState(RoundState.EndRound);
			if (GameManager.WinTeam() == Team.Blue)
			{
				GameManager.OnMainStatus("@", local: false, 5f, "Blue Win");
			}
			else if (GameManager.WinTeam() == Team.Red)
			{
				GameManager.OnMainStatus("@", local: false, 5f, "Red Win");
			}
			GameManager.LoadNextLevel(GameMode.TeamDeathmatch);
		}
	}
}
