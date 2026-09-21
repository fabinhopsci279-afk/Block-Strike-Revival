using Boomlagoon.JSON;
using System;
using UnityEngine;

public class mCloudManager : MonoBehaviour
{
	private vp_Timer.Handle Timer = new vp_Timer.Handle();

	private string Data;

	private void Start()
	{
		if (!SaveLoadManager.HasPlayerName())
		{
			NextGameEvent();
		}
		else if (PlayerPrefs.HasKey("SaveLastDay"))
		{
			if (PlayerPrefs.GetInt("SaveLastDay", 1) < DateTime.Now.Day)
			{
				SaveDataGame();
			}
		}
		else
		{
			PlayerPrefs.SetInt("SaveLastDay", DateTime.Now.Day);
		}
	}

	private void GetLastData()
	{
		mPopUp.ShowText(Localization.Get("Please wait") + "...");
		CloudManager.Load(LastLoadComplete, LastLoadError);
	}

	private void LastLoadComplete(string data)
	{
		Data = data;
		mPopUp.ShowPopUp(Localization.Get("Found old save data to load?"), Localization.Get("Cloud"), Localization.Get("No"), LastLoadNo, Localization.Get("Yes"), LastLoadYes);
	}

	private void LastLoadError(string error)
	{
		NextGameEvent();
	}

	private void LastLoadYes()
	{
		LoadComplete(Data);
	}

	private void LastLoadNo()
	{
		NextGameEvent();
	}

	private void NextGameEvent()
	{
		mPopUp.HidePopUp("Menu");
		vp_Timer.In(0.1f, delegate
		{
			EventManager.Dispatch("NextGameEvent2");
		});
	}

	public void LoadDataGame()
	{
		mPopUp.ShowPopUp(Localization.Get("Load Data"), Localization.Get("Cloud"), Localization.Get("No"), LoadDataGameNo, Localization.Get("Yes"), LoadDataGameYes);
	}

	private void LoadDataGameYes()
	{
		mPopUp.ShowText(Localization.Get("Please wait") + "...");
		CloudManager.Load(LoadComplete, LoadFailed);
	}

	private void LoadDataGameNo()
	{
		mPopUp.HidePopUp("Menu");
	}

	private void LoadComplete(string data)
	{
		MonoBehaviour.print("LoadComplete");
		DecryptDataGame(data);
		vp_Timer.In(0.5f, delegate
		{
			UIToast.Show(Localization.Get("Load data successfully"));
			PlayerPrefs.SetInt("SaveLastDay", DateTime.Now.Day);
		}, Timer);
		Timer.CancelOnLoad = false;
		mPopUp.HidePopUp("Menu");
		UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
	}

	private void LoadFailed(string error)
	{
		MonoBehaviour.print("Loading data failed: " + error);
		mPopUp.HidePopUp("Menu");
		UIToast.Show(Localization.Get("Loading data failed") + " " + error);
	}

	public void SaveDataGame()
	{
		mPopUp.ShowPopUp(Localization.Get("Save Data"), Localization.Get("Cloud"), Localization.Get("No"), SaveDataGameNo, Localization.Get("Yes"), SaveDataGameYes);
	}

	private void SaveDataGameYes()
	{
		MonoBehaviour.print("SaveDataGame");
		mPopUp.ShowText(Localization.Get("Please wait") + "...");
		string data = EncryptDataGame();
		CloudManager.Save(data, SaveComplete, SaveFailed);
	}

	private void SaveDataGameNo()
	{
		PlayerPrefs.SetInt("SaveLastDay", DateTime.Now.Day);
		mPopUp.HidePopUp("Menu");
	}

	private void SaveComplete()
	{
		mPopUp.HidePopUp("Menu");
		PlayerPrefs.SetInt("SaveLastDay", DateTime.Now.Day);
		UIToast.Show(Localization.Get("Save data successfully"));
	}

	private void SaveFailed(string error)
	{
		MonoBehaviour.print("Save data failed: " + error);
		mPopUp.HidePopUp("Menu");
		UIToast.Show(Localization.Get("Save data failed") + " " + error);
	}

