using UnityEngine;

public class UIKick : MonoBehaviour
{
	public GameObject Button;

	private PhotonPlayer SelectPlayer;

	private vp_Timer.Handle Timer = new vp_Timer.Handle();

	private bool isKick = true;

	private void Start()
	{
		EventManager.AddListener<int>("StatsSelectPlayer", OnSelectPlayer);
		EventManager.AddListener("StatsClose", OnCloseStats);
		EventManager.AddListener("StatsOpen", OnOpenStats);
	}

	private void OnSelectPlayer(int id)
	{
		SelectPlayer = PhotonPlayer.Find(id);
	}

	private void OnCloseStats()
	{
		SelectPlayer = null;
	}

	private void OnOpenStats()
	{
		if (PhotonNetwork.isMasterClient && SaveLoadManager.GetPlayerLevel() >= 15)
		{
			Button.SetActive(value: true);
		}
	}

	public void KickPlayer()
	{
		if (isKick)
		{
			if (SelectPlayer == null)
			{
				UIToast.Show(Localization.Get("Select a Player"));
				return;
			}
			if (OwnerPanel.IsOwner())
			{
				OwnerPanel.BanPlayer(SelectPlayer);
			}
			else
			{
				GameManager.SendKickPlayer(SelectPlayer);
			}
			isKick = false;
			vp_Timer.In(300f, delegate
			{
				isKick = true;
			}, Timer);
		}
		else
		{
			UIToast.Show(Utils.FloatToTime(Timer.DurationLeft));
		}
	}
}
