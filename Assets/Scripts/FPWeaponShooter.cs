using UnityEngine;

public class FPWeaponShooter : MonoBehaviour
{
	[Header("Motion")]
	public Vector3 MotionPositionRecoil = new Vector3(0f, 0f, -0.035f);

	public Vector3 MotionRotationRecoil = new Vector3(-10f, 0f, 0f);

	public float MotionRotationRecoilDeadZone = 0.5f;

	public float MotionRotationRecoilCameraFactor;

	public float MotionPositionRecoilCameraFactor;

	public float MotionPositionReset = 0.5f;

	public float MotionRotationReset = 0.5f;

	public float MotionPositionPause = 1f;

	public float MotionRotationPause = 1f;

	public float MotionDryFireRecoil = -0.1f;

	[Header("Fire Reload Settings")]
	public bool FireReload;

	public float FireReloadDelay = 0.2f;

	public float FireReloadDuration = 0.5f;

	public int FireReloadForce = 60;

	public Vector3 FireReloadPosition;

	public Vector3 FireReloadRotation;

	[Header("Knife Settings")]
	public bool KnifeWeapon;

	public float KnifeDelay = 0.1f;

	public int KnifeDelayForce = 50;

	public Vector3 KnifeDelayForcePosition;

	public Vector3 KnifeDelayForceRotation;

	public float KnifeAttackTime = 0.2f;

	public int KnifeAttackForce = 50;

	public Vector3 KnifeAttackForcePosition;

	public Vector3 KnifeAttackForceRotation;

	[Header("Show Settings")]
	public int ShowForce = 50;

	public Vector3 ShowPosition;

	public Vector3 ShowRotation;

	public float ShowDuration = 0.5f;

	public Vector3 ShowPosition2;

	public Vector3 ShowRotation2;

	[Disabled]
	public bool Show;

	[Header("Others")]
	public vp_FPWeapon FPWeapon;

	public Transform Muzzle;

	public MeshAtlas[] WeaponAtlas;

	public MeshAtlas[] HandsAtlas;

	private bool isDryFire;

	[Header("Sounds")]
	public AudioSource m_AudioSource;

	public AudioClip FireSound;

	public AudioClip ReloadSound;

	public AudioClip DryFireSound;

	private bool isSound = true;

	private void Start()
	{
		UpdateOptions();
		UpdateHandAtlas();
		EventManager.AddListener("UpdateOptions", UpdateOptions);
	}

	private void UpdateOptions()
	{
		isSound = SaveLoadManager.GetSound();
	}

	private void Update()
	{
		if (isDryFire && InputManager.GetButtonUp("Fire"))
		{
			isDryFire = false;
		}
	}

	public void Active()
	{
		FPWeapon.Activate();
		FPWeapon.Wield();
	}

	public void Deactive()
	{
		FPWeapon.Deactivate();
	}

	public void Reload(float duration)
	{
		if (!KnifeWeapon)
		{
			if (isSound)
			{
				m_AudioSource.PlayOneShot(ReloadSound);
			}
			FPWeapon.SetState("Reload");
			vp_Timer.In(duration, delegate
			{
				FPWeapon.SetState("Reload", enabled: false);
			});
		}
	}

	public void ShowWeapon()
	{
		if (!Show)
		{
			Show = true;
			FPWeapon.StopSprings();
			FPWeapon.AddSoftForce(ShowPosition, ShowRotation, ShowForce);
			if (ShowDuration != 0f)
			{
				vp_Timer.In(ShowDuration, delegate
				{
					if (Show)
					{
						FPWeapon.StopSprings();
						FPWeapon.AddSoftForce(ShowPosition2, ShowRotation2, ShowForce);
						vp_Timer.In(ShowDuration + 1f, delegate
						{
							Show = false;
						});
					}
				});
			}
			else
			{
				Show = false;
			}
		}
	}

	public void Fire()
	{
		if (Show)
		{
			FPWeapon.StopSprings();
			Show = false;
		}
		if (KnifeWeapon)
		{
			FireKnife();
		}
		else
		{
			FireWeapon();
		}
	}

