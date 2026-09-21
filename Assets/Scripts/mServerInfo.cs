using UnityEngine;

public class mServerInfo : MonoBehaviour
{
	public UILabel ServerNameLabel;

	public UILabel ModeLabel;

	public UILabel MapNameLabel;

	public UILabel PlayersLabel;

	public GameObject Password;

	private RoomInfo m_RoomInfo;

	public void SetData(RoomInfo info)
	{
		m_RoomInfo = info;
		ServerNameLabel.text = info.name;
		ModeLabel.text = Localization.Get(info.GetGameMode().ToString());
		MapNameLabel.text = info.GetMapName();
		PlayersLabel.text = info.playerCount + "/" + info.maxPlayers;
		if (string.IsNullOrEmpty(info.GetPassword()))
		{
			Password.SetActive(value: false);
		}
		else
		{
			Password.SetActive(value: true);
		}
	}

	public void OnClick()
	{
		if (m_RoomInfo.playerCount != m_RoomInfo.maxPlayers)
		{
			if (string.IsNullOrEmpty(m_RoomInfo.GetPassword()) || Application.isEditor)
			{
				mPhotonSettings.OnJoinServer(m_RoomInfo);
			}
			else
			{
				mPopUp.ShowInput(string.Empty, Localization.Get("Password"), 4, UIInput.KeyboardType.NumberPad, null, Localization.Get("Back"), OnBack, "Ok", OnNext);
			}
		}
	}

	private void OnBack()
	{
		mPopUp.HidePopUp("ServerList");
	}

	private void OnNext()
	{
		if (m_RoomInfo.GetPassword() == mPopUp.GetInputText())
		{
			mPhotonSettings.OnJoinServer(m_RoomInfo);
		}
		else
		{
			UIToast.Show(Localization.Get("Password is incorrect"));
		}
	}
}
