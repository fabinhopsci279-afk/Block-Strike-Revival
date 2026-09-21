using System;
using System.Collections.Generic;
using UnityEngine;

public class vp_FPInventory : vp_Inventory
{
	[Serializable]
	public class MiscSection
	{
		public bool ResetOnRespawn = true;
	}

	private vp_FPPlayerEventHandler m_Player;

	private vp_FPWeaponHandler m_WeaponHandler;

	protected Dictionary<int, vp_ItemIdentifier> m_WeaponIdentifiers = new Dictionary<int, vp_ItemIdentifier>();

	protected vp_ItemIdentifier m_WeaponIdentifierResult;

	protected string m_MissingHandlerError = "Error (vp_FPInventory) this component must be on the same transform as a vp_FPPlayerEventHandler + vp_FPWeaponHandler.";

	[SerializeField]
	protected MiscSection m_Misc;

	protected vp_FPPlayerEventHandler Player
	{
		get
		{
			if (m_Player == null)
			{
				m_Player = base.transform.GetComponent<vp_FPPlayerEventHandler>();
			}
			return m_Player;
		}
	}

	protected vp_FPWeaponHandler WeaponHandler
	{
		get
		{
			if (m_WeaponHandler == null)
			{
				m_WeaponHandler = base.transform.GetComponent<vp_FPWeaponHandler>();
			}
			return m_WeaponHandler;
		}
	}

	public vp_ItemIdentifier CurrentWeaponIdentifier
	{
		get
		{
			if (!Application.isPlaying)
			{
				return null;
			}
			return GetWeaponIdentifier(WeaponHandler.CurrentWeaponIndex);
		}
	}

	protected virtual vp_ItemInstance CurrentWeaponInstance
	{
		get
		{
			if (Application.isPlaying && WeaponHandler.CurrentWeaponIndex == 0)
			{
				return null;
			}
			if (CurrentWeaponIdentifier == null)
			{
				MissingIdentifierError();
				return null;
			}
			return GetItem(CurrentWeaponIdentifier.Type, CurrentWeaponIdentifier.ID);
		}
	}

	protected virtual int OnValue_CurrentWeaponAmmoCount
	{
		get
		{
			return (CurrentWeaponInstance as vp_UnitBankInstance)?.Count ?? 0;
		}
		set
		{
			(CurrentWeaponInstance as vp_UnitBankInstance)?.TryGiveUnits(value);
		}
	}

	protected virtual int OnValue_CurrentWeaponMaxAmmoCount => (CurrentWeaponInstance as vp_UnitBankInstance)?.Capacity ?? 0;

	protected virtual int OnValue_CurrentWeaponClipCount
	{
		get
		{
			vp_UnitBankInstance vp_UnitBankInstance = CurrentWeaponInstance as vp_UnitBankInstance;
			if (vp_UnitBankInstance == null)
			{
				return 0;
			}
			return GetUnitCount(vp_UnitBankInstance.UnitType);
		}
	}

	protected virtual vp_ItemIdentifier GetWeaponIdentifier(int index)
	{
		if (!Application.isPlaying)
		{
			return null;
		}
		if (!m_WeaponIdentifiers.TryGetValue(index, out m_WeaponIdentifierResult))
		{
			if (index < 1 || index > WeaponHandler.Weapons.Count)
			{
				return null;
			}
			if (WeaponHandler.Weapons[index - 1] == null)
			{
				return null;
			}
			m_WeaponIdentifierResult = WeaponHandler.Weapons[index - 1].GetComponent<vp_ItemIdentifier>();
			if (m_WeaponIdentifierResult == null)
			{
				return null;
			}
			if (m_WeaponIdentifierResult.Type == null)
			{
				return null;
			}
			m_WeaponIdentifiers.Add(index, m_WeaponIdentifierResult);
		}
		return m_WeaponIdentifierResult;
	}

	protected override void Awake()
	{
		base.Awake();
		if (Player == null || WeaponHandler == null)
		{
			UnityEngine.Debug.LogError(m_MissingHandlerError);
		}
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		if (Player != null)
		{
			Player.Register(this);
		}
		UnwieldMissingWeapon();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		if (Player != null)
		{
			Player.Unregister(this);
		}
	}

	protected virtual bool MissingIdentifierError(int weaponIndex = 0)
	{
		if (!Application.isPlaying)
		{
			return false;
		}
		string str = string.Empty;
		if (weaponIndex > 0 && WeaponHandler != null && WeaponHandler.Weapons.Count > weaponIndex - 1)
		{
			str = "'" + WeaponHandler.Weapons[weaponIndex - 1].name + "' ";
		}
		UnityEngine.Debug.LogWarning(string.Format("Warning: ({0}{1}) Weapon gameobject " + str + "lacks a properly set up vp_ItemIdentifier component!", vp_Utility.GetErrorLocation(1, showOnlyLast: true), (weaponIndex == 0) ? string.Empty : ("(" + weaponIndex.ToString() + ")")));
		return false;
	}

	protected override void DoAddItem(vp_ItemType type, int id)
	{
		bool flag = HaveItem(type, id);
		base.DoAddItem(type, id);
		if (!flag)
		{
			TryWield(GetItem(type, id));
		}
	}

	protected override void DoRemoveItem(vp_ItemInstance item)
	{
		Unwield(item);
		base.DoRemoveItem(item);
	}

	protected override void DoAddUnitBank(vp_UnitBankType unitBankType, int id, int unitsLoaded)
	{
		bool flag = HaveItem(unitBankType, id);
		base.DoAddUnitBank(unitBankType, id, unitsLoaded);
		if (!flag)
		{
			TryWield(GetItem(unitBankType, id));
		}
	}

