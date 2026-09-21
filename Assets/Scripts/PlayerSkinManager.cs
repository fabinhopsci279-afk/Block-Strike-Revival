using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkinManager : MonoBehaviour
{
	[Header("Player")]
	public GameObject PlayerSkinCamera;

	public GameObject PlayerSkinBlue;

	public SkinnedMeshAtlas PlayerSkinBlueAtlas;

	public GameObject PlayerSkinRed;

	public SkinnedMeshAtlas PlayerSkinRedAtlas;

	[Header("Buy Button")]
	public UISprite BuySprite;

	public UILabel BuyLabel;

	public UISprite BuyMoney;

	private List<string> SkinNames = new List<string>();

	private int SelectSkinName;

	private PlayerSkinShopData SelectedPlayerSkin;

	private ObscuredBool isBuy = false;

	public void OnActivePanel()
	{
		mPanelManager.ShowPanel("PlayerSkinShop", activePanelUp: true, activeInApp: false);
		PlayerSkinCamera.SetActive(value: true);
		SelectSkinName = 0;
		SkinNames.Clear();
		SkinNames = GetPlayerSkinNames();
		SelectSkin(SkinNames[SelectSkinName]);
	}

	public void OnDeactivePanel()
	{
		PlayerSkinCamera.SetActive(value: false);
	}

	private void OnDisconnectedFromPhoton()
	{
		OnDeactivePanel();
	}

	private void SelectSkin(string skinName)
	{
		SelectedPlayerSkin = GetPlayerSkinData(skinName);
		PlayerSkinBlueAtlas.spriteName = "1-" + SelectedPlayerSkin.SkinID;
		PlayerSkinRedAtlas.spriteName = "2-" + SelectedPlayerSkin.SkinID;
		PlayerSkinRed.GetComponent<Animator>().SetBool("Anim", value: true);
		isBuy = SaveLoadManager.GetPlayerSkin(SelectedPlayerSkin.SkinID);
		if ((int)SelectedPlayerSkin.SkinPrice == 0)
		{
			isBuy = true;
			SaveLoadManager.SetPlayerSkin(SelectedPlayerSkin.SkinID);
		}
		if ((bool)isBuy)
		{
			SelectPanel();
		}
		else
		{
			BuyPanel();
		}
	}

	private void BuyPanel()
	{
		BuyMoney.alpha = 1f;
		BuyLabel.transform.localPosition = Vector3.right * 7f;
		BuyLabel.text = SelectedPlayerSkin.SkinPrice.ToString();
		BuyMoney.spriteName = SelectedPlayerSkin.Money.ToString();
		BuyMoney.UpdateAnchors();
	}

	private void SelectPanel()
	{
		BuyMoney.alpha = 1f;
		if ((int)SelectedPlayerSkin.SkinID == SaveLoadManager.GetPlayerSkinSelected())
		{
			BuyLabel.transform.localPosition = Vector3.zero;
			BuyLabel.text = Localization.Get("Selected");
			BuyMoney.alpha = 0f;
		}
		else
		{
			BuyLabel.transform.localPosition = Vector3.zero;
			BuyLabel.text = Localization.Get("Select");
			BuyMoney.alpha = 0f;
		}
	}

	public void OnBuyClick()
	{
		if ((int)SelectedPlayerSkin.SkinID == SaveLoadManager.GetPlayerSkinSelected())
		{
			return;
		}
		if ((bool)isBuy)
		{
			SaveLoadManager.SetPlayerSkinSelected(SelectedPlayerSkin.SkinID);
		}
		else if (SelectedPlayerSkin.Money == MoneyType.Gold)
		{
			int gold = SaveLoadManager.GetGold();
			if (gold < (int)SelectedPlayerSkin.SkinPrice)
			{
				UIToast.Show(Localization.Get("Not enough money"));
				return;
			}
			gold -= (int)SelectedPlayerSkin.SkinPrice;
			SaveLoadManager.SetGold(gold);
			SaveLoadManager.SetPlayerSkin(SelectedPlayerSkin.SkinID);
			SaveLoadManager.SetPlayerSkinSelected(SelectedPlayerSkin.SkinID);
			EventManager.Dispatch("UpdateMoney");
		}
		else
		{
			int money = SaveLoadManager.GetMoney();
			if (money < (int)SelectedPlayerSkin.SkinPrice)
			{
				UIToast.Show(Localization.Get("Not enough money"));
				return;
			}
			money -= (int)SelectedPlayerSkin.SkinPrice;
			SaveLoadManager.SetMoney(money);
			SaveLoadManager.SetPlayerSkin(SelectedPlayerSkin.SkinID);
			SaveLoadManager.SetPlayerSkinSelected(SelectedPlayerSkin.SkinID);
			EventManager.Dispatch("UpdateMoney");
		}
		SelectSkin(SelectedPlayerSkin.SkinName);
	}

	private List<string> GetPlayerSkinNames()
	{
		List<string> list = new List<string>();
		for (int i = 0; i < GameSettings.instance.PlayerSkinShop.Count; i++)
		{
			list.Add(GameSettings.instance.PlayerSkinShop[i].SkinName);
		}
		return list;
	}

	private PlayerSkinShopData GetPlayerSkinData(string skinName)
	{
		for (int i = 0; i < GameSettings.instance.PlayerSkinShop.Count; i++)
		{
			if (skinName == GameSettings.instance.PlayerSkinShop[i].SkinName)
			{
				return GameSettings.instance.PlayerSkinShop[i];
			}
		}
		return null;
	}

	public void OnNextClick()
	{
		SelectSkinName++;
		if (SelectSkinName >= SkinNames.Count)
		{
			SelectSkinName = 0;
		}
		SelectSkin(SkinNames[SelectSkinName]);
	}

	public void OnPrevClick()
	{
		SelectSkinName--;
		if (SelectSkinName <= -1)
		{
			SelectSkinName = SkinNames.Count - 1;
		}
		SelectSkin(SkinNames[SelectSkinName]);
	}
}