	public string EncryptDataGame()
	{
		JSONObject jSONObject = new JSONObject();
		JSONObject jSONObject2 = new JSONObject();
		jSONObject2.Add("1", SaveLoadManager.GetPlayerName());
		jSONObject2.Add("2", SaveLoadManager.GetMoney());
		jSONObject2.Add("3", SaveLoadManager.GetGold());
		jSONObject2.Add("4", SaveLoadManager.GetPlayerXP());
		jSONObject2.Add("5", SaveLoadManager.GetPlayerLevel());
		jSONObject2.Add("6", SaveLoadManager.GetFriendsLine());
		jSONObject2.Add("7", SaveLoadManager.GetOpenCase());
		jSONObject2.Add("8", SaveLoadManager.GetDeaths());
		jSONObject2.Add("9", SaveLoadManager.GetKills());
		jSONObject2.Add("10", SaveLoadManager.GetHeadshot());
		jSONObject2.Add("11", SaveLoadManager.GetCaseTime());
		jSONObject2.Add("12", SaveLoadManager.GetWeaponSelected(WeaponTypeList.Knife));
		jSONObject2.Add("13", SaveLoadManager.GetWeaponSelected(WeaponTypeList.Pistol));
		jSONObject2.Add("14", SaveLoadManager.GetWeaponSelected(WeaponTypeList.Rifle));
		JSONArray jSONArray = new JSONArray();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			JSONObject jSONObject3 = new JSONObject();
			WeaponType weaponType = GameSettings.instance.Weapons[i];
			jSONObject3.Add("1", (int)weaponType.WeaponID);
			jSONObject3.Add("2", SaveLoadManager.GetWeapon(weaponType) ? 1 : 0);
			jSONObject3.Add("3", SaveLoadManager.GetWeaponUpgrade(weaponType));
			string text = string.Empty;
			for (int j = 0; j < GameSettings.instance.WeaponsShop[i].Skins.Count; j++)
			{
				WeaponShopData.WeaponSkin weaponSkin = GameSettings.instance.WeaponsShop[i].Skins[j];
				if (SaveLoadManager.GetWeaponSkin(weaponType, weaponSkin.SkinID))
				{
					text = text + weaponSkin.SkinID + "#";
				}
			}
			jSONObject3.Add("4", text);
			jSONObject3.Add("5", SaveLoadManager.GetWeaponSkinSelected(weaponType));
			jSONArray.Add(jSONObject3);
		}
		JSONArray jSONArray2 = new JSONArray();
		for (int k = 0; k < GameSettings.instance.PlayerSkinShop.Count; k++)
		{
			PlayerSkinShopData playerSkinShopData = GameSettings.instance.PlayerSkinShop[k];
			if (SaveLoadManager.GetPlayerSkin(playerSkinShopData.SkinID))
			{
				jSONArray2.Add((int)playerSkinShopData.SkinID);
			}
		}
		jSONObject.Add("1", jSONObject2);
		jSONObject.Add("2", jSONArray);
		jSONObject.Add("3", jSONArray2);
		return Utils.EncryptData(jSONObject.ToString(), "12345678901234567890123456789012");
	}

	public void DecryptDataGame(string data)
	{
		data = Utils.DecryptData(data, "12345678901234567890123456789012");
		MonoBehaviour.print(data);
		PlayerPrefs.DeleteAll();
		JSONObject jSONObject = JSONObject.Parse(data);
		JSONObject @object = jSONObject.GetObject("1");
		SaveLoadManager.SetPlayerName(@object.GetString("1"));
		SaveLoadManager.SetMoney((int)@object.GetNumber("2"));
		SaveLoadManager.SetGold((int)@object.GetNumber("3"));
		SaveLoadManager.SetPlayerXP((int)@object.GetNumber("4"));
		SaveLoadManager.SetPlayerLevel((int)@object.GetNumber("5"));
		SaveLoadManager.SetFriendsLine(@object.GetString("6"));
		SaveLoadManager.SetOpenCase((int)@object.GetNumber("7"));
		SaveLoadManager.SetDeaths((int)@object.GetNumber("8"));
		SaveLoadManager.SetKills((int)@object.GetNumber("9"));
		SaveLoadManager.SetHeadshot((int)@object.GetNumber("10"));
		SaveLoadManager.SetCaseTime(@object.GetNumber("11"));
		SaveLoadManager.SetWeaponSelected(WeaponTypeList.Knife, (int)@object.GetNumber("12"));
		SaveLoadManager.SetWeaponSelected(WeaponTypeList.Pistol, (int)@object.GetNumber("13"));
		SaveLoadManager.SetWeaponSelected(WeaponTypeList.Rifle, (int)@object.GetNumber("14"));
		JSONArray array = jSONObject.GetArray("2");
		for (int i = 0; i < array.Length; i++)
		{
			JSONObject obj = array[i].Obj;
			WeaponType weaponType = WeaponManager.GetWeaponType((int)obj.GetNumber("1"));
			if (obj.GetNumber("2") == 1.0)
			{
				SaveLoadManager.SetWeapon(weaponType);
			}
			int num = (int)obj.GetNumber("3");
			if (num != 0)
			{
				SaveLoadManager.SetWeaponUpgrade(weaponType, num);
			}
			string[] array2 = obj.GetString("4").Split("#"[0]);
			for (int j = 0; j < array2.Length - 1; j++)
			{
				SaveLoadManager.SetWeaponSkin(weaponType, int.Parse(array2[j]));
			}
			int num2 = (int)obj.GetNumber("5");
			if (num2 != 0)
			{
				SaveLoadManager.SetWeaponSkinSelected(weaponType, num2);
			}
		}
		JSONArray array3 = jSONObject.GetArray("3");
		for (int k = 0; k < array3.Length; k++)
		{
			SaveLoadManager.SetPlayerSkin((int)array3[k].Number);
		}
	}
}
