using System;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
	public static bool isLoadedAds;

	private static Action RewardedVideoComplete;

	private static Action RewardedVideoFailed;

	private static Action RewardedVideoAborted;

	private void Start()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		Init();
	}

	public void Init()
	{
	}

	public static void AdmobLoad()
	{
	}

	public static void ShowInterstitial()
	{
	}

	public static void ShowVideo()
	{
	}

	public static void ShowRewardedVideo(Action complete, Action failed, Action aborted)
	{
		vp_Timer.In(0.5f, delegate
		{
			complete();
		});
	}

	private static void RewardedVideoFinished(int isComplete)
	{
		switch (isComplete)
		{
		case 0:
			if (RewardedVideoComplete != null)
			{
				RewardedVideoComplete();
			}
			break;
		case 1:
			if (RewardedVideoAborted != null)
			{
				RewardedVideoAborted();
			}
			break;
		default:
			if (RewardedVideoFailed != null)
			{
				RewardedVideoFailed();
			}
			break;
		}
		RewardedVideoComplete = null;
		RewardedVideoFailed = null;
		RewardedVideoAborted = null;
	}

	public void onInterstitialLoaded()
	{
		vp_Timer.In(0.5f, delegate
		{
			isLoadedAds = true;
			MonoBehaviour.print("Interstitial loaded");
		});
	}

	public void onInterstitialFailedToLoad()
	{
		vp_Timer.In(0.5f, delegate
		{
			isLoadedAds = true;
			MonoBehaviour.print("Interstitial failed");
		});
	}

	public void onInterstitialShown()
	{
		MonoBehaviour.print("Interstitial opened");
	}

	public void onInterstitialClosed()
	{
		MonoBehaviour.print("Interstitial closed");
	}

	public void onInterstitialClicked()
	{
		MonoBehaviour.print("Interstitial clicked");
	}

	public void onSkippableVideoLoaded()
	{
		MonoBehaviour.print("Video loaded");
	}

	public void onSkippableVideoFailedToLoad()
	{
		MonoBehaviour.print("Skippable Video failed");
	}

	public void onSkippableVideoShown()
	{
		MonoBehaviour.print("Skippable Video opened");
	}

	public void onSkippableVideoClosed()
	{
		RewardedVideoFinished(1);
		if (SaveLoadManager.GetConsole())
		{
			vp_Timer.In(0.5f, delegate
			{
				MonoBehaviour.print("NonSkippable Video closed");
			});
		}
	}

	public void onSkippableVideoFinished()
	{
		RewardedVideoFinished(0);
		if (SaveLoadManager.GetConsole())
		{
			vp_Timer.In(0.5f, delegate
			{
				MonoBehaviour.print("Skippable Video finished");
			});
		}
	}

	public void onRewardedVideoLoaded()
	{
		if (SaveLoadManager.GetConsole())
		{
			vp_Timer.In(0.5f, delegate
			{
				MonoBehaviour.print("Rewarded Video loaded");
			});
		}
	}

	public void onRewardedVideoFailedToLoad()
	{
		if (SaveLoadManager.GetConsole())
		{
			vp_Timer.In(0.5f, delegate
			{
				MonoBehaviour.print("Rewarded Video failed");
			});
		}
	}

	public void onRewardedVideoShown()
	{
		if (SaveLoadManager.GetConsole())
		{
			vp_Timer.In(0.5f, delegate
			{
				MonoBehaviour.print("Rewarded Video opened");
			});
		}
	}

	public void onRewardedVideoClosed()
	{
		RewardedVideoFinished(1);
		if (SaveLoadManager.GetConsole())
		{
			vp_Timer.In(0.5f, delegate
			{
				MonoBehaviour.print("Rewarded Video closed");
			});
		}
	}

	public void onRewardedVideoFinished(int amount, string name)
	{
		RewardedVideoFinished(0);
		if (SaveLoadManager.GetConsole())
		{
			vp_Timer.In(0.5f, delegate
			{
				MonoBehaviour.print("Rewarded Video finished: Reward: " + amount + name);
			});
		}
	}

	private static string GetAdmobID()
	{
		switch (DateTime.Now.DayOfWeek)
		{
		case DayOfWeek.Sunday:
			return "ca-app-pub-4175914389429655/1779067961";
		case DayOfWeek.Monday:
			return "ca-app-pub-4175914389429655/1918668767";
		case DayOfWeek.Tuesday:
			return "ca-app-pub-4175914389429655/3395401962";
		case DayOfWeek.Wednesday:
			return "ca-app-pub-4175914389429655/4872135162";
		case DayOfWeek.Thursday:
			return "ca-app-pub-4175914389429655/6348868360";
		case DayOfWeek.Friday:
			return "ca-app-pub-4175914389429655/7825601561";
		case DayOfWeek.Saturday:
			return "ca-app-pub-4175914389429655/9302334760";
		default:
			return "ca-app-pub-4175914389429655/1918668767";
		}
	}
}
