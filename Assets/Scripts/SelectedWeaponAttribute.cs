using UnityEngine;

public class SelectedWeaponAttribute : PropertyAttribute
{
	public WeaponTypeList weaponType;

	public int selected;

	public SelectedWeaponAttribute(WeaponTypeList weapon)
	{
		weaponType = weapon;
	}
}
