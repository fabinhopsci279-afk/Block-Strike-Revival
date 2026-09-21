using CodeStage.AntiCheat.ObscuredTypes;
using ExitGames.Client.Photon;
using Photon;
using UnityEngine;

public class mPhotonSettings : PunBehaviour
{
	public int SelectRegion = -1;

	public string[] Regions;

	private string SelectMap;

	private static mPhotonSettings instance;

	private void Awake()
	{
		PhotonClassesManager.Add(this);
	}

	private void Start()
	{
		instance = this;
		PhotonNetwork.automaticallySyncScene = true;
	}

	public void OnConnectToPhoton(int sr)
	{
		if (instance.SelectRegion != sr)
		{
			if (PhotonNetwork.connected)
			{
				PhotonNetwork.Disconnect();
			}
			instance.SelectRegion = sr;
			mPopUp.ShowText(Localization.Get("Connecting") + "...");
			string str = ObscuredPrefs.GetBool("SuperAK", defaultValue: false) ? VersionManager.bundleIdentifier : VersionManager.bundleVersion;
			PhotonNetwork.ConnectToMaster(Regions[SelectRegion], 5055, GameSettings.instance.PhotonID, str + "AW1");
		}
		else
		{
			mPanelManager.ShowPanel("Server");
		}
	}

	private new void OnConnectedToPhoton()
	{
		PhotonNetwork.playerName = SaveLoadManager.GetPlayerName();
		PhotonNetwork.player.SetLevel(SaveLoadManager.GetPlayerLevel());
		mPopUp.HidePopUp("Server");
	}

	private new void OnDisconnectedFromPhoton()
	{
		SelectRegion = -1;
		mPopUp.HidePopUp("Menu");
	}

	private new void OnFailedToConnectToPhoton(DisconnectCause cause)
	{
		UIToast.Show("Failed: " + cause.ToString());
	}

	private new void OnConnectionFail(DisconnectCause cause)
	{
		UIToast.Show("Fail: " + cause.ToString());
	}

	public static void OnCreateServer()
	{
		mPopUp.ShowText(Localization.Get("Creating Server") + "...");
		PhotonNetwork.playerName = SaveLoadManager.GetPlayerName();
		PhotonNetwork.player.ClearProperties();
		GameMode gameMode = mCreateServer.GetGameMode();
		string serverName = mCreateServer.GetServerName();
		int num = Mathf.Clamp(mCreateServer.GetMaxPlayers(), 4, 12);
		string password = mCreateServer.GetPassword();
		instance.SelectMap = mCreateServer.GetMap();
		Hashtable customRoomProperties = PhotonNetwork.room.CreateRoomHashtable(instance.SelectMap, password, gameMode);
		PhotonNetwork.CreateRoom(serverName, new RoomOptions
		{
			maxPlayers = (byte)num,
			isOpen = true,
			isVisible = true,
			customRoomProperties = customRoomProperties,
			customRoomPropertiesForLobby = new string[3]
			{
				"mapName",
				"password",
				"mode"
			}
		}, null);
	}

	public static void OnJoinServer(RoomInfo room)
	{
		mPopUp.ShowText(Localization.Get("Connecting") + "...");
		PhotonNetwork.playerName = SaveLoadManager.GetPlayerName();
		PhotonNetwork.player.ClearProperties();
		instance.SelectMap = room.GetMapName();
		PhotonNetwork.JoinRoom(room.name);
	}

	private new void OnJoinedRoom()
	{
		mPopUp.ShowText(Localization.Get("Loading") + "...");
		PhotonNetwork.isMessageQueueRunning = false;
		PhotonNetwork.LoadLevel(SelectMap);
	}

	private void OnPhotonJoinRoomFailed()
	{
		mPopUp.ShowText(Localization.Get("The server is full"), 2f, "Menu");
	}
}
