using CodeStage.AntiCheat.ObscuredTypes;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SaveLoadManager
{
	public static bool HasPlayerName()
	{
		return ObscuredPrefs.HasKey("PlayerName");
	}

	public static void SetPlayerName(string name)
	{
		ObscuredPrefs.SetString("PlayerName", name);
	}

	public static string GetPlayerName()
	{
		return ObscuredPrefs.GetString("PlayerName", "Player " + Random.Range(0, 9999));
	}

	public static void SetMoney(int money)
	{
		ObscuredPrefs.SetInt("Money", money);
	}

	public static void SetMoney1(int money)
	{
		ObscuredPrefs.SetInt("Money", GetMoney() + money);
	}

	public static int GetMoney()
	{
		int @int = ObscuredPrefs.GetInt("Money", 100);
		return Mathf.Clamp(@int, 0, 999999);
	}

	public static void SetGold(int gold)
	{
		ObscuredPrefs.SetInt("Gold", gold);
	}

	public static void SetGold1(int gold)
	{
		ObscuredPrefs.SetInt("Gold", GetGold() + gold);
	}

	public static int GetGold()
	{
		return ObscuredPrefs.GetInt("Gold", 10);
	}

	public static void SetPlayerXP(int xp)
	{
		ObscuredPrefs.SetInt("PlayerXP", xp);
	}

	public static int GetPlayerXP()
	{
		return ObscuredPrefs.GetInt("PlayerXP", 0);
	}

	public static void SetPlayerLevel(int level)
	{
		ObscuredPrefs.SetInt("PlayerLevel", level);
	}

	public static int GetPlayerLevel()
	{
		return ObscuredPrefs.GetInt("PlayerLevel", 1);
	}

	public static string[] GetFriends()
	{
		if (PlayerPrefs.HasKey("Friends"))
		{
			return PlayerPrefs.GetString("Friends").Split("|"[0]);
		}
		return new string[0];
	}

	public static void SetFriend(string friend)
	{
		List<string> list = GetFriends().ToList();
		list.Add(friend);
		SetFriends(list.ToArray());
	}

	public static void SetFriends(string[] friends)
	{
		string text = string.Empty;
		for (int i = 0; i < friends.Length; i++)
		{
			text = ((friends.Length - 1 == i) ? (text + friends[i]) : (text + friends[i] + "|"));
		}
		PlayerPrefs.SetString("Friends", text);
	}

	public static string GetFriendsLine()
	{
		if (PlayerPrefs.HasKey("Friends"))
		{
			return PlayerPrefs.GetString("Friends");
		}
		return string.Empty;
	}

	public static void SetFriendsLine(string friends)
	{
		PlayerPrefs.SetString("Friends", friends);
	}

	public static void SetOpenCase(int cas)
	{
		ObscuredPrefs.SetInt("OpenCase", cas);
	}

	public static void SetOpenCase1(int cas)
	{
		ObscuredPrefs.SetInt("OpenCase", GetOpenCase() + cas);
	}

	public static int GetOpenCase()
	{
		return ObscuredPrefs.GetInt("OpenCase", 0);
	}

	public static void SetSensitivity(float value)
	{
		PlayerPrefs.SetFloat("Sensitivity", value);
	}

	public static float GetSensitivity()
	{
		return PlayerPrefs.GetFloat("Sensitivity", 0.2f);
	}

	public static void SetSound(bool active)
	{
		PlayerPrefs.SetInt("Sound", active ? 1 : 0);
	}

	public static bool GetSound()
	{
		int @int = PlayerPrefs.GetInt("Sound", 1);
		return @int == 1;
	}

	public static void SetFPSMeter(bool active)
	{
		PlayerPrefs.SetInt("FPSMeter", active ? 1 : 0);
	}

	public static bool GetFPSMeter()
	{
		int @int = PlayerPrefs.GetInt("FPSMeter", 0);
		return @int == 1;
	}

	public static void SetConsole(bool active)
	{
		PlayerPrefs.SetInt("Console", active ? 1 : 0);
	}

	public static bool GetConsole()
	{
		int @int = PlayerPrefs.GetInt("Console", 0);
		return @int == 1;
	}

	public static void SetChat(bool active)
	{
		PlayerPrefs.SetInt("Chat", active ? 1 : 0);
	}

	public static bool GetChat()
	{
		int @int = PlayerPrefs.GetInt("Chat", 1);
		return @int == 1;
	}

	public static void SetShowDamage(bool active)
	{
		PlayerPrefs.SetInt("ShowDamage", active ? 1 : 0);
	}

	public static bool GetShowDamage()
	{
		int @int = PlayerPrefs.GetInt("ShowDamage", 0);
		return @int == 1;
	}

	public static void SetColorCrosshair(int color)
	{
		PlayerPrefs.SetInt("ColorCrosshair", color);
	}

	public static int GetColorCrosshair()
	{
		return PlayerPrefs.GetInt("ColorCrosshair", 0);
	}

	public static void SetRagdoll(bool active)
	{
		PlayerPrefs.SetInt("Ragdoll", active ? 1 : 0);
	}

	public static bool GetRagdoll()
	{
		int @int = PlayerPrefs.GetInt("Ragdoll", 1);
		return @int == 1;
	}

	public static bool GetWeapon(WeaponType weapon)
	{
		return GetWeapon(weapon.WeaponID);
	}

	public static bool GetWeapon(int id)
	{
		return id == 4 || id == 3 || id == 12 || ObscuredPrefs.GetBool("Weapon" + id);
	}

	public static void SetWeapon(WeaponType weapon)
	{
		ObscuredPrefs.SetBool("Weapon" + weapon.WeaponID, value: true);
	}

	public static void SetWeapon(WeaponType weapon, bool active)
	{
		ObscuredPrefs.SetBool("Weapon" + weapon.WeaponID, active);
	}

	public static int GetWeaponSelected(WeaponTypeList weaponType)
	{
		switch (weaponType)
		{
		case WeaponTypeList.Knife:
			return ObscuredPrefs.GetInt("SelectKnife", 4);
		case WeaponTypeList.Pistol:
			return ObscuredPrefs.GetInt("SelectPistol", 3);
		case WeaponTypeList.Rifle:
			return ObscuredPrefs.GetInt("SelectRifle", 12);
		default:
			return 0;
		}
	}

	public static void SetWeaponSelected(WeaponTypeList weaponType, int weaponID)
	{
		switch (weaponType)
		{
		case WeaponTypeList.Knife:
			ObscuredPrefs.SetInt("SelectKnife", weaponID);
			break;
		case WeaponTypeList.Pistol:
			ObscuredPrefs.SetInt("SelectPistol", weaponID);
			break;
		case WeaponTypeList.Rifle:
			ObscuredPrefs.SetInt("SelectRifle", weaponID);
			break;
		}
	}

	public static int GetWeaponUpgrade(WeaponType weapon)
	{
		return ObscuredPrefs.GetInt("WeaponUpgrade" + weapon.WeaponID, 0);
	}

	public static void SetWeaponUpgrade(WeaponType weapon, int upgrade)
	{
		ObscuredPrefs.SetInt("WeaponUpgrade" + weapon.WeaponID, upgrade);
	}

	public static void SetWeaponUpgrade(int id, int upgrade)
	{
		ObscuredPrefs.SetInt("WeaponUpgrade" + id, upgrade);
	}

	public static bool GetWeaponSkin(WeaponType weapon, int skin)
	{
		return GetWeaponSkin(weapon.WeaponID, skin);
	}

	public static bool GetWeaponSkin(int weaponID, int skin)
	{
		return skin == 0 || ObscuredPrefs.GetBool("WeaponSkin" + weaponID + "-" + skin, defaultValue: false);
	}

	public static void SetWeaponSkin(WeaponType weapon, int skin)
	{
		ObscuredPrefs.SetBool("WeaponSkin" + weapon.WeaponID + "-" + skin, value: true);
	}

	public static void SetWeaponSkin(int weaponID, int skin)
	{
		ObscuredPrefs.SetBool("WeaponSkin" + weaponID + "-" + skin, value: true);
	}

	public static int GetWeaponSkinSelected(WeaponType weapon)
	{
		return GetWeaponSkinSelected(weapon.WeaponID);
	}

	public static int GetWeaponSkinSelected(int weaponID)
	{
		return ObscuredPrefs.GetInt("WeaponSkinSelected" + weaponID, 0);
	}

	public static void SetWeaponSkinSelected(WeaponType weapon, int skin)
	{
		ObscuredPrefs.SetInt("WeaponSkinSelected" + weapon.WeaponID, skin);
	}

	public static void SetDeaths(int deaths)
	{
		ObscuredPrefs.SetInt("Deaths", deaths);
	}

	public static void SetDeaths1()
	{
		ObscuredPrefs.SetInt("Deaths", GetDeaths() + 1);
	}

	public static int GetDeaths()
	{
		return ObscuredPrefs.GetInt("Deaths", 0);
	}

	public static void SetKills(int kills)
	{
		ObscuredPrefs.SetInt("Kills", kills);
	}

	public static void SetKills1()
	{
		ObscuredPrefs.SetInt("Kills", GetKills() + 1);
	}

	public static int GetKills()
	{
		return ObscuredPrefs.GetInt("Kills", 0);
	}

	public static void SetHeadshot(int headshot)
	{
		ObscuredPrefs.SetInt("Headshot", headshot);
	}

	public static void SetHeadshot1()
	{
		ObscuredPrefs.SetInt("Headshot", GetHeadshot() + 1);
	}

	public static int GetHeadshot()
	{
		return ObscuredPrefs.GetInt("Headshot", 0);
	}

	public static bool GetPlayerSkin(int skinID)
	{
		return ObscuredPrefs.GetBool("PlayerSkin" + skinID, defaultValue: false);
	}

	public static void SetPlayerSkin(int skinID)
	{
		ObscuredPrefs.SetBool("PlayerSkin" + skinID, value: true);
	}

	public static int GetPlayerSkinSelected()
	{
		return ObscuredPrefs.GetInt("SelectPlayerSkin", 0);
	}

	public static void SetPlayerSkinSelected(int skinID)
	{
		ObscuredPrefs.SetInt("SelectPlayerSkin", skinID);
	}

	public static double GetCaseTime()
	{
		return Mathf.Clamp((float)ObscuredPrefs.GetDouble("CaseTime", 900.0), 0f, 900f);
	}

	public static void SetCaseTime(double time)
	{
		ObscuredPrefs.SetDouble("CaseTime", time);
	}
}
