using System.Collections.Generic;
using UnityEngine;

public class UIPlayerStatistics : MonoBehaviour
{
	[Header("Parents")]
	public Transform BlueTeamParent;

	public Transform RedTeamParent;

	public Transform DeadsParent;

	[Header("Panel Labels")]
	public UILabel BlueLabel;

	public UILabel RedLabel;

	public UILabel SpectatorsLabel;

	[Header("Server")]
	public UILabel ServerNameLabel;

	public UILabel ModeLabel;

	public UILabel MapLabel;

	public UILabel PlayersLabel;

	[Header("Others")]
	public GameObject Element;

	public UITable Table;

	private bool isShow;

	private List<Transform> PlayerList = new List<Transform>();

	private List<Transform> PlayerListPool = new List<Transform>();

	private void Update()
	{
		if (InputManager.GetButtonDown("Statistics"))
		{
			Show();
		}
	}

	private void Show()
	{
		UIPanelManager.ShowPanel("Statistics");
		ClearList();
		EventManager.Dispatch("StatsOpen");
		ServerNameLabel.text = PhotonNetwork.room.name;
		PlayersLabel.text = Localization.Get("Players") + ": " + PhotonNetwork.room.playerCount + "/" + PhotonNetwork.room.maxPlayers;
		MapLabel.text = Localization.Get("Map") + ": " + PhotonNetwork.room.GetMapName();
		ModeLabel.text = Localization.Get("Mode") + ": " + Localization.Get(PhotonNetwork.room.GetGameMode().ToString());
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		List<PhotonPlayer> list = new List<PhotonPlayer>();
		List<PhotonPlayer> list2 = new List<PhotonPlayer>();
		List<PhotonPlayer> list3 = new List<PhotonPlayer>();
		for (int i = 0; i < playerList.Length; i++)
		{
			if (playerList[i].GetTeam() == Team.Blue)
			{
				if (playerList[i].GetDead())
				{
					list3.Add(playerList[i]);
				}
				else
				{
					list.Add(playerList[i]);
				}
			}
			if (playerList[i].GetTeam() == Team.Red)
			{
				if (playerList[i].GetDead())
				{
					list3.Add(playerList[i]);
				}
				else
				{
					list2.Add(playerList[i]);
				}
			}
		}
		list.Sort(SortByKills);
		list2.Sort(SortByKills);
		list3.Sort(SortByKills);
		BlueTeamParent.gameObject.SetActive(list.Count >= 1);
		RedTeamParent.gameObject.SetActive(list2.Count >= 1);
		DeadsParent.gameObject.SetActive(list3.Count >= 1);
		if (PhotonNetwork.room.GetGameMode() == GameMode.ZombieSurvival)
		{
			BlueLabel.text = Localization.Get("Survivors") + " [" + list.Count + "]";
		}
		else
		{
			BlueLabel.text = Localization.Get("Blue Team") + " [" + list.Count + "]";
		}
		if (PhotonNetwork.room.GetGameMode() == GameMode.ZombieSurvival)
		{
			RedLabel.text = Localization.Get("Zombie") + " [" + list2.Count + "]";
		}
		else
		{
			RedLabel.text = Localization.Get("Red Team") + " [" + list2.Count + "]";
		}
		SpectatorsLabel.text = Localization.Get("Spectators") + " [" + list3.Count + "]";
		for (int j = 0; j < list.Count; j++)
		{
			Transform transform = GetGameObject().transform;
			transform.GetComponent<UIPlayerStatisticsElement>().SetData(list[j]);
			transform.SetParent(BlueTeamParent);
			if (j == 0)
			{
				transform.localPosition = Vector3.down * 20f;
			}
			else
			{
				transform.localPosition = Vector3.down * (20 + 18 * j);
			}
			PlayerList.Add(transform);
		}
		for (int k = 0; k < list2.Count; k++)
		{
			Transform transform2 = GetGameObject().transform;
			transform2.GetComponent<UIPlayerStatisticsElement>().SetData(list2[k]);
			transform2.SetParent(RedTeamParent);
			if (k == 0)
			{
				transform2.localPosition = Vector3.down * 20f;
			}
			else
			{
				transform2.localPosition = Vector3.down * (20 + 18 * k);
			}
			PlayerList.Add(transform2);
		}
		for (int l = 0; l < list3.Count; l++)
		{
			Transform transform3 = GetGameObject().transform;
			transform3.GetComponent<UIPlayerStatisticsElement>().SetData(list3[l]);
			transform3.SetParent(DeadsParent);
			if (l == 0)
			{
				transform3.localPosition = Vector3.down * 20f;
			}
			else
			{
				transform3.localPosition = Vector3.down * (20 + 18 * l);
			}
			PlayerList.Add(transform3);
		}
		Table.Reposition();
	}

	public void Close()
	{
		UIPanelManager.ShowPanel("Display");
		ClearList();
		EventManager.Dispatch("StatsClose");
	}

	private GameObject GetGameObject()
	{
		GameObject gameObject;
		if (PlayerListPool.Count != 0)
		{
			gameObject = PlayerListPool[0].gameObject;
			PlayerListPool.RemoveAt(0);
		}
		else
		{
			gameObject = NGUITools.AddChild(Table.gameObject, Element);
		}
		gameObject.SetActive(value: true);
		return gameObject;
	}

	private void ClearList()
	{
		if (PlayerList.Count != 0)
		{
			for (int i = 0; i < PlayerList.Count; i++)
			{
				PlayerList[i].gameObject.SetActive(value: false);
				PlayerListPool.Add(PlayerList[i]);
			}
			PlayerList.Clear();
		}
	}

	public static int SortByKills(PhotonPlayer a, PhotonPlayer b)
	{
		return b.GetKills().CompareTo(a.GetKills());
	}
}
