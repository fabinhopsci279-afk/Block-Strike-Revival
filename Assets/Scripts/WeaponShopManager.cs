using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShopManager : MonoBehaviour
{
	[Header("Upgrade")]
	public UISprite UpgradeSprite;

	public UILabel UpgradeLabel;

	public UISprite UpgradeMoney;

	[Header("Buy")]
	public UISprite BuySprite;

	public UILabel BuyLabel;

	public UISprite BuyMoney;

	[Header("Skins")]
	public UISprite SkinSprite;

	public UILabel SkinNameLabel;

	public UILabel SkinQualityLabel;

	public UILabel SkinSelectLabel;

	public GameObject SkinOnlyCaseLabel;

	public UILabel SkinPriceLabel;

	private int SelectSkin;

	private List<int> SkinsWeaponList = new List<int>();

	[Header("Weapon Info")]
	public UILabel WeaponNameLabel;

	public UILabel DamageLabel;

	public UILabel FireRateLabel;

	public UILabel AccuracyLabel;

	public UILabel AmmoLabel;

	public UILabel MaxAmmoLabel;

	public UILabel MobilityLabel;

	private List<string> WeaponNames = new List<string>();

	private int SelectWeapon;

	[Header("Others")]
	public mWeaponCamera WeaponCamera;

	private WeaponType WeaponData;

	private WeaponShopData WeaponStoreData;

	private ObscuredInt WeaponUpgrade;

	private ObscuredBool HasWeapon = false;

	public void OpenPanel(int type)
	{
		mPanelManager.ShowPanel("WeaponsShop", activePanelUp: true, activeInApp: false);
		SelectWeapon = 0;
		WeaponNames.Clear();
		WeaponNames = GetWeaponsName((WeaponTypeList)type);
		WeaponCamera.Active(WeaponNames[SelectWeapon]);
		UpdateSelectWeapon(WeaponNames[SelectWeapon]);
	}

	public void ClosePanel()
	{
		WeaponCamera.Deactive();
	}

	private void OnDisconnectedFromPhoton()
	{
		ClosePanel();
	}

	private void UpdateSelectWeapon(string weaponName)
	{
		WeaponData = GetWeaponType(weaponName);
		WeaponStoreData = GetWeaponShopData(WeaponData.WeaponID);
		WeaponUpgrade = SaveLoadManager.GetWeaponUpgrade(WeaponData);
		WeaponCamera.ShowWeapon(WeaponData.WeaponName);
		HasWeapon = SaveLoadManager.GetWeapon(WeaponData);
		if ((int)WeaponStoreData.WeaponPrice == 0)
		{
			HasWeapon = true;
			SaveLoadManager.SetWeapon(WeaponData);
		}
		if ((bool)HasWeapon)
		{
			SelectPanel();
		}
		else
		{
			HasWeaponPanel();
		}
		UpdateWeaponInfo();
		UpdateSkinList();
		UpdateSkin(GetSelectSkinID());
	}

	private void HasWeaponPanel()
	{
		ResetPanel();
		BuyLabel.transform.localPosition = Vector3.right * 7f;
		BuyLabel.text = WeaponStoreData.WeaponPrice.ToString();
		BuyMoney.spriteName = WeaponStoreData.Money.ToString();
		BuyMoney.UpdateAnchors();
		UpgradeSprite.alpha = 0f;
	}

	private void SelectPanel()
	{
		ResetPanel();
		if ((int)WeaponData.WeaponID == SaveLoadManager.GetWeaponSelected(WeaponData.Weapon))
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
		if (WeaponStoreData.Upgrades.Count == (int)WeaponUpgrade)
		{
			UpgradeSprite.alpha = 0f;
			return;
		}
		UpgradeLabel.text = WeaponStoreData.Upgrades[WeaponUpgrade].UpgradePrice.ToString();
		UpgradeMoney.spriteName = WeaponStoreData.Upgrades[WeaponUpgrade].Money.ToString();
		UpgradeMoney.UpdateAnchors();
	}

	private void UpdateWeaponInfo()
	{
		WeaponNameLabel.text = WeaponData.WeaponName;
		DamageLabel.text = Localization.Get("Damage") + ": " + GetDamage();
		FireRateLabel.text = Localization.Get("FireRate") + ": " + GetFireRate();
		AccuracyLabel.text = Localization.Get("Accuracy") + ": " + GetAccuracy();
		AmmoLabel.text = Localization.Get("Ammo") + ": " + GetAmmo();
		MaxAmmoLabel.text = Localization.Get("MaxAmmo") + ": " + GetMaxAmmo();
		MobilityLabel.text = Localization.Get("Mobility") + ": " + GetMobility();
	}

	private void ResetPanel()
	{
		BuyMoney.alpha = 1f;
		UpgradeMoney.alpha = 1f;
		UpgradeSprite.alpha = 1f;
	}

	public void OnNextClick()
	{
		SelectWeapon++;
		if (SelectWeapon >= WeaponNames.Count)
		{
			SelectWeapon = 0;
		}
		UpdateSelectWeapon(WeaponNames[SelectWeapon]);
	}

	public void OnPrevClick()
	{
		SelectWeapon--;
		if (SelectWeapon <= -1)
		{
			SelectWeapon = WeaponNames.Count - 1;
		}
		UpdateSelectWeapon(WeaponNames[SelectWeapon]);
	}

	public void OnUpgradeClick()
	{
		if (WeaponStoreData.Upgrades.Count == (int)WeaponUpgrade)
		{
			return;
		}
		WeaponShopData.WeaponUpgrade weaponUpgrade = WeaponStoreData.Upgrades[WeaponUpgrade];
		if (weaponUpgrade.Money == MoneyType.Gold)
		{
			int gold = SaveLoadManager.GetGold();
			if (gold < (int)weaponUpgrade.UpgradePrice)
			{
				UIToast.Show(Localization.Get("Not enough money"));
				return;
			}
			gold -= (int)weaponUpgrade.UpgradePrice;
			SaveLoadManager.SetGold(gold);
			EventManager.Dispatch("UpdateMoney");
		}
		else
		{
			int money = SaveLoadManager.GetMoney();
			if (money < (int)weaponUpgrade.UpgradePrice)
			{
				UIToast.Show(Localization.Get("Not enough money"));
				return;
			}
			money -= (int)weaponUpgrade.UpgradePrice;
			SaveLoadManager.SetMoney(money);
			EventManager.Dispatch("UpdateMoney");
		}
		WeaponUpgrade = ++WeaponUpgrade;
		SaveLoadManager.SetWeaponUpgrade(WeaponData, WeaponUpgrade);
		UpdateSelectWeapon(WeaponData.WeaponName);
	}

	public void OnBuyClick()
	{
		if ((int)WeaponData.WeaponID == SaveLoadManager.GetWeaponSelected(WeaponData.Weapon))
		{
			return;
		}
		if ((bool)HasWeapon)
		{
			SaveLoadManager.SetWeaponSelected(WeaponData.Weapon, WeaponData.WeaponID);
		}
		else
		{
			if (WeaponStoreData.Money == MoneyType.Gold)
			{
				int gold = SaveLoadManager.GetGold();
				if (gold < (int)WeaponStoreData.WeaponPrice)
				{
					UIToast.Show(Localization.Get("Not enough money"));
					return;
				}
				gold -= (int)WeaponStoreData.WeaponPrice;
				SaveLoadManager.SetGold(gold);
				SaveLoadManager.SetWeapon(WeaponData);
				SaveLoadManager.SetWeaponSelected(WeaponData.Weapon, WeaponData.WeaponID);
				EventManager.Dispatch("UpdateMoney");
			}
			else
			{
				int money = SaveLoadManager.GetMoney();
				if (money < (int)WeaponStoreData.WeaponPrice)
				{
					UIToast.Show(Localization.Get("Not enough money"));
					return;
				}
				money -= (int)WeaponStoreData.WeaponPrice;
				SaveLoadManager.SetMoney(money);
				SaveLoadManager.SetWeapon(WeaponData);
				SaveLoadManager.SetWeaponSelected(WeaponData.Weapon, WeaponData.WeaponID);
				EventManager.Dispatch("UpdateMoney");
			}
			SaveLoadManager.SetWeapon(WeaponData);
			SaveLoadManager.SetWeaponSelected(WeaponData.Weapon, WeaponData.WeaponID);
		}
		UpdateSelectWeapon(WeaponData.WeaponName);
		WeaponManager.UpdateData();
	}

	public static WeaponShopData GetWeaponShopData(int weaponID)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((int)GameSettings.instance.Weapons[i].WeaponID == weaponID)
			{
				return GameSettings.instance.WeaponsShop[i];
			}
		}
		UnityEngine.Debug.LogError("Weapon not find");
		return null;
	}

	public static WeaponType GetWeaponType(string weaponName)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].WeaponName == weaponName)
			{
				return GameSettings.instance.Weapons[i];
			}
		}
		UnityEngine.Debug.LogError("Weapon not find");
		return null;
	}

	public static List<string> GetWeaponsName(WeaponTypeList type)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].Weapon == type)
			{
				switch ((int)GameSettings.instance.Weapons[i].WeaponID)
				{
				case 3:
				case 4:
				case 12:
					list.Insert(0, GameSettings.instance.Weapons[i].WeaponName);
					break;
				default:
					list.Add(GameSettings.instance.Weapons[i].WeaponName);
					break;
				case 17:
				case 20:
					break;
				}
			}
		}
		return list;
	}

	public string GetDamage()
	{
		int num = Mathf.FloorToInt(((int)WeaponData.FaceDamage * (int)WeaponData.FireBullets + (int)WeaponData.BodyDamage * (int)WeaponData.FireBullets + (int)WeaponData.HandDamage * (int)WeaponData.FireBullets + (int)WeaponData.LegDamage * (int)WeaponData.FireBullets) / 4);
		if ((int)WeaponUpgrade != 0)
		{
			WeaponShopData.WeaponUpgrade weaponUpgrade = WeaponStoreData.Upgrades[(int)WeaponUpgrade - 1];
			int num2 = Mathf.FloorToInt(((int)weaponUpgrade.FaceDamage * (int)WeaponData.FireBullets + (int)weaponUpgrade.BodyDamage * (int)WeaponData.FireBullets + (int)weaponUpgrade.HandDamage * (int)WeaponData.FireBullets + (int)weaponUpgrade.LegDamage * (int)WeaponData.FireBullets) / 4);
			num2 -= num;
			if (num2 != 0)
			{
				return num + " [00Ff22] + " + num2;
			}
		}
		if (!WeaponData.WeaponFire)
		{
			return "-";
		}
		return num.ToString();
	}

	public string GetFireRate()
	{
		if (WeaponData.Weapon == WeaponTypeList.Knife || !WeaponData.WeaponFire)
		{
			return "-";
		}
		int num = Mathf.FloorToInt(100f - (float)WeaponData.FireRate * 100f / 1.5f);
		if ((int)WeaponUpgrade != 0)
		{
			WeaponShopData.WeaponUpgrade weaponUpgrade = WeaponStoreData.Upgrades[(int)WeaponUpgrade - 1];
			int num2 = Mathf.FloorToInt(100f - (float)weaponUpgrade.FireRate * 100f / 1.5f);
			num2 -= num;
			if (num2 != 0)
			{
				return num + "% [00Ff22] + " + num2 + "%";
			}
		}
		return num + "%";
	}

	public string GetAccuracy()
	{
		if (WeaponData.Weapon == WeaponTypeList.Knife || !WeaponData.WeaponFire)
		{
			return "-";
		}
		int num = Mathf.FloorToInt(100f - (float)WeaponData.FireAccuracy - (float)WeaponData.Accuracy);
		if ((int)WeaponUpgrade != 0)
		{
			WeaponShopData.WeaponUpgrade weaponUpgrade = WeaponStoreData.Upgrades[(int)WeaponUpgrade - 1];
			int num2 = Mathf.FloorToInt(100f - (float)weaponUpgrade.FireAccuracy - (float)weaponUpgrade.Accuracy);
			num2 -= num;
			if (num2 != 0)
			{
				return num + "% [00Ff22] + " + num2 + "%";
			}
		}
		return num + "%";
	}

	public string GetAmmo()
	{
		if (WeaponData.Weapon == WeaponTypeList.Knife || !WeaponData.WeaponFire)
		{
			return "-";
		}
		int num = WeaponData.Ammo;
		if ((int)WeaponUpgrade != 0)
		{
			WeaponShopData.WeaponUpgrade weaponUpgrade = WeaponStoreData.Upgrades[(int)WeaponUpgrade - 1];
			int num2 = weaponUpgrade.Ammo;
			num2 -= num;
			if (num2 != 0)
			{
				return num + " [00Ff22] + " + num2;
			}
		}
		return num.ToString();
	}

	public string GetMaxAmmo()
	{
		if (WeaponData.Weapon == WeaponTypeList.Knife || !WeaponData.WeaponFire)
		{
			return "-";
		}
		int num = WeaponData.MaxAmmo;
		if ((int)WeaponUpgrade != 0)
		{
			WeaponShopData.WeaponUpgrade weaponUpgrade = WeaponStoreData.Upgrades[(int)WeaponUpgrade - 1];
			int num2 = weaponUpgrade.MaxAmmo;
			num2 -= num;
			if (num2 != 0)
			{
				return num + " [00Ff22] + " + num2;
			}
		}
		return num.ToString();
	}

	public string GetMobility()
	{
		int num = Mathf.FloorToInt(100f - (float)WeaponData.Mass * 1000f);
		if ((int)WeaponUpgrade != 0)
		{
			WeaponShopData.WeaponUpgrade weaponUpgrade = WeaponStoreData.Upgrades[(int)WeaponUpgrade - 1];
			int num2 = Mathf.FloorToInt(100f - (float)weaponUpgrade.Mass * 1000f);
			num2 -= num;
			if (num2 != 0)
			{
				return num + "% [00Ff22] + " + num2 + "%";
			}
		}
		if (!WeaponData.WeaponFire)
		{
			return "-";
		}
		return num + "%";
	}

	public void UpdateSkin(int selectSkin)
	{
		SelectSkin = selectSkin;
		SkinSprite.alpha = 1f;
		int num = SkinsWeaponList[SelectSkin];
		WeaponShopData.WeaponSkin weaponSkin = WeaponStoreData.Skins[num];
		switch ((int)weaponSkin.Rarity)
		{
		case 0:
			SkinNameLabel.text = weaponSkin.SkinName;
			SkinQualityLabel.text = Localization.Get("Normal quality");
			break;
		case 1:
			SkinNameLabel.text = weaponSkin.SkinName;
			SkinQualityLabel.text = Localization.Get("Normal quality");
			break;
		case 2:
			SkinNameLabel.text = "[00aff0]" + weaponSkin.SkinName;
			SkinQualityLabel.text = "[00aff0]" + Localization.Get("Basic quality");
			break;
		case 3:
			SkinNameLabel.text = "[ff0000]" + weaponSkin.SkinName;
			SkinQualityLabel.text = "[ff0000]" + Localization.Get("Professional quality");
			break;
		case 4:
			SkinNameLabel.text = "[E00061]" + weaponSkin.SkinName;
			SkinQualityLabel.text = "[E00061]" + Localization.Get("Legendary quality");
			break;
		}
		WeaponCamera.SetSkin(WeaponData.WeaponID, weaponSkin.SkinID);
		SkinOnlyCaseLabel.SetActive(value: false);
		SkinPriceLabel.gameObject.SetActive(value: false);
		if (SaveLoadManager.GetWeaponSkin(WeaponData, num))
		{
			if (num == SaveLoadManager.GetWeaponSkinSelected(WeaponData))
			{
				SkinSelectLabel.text = Localization.Get("Selected");
			}
			else
			{
				SkinSelectLabel.text = Localization.Get("Select");
			}
		}
		else if ((int)weaponSkin.Price != 0)
		{
			SkinPriceLabel.gameObject.SetActive(value: true);
			SkinPriceLabel.text = weaponSkin.Price.ToString();
			SkinSelectLabel.text = Localization.Get("Buy");
		}
		else
		{
			SkinOnlyCaseLabel.SetActive(value: true);
			SkinSprite.alpha = 0.5f;
			SkinSelectLabel.text = Localization.Get("Select");
		}
	}

	public void OnLastSkinClick()
	{
		SelectSkin--;
		if (SelectSkin < 0)
		{
			SelectSkin = SkinsWeaponList.Count - 1;
		}
		UpdateSkin(SelectSkin);
	}

	public void OnNextSkinClick()
	{
		SelectSkin++;
		if (SelectSkin >= SkinsWeaponList.Count)
		{
			SelectSkin = 0;
		}
		UpdateSkin(SelectSkin);
	}

	public void OnSelectSkinClick()
	{
		int num = SkinsWeaponList[SelectSkin];
		WeaponShopData.WeaponSkin weaponSkin = WeaponStoreData.Skins[num];
		if (num == SaveLoadManager.GetWeaponSkinSelected(WeaponData))
		{
			return;
		}
		if (!SaveLoadManager.GetWeaponSkin(WeaponData, num))
		{
			if ((int)weaponSkin.Price == 0)
			{
				return;
			}
			int gold = SaveLoadManager.GetGold();
			if (gold < (int)weaponSkin.Price)
			{
				UIToast.Show(Localization.Get("Not enough money"));
				return;
			}
			gold -= (int)weaponSkin.Price;
			SaveLoadManager.SetGold(gold);
			SaveLoadManager.SetWeaponSkin(WeaponData, num);
			EventManager.Dispatch("UpdateMoney");
		}
		SaveLoadManager.SetWeaponSkinSelected(WeaponData, num);
		UpdateSkin(SelectSkin);
	}

	private void UpdateSkinList()
	{
		SelectSkin = 0;
		List<int> list = new List<int>();
		for (int i = 0; i < WeaponStoreData.Skins.Count; i++)
		{
			if ((int)WeaponStoreData.Skins[i].Rarity == 0)
			{
				list.Add(i);
			}
			else
			{
				list.Add(i);
			}
		}
		SkinsWeaponList = list;
	}

	private int GetSelectSkinID()
	{
		int weaponSkinSelected = SaveLoadManager.GetWeaponSkinSelected(WeaponData);
		for (int i = 0; i < SkinsWeaponList.Count; i++)
		{
			if (weaponSkinSelected == SkinsWeaponList[i])
			{
				return i;
			}
		}
		return 0;
	}
}
