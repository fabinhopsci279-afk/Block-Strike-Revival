using UnityEngine;

[RequireComponent(typeof(vp_FPWeapon))]
public class vp_FPWeaponReloader : MonoBehaviour
{
	protected vp_FPWeapon m_Weapon;

	protected vp_FPPlayerEventHandler m_Player;

	protected AudioSource m_Audio;

	public AudioClip SoundReload;

	public AnimationClip AnimationReload;

	public float ReloadDuration = 1f;

	protected vp_SimpleInventory m_SimpleInventory;

	protected virtual float OnValue_CurrentWeaponReloadDuration => ReloadDuration;

	protected virtual void Awake()
	{
		m_Audio = GetComponent<AudioSource>();
		m_Player = (vp_FPPlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_FPPlayerEventHandler));
		m_SimpleInventory = (vp_SimpleInventory)base.transform.root.GetComponent(typeof(vp_SimpleInventory));
	}

	protected virtual void Start()
	{
		m_Weapon = base.transform.GetComponent<vp_FPWeapon>();
	}

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

	protected virtual bool CanStart_Reload()
	{
		if (!m_Player.CurrentWeaponWielded.Get())
		{
			return false;
		}
		if (m_SimpleInventory != null)
		{
			return m_Player.RemoveClip.Try();
		}
		return (m_Player.CurrentWeaponMaxAmmoCount.Get() == 0 || m_Player.CurrentWeaponAmmoCount.Get() != m_Player.CurrentWeaponMaxAmmoCount.Get()) && m_Player.CurrentWeaponClipCount.Get() >= 1;
	}

	protected virtual void OnStart_Reload()
	{
		m_Player.Reload.AutoDuration = m_Player.CurrentWeaponReloadDuration.Get();
		if (m_Player.Reload.AutoDuration == 0f && AnimationReload != null)
		{
			m_Player.Reload.AutoDuration = AnimationReload.length;
		}
		if (AnimationReload != null)
		{
			m_Weapon.WeaponModel.GetComponent<Animation>().CrossFade(AnimationReload.name);
		}
		if (m_Audio != null)
		{
			m_Audio.pitch = Time.timeScale;
			m_Audio.PlayOneShot(SoundReload);
		}
	}

	protected virtual void OnStop_Reload()
	{
		string text = m_Player.CurrentWeaponName.Get();
		if (!string.IsNullOrEmpty(text))
		{
			m_Player.AddAmmo.Try(new object[1]
			{
				text
			});
		}
		m_Player.RefillCurrentWeapon.Try();
	}
}
