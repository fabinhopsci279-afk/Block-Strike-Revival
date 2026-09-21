using System.Collections.Generic;
using UnityEngine;

public class UISelectWeapon : MonoBehaviour
{
	public UILabel Label;

	private string Text;

	private WeaponTypeList WeaponType;

	private void Start()
	{
		UIEventListener.Get(Label.cachedGameObject).onPress = OnLabelPress;
	}

	public void UpdateLabel(int weapon)
	{
		WeaponType = (WeaponTypeList)weapon;
		List<string> weaponsName = GetWeaponsName(WeaponType);
		Text = string.Join("\n", weaponsName.ToArray());
		Label.text = Text;
		Label.ResizeCollider();
	}

	private void OnLabelPress(GameObject sender, bool pressed)
	{
		if (pressed)
		{
			Vector2 v = UnityEngine.Input.mousePosition;
			Camera camera = NGUITools.FindCameraForLayer(base.gameObject.layer);
			Vector3 worldPos = camera.ScreenToWorldPoint(v);
			string wordAtPosition = Label.GetWordAtPosition(worldPos);
			int weaponID = GetWeaponID(wordAtPosition);
			if (weaponID != -1 && !string.IsNullOrEmpty(wordAtPosition))
			{
				Text = NGUIText.StripSymbols(Text);
				Label.text = Text.Replace(wordAtPosition, "[ff0000]" + wordAtPosition + "[-]");
				SaveLoadManager.SetWeaponSelected(WeaponType, weaponID);
				WeaponManager.SetWeaponType(WeaponType, weaponID);
			}
		}
	}

	private List<string> GetWeaponsName(WeaponTypeList type)
	{
		List<string> list = new List<string>();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].Weapon != type)
			{
				continue;
			}
			int num = GameSettings.instance.Weapons[i].WeaponID;
			string text = GameSettings.instance.Weapons[i].WeaponName;
			if (SaveLoadManager.GetWeapon(num))
			{
				if (SaveLoadManager.GetWeaponSelected(type) == num)
				{
					text = "[ff0000]" + text + "[-]";
				}
				switch (num)
				{
				case 3:
				case 4:
				case 12:
					list.Insert(0, text);
					break;
				default:
					list.Add(text);
					break;
				case 17:
				case 20:
					break;
				}
			}
		}
		return list;
	}

	private int GetWeaponID(string weaponName)
	{
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if (GameSettings.instance.Weapons[i].WeaponName == weaponName)
			{
				return GameSettings.instance.Weapons[i].WeaponID;
			}
		}
		return -1;
	}
}
