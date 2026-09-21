using UnityEngine;

public class UIPlayerStatisticsElement : MonoBehaviour
{
	public GameObject SelectSprite;

	public UILabel PlayerName;

	public UILabel PlayerKills;

	public UILabel PlayerDeaths;

	public UILabel PlayerPing;

	private PhotonPlayer PlayerInfo;

	private GameObject m_GameObject;

	private void Start()
	{
		m_GameObject = base.gameObject;
		EventManager.AddListener<int>("StatsSelectPlayer", OnSelectPlayer);
	}

	private void OnDisable()
	{
		SelectSprite.SetActive(value: false);
	}

	public void SetData(PhotonPlayer playerInfo)
	{
		PlayerInfo = playerInfo;
		PlayerName.text = "[" + playerInfo.GetLevel() + "] " + PlayerInfo.name;
		PlayerKills.text = PlayerInfo.GetKills().ToString();
		PlayerDeaths.text = PlayerInfo.GetDeaths().ToString();
		PlayerPing.text = PlayerInfo.GetPing().ToString();
		base.name = PlayerName.text;
		if (playerInfo.isLocal)
		{
			PlayerName.color = Color.green;
			PlayerKills.color = Color.green;
			PlayerDeaths.color = Color.green;
			PlayerPing.color = Color.green;
		}
		else
		{
			PlayerName.color = Color.white;
			PlayerKills.color = Color.white;
			PlayerDeaths.color = Color.white;
			PlayerPing.color = Color.white;
		}
		if (playerInfo.isMasterClient)
		{
			PlayerName.color = Color.magenta;
		}
		UpdateData();
	}

	private void UpdateData()
	{
		vp_Timer.In(3f, delegate
		{
			if (m_GameObject.activeSelf)
			{
				SetData(PlayerInfo);
			}
		});
	}

	private void OnSelectPlayer(int id)
	{
		if (PlayerInfo != null && PlayerInfo.ID == id)
		{
			SelectSprite.SetActive(value: true);
		}
		else
		{
			SelectSprite.SetActive(value: false);
		}
	}

	private void OnClick()
	{
		if (PlayerInfo != null && PlayerInfo.ID != PhotonNetwork.player.ID)
		{
			EventManager.Dispatch("StatsSelectPlayer", PlayerInfo.ID);
		}
	}
}
