using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mServerList : MonoBehaviour
{
	private int SelectMode = -1;

	public UILabel ServerInfoLabel;

	public UILabel ModeLabel;

	public GameObject ServerListElement;

	public GameObject ServerListParent;

	private List<GameObject> ServerList = new List<GameObject>();

	private List<GameObject> ServerListPool = new List<GameObject>();

	private bool isCreatingServerList;

	private int MaxPlayers;

	private int MaxServers;

	private void Start()
	{
		OnLocalize();
	}

	private void OnEnable()
	{
		Localization.onLocalize = (Localization.OnLocalizeNotification)Delegate.Combine(Localization.onLocalize, new Localization.OnLocalizeNotification(OnLocalize));
	}

	private void OnDisable()
	{
		Localization.onLocalize = (Localization.OnLocalizeNotification)Delegate.Remove(Localization.onLocalize, new Localization.OnLocalizeNotification(OnLocalize));
	}

	private void OnLocalize()
	{
		ModeLabel.text = Localization.Get("Mode") + ": " + Localization.Get("All");
	}

	public void UpdateServerList()
	{
		if (!isCreatingServerList)
		{
			ServerListParent.GetComponent<UIScrollView>().ResetPosition();
			StartCoroutine("CreateServerList");
		}
	}

	private IEnumerator CreateServerList()
	{
		isCreatingServerList = true;
		ClearServerList();
		MaxPlayers = 0;
		MaxServers = 0;
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		int count = -1;
		for (int j = 0; j < roomList.Length; j++)
		{
			if (SelectMode == -1 || (SelectMode != -1 && SelectMode == (int)roomList[j].GetGameMode()))
			{
				MaxServers++;
				MaxPlayers += roomList[j].playerCount;
			}
		}
		UpdateServerInfo();
		for (int i = 0; i < roomList.Length; i++)
		{
			MaxServers++;
			MaxPlayers += roomList[i].playerCount;
			if (SelectMode == -1 || (SelectMode != -1 && SelectMode == (int)roomList[i].GetGameMode()))
			{
				Transform element = GetElement();
				yield return new WaitForSeconds(0.01f);
				count++;
				element.GetComponent<mServerInfo>().SetData(roomList[i]);
				element.localPosition = Vector3.up * (100 - 30 * count);
				element.gameObject.SetActive(value: true);
				ServerList.Add(element.gameObject);
			}
		}
		isCreatingServerList = false;
	}

	private Transform GetElement()
	{
		GameObject gameObject;
		if (ServerListPool.Count != 0)
		{
			gameObject = ServerListPool[0];
			ServerListPool.RemoveAt(0);
		}
		else
		{
			gameObject = NGUITools.AddChild(ServerListParent, ServerListElement);
		}
		return gameObject.transform;
	}

	private void ClearServerList()
	{
		for (int i = 0; i < ServerList.Count; i++)
		{
			ServerList[i].SetActive(value: false);
			ServerListPool.Add(ServerList[i]);
		}
		ServerList.Clear();
	}

	private void UpdateServerInfo()
	{
		string text = Localization.Get("Players") + ": " + MaxPlayers + "\n" + Localization.Get("Servers") + ": " + MaxServers + "\n" + Localization.Get("Ping") + ": " + PhotonNetwork.GetPing();
		ServerInfoLabel.text = text;
	}

	public void SetNextMode()
	{
		SelectMode++;
		int length = Enum.GetValues(typeof(GameMode)).Length;
		if (SelectMode >= length)
		{
			SelectMode = -1;
			ModeLabel.text = Localization.Get("Mode") + ": " + Localization.Get("All");
		}
		else
		{
			UILabel modeLabel = ModeLabel;
			string str = Localization.Get("Mode");
			GameMode selectMode = (GameMode)SelectMode;
			modeLabel.text = str + ": " + Localization.Get(selectMode.ToString());
		}
		if (!isCreatingServerList)
		{
			isCreatingServerList = false;
			StopCoroutine("CreateServerList");
		}
		ServerListParent.GetComponent<UIScrollView>().ResetPosition();
		StartCoroutine("CreateServerList");
	}
}
