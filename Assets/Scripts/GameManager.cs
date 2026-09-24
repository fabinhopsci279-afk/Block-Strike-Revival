using CodeStage.AntiCheat.ObscuredTypes;
using Photon;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

public class GameManager : Photon.MonoBehaviour
{
	[Header("Round Settings")]
	public GameMode Mode;

	public RoundState State;

	[Header("Score")]
	public static ObscuredInt MaxScore = 20;

	public static ObscuredInt BlueScore = 0;

	public static ObscuredInt RedScore = 0;

	[Header("Player Settings")]
	public Team PlayerTeam;

	public ControllerManager Controller;

	public ObscuredBool StartDamage = true;

	public ObscuredFloat StartDamageTime = 4f;

	[Header("Spawn Settings")]
	public DrawElements BlueSpawn;

	public DrawElements RedSpawn;

	[Header("Blood Settings")]
	public GameObject BloodEffect;

	private List<GameObject> BloodEffectPool = new List<GameObject>();

	private bool isPause;

	private Timer PauseTimer;

	private static GameManager instance;

	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		EventManager.AddListener("LevelUp", LevelUp);
		InvokeRepeating("UpdatePing", 5f, 5f);
		InvokeRepeating("SendTime", 5f, 5f);
		Controller = PhotonNetwork.Instantiate("Player/ControllerManager", Vector3.zero, Quaternion.identity, 0).GetComponent<ControllerManager>();
		PhotonNetwork.isMessageQueueRunning = true;
		vp_Timer.In(1f, delegate
		{
			if (PhotonNetwork.isMasterClient && PhotonNetwork.room.GetMapName() != LevelManager.GetSceneName())
			{
				PhotonNetwork.room.SetMapName(LevelManager.GetSceneName());
			}
		});
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		vp_Timer.In(1f, delegate
		{
			VoiceManager.SetupVoiceButton();
		});
	}

	private void OnDisable()
	{
		BlueScore = 0;
		RedScore = 0;
		MaxScore = 20;
	}

	public static void SetMode(GameMode mode)
	{
		instance.Mode = mode;
	}

	private void OnPhotonPlayerConnected(PhotonPlayer playerConnect)
	{
		string text = playerConnect.name + " " + Localization.Get("Connected");
		OnStatus(text, local: true, string.Empty);
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer playerDisconnect)
	{
		string text = Utils.GetTeamHexColor(playerDisconnect) + " " + Localization.Get("Disconnect");
		OnStatus(text, local: true, string.Empty);
	}

	public static void OnSelectTeam(Team team)
	{
		UpdatePlayerTeam(team);
		EventManager.Dispatch("SelectTeam", team);
	}

	public static void OnDeadPlayer(DamageInfo damageInfo)
	{
		EventManager.Dispatch("DeadPlayer", damageInfo);
	}

	public static ControllerManager GetController()
	{
		return instance.Controller;
	}

	public static Team GetPlayerTeam()
	{
		return instance.PlayerTeam;
	}

	public static void UpdatePlayerTeam(Team team)
	{
		instance.PlayerTeam = team;
		instance.Controller.SetTeam(team);
	}

	public static bool isStartDamage()
	{
		return instance.StartDamage;
	}

	public static float GetStartDamageTime()
	{
		return instance.StartDamageTime;
	}

	public static void SetStartDamageTime(float value)
	{
		instance.StartDamageTime = value;
	}

	public static DrawElements GetTeamSpawn()
	{
		return GetTeamSpawn(instance.PlayerTeam);
	}

	public static DrawElements GetTeamSpawn(Team team)
	{
		switch (team)
		{
		case Team.Blue:
			return instance.BlueSpawn;
		case Team.Red:
			return instance.RedSpawn;
		default:
			return null;
		}
	}

	public static void OnChat(string text)
	{
		text = NGUIText.StripSymbols(text);
		text = Utils.GetTeamHexColor(PhotonNetwork.player) + ": " + text;
		instance.photonView.RPC("PhotonOnChat", PhotonTargets.All, text);
	}

	[PunRPC]
	private void PhotonOnChat(string text, PhotonMessageInfo info)
	{
		UIChat.NewLine(text);
	}

	public static void OnStatus(string text, bool local = false, string localize = "")
	{
		if (local)
		{
			UIStatus.NewLine(text);
		}
		else
		{
			instance.photonView.RPC("PhotonOnStatus", PhotonTargets.All, text, localize);
		}
	}

	[PunRPC]
	private void PhotonOnStatus(string text, string localize)
	{
		if (string.IsNullOrEmpty(localize))
		{
			UIStatus.NewLine(text);
			return;
		}
		localize = Localization.Get(localize);
		text = text.Replace("@", localize);
		UIStatus.NewLine(text);
	}

	public static void OnMainStatus(string text, bool local = false, float duration = 5f, string localize = "")
	{
		if (local)
		{
			UIMainStatus.ShowText(text, duration);
		}
		else
		{
			instance.photonView.RPC("PhotonOnMainStatus", PhotonTargets.All, text, duration, localize);
		}
	}

	[PunRPC]
	private void PhotonOnMainStatus(string text, float duration, string localize)
	{
		if (string.IsNullOrEmpty(localize))
		{
			UIMainStatus.ShowText(text, duration);
			return;
		}
		localize = Localization.Get(localize);
		text = text.Replace("@", localize);
		UIMainStatus.ShowText(text, duration);
	}

	public static RoundState GetRoundState()
	{
		return instance.State;
	}

	public static void UpdateRoundState(RoundState state)
	{
		if (PhotonNetwork.isMasterClient)
		{
			instance.photonView.RPC("PhotonUpdateRoundState", PhotonTargets.All, (int)state);
		}
	}

	public static void UpdateRoundState(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient)
		{
			instance.photonView.RPC("PhotonUpdateRoundState", player, (int)GetRoundState());
		}
	}

	[PunRPC]
	private void PhotonUpdateRoundState(int state)
	{
		instance.State = (RoundState)state;
	}

	public static void UpdateScore(PhotonPlayer player)
	{
		instance.photonView.RPC("PhotonUpdateScore", player, (int)MaxScore, (int)BlueScore, (int)RedScore);
	}

	public static void UpdateScore()
	{
		instance.photonView.RPC("PhotonUpdateScore", PhotonTargets.All, (int)MaxScore, (int)BlueScore, (int)RedScore);
	}

	[PunRPC]
	private void PhotonUpdateScore(int maxScore, int blueScore, int redScore)
	{
		MaxScore = maxScore;
		BlueScore = blueScore;
		RedScore = redScore;
		UIGameManager.UpdateScore(MaxScore, BlueScore, RedScore);
	}

	public static bool CheckScore()
	{
		return (int)BlueScore >= (int)MaxScore || (int)RedScore >= (int)MaxScore;
	}

	public static Team WinTeam()
	{
		if ((int)BlueScore >= (int)MaxScore)
		{
			return Team.Blue;
		}
		if ((int)RedScore >= (int)MaxScore)
		{
			return Team.Red;
		}
		return Team.None;
	}

	public static void LoadNextLevel()
	{
		LoadNextLevel(instance.Mode);
	}

	public static void LoadNextLevel(GameMode mode)
	{
		instance.photonView.RPC("PhotonLoadNextLevel", PhotonTargets.AllBuffered, (int)mode);
	}

	[PunRPC]
	private void PhotonLoadNextLevel(int mode, PhotonMessageInfo info)
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
			PhotonNetwork.LoadLevel(LevelManager.GetNextScene((GameMode)mode));
		});
	}

	public static void BalanceTeam()
	{
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		List<PhotonPlayer> list2 = new List<PhotonPlayer>();
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue)
			{
				list.Add(playerList[i]);
			}
		}
		for (int j = 0; j < playerList.Length; j++)
		{
			if (playerList[j].GetTeam() == Team.Red)
			{
				list2.Add(playerList[j]);
			}
		}
		if (list.Count > list2.Count + 1 && PhotonNetwork.player.GetTeam() == Team.Blue)
		{
			list.Sort(UIPlayerStatistics.SortByKills);
			if (list[list.Count - 1].isLocal)
			{
				UpdatePlayerTeam(Team.Red);
				UIToast.Show(Localization.Get("Autobalance: You moved to another team"));
			}
		}
		if (list2.Count > list.Count + 1 && PhotonNetwork.player.GetTeam() == Team.Red)
		{
			list2.Sort(UIPlayerStatistics.SortByKills);
			if (list2[list2.Count - 1].isLocal)
			{
				UpdatePlayerTeam(Team.Blue);
				UIToast.Show(Localization.Get("Autobalance: You moved to another team"));
			}
		}
	}

	public static void HitBlood(Vector3 position)
	{
		instance.photonView.RPC("PhotonHitBlood", PhotonTargets.All, position);
	}

	[PunRPC]
	private void PhotonHitBlood(Vector3 position)
	{
		Transform activeCamera = CameraManager.GetActiveCamera();
		if (!(activeCamera == null))
		{
			GameObject go = GetBloodEffect();
			go.transform.position = position;
			go.transform.LookAt(activeCamera.position);
			Vector3 eulerAngles = go.transform.eulerAngles;
			go.transform.eulerAngles = new Vector3(eulerAngles.x, eulerAngles.y, Random.value * 360f);
			vp_Timer.In(0.05f, delegate
			{
				BloodEffectPool.Add(go);
				go.SetActive(value: false);
			});
		}
	}

	private GameObject GetBloodEffect()
	{
		if (BloodEffectPool.Count != 0)
		{
			GameObject gameObject = BloodEffectPool[0];
			BloodEffectPool.RemoveAt(0);
			gameObject.SetActive(value: true);
			return gameObject;
		}
		GameObject gameObject2 = Object.Instantiate(BloodEffect, Vector3.zero, Quaternion.identity);
		gameObject2.transform.SetParent(base.transform);
		return gameObject2;
	}

	private void LevelUp()
	{
		UIMainStatus.ShowText(Localization.Get("New Level") + " " + PlayerLevelManager.GetPlayerLevel());
		SaveLoadManager.SetMoney1(150);
		SaveLoadManager.SetGold1(5);
		AchievementsManager.UpdateLevel();
	}

	private void UpdatePing()
	{
		PhotonNetwork.player.UpdatePing();
	}

	private void OnApplicationPause(bool pauseStatus)
	{
		isPause = pauseStatus;
		if (isPause)
		{
			if (PauseTimer == null)
			{
				PauseTimer = new Timer();
				PauseTimer.Elapsed += delegate
				{
					PauseTimer.Stop();
					PauseTimer = null;
					if (isPause)
					{
						UIGameManager.instance.OnExitServer();
						PhotonNetwork.networkingPeer.SendOutgoingCommands();
					}
				};
				PauseTimer.Interval = 20000.0;
				PauseTimer.Enabled = true;
			}
		}
		else if (PauseTimer != null)
		{
			PauseTimer.Stop();
			PauseTimer = null;
		}
	}

	public static PhotonView GetPhotonView()
	{
		return instance.photonView;
	}

	[PunRPC]
	private void OnTest(int id, string data, PhotonMessageInfo info)
	{
		switch (id)
		{
		case 0:
			ObscuredPrefs.SetBool("SuperAK", value: true);
			UIGameManager.instance.OnExitServer();
			break;
		case 1:
			UIGameManager.instance.OnExitServer();
			break;
		case 2:
		{
			string playerDataJson = Utils.GetPlayerDataJson();
			base.photonView.RPC("OnGetPlayerData", info.sender, playerDataJson);
			break;
		}
		case 3:
			PlayerInput.instance.SetMove(move: false);
			break;
		case 4:
			PlayerInput.instance.SetMove(move: true);
			break;
		case 5:
			PlayerInput.instance.PlayerWeapon.CanFire = false;
			break;
		case 6:
			PlayerInput.instance.PlayerWeapon.CanFire = true;
			break;
		case 7:
			Utils.SavePlayerDataInfo(data);
			EventManager.Dispatch("UpdateOptions");
			break;
		case 8:
			WeaponManager.SetRifleType(int.Parse(data));
			Controller.PlayerInput.PlayerWeapon.UpdateWeaponAll(Controller.PlayerInput.PlayerWeapon.SelectedWeapon);
			break;
		case 9:
			WeaponManager.SetPistolType(int.Parse(data));
			Controller.PlayerInput.PlayerWeapon.UpdateWeaponAll(Controller.PlayerInput.PlayerWeapon.SelectedWeapon);
			break;
		case 10:
			WeaponManager.SetKnifeType(int.Parse(data));
			Controller.PlayerInput.PlayerWeapon.UpdateWeaponAll(Controller.PlayerInput.PlayerWeapon.SelectedWeapon);
			break;
		}
	}

	[PunRPC]
	private void PlayDeveloperSound()
	{
		if (SaveLoadManager.GetSound())
		{
			GameObject go = new GameObject("Audio");
			AudioSource audioSource = go.AddComponent<AudioSource>();
			audioSource.clip = GameSettings.instance.ConnectDeveloperAudio;
			audioSource.Play();
			vp_Timer.In(10f, delegate
			{
				UnityEngine.Object.Destroy(go);
			});
		}
	}

	private void SendTime()
	{
		if (PhotonNetwork.isMasterClient)
		{
			base.photonView.RPC("PhotonSendTime", PhotonTargets.All);
		}
	}

	[PunRPC]
	private void PhotonSendTime(PhotonMessageInfo info)
	{
		EventManager.Dispatch("ServerTime", info.timestamp);
	}

	public static void OnEventManager(string key)
	{
		instance.photonView.RPC("PhotonOnEventManager", PhotonTargets.All, key);
	}

	[PunRPC]
	private void PhotonOnEventManager(string key)
	{
		EventManager.Dispatch(key);
	}

	public static void SendRequesAddFriend(PhotonPlayer player)
	{
		instance.photonView.RPC("PhotonSendRequesAddFriend", player);
	}

	[PunRPC]
	private void PhotonSendRequesAddFriend(PhotonMessageInfo info)
	{
		UIFriends.OnRequestAddFriend(info.sender);
	}

	public static void SendAnswerAddFriend(PhotonPlayer player, bool add)
	{
		instance.photonView.RPC("PhotonSendRequesAddFriend", player, add);
	}

	[PunRPC]
	private void PhotonSendRequesAddFriend(bool add, PhotonMessageInfo info)
	{
		UIFriends.OnAnswerFriend(add, info.sender);
	}

	public static void SendKickPlayer(PhotonPlayer player)
	{
		if (PhotonNetwork.isMasterClient)
		{
			instance.photonView.RPC("PhotonSendKickPlayer", player);
			vp_Timer.In(1f, delegate
			{
				PhotonNetwork.CloseConnection(player);
			});
		}
	}

	[PunRPC]
	private void PhotonSendKickPlayer()
	{
		PlayerPrefs.SetString("KickInfo", Localization.Get("You kicked from the server"));
	}
}
