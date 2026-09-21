using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vp_SimpleInventory : MonoBehaviour
{
	protected class InventoryWeaponStatusComparer : IComparer
	{
		int IComparer.Compare(object x, object y)
		{
			return new CaseInsensitiveComparer().Compare(((InventoryWeaponStatus)x).Name, ((InventoryWeaponStatus)y).Name);
		}
	}

	[Serializable]
	public class InventoryItemStatus
	{
		public string Name = "Unnamed";

		public int Have;

		public int CanHave = 1;

		public bool ClearOnDeath = true;
	}

	[Serializable]
	public class InventoryWeaponStatus : InventoryItemStatus
	{
		public string ClipType = string.Empty;

		public int LoadedAmmo;

		public int MaxAmmo = 10;
	}

	protected vp_FPPlayerEventHandler m_Player;

	[SerializeField]
	protected List<InventoryItemStatus> m_ItemTypes;

	[SerializeField]
	protected List<InventoryWeaponStatus> m_WeaponTypes;

	protected Dictionary<string, InventoryItemStatus> m_ItemStatusDictionary;

	protected InventoryWeaponStatus m_CurrentWeaponStatus;

	protected int m_RefreshWeaponStatusIterations;

	public InventoryWeaponStatus CurrentWeaponStatus
	{
		get
		{
			return m_CurrentWeaponStatus;
		}
		set
		{
			m_CurrentWeaponStatus = value;
		}
	}

	public List<InventoryItemStatus> Weapons
	{
		get
		{
			List<InventoryItemStatus> list = new List<InventoryItemStatus>();
			foreach (InventoryWeaponStatus weaponType in m_WeaponTypes)
			{
				list.Add(weaponType);
			}
			return list;
		}
	}

	public List<InventoryItemStatus> EquippedWeapons
	{
		get
		{
			List<InventoryItemStatus> list = new List<InventoryItemStatus>();
			foreach (InventoryItemStatus value in m_ItemStatusDictionary.Values)
			{
				if (value.GetType() == typeof(InventoryWeaponStatus) && value.Have == 1)
				{
					list.Add(value);
				}
			}
			return list;
		}
	}

	protected Dictionary<string, InventoryItemStatus> ItemStatusDictionary
	{
		get
		{
			if (m_ItemStatusDictionary == null)
			{
				m_ItemStatusDictionary = new Dictionary<string, InventoryItemStatus>();
				for (int num = m_ItemTypes.Count - 1; num > -1; num--)
				{
					if (!m_ItemStatusDictionary.ContainsKey(m_ItemTypes[num].Name))
					{
						m_ItemStatusDictionary.Add(m_ItemTypes[num].Name, m_ItemTypes[num]);
					}
					else
					{
						m_ItemTypes.Remove(m_ItemTypes[num]);
					}
				}
				for (int num2 = m_WeaponTypes.Count - 1; num2 > -1; num2--)
				{
					if (!m_ItemStatusDictionary.ContainsKey(m_WeaponTypes[num2].Name))
					{
						m_ItemStatusDictionary.Add(m_WeaponTypes[num2].Name, m_WeaponTypes[num2]);
					}
					else
					{
						m_WeaponTypes.Remove(m_WeaponTypes[num2]);
					}
				}
			}
			return m_ItemStatusDictionary;
		}
	}

	protected virtual int OnValue_CurrentWeaponAmmoCount
	{
		get
		{
			return (m_CurrentWeaponStatus != null) ? m_CurrentWeaponStatus.LoadedAmmo : 0;
		}
		set
		{
			if (m_CurrentWeaponStatus != null)
			{
				m_CurrentWeaponStatus.LoadedAmmo = value;
			}
		}
	}

	protected virtual int OnValue_CurrentWeaponClipCount
	{
		get
		{
			if (m_CurrentWeaponStatus == null)
			{
				return 0;
			}
			if (!ItemStatusDictionary.TryGetValue(m_CurrentWeaponStatus.ClipType, out InventoryItemStatus value))
			{
				return 0;
			}
			return value.Have;
		}
	}

	protected virtual string OnValue_CurrentWeaponClipType => (m_CurrentWeaponStatus != null) ? m_CurrentWeaponStatus.ClipType : string.Empty;

	protected virtual void OnEnable()
	{
		if (m_Player != null)
		{
			m_Player.Register(this);
		}
	}

	protected virtual void OnDisable()
	{
		if (m_Player != null)
		{
			m_Player.Unregister(this);
		}
	}

	private void Awake()
	{
		m_Player = (vp_FPPlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_FPPlayerEventHandler));
		IComparer comparer = new InventoryWeaponStatusComparer();
		List<InventoryWeaponStatus> weaponTypes = m_WeaponTypes;
		IComparer comparer2 = comparer;
		weaponTypes.Sort(comparer2.Compare);
	}

	public bool HaveItem(object name)
	{
		InventoryItemStatus value;
		return ItemStatusDictionary.TryGetValue((string)name, out value) && value.Have >= 1;
	}

	private InventoryItemStatus GetItemStatus(string name)
	{
		if (!ItemStatusDictionary.TryGetValue(name, out InventoryItemStatus value))
		{
			UnityEngine.Debug.LogError("Error: (" + this + "). Unknown item type: '" + name + "'.");
		}
		return value;
	}

	private InventoryWeaponStatus GetWeaponStatus(string name)
	{
		if (name == null)
		{
			return null;
		}
		if (!ItemStatusDictionary.TryGetValue(name, out InventoryItemStatus value))
		{
			UnityEngine.Debug.LogError("Error: (" + this + "). Unknown item type: '" + name + "'.");
			return null;
		}
		if (value.GetType() != typeof(InventoryWeaponStatus))
		{
			UnityEngine.Debug.LogError("Error: (" + this + "). Item is not a weapon: '" + name + "'.");
			return null;
		}
		return (InventoryWeaponStatus)value;
	}

	protected void RefreshWeaponStatus()
	{
		if (!m_Player.CurrentWeaponWielded.Get() && m_RefreshWeaponStatusIterations < 50)
		{
			m_RefreshWeaponStatusIterations++;
			vp_Timer.In(0.1f, RefreshWeaponStatus);
			return;
		}
		m_RefreshWeaponStatusIterations = 0;
		string text = m_Player.CurrentWeaponName.Get();
		if (!string.IsNullOrEmpty(text))
		{
			m_CurrentWeaponStatus = GetWeaponStatus(text);
		}
	}

	protected virtual int OnMessage_GetItemCount(string name)
	{
		if (!ItemStatusDictionary.TryGetValue(name, out InventoryItemStatus value))
		{
			return 0;
		}
		return value.Have;
	}

	protected virtual bool OnAttempt_DepleteAmmo()
	{
		if (m_CurrentWeaponStatus == null)
		{
			return false;
		}
		if (m_CurrentWeaponStatus.LoadedAmmo < 1)
		{
			return m_CurrentWeaponStatus.MaxAmmo == 0;
		}
		m_CurrentWeaponStatus.LoadedAmmo--;
		return true;
	}

	protected virtual bool OnAttempt_AddAmmo(object arg)
	{
		object[] array = (object[])arg;
		string name = (string)array[0];
		int num = (array.Length == 2) ? ((int)array[1]) : (-1);
		InventoryWeaponStatus weaponStatus = GetWeaponStatus(name);
		if (weaponStatus == null)
		{
			return false;
		}
		if (num == -1)
		{
			weaponStatus.LoadedAmmo = weaponStatus.MaxAmmo;
		}
		else
		{
			weaponStatus.LoadedAmmo = Mathf.Min(weaponStatus.LoadedAmmo + num, weaponStatus.MaxAmmo);
		}
		return true;
	}

	protected virtual bool OnAttempt_AddItem(object args)
	{
		object[] array = (object[])args;
		string name = (string)array[0];
		int num = (array.Length != 2) ? 1 : ((int)array[1]);
		InventoryItemStatus itemStatus = GetItemStatus(name);
		if (itemStatus == null)
		{
			return false;
		}
		itemStatus.CanHave = Mathf.Max(1, itemStatus.CanHave);
		if (itemStatus.Have >= itemStatus.CanHave)
		{
			return false;
		}
		itemStatus.Have = Mathf.Min(itemStatus.Have + num, itemStatus.CanHave);
		return true;
	}

	protected virtual bool OnAttempt_RemoveItem(object args)
	{
		object[] array = (object[])args;
		string name = (string)array[0];
		int num = (array.Length != 2) ? 1 : ((int)array[1]);
		InventoryItemStatus itemStatus = GetItemStatus(name);
		if (itemStatus == null)
		{
			return false;
		}
		if (itemStatus.Have <= 0)
		{
			return false;
		}
		itemStatus.Have = Mathf.Max(itemStatus.Have - num, 0);
		return true;
	}

	protected virtual bool OnAttempt_RemoveClip()
	{
		return m_CurrentWeaponStatus != null && GetItemStatus(m_CurrentWeaponStatus.ClipType) != null && m_CurrentWeaponStatus.LoadedAmmo < m_CurrentWeaponStatus.MaxAmmo && m_Player.RemoveItem.Try(new object[1]
		{
			m_CurrentWeaponStatus.ClipType
		});
	}

	protected virtual bool CanStart_SetWeapon()
	{
		int num = (int)m_Player.SetWeapon.Argument;
		return num == 0 || (num >= 0 && num <= m_WeaponTypes.Count && HaveItem(m_WeaponTypes[num - 1].Name));
	}

	protected virtual void OnStop_SetWeapon()
	{
		RefreshWeaponStatus();
	}

	protected virtual void OnStart_Dead()
	{
		if (m_ItemStatusDictionary != null)
		{
			foreach (InventoryItemStatus value in m_ItemStatusDictionary.Values)
			{
				if (value.ClearOnDeath)
				{
					value.Have = 0;
					if (value.GetType() == typeof(InventoryWeaponStatus))
					{
						((InventoryWeaponStatus)value).LoadedAmmo = 0;
					}
				}
			}
		}
	}
}
