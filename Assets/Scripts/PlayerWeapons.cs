using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
	[Serializable]
	public class WeaponData
	{
		public bool Enabled;

		public ObscuredInt WeaponID;

		public ObscuredBool WeaponFire;

		public ObscuredString WeaponName;

		public FPWeaponShooter WeaponScript;

		public ObscuredInt Damage;

		public ObscuredFloat FireRate;

		public ObscuredFloat Accuracy;

		public ObscuredFloat FireAccuracy;

		public ObscuredInt FireBullets;

		public ObscuredFloat ReloadTime;

		public ObscuredFloat LastFireRate;

		public ObscuredInt Ammo;

		public ObscuredInt AmmoFirst;

		public ObscuredInt AmmoMax;

		public ObscuredFloat Distance;

		public ObscuredFloat Mass;

		public ObscuredBool RifleScope;

		public ObscuredInt RifleScopeSize;

		public ObscuredFloat RifleScopeSensitivity;

		public ObscuredFloat RifleScopeRecoil;
	}

	public WeaponTypeList SelectedWeapon;

	public bool CanFire = true;

	public bool isDebug;

	[Disabled]
	public bool isScope;

	[Disabled]
	public bool isReload;

	private vp_Timer.Handle ReloadTime = new vp_Timer.Handle();

	[Disabled]
	public bool Wielded;

	private vp_Timer.Handle WieldedTime = new vp_Timer.Handle();

	[Header("Weapons Data")]
	public WeaponData KnifeData = new WeaponData();

	public WeaponData PistolData = new WeaponData();

	public WeaponData RifleData = new WeaponData();

	private bool isUpdateWeapons;

	[Header("Others")]
	public LayerMask FireLayers;

	public PlayerInput m_PlayerInput;

	public Camera PlayerCamera;

	public Camera WeaponCamera;

	private Dictionary<string, GameObject> Weapons = new Dictionary<string, GameObject>();

	private void Start()
	{
	}

	private void OnEnable()
	{
		UICrosshair.SetActiveCrosshair(active: true);
	}

	private void OnDisable()
	{
		UICrosshair.SetActiveCrosshair(active: false);
		isReload = false;
		if (ReloadTime.Active)
		{
			ReloadTime.Cancel();
		}
		if (WieldedTime.Active)
		{
			WieldedTime.Cancel();
		}
	}

	private void Update()
	{
		if (InputManager.GetButton("Fire") && (bool)GameObject.Find("Display"))
		{
			FireWeapon();
		}
		if (InputManager.GetButtonDown("Aim"))
		{
			ScopeWeapon();
		}
		if (InputManager.GetButtonDown("Reload"))
		{
			ReloadWeapon();
		}
		if (InputManager.GetButtonDown("Pause"))
		{
			DeactiveScope();
		}
		if (InputManager.GetButtonDown("SelectWeapon"))
		{
			UpdateSelectWeapon();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1) && SelectedWeapon != WeaponTypeList.Rifle && (int)RifleData.WeaponID != 0)
		{
			UpdateWeaponAll(WeaponTypeList.Rifle);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2) && SelectedWeapon != WeaponTypeList.Pistol && (int)PistolData.WeaponID != 0)
		{
			UpdateWeaponAll(WeaponTypeList.Pistol);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3) && SelectedWeapon != WeaponTypeList.Knife && (int)KnifeData.WeaponID != 0)
		{
			UpdateWeaponAll(WeaponTypeList.Knife);
		}
	}

	private void UpdateSelectWeapon()
	{
		if (SelectedWeapon == WeaponTypeList.Knife)
		{
			if (RifleData.Enabled)
			{
				SetWeapon(WeaponTypeList.Rifle);
			}
			else if (PistolData.Enabled)
			{
				SetWeapon(WeaponTypeList.Pistol);
			}
		}
		else if (SelectedWeapon == WeaponTypeList.Pistol)
		{
			if (KnifeData.Enabled)
			{
				SetWeapon(WeaponTypeList.Knife);
			}
			else if (RifleData.Enabled)
			{
				SetWeapon(WeaponTypeList.Rifle);
			}
		}
		else if (SelectedWeapon == WeaponTypeList.Rifle)
		{
			if (PistolData.Enabled)
			{
				SetWeapon(WeaponTypeList.Pistol);
			}
			else if (KnifeData.Enabled)
			{
				SetWeapon(WeaponTypeList.Knife);
			}
		}
	}

	public WeaponData GetSelectedWeaponData()
	{
		return GetWeaponData(SelectedWeapon);
	}

	public WeaponData GetWeaponData(WeaponTypeList weapon)
	{
		switch (weapon)
		{
		case WeaponTypeList.Knife:
			return KnifeData;
		case WeaponTypeList.Pistol:
			return PistolData;
		case WeaponTypeList.Rifle:
			return RifleData;
		default:
			return null;
		}
	}

	public void UpdateWeaponAll()
	{
		UpdateWeaponAll(WeaponManager.DefaultWeaponType);
	}

	public void UpdateWeaponAll(WeaponTypeList defaultWeapon)
	{
		vp_Timer.In(0.03f, delegate
		{
			isUpdateWeapons = true;
			DeactiveAll();
			UpdateWeaponData(WeaponTypeList.Knife);
			UpdateWeaponData(WeaponTypeList.Pistol);
			UpdateWeaponData(WeaponTypeList.Rifle);
			vp_Timer.In(0.05f, delegate
			{
				SetWeapon(defaultWeapon, checkSelectedWeapon: false);
				isUpdateWeapons = false;
			});
			int num = 0;
			if (KnifeData.Enabled)
			{
				num++;
			}
			if (PistolData.Enabled)
			{
				num++;
			}
			if (RifleData.Enabled)
			{
				num++;
			}
			if (num >= 2)
			{
				UIGameManager.SetActiveSelectWeapon(active: true);
			}
			else
			{
				UIGameManager.SetActiveSelectWeapon(active: false);
			}
		});
	}

	public void UpdateWeaponData(WeaponTypeList weaponType)
	{
		WeaponType weaponType2 = null;
		WeaponData weaponData = null;
		switch (weaponType)
		{
		case WeaponTypeList.Knife:
			if (WeaponManager.HasKnifeType())
			{
				weaponType2 = WeaponManager.GetKnifeType();
				weaponData = KnifeData;
			}
			break;
		case WeaponTypeList.Pistol:
			if (WeaponManager.HasPistolType())
			{
				weaponType2 = WeaponManager.GetPistolType();
				weaponData = PistolData;
			}
			break;
		case WeaponTypeList.Rifle:
			if (WeaponManager.HasRifleType())
			{
				weaponType2 = WeaponManager.GetRifleType();
				weaponData = RifleData;
			}
			break;
		}
		if (weaponType2 != null && (weaponData.WeaponScript == null || weaponData.WeaponName != weaponType2.WeaponName) && !Weapons.ContainsKey(weaponType2.WeaponName))
		{
			GameObject fpsPrefab = weaponType2.FpsPrefab;
			fpsPrefab = Utils.AddChild(fpsPrefab, m_PlayerInput.FPCamera.transform);
			Weapons.Add(weaponType2.WeaponName, fpsPrefab);
			fpsPrefab.SetActive(value: true);
		}
		WeaponData weaponData2 = new WeaponData();
		if (weaponType2 != null)
		{
			weaponData2.Enabled = true;
			weaponData2.WeaponID = weaponType2.WeaponID;
			weaponData2.WeaponFire = weaponType2.WeaponFire;
			weaponData2.WeaponName = weaponType2.WeaponName;
			weaponData2.WeaponScript = Weapons[weaponType2.WeaponName].GetComponent<FPWeaponShooter>();
			weaponData2.Damage = weaponType2.BodyDamage;
			weaponData2.FireRate = weaponType2.FireRate;
			weaponData2.Accuracy = weaponType2.Accuracy;
			weaponData2.FireAccuracy = weaponType2.FireAccuracy;
			weaponData2.FireBullets = weaponType2.FireBullets;
			weaponData2.ReloadTime = weaponType2.ReloadTime;
			weaponData2.Ammo = weaponType2.Ammo;
			weaponData2.AmmoFirst = weaponType2.Ammo;
			weaponData2.AmmoMax = weaponType2.MaxAmmo;
			weaponData2.Distance = weaponType2.Distance;
			weaponData2.Mass = weaponType2.Mass;
			weaponData2.RifleScope = weaponType2.RifleScope;
			weaponData2.RifleScopeSize = weaponType2.RifleScopeSize;
			weaponData2.RifleScopeSensitivity = weaponType2.RifleScopeSensitivity;
			weaponData2.RifleScopeRecoil = weaponType2.RifleScopeRecoil;
			weaponData2.WeaponScript.UpdateHandAtlas();
			weaponData2.WeaponScript.UpdateWeaponAtlas(weaponData2.WeaponID);
		}
		else
		{
			weaponData2.Enabled = false;
		}
		weaponData = weaponData2;
		switch (weaponType)
		{
		case WeaponTypeList.Knife:
			KnifeData = weaponData;
			break;
		case WeaponTypeList.Pistol:
			PistolData = weaponData;
			break;
		case WeaponTypeList.Rifle:
			RifleData = weaponData;
			break;
		}
	}

	public void SetWeapon(WeaponTypeList weapon, bool checkSelectedWeapon = true)
	{
		if ((!checkSelectedWeapon || SelectedWeapon != weapon) && GetWeaponData(weapon).Enabled)
		{
			SelectedWeapon = weapon;
			DeactiveScope();
			DeactiveAll();
			UICrosshair.SetAccuracy(GetSelectedWeaponData().Accuracy);
			UIGameManager.SetAmmo(GetSelectedWeaponData().Ammo, GetSelectedWeaponData().AmmoMax);
			UIGameManager.SetActiveRifleScope(GetSelectedWeaponData().RifleScope);
			m_PlayerInput.SetPlayerSpeed(GetSelectedWeaponData().Mass);
			if (ReloadTime.Active)
			{
				ReloadTime.Cancel();
				isReload = false;
			}
			if (WieldedTime.Active)
			{
				WieldedTime.Cancel();
			}
			Wielded = true;
			vp_Timer.In(0.5f, delegate
			{
				Wielded = false;
			}, WieldedTime);
			switch (weapon)
			{
			case WeaponTypeList.Knife:
				KnifeData.WeaponScript.Active();
				break;
			case WeaponTypeList.Pistol:
				PistolData.WeaponScript.Active();
				break;
			case WeaponTypeList.Rifle:
				RifleData.WeaponScript.Active();
				break;
			}
			m_PlayerInput.Controller.SetWeapon(GetSelectedWeaponData().WeaponID);
		}
	}

	private void DeactiveAll()
	{
		if (KnifeData.WeaponScript != null)
		{
			KnifeData.WeaponScript.Deactive();
		}
		if (PistolData.WeaponScript != null)
		{
			PistolData.WeaponScript.Deactive();
		}
		if (RifleData.WeaponScript != null)
		{
			RifleData.WeaponScript.Deactive();
		}
	}

	public void FireWeapon()
	{
		if (CanFire && !isReload && GameManager.GetRoundState() != RoundState.EndRound)
		{
			switch (SelectedWeapon)
			{
			case WeaponTypeList.Knife:
				Fire(KnifeData);
				break;
			case WeaponTypeList.Pistol:
				Fire(PistolData);
				break;
			case WeaponTypeList.Rifle:
				Fire(RifleData);
				break;
			}
		}
	}

	private void Fire(WeaponData weapon)
	{
		if (Wielded || (float)weapon.LastFireRate > Time.time || isUpdateWeapons || !weapon.WeaponFire)
		{
			return;
		}
		if (weapon.WeaponScript.KnifeWeapon)
		{
			weapon.LastFireRate = Time.time + (float)weapon.FireRate;
			weapon.WeaponScript.Fire();
			vp_Timer.In(weapon.WeaponScript.KnifeDelay, delegate
			{
				FireData(weapon);
			});
		}
		else if ((int)weapon.Ammo > 0)
		{
			WeaponData weaponData = weapon;
			weaponData.Ammo = --weaponData.Ammo;
			weapon.LastFireRate = Time.time + (float)weapon.FireRate;
			weapon.WeaponScript.Fire();
			FireData(weapon);
			if (isScope && (float)weapon.RifleScopeRecoil != 0f)
			{
				m_PlayerInput.FPCamera.Pitch -= weapon.RifleScopeRecoil;
				float num = (float)weapon.RifleScopeRecoil / 2f;
				m_PlayerInput.FPCamera.Yaw += UnityEngine.Random.Range(0f - num, num);
			}
		}
		else if ((int)weapon.AmmoMax == 0)
		{
			DryFire(weapon);
		}
		else
		{
			Reload(weapon);
		}
	}

	private void FireData(WeaponData weapon)
	{
		EventManager.Dispatch("Fire");
		UIGameManager.SetAmmo(weapon.Ammo, weapon.AmmoMax);
		Vector2 vector = Vector3.zero;
		for (int i = 0; i < (int)weapon.FireBullets; i++)
		{
			vector = UICrosshair.Fire(weapon.FireAccuracy);
			vector = Utils.RandomAccuracy(vector);
			Ray ray = m_PlayerInput.FPCamera.GetComponent<Camera>().ViewportPointToRay(new Vector3(0.5f + vector.x, 0.5f + vector.y, 0f));
			if (Physics.Raycast(ray, out RaycastHit hitInfo, weapon.Distance, FireLayers) && hitInfo.collider.CompareTag("PlayerSkin"))
			{
				GameManager.HitBlood(hitInfo.point);
				DamageInfo value = DamageInfo.Create(weapon.Damage, m_PlayerInput.PlayerTransform.position, m_PlayerInput.PlayerTeam, weapon.WeaponID, PhotonNetwork.player.ID);
				hitInfo.transform.SendMessage("Damage", value, SendMessageOptions.DontRequireReceiver);
			}
		}
	}

	private void DryFire(WeaponData weapon)
	{
		weapon.LastFireRate = Time.time + (float)weapon.FireRate * 2f;
		weapon.WeaponScript.DryFire();
	}

	private void ReloadWeapon()
	{
		if (!isReload)
		{
			switch (SelectedWeapon)
			{
			case WeaponTypeList.Knife:
				KnifeData.WeaponScript.ShowWeapon();
				break;
			case WeaponTypeList.Pistol:
				Reload(PistolData);
				break;
			case WeaponTypeList.Rifle:
				Reload(RifleData);
				break;
			}
		}
	}

	private void Reload(WeaponData weapon)
	{
		if (isScope)
		{
			DeactiveScope();
		}
		if ((int)weapon.Ammo == (int)weapon.AmmoFirst || (int)weapon.AmmoMax == 0)
		{
			weapon.WeaponScript.ShowWeapon();
			return;
		}
		isReload = true;
		weapon.WeaponScript.Reload(weapon.ReloadTime);
		vp_Timer.In((float)weapon.ReloadTime + 0.5f, delegate
		{
			isReload = false;
			if ((int)weapon.AmmoMax > (int)weapon.AmmoFirst)
			{
				WeaponData weaponData = weapon;
				WeaponData weaponData2 = weaponData;
				weaponData2.AmmoMax = (int)weaponData2.AmmoMax - ((int)weapon.AmmoFirst - (int)weapon.Ammo);
				weapon.Ammo = weapon.AmmoFirst;
			}
			else
			{
				int num = weapon.Ammo;
				WeaponData weaponData3 = weapon;
				WeaponData weaponData4 = weaponData3;
				weaponData4.Ammo = (int)weaponData4.Ammo + (int)weapon.AmmoMax;
				weapon.Ammo = Mathf.Min(weapon.AmmoFirst, weapon.Ammo);
				WeaponData weaponData5 = weapon;
				WeaponData weaponData6 = weaponData5;
				weaponData6.AmmoMax = (int)weaponData6.AmmoMax - ((int)weapon.Ammo - num);
				weapon.AmmoMax = Mathf.Max(0, weapon.AmmoMax);
			}
			UIGameManager.SetAmmo(weapon.Ammo, weapon.AmmoMax);
		}, ReloadTime);
	}

	private void ScopeWeapon(bool check = true)
	{
		if ((!check || !isReload) && ((bool)GetSelectedWeaponData().RifleScope || !check))
		{
			isScope = !isScope;
			PlayerCamera.fieldOfView = (isScope ? ((int)GetSelectedWeaponData().RifleScopeSize) : 60);
			WeaponCamera.fieldOfView = (isScope ? 1 : 60);
			UICrosshair.SetActiveRifleScope(isScope);
			GetSelectedWeaponData().WeaponScript.ScopeRifle();
		}
	}

	public void DeactiveScope()
	{
		if (isScope)
		{
			ScopeWeapon(check: false);
		}
	}
}
