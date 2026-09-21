using Photon;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class mFriendManager : PunBehaviour
{
	public List<string> Friends = new List<string>();

	public UILabel OfflineLabel;

	public UILabel NameLabel;

	public UILabel InRoomLabel;

	public UILabel DeleteFriend;

	public UILabel JoinLabel;

	public UIGrid Grid;

	public UIScrollView Scroll;

	public GameObject FriendElement;

	private FriendInfo SelectedFriend;

	private RoomInfo SelectRoom;

	private List<GameObject> FriendsList = new List<GameObject>();

	private List<GameObject> FriendsListPool = new List<GameObject>();

	private void Start()
	{
		PhotonClassesManager.Add(this);
	}

	public void OnStart()
	{
		OnClearList();
		Friends = SaveLoadManager.GetFriends().ToList();
		Friends.Remove(string.Empty);
		if (Friends.Count != 0)
		{
			for (int i = 0; i < Friends.Count; i++)
			{
				GameObject gameObject = OnGetFriendElement();
				gameObject.SetActive(value: true);
				gameObject.name = Friends[i];
				gameObject.GetComponent<UILabel>().text = Friends[i];
				FriendsList.Add(gameObject);
			}
			DeleteFriend.gameObject.SetActive(value: false);
			SelectedFriend = null;
			Scroll.SetDragAmount(0f, 0f, updateScrollbars: false);
			PhotonNetwork.FindFriends(Friends.ToArray());
			Grid.repositionNow = true;
		}
	}

	private new void OnUpdatedFriendList()
	{
	}

	private void OnClearList()
	{
		SelectedFriend = null;
		OfflineLabel.gameObject.SetActive(value: false);
		NameLabel.gameObject.SetActive(value: false);
		DeleteFriend.gameObject.SetActive(value: false);
		InRoomLabel.gameObject.SetActive(value: false);
		JoinLabel.gameObject.SetActive(value: false);
		if (FriendsList.Count != 0)
		{
			for (int i = 0; i < FriendsList.Count; i++)
			{
				FriendsList[i].gameObject.SetActive(value: false);
				FriendsListPool.Add(FriendsList[i]);
			}
			FriendsList.Clear();
		}
	}

	private GameObject OnGetFriendElement()
	{
		GameObject result;
		if (FriendsListPool.Count != 0)
		{
			result = FriendsListPool[0].gameObject;
			FriendsListPool.RemoveAt(0);
		}
		else
		{
			result = NGUITools.AddChild(Grid.gameObject, FriendElement);
		}
		return result;
	}

	public void SelectFriend(string sf)
	{
		int num = 0;
		while (true)
		{
			if (num < PhotonNetwork.Friends.Count)
			{
				if (sf == PhotonNetwork.Friends[num].Name)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		SelectedFriend = PhotonNetwork.Friends[num];
		OfflineLabel.gameObject.SetActive(value: true);
		NameLabel.gameObject.SetActive(value: true);
		if (PhotonNetwork.Friends[num].IsOnline)
		{
			OfflineLabel.text = Localization.Get("Online");
			OfflineLabel.color = Color.green;
		}
		else
		{
			OfflineLabel.text = Localization.Get("Offline");
			OfflineLabel.color = Color.red;
		}
		NameLabel.text = Localization.Get("Name") + ": " + PhotonNetwork.Friends[num].Name;
		DeleteFriend.gameObject.SetActive(value: true);
		if (PhotonNetwork.Friends[num].IsInRoom)
		{
			InRoomLabel.gameObject.SetActive(value: true);
			InRoomLabel.text = Localization.Get("Server") + ": " + PhotonNetwork.Friends[num].Room;
			JoinLabel.gameObject.SetActive(value: true);
		}
		else
		{
			InRoomLabel.gameObject.SetActive(value: false);
			JoinLabel.gameObject.SetActive(value: false);
		}
	}

	public void OnJoinClick()
	{
		RoomInfo[] roomList = PhotonNetwork.GetRoomList();
		for (int i = 0; i < roomList.Length; i++)
		{
			if (roomList[i].name == SelectedFriend.Room)
			{
				SelectRoom = roomList[i];
				break;
			}
		}
		if (string.IsNullOrEmpty(SelectRoom.GetPassword()))
		{
			mPhotonSettings.OnJoinServer(SelectRoom);
		}
		else
		{
			mPopUp.ShowInput(string.Empty, Localization.Get("Password"), 4, UIInput.KeyboardType.NumberPad, null, Localization.Get("Back"), OnJoinCancel, "Ok", OnJoinApply);
		}
	}

	private void OnJoinCancel()
	{
		mPopUp.HidePopUp("Friends");
	}

	private void OnJoinApply()
	{
		if (SelectRoom.GetPassword() == mPopUp.GetInputText())
		{
			mPhotonSettings.OnJoinServer(SelectRoom);
		}
		else
		{
			UIToast.Show(Localization.Get("Password is incorrect"));
		}
	}

	public void OnDeleteClick()
	{
		if (SelectedFriend == null)
		{
			UIToast.Show(Localization.Get("No player is selected"));
		}
		else
		{
			mPopUp.ShowPopUp(Localization.Get("Do you want to remove") + " " + SelectedFriend.Name + "?", Localization.Get("Delete"), Localization.Get("No"), OnDeleteNo, Localization.Get("Yes"), OnDeleteYes);
		}
	}

	private void OnDeleteNo()
	{
		mPopUp.HidePopUp("Friends");
	}

	private void OnDeleteYes()
	{
		Friends.Remove(SelectedFriend.Name);
		SaveLoadManager.SetFriends(Friends.ToArray());
		mPopUp.HidePopUp("Friends");
		OnStart();
	}

	public void OnAddFriend()
	{
		mPopUp.ShowInput(string.Empty, Localization.Get("Add Friend"), 15, UIInput.KeyboardType.Default, null, Localization.Get("Back"), OnAddFriendBack, "Ok", OnAddFriendOk);
	}

	private void OnAddFriendBack()
	{
		mPopUp.HidePopUp("Friends");
	}

	private void OnAddFriendOk()
	{
		string inputText = mPopUp.GetInputText();
		if (Friends.Contains(inputText))
		{
			UIToast.Show(Localization.Get("Player already in friends"));
			return;
		}
		Friends.Add(inputText);
		SaveLoadManager.SetFriends(Friends.ToArray());
		mPopUp.HidePopUp("Friends");
		OnStart();
	}
}
