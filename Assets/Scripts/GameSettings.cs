using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameSettings : ScriptableObject
{
	public string PhotonID;

	public GameObject PlayerController;

	public GameObject PlayerSkin;

	public float PlayerDefaultMove = 0.18f;

	public List<WeaponType> Weapons = new List<WeaponType>();

	public List<WeaponShopData> WeaponsShop = new List<WeaponShopData>();

	public List<PlayerSkinShopData> PlayerSkinShop = new List<PlayerSkinShopData>();

	public UIAtlas WeaponAtlas;

	public AudioClip ConnectDeveloperAudio;

	private static GameSettings Instance;

	public static GameSettings instance
	{
		get
		{
			if (Instance == null)
			{
				Instance = (Resources.Load("Others/GameSettings") as GameSettings);
			}
			return Instance;
		}
	}
}