	protected override void DoRemoveUnitBank(vp_UnitBankInstance bank)
	{
		Unwield(bank);
		base.DoRemoveUnitBank(bank);
	}

	public override bool DoAddUnits(vp_UnitBankInstance bank, int amount)
	{
		bool flag = base.DoAddUnits(bank, amount);
		if (flag && bank.IsInternal && (!Application.isPlaying || WeaponHandler.CurrentWeaponIndex != 0))
		{
			vp_UnitBankInstance vp_UnitBankInstance = CurrentWeaponInstance as vp_UnitBankInstance;
			if (vp_UnitBankInstance != null && bank.UnitType == vp_UnitBankInstance.UnitType && vp_UnitBankInstance.Count == 0)
			{
				Player.AutoReload.Try();
			}
		}
		return flag;
	}

	public override bool DoRemoveUnits(vp_UnitBankInstance bank, int amount)
	{
		bool result = base.DoRemoveUnits(bank, amount);
		if (bank.Count == 0)
		{
			vp_Timer.In(0.3f, delegate
			{
				Player.AutoReload.Try();
			});
		}
		return result;
	}

	protected virtual void UnwieldMissingWeapon()
	{
		if (Application.isPlaying && WeaponHandler.CurrentWeaponIndex >= 1 && (!(CurrentWeaponIdentifier != null) || !HaveItem(CurrentWeaponIdentifier.Type, CurrentWeaponIdentifier.ID)))
		{
			if (CurrentWeaponIdentifier == null)
			{
				MissingIdentifierError(WeaponHandler.CurrentWeaponIndex);
			}
			Player.SetWeapon.TryStart(0);
		}
	}

	protected virtual void TryWield(vp_ItemInstance item)
	{
		if (!Application.isPlaying || Player.Dead.Active)
		{
			return;
		}
		int num = 1;
		while (true)
		{
			if (num < WeaponHandler.Weapons.Count + 1)
			{
				vp_ItemIdentifier weaponIdentifier = GetWeaponIdentifier(num);
				if (!(weaponIdentifier == null) && !(item.Type != weaponIdentifier.Type) && (weaponIdentifier.ID == 0 || item.ID == weaponIdentifier.ID))
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		Player.SetWeapon.TryStart(num);
	}

	protected virtual void Unwield(vp_ItemInstance item)
	{
		if (Application.isPlaying && WeaponHandler.CurrentWeaponIndex != 0)
		{
			if (CurrentWeaponIdentifier == null)
			{
				MissingIdentifierError();
			}
			else if (!(item.Type != CurrentWeaponIdentifier.Type) && (CurrentWeaponIdentifier.ID == 0 || item.ID == CurrentWeaponIdentifier.ID))
			{
				Player.SetWeapon.TryStart(0);
				vp_Timer.In(0.35f, delegate
				{
					Player.SetNextWeapon.Try();
				});
				vp_Timer.In(1f, UnwieldMissingWeapon);
			}
		}
	}

	public override void Refresh()
	{
		base.Refresh();
		UnwieldMissingWeapon();
	}

	protected virtual bool CanStart_SetWeapon()
	{
		int num = (int)Player.SetWeapon.Argument;
		if (num == 0)
		{
			return true;
		}
		vp_ItemIdentifier weaponIdentifier = GetWeaponIdentifier(num);
		if (weaponIdentifier == null)
		{
			return num >= 1 && num <= WeaponHandler.Weapons.Count && MissingIdentifierError(num);
		}
		return HaveItem(weaponIdentifier.Type, weaponIdentifier.ID);
	}

	protected virtual bool OnAttempt_DepleteAmmo()
	{
		if (CurrentWeaponIdentifier == null)
		{
			return MissingIdentifierError();
		}
		return TryDeduct(CurrentWeaponIdentifier.Type as vp_UnitBankType, CurrentWeaponIdentifier.ID, 1);
	}

	protected virtual bool OnAttempt_RefillCurrentWeapon()
	{
		if (CurrentWeaponIdentifier == null)
		{
			return MissingIdentifierError();
		}
		return TryReload(CurrentWeaponIdentifier.Type as vp_UnitBankType, CurrentWeaponIdentifier.ID);
	}

	public override void Reset()
	{
		if (m_Misc.ResetOnRespawn)
		{
			base.Reset();
		}
	}

	protected virtual int OnMessage_GetItemCount(string itemTypeObjectName)
	{
		vp_ItemInstance item = GetItem(base.name);
		if (item == null)
		{
			return 0;
		}
		vp_UnitBankInstance vp_UnitBankInstance = item as vp_UnitBankInstance;
		if (vp_UnitBankInstance != null && vp_UnitBankInstance.IsInternal)
		{
			return GetItemCount(vp_UnitBankInstance.UnitType);
		}
		return GetItemCount(item.Type);
	}

	protected virtual bool OnAttempt_AddItem(object args)
	{
		object[] array = (object[])args;
		vp_ItemType vp_ItemType = array[0] as vp_ItemType;
		if (vp_ItemType == null)
		{
			return false;
		}
		int amount = (array.Length != 2) ? 1 : ((int)array[1]);
		return TryGiveItems(vp_ItemType, amount);
	}

	protected virtual bool OnAttempt_RemoveItem(object args)
	{
		object[] array = (object[])args;
		vp_ItemType vp_ItemType = array[0] as vp_ItemType;
		if (vp_ItemType == null)
		{
			return false;
		}
		int amount = (array.Length != 2) ? 1 : ((int)array[1]);
		return TryRemoveItems(vp_ItemType, amount);
	}
}
