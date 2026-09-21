using UnityEngine;

public class mInAppManager : MonoBehaviour
{
	private MoneyType Money;

	private bool isRewardedVideo;

	private void Start()
	{
		EventManager.AddListener("UpdateInApp", UpdateInApp);
		UpdateInApp();
	}

	private void UpdateInApp()
	{
		if ((bool)InAppManager.AllSkins)
		{
			UpdateAllSkins();
		}
		if ((bool)InAppManager.AllWeapons)
		{
			UpdateAllWeapons();
		}
	}

	private void UpdateAllSkins()
	{
		for (int i = 0; i < GameSettings.instance.WeaponsShop.Count; i++)
		{
			for (int j = 0; j < GameSettings.instance.WeaponsShop[i].Skins.Count; j++)
			{
				SaveLoadManager.SetWeaponSkin(i + 1, GameSettings.instance.WeaponsShop[i].Skins[j].SkinID);
			}
		}
		for (int k = 0; k < GameSettings.instance.PlayerSkinShop.Count; k++)
		{
			SaveLoadManager.SetPlayerSkin(GameSettings.instance.PlayerSkinShop[k].SkinID);
		}
	}

	private void UpdateAllWeapons()
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			SaveLoadManager.SetWeapon(GameSettings.instance.Weapons[i]);
		}
	}

	public void OnRewardedVideo(int money)
	{
		if (!isRewardedVideo)
		{
			Money = (MoneyType)money;
			UIToast.Show(Localization.Get("Please wait") + "...");
			isRewardedVideo = true;
			vp_Timer.In(0.5f, delegate
			{
				AdsManager.ShowRewardedVideo(RewardedVideoComplete, RewardedVideoFailed, RewardedVideoAborted);
			});
		}
	}

	private void RewardedVideoComplete()
	{
		vp_Timer.In(0.3f, delegate
		{
			if (Money == MoneyType.Money)
			{
				SaveLoadManager.SetMoney1(50);
				UIToast.Show("+50 " + Localization.Get("Money"));
			}
			else
			{
				SaveLoadManager.SetGold1(1);
				UIToast.Show("+1 " + Localization.Get("Gold"));
			}
			EventManager.Dispatch("UpdateMoney");
			isRewardedVideo = false;
		});
	}

	private void RewardedVideoAborted()
	{
		isRewardedVideo = false;
	}

	private void RewardedVideoFailed()
	{
		isRewardedVideo = false;
		UIToast.Show(Localization.Get("Video not available"), 3f);
	}
}
