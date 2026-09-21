using System;
using System.Collections.Generic;
using UnityEngine;

public class mWeaponCamera : MonoBehaviour
{
	[Serializable]
	public class WeaponsClass
	{
		public string WeaponName;

		public GameObject Weapon;
	}

	public Transform WeaponPoint;

	public List<WeaponsClass> Weapons = new List<WeaponsClass>();

	private GameObject ActiveWeapon;

	private void Update()
	{
	}

	public void Active(string weaponName)
	{
		base.gameObject.SetActive(value: true);
		ShowWeapon(weaponName);
	}

	public void Deactive()
	{
		base.gameObject.SetActive(value: false);
		DeactiveAll();
	}

	public void ShowWeapon(string weaponName)
	{
		for (int i = 0; i < Weapons.Count; i++)
		{
			if (weaponName == Weapons[i].WeaponName)
			{
				Weapons[i].Weapon.SetActive(value: true);
				ActiveWeapon = Weapons[i].Weapon;
			}
			else
			{
				Weapons[i].Weapon.SetActive(value: false);
			}
		}
	}

	public void SetSkin(int weaponID, int skin)
	{
		MeshAtlas[] componentsInChildren = ActiveWeapon.GetComponentsInChildren<MeshAtlas>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].spriteName = weaponID + "-" + skin;
		}
	}

	public void DeactiveAll()
	{
		for (int i = 0; i < Weapons.Count; i++)
		{
			Weapons[i].Weapon.SetActive(value: false);
		}
	}
}
