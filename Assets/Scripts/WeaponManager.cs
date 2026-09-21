using UnityEngine;

public class WeaponManager : MonoBehaviour
{
	public bool isUpgradeWeapon = true;

	[SelectedWeapon(WeaponTypeList.Knife)]
	public int SelectedKnife;

	[SelectedWeapon(WeaponTypeList.Pistol)]
	public int SelectedPistol;

	[SelectedWeapon(WeaponTypeList.Rifle)]
	public int SelectedRifle;

	public static WeaponTypeList DefaultWeaponType = WeaponTypeList.Rifle;

	public static bool SelectWeaponInGame = true;

	private static WeaponManager instance;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
			Object.DontDestroyOnLoad(base.gameObject);
		}
	}

	public static void Init()
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject("WeaponManager");
			gameObject.AddComponent<WeaponManager>();
		}
		UpdateData();
	}

	public static void UpdateData()
	{
		SelectWeaponInGame = true;
		instance.SelectedKnife = SaveLoadManager.GetWeaponSelected(WeaponTypeList.Knife);
		instance.SelectedPistol = SaveLoadManager.GetWeaponSelected(WeaponTypeList.Pistol);
		instance.SelectedRifle = SaveLoadManager.GetWeaponSelected(WeaponTypeList.Rifle);
	}

	public static WeaponType GetKnifeType()
	{
		return GetWeaponType(instance.SelectedKnife);
	}

	public static WeaponType GetPistolType()
	{
		return GetWeaponType(instance.SelectedPistol);
	}

	public static WeaponType GetRifleType()
	{
		return GetWeaponType(instance.SelectedRifle);
	}

	public static void SetKnifeType(int weaponID)
	{
		instance.SelectedKnife = weaponID;
	}

	public static void SetPistolType(int weaponID)
	{
		instance.SelectedPistol = weaponID;
	}

	public static void SetRifleType(int weaponID)
	{
		instance.SelectedRifle = weaponID;
	}

	public static void SetWeaponType(WeaponTypeList weapon, int weaponID)
	{
		if (SelectWeaponInGame)
		{
			switch (weapon)
			{
			case WeaponTypeList.Knife:
				SetKnifeType(weaponID);
				break;
			case WeaponTypeList.Pistol:
				SetPistolType(weaponID);
				break;
			case WeaponTypeList.Rifle:
				SetRifleType(weaponID);
				break;
			}
		}
	}

	public static bool HasKnifeType()
	{
		return instance.SelectedKnife != 0;
	}

	public static bool HasPistolType()
	{
		return instance.SelectedPistol != 0;
	}

	public static bool HasRifleType()
	{
		return instance.SelectedRifle != 0;
	}

	public static WeaponType GetWeaponType(int weaponID)
	{
		if (instance.isUpgradeWeapon)
		{
			return GetWeaponTypeUpgrade(weaponID);
		}
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((int)GameSettings.instance.Weapons[i].WeaponID == weaponID)
			{
				return WeaponType.CreateNewClass(GameSettings.instance.Weapons[i]);
			}
		}
		UnityEngine.Debug.LogError("Weapon not find");
		return null;
	}

	public static WeaponType GetWeaponTypeUpgrade(int weaponID)
	{
		WeaponType weaponType = new WeaponType();
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			if ((int)GameSettings.instance.Weapons[i].WeaponID == weaponID)
			{
				weaponType = WeaponType.CreateNewClass(GameSettings.instance.Weapons[i]);
				weaponID = i;
				break;
			}
		}
		int weaponUpgrade = SaveLoadManager.GetWeaponUpgrade(weaponType);
		if (weaponUpgrade != 0)
		{
			WeaponShopData.WeaponUpgrade weaponUpgrade2 = GameSettings.instance.WeaponsShop[weaponID].Upgrades[weaponUpgrade - 1];
			weaponType.FaceDamage = weaponUpgrade2.FaceDamage;
			weaponType.BodyDamage = weaponUpgrade2.BodyDamage;
			weaponType.HandDamage = weaponUpgrade2.HandDamage;
			weaponType.LegDamage = weaponUpgrade2.LegDamage;
			weaponType.FireRate = weaponUpgrade2.FireRate;
			weaponType.Accuracy = weaponUpgrade2.Accuracy;
			weaponType.FireAccuracy = weaponUpgrade2.FireAccuracy;
			weaponType.Ammo = weaponUpgrade2.Ammo;
			weaponType.MaxAmmo = weaponUpgrade2.MaxAmmo;
			weaponType.Mass = weaponUpgrade2.Mass;
		}
		return weaponType;
	}

	public static int GetMemberDamage(PlayerSkinMember member, int weaponID)
	{
		return GetMemberDamage(member, GetWeaponType(weaponID));
	}

	public static int GetMemberDamage(PlayerSkinMember member, WeaponType weapon)
	{
		switch (member)
		{
		case PlayerSkinMember.Face:
			if ((int)weapon.FaceDamage == 100)
			{
				return 100;
			}
			return (int)weapon.FaceDamage + UnityEngine.Random.Range(-5, 5);
		case PlayerSkinMember.Body:
			return (int)weapon.BodyDamage + UnityEngine.Random.Range(-4, 4);
		case PlayerSkinMember.Hands:
			return (int)weapon.HandDamage + UnityEngine.Random.Range(-3, 3);
		case PlayerSkinMember.Legs:
			return (int)weapon.LegDamage + UnityEngine.Random.Range(-2, 2);
		default:
			return weapon.BodyDamage;
		}
	}

	public static void SetUpgradeWeapon(bool active)
	{
		instance.isUpgradeWeapon = active;
	}
}