	private void FireWeapon()
	{
		if (isSound)
		{
			m_AudioSource.clip = FireSound;
			m_AudioSource.Play();
		}
		if (!KnifeWeapon && UnityEngine.Random.value > 0.2f)
		{
			Transform muzzle = Muzzle;
			Vector3 localEulerAngles = Muzzle.localEulerAngles;
			float x = localEulerAngles.x;
			Vector3 localEulerAngles2 = Muzzle.localEulerAngles;
			muzzle.localEulerAngles = new Vector3(x, localEulerAngles2.y, UnityEngine.Random.value * 360f);
			Muzzle.gameObject.SetActive(value: true);
			vp_Timer.In(0.02f, delegate
			{
				Muzzle.gameObject.SetActive(value: false);
			});
		}
		FPWeapon.ResetSprings(MotionPositionReset, MotionRotationReset, MotionPositionPause, MotionRotationPause);
		if (MotionRotationRecoil.z == 0f)
		{
			FPWeapon.AddForce2(MotionPositionRecoil, MotionRotationRecoil);
		}
		else
		{
			FPWeapon.AddForce2(MotionPositionRecoil, Vector3.Scale(MotionRotationRecoil, Vector3.one + Vector3.back) + ((!(UnityEngine.Random.value >= 0.5f)) ? Vector3.forward : Vector3.back) * UnityEngine.Random.Range(MotionRotationRecoil.z * MotionRotationRecoilDeadZone, MotionRotationRecoil.z));
		}
		if (FireReload)
		{
			vp_Timer.In(FireReloadDelay, delegate
			{
				FPWeapon.AddSoftForce(FireReloadPosition, FireReloadRotation, FireReloadForce);
				vp_Timer.In(FireReloadDuration, delegate
				{
					FPWeapon.StopSprings();
				});
			});
		}
	}

	private void FireKnife()
	{
		if (KnifeDelay != 0f)
		{
			FPWeapon.AddSoftForce(KnifeDelayForcePosition, KnifeDelayForceRotation, KnifeDelayForce);
		}
		vp_Timer.In(KnifeDelay, delegate
		{
			FPWeapon.StopSprings();
			if (isSound)
			{
				m_AudioSource.clip = FireSound;
				m_AudioSource.Play();
			}
			FPWeapon.AddSoftForce(KnifeAttackForcePosition, KnifeAttackForceRotation, KnifeAttackForce);
			vp_Timer.In(KnifeAttackTime, delegate
			{
				FPWeapon.StopSprings();
			});
		});
	}

	public void DryFire()
	{
		if (!KnifeWeapon && !isDryFire)
		{
			if (isSound)
			{
				m_AudioSource.PlayOneShot(DryFireSound);
			}
			isDryFire = true;
			FPWeapon.AddForce2(MotionPositionRecoil * MotionDryFireRecoil, MotionRotationRecoil * MotionDryFireRecoil);
		}
	}

	public void ScopeRifle()
	{
		if (Show)
		{
			FPWeapon.StopSprings();
			Show = false;
		}
	}

	public void UpdateHandAtlas()
	{
		string playerSkin = SkinsManager.GetPlayerSkin(PlayerInput.instance.PlayerTeam);
		for (int i = 0; i < HandsAtlas.Length; i++)
		{
			if (HandsAtlas[i].spriteName != playerSkin)
			{
				HandsAtlas[i].spriteName = playerSkin;
			}
		}
	}

	public void UpdateWeaponAtlas(int weaponID)
	{
		string weaponSkin = SkinsManager.GetWeaponSkin(weaponID);
		if (string.IsNullOrEmpty(weaponSkin))
		{
			return;
		}
		for (int i = 0; i < WeaponAtlas.Length; i++)
		{
			if (WeaponAtlas[i].mSpriteName != weaponSkin)
			{
				WeaponAtlas[i].spriteName = weaponSkin;
			}
		}
	}
}
