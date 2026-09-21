using CodeStage.AntiCheat.ObscuredTypes;
using Photon;
using UnityEngine;

public class GunGame : Photon.MonoBehaviour
{
	private int SelectWeapon;

	private WeaponType WeaponData;

	private int AdsShow = 5;

	private int RevivalPlayer;

	public ObscuredInt MaxScore = 100;

	private ObscuredBool isFinishRound;

	private int PlayerKills;

	private int[] Weapons = new int[24]
	{
		3,
		13,
		6,
		2,
		21,
		9,
		25,
		26,
		14,
		24,
		12,
		7,
		18,
		19,
		1,
		5,
		15,
		8,
		23,
		11,
		10,
		16,
		4,
		22
	};

	private void Awake()
	{
		if (PhotonNetwork.room.GetGameMode() != GameMode.GunGame)
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
		WeaponManager.SetKnifeType(0);
		WeaponManager.SetPistolType(3);
		WeaponManager.SetRifleType(0);
		WeaponManager.SelectWeaponInGame = false;
		GameManager.SetMode(GameMode.GunGame);
		GameManager.MaxScore = MaxScore;
		WeaponData = WeaponType.CreateNewClass(WeaponManager.GetWeaponType(3));
		UISelectTeam.OnStart();
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
		playerInput.PlayerWeapon.UpdateWeaponAll(WeaponData.Weapon);
		if (WeaponData.Weapon != WeaponTypeList.Knife)
		{
			vp_Timer.In(0.1f, delegate
			{
				PlayerWeapons.WeaponData weaponData = playerInput.PlayerWeapon.GetWeaponData(WeaponData.Weapon);
				PlayerWeapons.WeaponData weaponData2 = weaponData;
				weaponData2.AmmoMax = (int)weaponData2.AmmoMax * 2;
				UIGameManager.SetAmmo(playerInput.PlayerWeapon.GetSelectedWeaponData().Ammo, playerInput.PlayerWeapon.GetSelectedWeaponData().AmmoMax);
			});
		}
	}

	private void OnUpdateWeapon()
	{
		if (SelectWeapon >= Weapons.Length - 1)
		{
			SelectWeapon = 0;
		}
		else
		{
			SelectWeapon++;
		}
		WeaponManager.SetKnifeType(0);
		WeaponManager.SetPistolType(0);
		WeaponManager.SetRifleType(0);
		WeaponData = WeaponManager.GetWeaponType(Weapons[SelectWeapon]);
		switch (WeaponData.Weapon)
		{
		case WeaponTypeList.Knife:
			WeaponManager.SetKnifeType(WeaponData.WeaponID);
			break;
		case WeaponTypeList.Pistol:
			WeaponManager.SetPistolType(WeaponData.WeaponID);
			break;
		case WeaponTypeList.Rifle:
			WeaponManager.SetRifleType(WeaponData.WeaponID);
			break;
		}
		PlayerInput playerInput = GameManager.GetController().PlayerInput;
		if (!playerInput.Dead)
		{
			playerInput.PlayerWeapon.CanFire = false;
			vp_Timer.In(0.2f, delegate
			{
				if (!playerInput.Dead)
				{
					playerInput.PlayerWeapon.UpdateWeaponAll(WeaponData.Weapon);
					vp_Timer.In(0.1f, delegate
					{
						playerInput.PlayerWeapon.CanFire = true;
						if (WeaponData.Weapon != WeaponTypeList.Knife)
						{
							PlayerWeapons.WeaponData weaponData = playerInput.PlayerWeapon.GetWeaponData(WeaponData.Weapon);
							PlayerWeapons.WeaponData weaponData2 = weaponData;
							weaponData2.AmmoMax = (int)weaponData2.AmmoMax * 2;
							UIGameManager.SetAmmo(playerInput.PlayerWeapon.GetSelectedWeaponData().Ammo, playerInput.PlayerWeapon.GetSelectedWeaponData().AmmoMax);
						}
					});
				}
				else
				{
					playerInput.PlayerWeapon.CanFire = true;
				}
			});
		}
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
				SaveLoadManager.SetMoney1(8);
				SaveLoadManager.SetHeadshot1();
			}
			else
			{
				PlayerLevelManager.UpdatePlayerXP(5);
				SaveLoadManager.SetMoney1(3);
			}
		}
		if (PlayerKills >= 1 || (PlayerKills >= 0 && WeaponData.Weapon == WeaponTypeList.Knife))
		{
			PlayerKills = 0;
			OnUpdateWeapon();
		}
		else
		{
			PlayerKills++;
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
			GameManager.LoadNextLevel(GameMode.GunGame);
		}
	}
}
