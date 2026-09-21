using UnityEngine;

public class CheatSystem : MonoBehaviour
{
	private int serverFalsePositives;

	private int falsePositives;

	private void Start()
	{
	}

	private void Quit()
	{
		MonoBehaviour.print("Cheat");
		if (PhotonNetwork.inRoom)
		{
			UIGameManager.instance.OnExitServer();
			vp_Timer.In(1f, delegate
			{
				Application.Quit();
			});
		}
		else
		{
			Application.Quit();
		}
	}

	public void UpdateTime(double serverTime)
	{
		double time = PhotonNetwork.time;
		if (serverTime + 1.0 > time)
		{
			falsePositives = 0;
			if (serverTime - time >= 1.0)
			{
				serverFalsePositives++;
				if (serverFalsePositives >= 3)
				{
					PlayerPrefs.SetString("KickInfo", Localization.Get("ServerAdminSpeedHack"));
					Quit();
				}
			}
		}
		else
		{
			falsePositives++;
			if (falsePositives >= 3)
			{
				Quit();
			}
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		vp_Timer.In(0.5f, delegate
		{
			EventManager.AddListener<double>("ServerTime", UpdateTime);
		});
	}
}
