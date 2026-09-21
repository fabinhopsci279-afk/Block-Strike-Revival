using UnityEngine;

public class CaseTimeManager : MonoBehaviour
{
	public static bool isFinished;

	private static CaseTimeManager instance;

	private void Start()
	{
		if (instance == null)
		{
			instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
			if (GetTime() == 0.0)
			{
				isFinished = true;
			}
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	private void UpdateTime(double serverTime)
	{
		if ((PhotonNetwork.isMasterClient && PhotonNetwork.inRoom && PhotonNetwork.room.playerCount <= 1) || isFinished)
		{
			return;
		}
		double time = PhotonNetwork.time;
		if (serverTime + 1.0 > time && serverTime - time <= 1.0)
		{
			double time2 = GetTime();
			time2 -= 5.0;
			if (time2 <= 0.0)
			{
				isFinished = true;
				UIToast.Show(Localization.Get("The new case is available"), 2f);
			}
			SetTime(time2);
		}
	}

	public double GetTime()
	{
		return SaveLoadManager.GetCaseTime();
	}

	public void SetTime(double time)
	{
		SaveLoadManager.SetCaseTime(time);
	}

	public static string GetCaseTime()
	{
		double time = instance.GetTime();
		int num = (int)((float)time / 60f);
		int num2 = (int)((float)time - (float)(num * 60));
		return $"{num:00}:{num2:00}";
	}

	public static void UpdateTimeCase()
	{
		isFinished = false;
		instance.SetTime(900.0);
	}

	private void OnLevelWasLoaded(int level)
	{
		vp_Timer.In(0.5f, delegate
		{
			EventManager.AddListener<double>("ServerTime", UpdateTime);
		});
	}
}
