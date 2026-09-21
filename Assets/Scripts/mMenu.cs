using DG.Tweening;
using System;
using UnityEngine;

public class mMenu : MonoBehaviour
{
	public UILabel PlayerNameLabel;

	public UILabel PlayerLevelLabel;

	public UIProgressBar PlayerXPSlider;

	public UILabel MoneyLabel;

	public UILabel GoldLabel;

	public UILabel VersionLabel;

	private void Start()
	{
		InputManager.Init();
		if (!SaveLoadManager.HasPlayerName() && SaveLoadManager.GetMoney() > 100)
		{
			PlayerPrefs.DeleteAll();
			SaveLoadManager.SetMoney(99);
			Application.Quit();
		}
		if (PlayerPrefs.HasKey("KickInfo"))
		{
			mPopUp.ShowText(PlayerPrefs.GetString("KickInfo"), 3f, "Menu");
			PlayerPrefs.DeleteKey("KickInfo");
		}
		AchievementsManager.UpdateMoney();
		VersionLabel.text = VersionManager.bundleVersion;
		PlayerLevelLabel.text = "Lvl " + PlayerLevelManager.GetPlayerLevel();
		PlayerXPSlider.value = PlayerLevelManager.GetPlayerXPPercent();
		UpdateMoney();
		UpdatePlayerName();
		WeaponManager.Init();
		string playerName = SaveLoadManager.GetPlayerName();
		string text = playerName.Replace(Environment.NewLine, string.Empty).Replace("\n", string.Empty).Replace("\r", string.Empty);
		if (playerName != text)
		{
			SaveLoadManager.SetPlayerName(text);
		}
		EventManager.AddListener("UpdatePlayerName", UpdatePlayerName);
		EventManager.AddListener("UpdateMoney", UpdateMoney);
	}

	public void OnExit()
	{
		Application.Quit();
	}

	private void UpdatePlayerName()
	{
		DOTween.To(() => PlayerNameLabel.text, delegate(string x)
		{
			PlayerNameLabel.text = x;
		}, SaveLoadManager.GetPlayerName(), 1f).SetOptions(richTextEnabled: true, ScrambleMode.All);
	}

	private void UpdateMoney()
	{
		if (MoneyLabel.text != SaveLoadManager.GetMoney().ToString())
		{
			DOTween.To(() => MoneyLabel.text, delegate(string x)
			{
				MoneyLabel.text = x;
			}, SaveLoadManager.GetMoney().ToString("n0"), 1f).SetOptions(richTextEnabled: true, ScrambleMode.All);
		}
		if (MoneyLabel.text != SaveLoadManager.GetGold().ToString())
		{
			DOTween.To(() => GoldLabel.text, delegate(string x)
			{
				GoldLabel.text = x;
			}, SaveLoadManager.GetGold().ToString("n0"), 1f).SetOptions(richTextEnabled: true, ScrambleMode.All);
		}
	}

	public void OnOthersApps()
	{
		Application.OpenURL("https://play.google.com/store/apps/developer?id=Rexet+Studio");
	}
}
