using System;
using System.Collections.Generic;
using UnityEngine;

public class mCreateServer : MonoBehaviour
{
	public GameMode SelectMode;

	public UIPopupList SelectModePopupList;

	public UIPopupList SelectMapPopupList;

	public UIInput ServerName;

	public mSelectElement MaxPlayers;

	public UIInput Password;

	private static mCreateServer instance;

	private void Start()
	{
		instance = this;
		ServerName.value = "Room " + UnityEngine.Random.Range(0, 9999);
		SelectModePopupList.Clear();
		for (int i = 0; i < Enum.GetValues(typeof(GameMode)).Length; i++)
		{
			UIPopupList selectModePopupList = SelectModePopupList;
			GameMode gameMode = (GameMode)i;
			selectModePopupList.AddItem(gameMode.ToString());
		}
		SelectModePopupList.value = SelectModePopupList.items[0];
		UpdateMaps();
	}

	private void UpdateMaps()
	{
		List<string> gameModeScenes = LevelManager.GetGameModeScenes(SelectMode);
		SelectMapPopupList.Clear();
		for (int i = 0; i < gameModeScenes.Count; i++)
		{
			SelectMapPopupList.AddItem(gameModeScenes[i]);
		}
		SelectMapPopupList.value = SelectMapPopupList.items[0];
	}

	public void OnSelectGameMode()
	{
		SelectMode = (GameMode)(int)Enum.Parse(typeof(GameMode), SelectModePopupList.value);
		UpdateMaps();
	}

	public void OnCheckServerName()
	{
		if (ServerName.value.Length < 4)
		{
			ServerName.value = "Room " + UnityEngine.Random.Range(0, 9999);
		}
		ServerName.value = NGUIText.StripSymbols(ServerName.value);
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		int num = 0;
		while (true)
		{
			if (num < roomList.Length)
			{
				if (roomList[num].name == ServerName.value)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		ServerName.value = "Room " + UnityEngine.Random.Range(0, 9999);
	}

	public static GameMode GetGameMode()
	{
		return instance.SelectMode;
	}

	public static string GetMap()
	{
		return instance.SelectMapPopupList.value;
	}

	public static string GetServerName()
	{
		return instance.ServerName.value;
	}

	public static int GetMaxPlayers()
	{
		return instance.MaxPlayers.valueInt;
	}

	public static string GetPassword()
	{
		return instance.Password.value;
	}

	public void CreateServer()
	{
		mPhotonSettings.OnCreateServer();
	}
}
