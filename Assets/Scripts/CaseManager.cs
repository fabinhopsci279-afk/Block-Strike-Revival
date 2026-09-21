using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CaseManager : MonoBehaviour
{
	public enum PrizeList
	{
		Money,
		Gold,
		Weapon
	}

	[Header("Case Settings")]
	public GameObject CaseCamera;

	public Transform CaseModel;

	public Transform CaseCap;

	public Transform MoneyModel;

	public Material MoneyMaterial;

	public Material GoldMaterial;

	[Header("UI Settings")]
	public UILabel ButtonLabel;

	public UILabel PrizeLabel;

	public GameObject BackButton;

	public UILabel InfoLabel;

	public UILabel PrizeInfoLabel;

	public UILabel SkinAvailableLabel;

	[Header("Others")]
	public mWeaponCamera WeaponCamera;

	public int PrizeSelect;

	private PrizeList Prize;

	private WeaponType WeaponData;

	private WeaponShopData.WeaponSkin SkinWeapon;

	private Vector3 StartRotation;

	private void Start()
	{
		StartRotation = CaseModel.localEulerAngles;
		if (SaveLoadManager.GetOpenCase() == 0)
		{
			CaseTimeManager.isFinished = true;
		}
	}

	public void Active()
	{
		mPanelManager.ShowPanel("Case", activePanelUp: false, activeInApp: false);
		CaseModel.localScale = Vector3.one * 1.7f;
		CaseCap.localEulerAngles = Vector3.zero;
		MoneyModel.localScale = Vector3.zero;
		PrizeLabel.alpha = 0f;
		PrizeInfoLabel.alpha = 0f;
		SkinAvailableLabel.alpha = 0f;
		if (CaseTimeManager.isFinished)
		{
			InfoLabel.gameObject.SetActive(value: false);
			ButtonLabel.gameObject.SetActive(value: false);
			OnOpenCase();
		}
		else
		{
			InfoLabel.gameObject.SetActive(value: true);
			ButtonLabel.gameObject.SetActive(value: true);
			InfoLabel.text = Localization.Get("Сase will be available through") + " " + CaseTimeManager.GetCaseTime() + " " + Localization.Get("of playing time");
		}
		CaseCamera.SetActive(value: true);
		WeaponCamera.WeaponPoint.localEulerAngles = Vector3.up * 25f;
	}

	public void Deactive()
	{
		CaseCamera.SetActive(value: false);
		WeaponCamera.DeactiveAll();
	}

	private void OnDisconnectedFromPhoton()
	{
		Deactive();
	}

	private void Update()
	{
		CaseModel.localEulerAngles = StartRotation + Vector3.up * Mathf.Cos(Time.time * 3f) * 3f;
	}

	private void GetCasePrize()
	{
		if (SaveLoadManager.GetOpenCase() == 1)
		{
			GetWeaponSkin(2);
			return;
		}
		if (SaveLoadManager.GetOpenCase() == 2)
		{
			GetWeaponSkin(1);
			return;
		}
		if (UnityEngine.Random.value > 0.4f)
		{
			if (UnityEngine.Random.value > 0.4f)
			{
				Prize = PrizeList.Money;
			}
			else
			{
				Prize = PrizeList.Gold;
			}
			return;
		}
		float num = UnityEngine.Random.value * 100f;
		if (num <= 1f)
		{
			GetWeaponSkin(4);
		}
		else if (num <= 8f)
		{
			GetWeaponSkin(3);
		}
		else if (num < 25f)
		{
			GetWeaponSkin(2);
		}
		else
		{
			GetWeaponSkin(1);
		}
	}

	private void GetWeaponSkin(int rarity)
	{
		Prize = PrizeList.Weapon;
		int randomWeapon = GetRandomWeapon(rarity);
		WeaponData = GameSettings.instance.Weapons[randomWeapon];
		List<WeaponShopData.WeaponSkin> list = new List<WeaponShopData.WeaponSkin>();
		for (int i = 1; i < GameSettings.instance.WeaponsShop[randomWeapon].Skins.Count; i++)
		{
			if ((int)GameSettings.instance.WeaponsShop[randomWeapon].Skins[i].Rarity == rarity && (int)GameSettings.instance.WeaponsShop[randomWeapon].Skins[i].Price == 0)
			{
				list.Add(GameSettings.instance.WeaponsShop[randomWeapon].Skins[i]);
			}
		}
		SkinWeapon = list[Random.Range(0, list.Count)];
	}

	private int GetRandomWeapon(int rarity)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			int num = GameSettings.instance.Weapons[i].WeaponID;
			if (num == 17 || num == 20)
			{
				continue;
			}
			for (int j = 0; j < GameSettings.instance.WeaponsShop[i].Skins.Count; j++)
			{
				if ((int)GameSettings.instance.WeaponsShop[i].Skins[j].Rarity == rarity)
				{
					list.Add(i);
					break;
				}
			}
		}
		return list[Random.Range(0, list.Count)];
	}

	public void OnWatchVideo()
	{
		Deactive();
		mPopUp.ShowText(Localization.Get("Please wait") + "...");
		vp_Timer.In(0.5f, delegate
		{
			RewardedVideoComplete();
		});
	}

	private void RewardedVideoComplete()
	{
		vp_Timer.In(0.3f, delegate
		{
			mPopUp.HidePopUp(string.Empty);
			mPanelManager.ShowPanel("Case", activePanelUp: false);
			CaseTimeManager.isFinished = true;
			Active();
		});
	}

	private void RewardedVideoAborted()
	{
		vp_Timer.In(0.3f, delegate
		{
			Active();
			mPopUp.HidePopUp(string.Empty);
			mPanelManager.ShowPanel("Case", activePanelUp: false);
		});
	}

	private void RewardedVideoFailed()
	{
		UIToast.Show(Localization.Get("Video not available"));
		Active();
		mPopUp.HidePopUp(string.Empty);
		mPanelManager.ShowPanel("Case", activePanelUp: false);
	}

	private void OnOpenCase()
	{
		SaveLoadManager.SetOpenCase1(1);
		ButtonLabel.gameObject.SetActive(value: false);
		BackButton.SetActive(value: false);
		InfoLabel.gameObject.SetActive(value: false);
		GetCasePrize();
		CaseTimeManager.UpdateTimeCase();
		vp_Timer.In(1f, delegate
		{
			ShowPrize();
		});
	}

	private void ShowPrize()
	{
		CaseCap.DOLocalRotate(new Vector3(0f, 90f, 0f), 1f, RotateMode.LocalAxisAdd);
		CaseModel.DOScale(Vector3.zero, 1f);
		if (Prize == PrizeList.Weapon)
		{
			ShowWeapon();
		}
		else
		{
			ShowMoney();
		}
	}

	private void ShowWeapon()
	{
		for (int i = 0; i < WeaponCamera.Weapons.Count; i++)
		{
			if (WeaponData.WeaponName == WeaponCamera.Weapons[i].WeaponName)
			{
				WeaponCamera.Weapons[i].Weapon.SetActive(value: true);
				MeshAtlas[] componentsInChildren = WeaponCamera.Weapons[i].Weapon.GetComponentsInChildren<MeshAtlas>();
				for (int j = 0; j < componentsInChildren.Length; j++)
				{
					componentsInChildren[j].spriteName = WeaponData.WeaponID + "-" + SkinWeapon.SkinID;
				}
			}
			else
			{
				WeaponCamera.Weapons[i].Weapon.SetActive(value: false);
			}
		}
		WeaponCamera.WeaponPoint.localScale = Vector3.zero;
		WeaponCamera.WeaponPoint.DOScale(Vector3.one, 1f).OnComplete(OnCompleteTween);
	}

	private void ShowMoney()
	{
		if (Prize == PrizeList.Money)
		{
			MoneyModel.GetComponent<Renderer>().material = MoneyMaterial;
		}
		else
		{
			MoneyModel.GetComponent<Renderer>().material = GoldMaterial;
		}
		MoneyModel.DOScale(Vector3.one * 0.2f, 1f).OnComplete(OnCompleteTween);
	}

	private void OnCompleteTween()
	{
		if (Prize == PrizeList.Money)
		{
			PrizeLabel.text = "+55 " + Localization.Get("Money");
			SaveLoadManager.SetMoney1(55);
		}
		else if (Prize == PrizeList.Gold)
		{
			PrizeLabel.text = "+2 " + Localization.Get("Gold");
			SaveLoadManager.SetGold1(2);
		}
		else
		{
			TweenAlpha.Begin(PrizeInfoLabel.cachedGameObject, 0.5f, 1f);
			PrizeLabel.text = WeaponData.WeaponName + " " + SkinWeapon.SkinName;
			if (SaveLoadManager.GetWeaponSkin(WeaponData, SkinWeapon.SkinID))
			{
				TweenAlpha.Begin(SkinAvailableLabel.cachedGameObject, 0.5f, 1f);
			}
			SaveLoadManager.SetWeaponSkin(WeaponData, SkinWeapon.SkinID);
			switch ((int)SkinWeapon.Rarity)
			{
			case 1:
				PrizeInfoLabel.text = Localization.Get("Normal quality");
				PrizeLabel.text = SkinWeapon.SkinName;
				break;
			case 2:
				PrizeInfoLabel.text = "[00aff0]" + Localization.Get("Basic quality");
				PrizeLabel.text = "[00aff0]" + SkinWeapon.SkinName;
				break;
			case 3:
				PrizeInfoLabel.text = "[ff0000]" + Localization.Get("Professional quality");
				PrizeLabel.text = "[ff0000]" + SkinWeapon.SkinName;
				break;
			case 4:
				PrizeInfoLabel.text = "[E00061]" + Localization.Get("Legendary quality");
				PrizeLabel.text = "[E00061]" + SkinWeapon.SkinName;
				break;
			}
		}
		EventManager.Dispatch("UpdateMoney");
		TweenAlpha.Begin(PrizeLabel.cachedGameObject, 0.5f, 1f);
		vp_Timer.In(1f, delegate
		{
			BackButton.SetActive(value: true);
		});
	}
}
