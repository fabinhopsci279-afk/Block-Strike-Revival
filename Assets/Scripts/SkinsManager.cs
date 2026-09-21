using System.Collections.Generic;
using UnityEngine;

public class SkinsManager
{
	public static string GetPlayerSkin(Team team)
	{
		return ((int)team + "-" + SaveLoadManager.GetPlayerSkinSelected()).ToString();
	}

	public static string GetWeaponSkin(int weaponID)
	{
		return weaponID + "-" + SaveLoadManager.GetWeaponSkinSelected(weaponID);
	}

	public static string GetRandomWeaponSkin(int weaponID)
	{
		UIAtlas weaponAtlas = GameSettings.instance.WeaponAtlas;
		List<int> list = new List<int>();
		for (int i = 0; i < weaponAtlas.spriteList.Count; i++)
		{
			string name = weaponAtlas.spriteList[i].name;
			name = name.Substring(0, name.LastIndexOf("-"));
			if (weaponID.ToString() == name)
			{
				list.Add(i);
			}
		}
		if (list.Count == 0)
		{
			return string.Empty;
		}
		return weaponAtlas.spriteList[list[Random.Range(0, list.Count)]].name;
	}
}
