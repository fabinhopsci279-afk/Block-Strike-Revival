using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIFriends : MonoBehaviour
{
	private PhotonPlayer SelectPlayer;

	private List<string> PlayerWait = new List<string>();

	private List<string> PlayerIgnore = new List<string>();

	private List<PhotonPlayer> Player = new List<PhotonPlayer>();

	private static UIFriends instance;

	private void Start()
	{
		instance = this;
		EventManager.AddListener<int>("StatsSelectPlayer", OnSelectPlayer);
		EventManager.AddListener("StatsClose", OnCloseStats);
	}

	private void OnSelectPlayer(int id)
	{
		SelectPlayer = PhotonPlayer.Find(id);
	}

	private void OnCloseStats()
	{
		SelectPlayer = null;
	}

	public void AddFriend()
	{
		if (SelectPlayer == null)
		{
			UIToast.Show(Localization.Get("Select a Player"));
			return;
		}
		if (PlayerWait.Contains(SelectPlayer.name))
		{
			UIToast.Show(Localization.Get("Waiting for confirmation"));
			return;
		}
		if (PlayerIgnore.Contains(SelectPlayer.name))
		{
			UIToast.Show(SelectPlayer.name + " " + Localization.Get("rejected a request"));
			return;
		}
		if (SaveLoadManager.GetFriends().ToList().Contains(SelectPlayer.name))
		{
			UIToast.Show(Localization.Get("Player already in friends"));
			return;
		}
		PlayerWait.Add(SelectPlayer.name);
		UIToast.Show(Localization.Get("Request has been sent"));
		GameManager.SendRequesAddFriend(SelectPlayer);
	}

	public static void OnAnswerFriend(bool add, PhotonPlayer player)
	{
		instance.PlayerWait.Remove(player.name);
		if (add)
		{
			UIToast.Show(player.name + " " + Localization.Get("accepted a request"));
			SaveLoadManager.SetFriend(player.name);
		}
		else
		{
			UIToast.Show(player.name + " " + Localization.Get("rejected a request"));
			instance.PlayerIgnore.Add(player.name);
		}
	}

	public static void OnRequestAddFriend(PhotonPlayer player)
	{
		instance.Player.Add(player);
		UINotification.AddNotification(Localization.Get("Add Friend"), player.name + " " + Localization.Get("wants to add to friends"), Localization.Get("Yes"), Localization.Get("No"), instance.OnYes, instance.OnNo);
	}

	private void OnYes()
	{
		GameManager.SendAnswerAddFriend(Player[0], add: true);
		SaveLoadManager.SetFriend(Player[0].name);
		Player.RemoveAt(0);
	}

	private void OnNo()
	{
		GameManager.SendAnswerAddFriend(Player[0], add: false);
		Player.RemoveAt(0);
	}
}
