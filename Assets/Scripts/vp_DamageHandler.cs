using UnityEngine;

public class vp_DamageHandler : MonoBehaviour
{
	public float MaxHealth = 1f;

	public GameObject[] DeathSpawnObjects;

	public float MinDeathDelay;

	public float MaxDeathDelay;

	public float m_CurrentHealth;

	protected AudioSource m_Audio;

	public AudioClip DeathSound;

	public float ImpactDamageThreshold = 10f;

	public float ImpactDamageMultiplier;

	[HideInInspector]
	public bool Respawns;

	[HideInInspector]
	public float MinRespawnTime = -99999f;

	[HideInInspector]
	public float MaxRespawnTime = -99999f;

	[HideInInspector]
	public float RespawnCheckRadius = -99999f;

	[HideInInspector]
	public AudioClip RespawnSound;

	[HideInInspector]
	public GameObject DeathEffect;

	protected Vector3 m_StartPosition;

	protected Quaternion m_StartRotation;

	protected virtual void Awake()
	{
		m_Audio = GetComponent<AudioSource>();
		m_CurrentHealth = MaxHealth;
		CheckForObsoleteParams();
	}

	protected virtual void OnEnable()
	{
	}

	protected virtual void OnDisable()
	{
	}

	public virtual void Damage(float damage)
	{
		if (base.enabled && vp_Utility.IsActive(base.gameObject) && !(m_CurrentHealth <= 0f))
		{
			m_CurrentHealth = Mathf.Min(m_CurrentHealth - damage, MaxHealth);
			if (m_CurrentHealth <= 0f)
			{
				vp_Timer.In(UnityEngine.Random.Range(MinDeathDelay, MaxDeathDelay), delegate
				{
					SendMessage("Die");
				});
			}
		}
	}

	public virtual void Die()
	{
		if (!base.enabled || !vp_Utility.IsActive(base.gameObject))
		{
			return;
		}
		if (m_Audio != null)
		{
			m_Audio.pitch = Time.timeScale;
			m_Audio.PlayOneShot(DeathSound);
		}
		RemoveBulletHoles();
		vp_Utility.Activate(base.gameObject, activate: false);
		GameObject[] deathSpawnObjects = DeathSpawnObjects;
		foreach (GameObject gameObject in deathSpawnObjects)
		{
			if (gameObject != null)
			{
				vp_Utility.Instantiate(gameObject, base.transform.position, base.transform.rotation);
			}
		}
	}

	protected virtual void Reset()
	{
		m_CurrentHealth = MaxHealth;
	}

	protected virtual void RemoveBulletHoles()
	{
		vp_HitscanBullet[] componentsInChildren = GetComponentsInChildren<vp_HitscanBullet>(includeInactive: true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			vp_Utility.Destroy(componentsInChildren[i].gameObject);
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		float num = collision.relativeVelocity.sqrMagnitude * 0.1f;
		float num2 = (!(num <= ImpactDamageThreshold)) ? (num * ImpactDamageMultiplier) : 0f;
		if (num2 > 0f)
		{
			if (m_CurrentHealth - num2 <= 0f)
			{
				MaxDeathDelay = (MinDeathDelay = 0f);
			}
			Damage(num2);
		}
	}

	protected virtual void Respawn()
	{
	}

	protected virtual void Reactivate()
	{
	}

	private void CheckForObsoleteParams()
	{
		if (DeathEffect != null)
		{
			UnityEngine.Debug.LogWarning(this + "'DeathEffect' is obsolete! Please use the 'DeathSpawnObjects' array instead.");
		}
		string text = string.Empty;
		if (Respawns)
		{
			text += "Respawns, ";
		}
		if (MinRespawnTime != -99999f)
		{
			text += "MinRespawnTime, ";
		}
		if (MaxRespawnTime != -99999f)
		{
			text += "MaxRespawnTime, ";
		}
		if (RespawnCheckRadius != -99999f)
		{
			text += "RespawnCheckRadius, ";
		}
		if (RespawnSound != null)
		{
			text += "RespawnSound, ";
		}
		if (text != string.Empty)
		{
			text = text.Remove(text.LastIndexOf(", "));
			UnityEngine.Debug.LogWarning(string.Format("Warning + (" + this + ") The following parameters are obsolete: \"{0}\". Creating a temp vp_Respawner component. To remove this warning, see the UFPS menu -> Wizards -> Convert Old DamageHandlers.", text));
			CreateTempRespawner();
		}
	}

	public bool CreateTempRespawner()
	{
		if ((bool)GetComponent<vp_Respawner>() || (bool)GetComponent<vp_PlayerRespawner>())
		{
			DisableOldParams();
			return false;
		}
		CreateRespawnerForDamageHandler(this);
		DisableOldParams();
		return true;
	}

	public static int GenerateRespawnersForAllDamageHandlers()
	{
		vp_PlayerDamageHandler[] array = UnityEngine.Object.FindObjectsOfType(typeof(vp_PlayerDamageHandler)) as vp_PlayerDamageHandler[];
		if (array != null && array.Length > 0)
		{
			vp_PlayerDamageHandler[] array2 = array;
			foreach (vp_PlayerDamageHandler vp_PlayerDamageHandler in array2)
			{
				if (!(vp_PlayerDamageHandler.transform.GetComponent<vp_FPPlayerEventHandler>() == null))
				{
					vp_FPPlayerDamageHandler vp_FPPlayerDamageHandler = vp_PlayerDamageHandler.gameObject.AddComponent<vp_FPPlayerDamageHandler>();
					vp_FPPlayerDamageHandler.AllowFallDamage = vp_PlayerDamageHandler.AllowFallDamage;
					vp_FPPlayerDamageHandler.DeathEffect = vp_PlayerDamageHandler.DeathEffect;
					vp_FPPlayerDamageHandler.DeathSound = vp_PlayerDamageHandler.DeathSound;
					vp_FPPlayerDamageHandler.DeathSpawnObjects = vp_PlayerDamageHandler.DeathSpawnObjects;
					vp_FPPlayerDamageHandler.FallImpactPitch = vp_PlayerDamageHandler.FallImpactPitch;
					vp_FPPlayerDamageHandler.FallImpactSounds = vp_PlayerDamageHandler.FallImpactSounds;
					vp_FPPlayerDamageHandler.FallImpactThreshold = vp_PlayerDamageHandler.FallImpactThreshold;
					vp_FPPlayerDamageHandler.ImpactDamageMultiplier = vp_PlayerDamageHandler.ImpactDamageMultiplier;
					vp_FPPlayerDamageHandler.ImpactDamageThreshold = vp_PlayerDamageHandler.ImpactDamageThreshold;
					vp_FPPlayerDamageHandler.m_Audio = vp_PlayerDamageHandler.m_Audio;
					vp_FPPlayerDamageHandler.m_CurrentHealth = vp_PlayerDamageHandler.m_CurrentHealth;
					vp_FPPlayerDamageHandler.m_StartPosition = vp_PlayerDamageHandler.m_StartPosition;
					vp_FPPlayerDamageHandler.m_StartRotation = vp_PlayerDamageHandler.m_StartRotation;
					vp_FPPlayerDamageHandler.MaxDeathDelay = vp_PlayerDamageHandler.MaxDeathDelay;
					vp_FPPlayerDamageHandler.MaxHealth = vp_PlayerDamageHandler.MaxHealth;
					vp_FPPlayerDamageHandler.MaxRespawnTime = vp_PlayerDamageHandler.MaxRespawnTime;
					vp_FPPlayerDamageHandler.MinDeathDelay = vp_PlayerDamageHandler.MinDeathDelay;
					vp_FPPlayerDamageHandler.MinRespawnTime = vp_PlayerDamageHandler.MinRespawnTime;
					vp_FPPlayerDamageHandler.RespawnCheckRadius = vp_PlayerDamageHandler.RespawnCheckRadius;
					vp_FPPlayerDamageHandler.Respawns = vp_PlayerDamageHandler.Respawns;
					vp_FPPlayerDamageHandler.RespawnSound = vp_PlayerDamageHandler.RespawnSound;
					UnityEngine.Object.DestroyImmediate(vp_PlayerDamageHandler);
				}
			}
		}
		vp_DamageHandler[] array3 = UnityEngine.Object.FindObjectsOfType(typeof(vp_DamageHandler)) as vp_DamageHandler[];
		vp_DamageHandler[] array4 = UnityEngine.Object.FindObjectsOfType(typeof(vp_FPPlayerDamageHandler)) as vp_DamageHandler[];
		int num = 0;
		vp_DamageHandler[] array5 = array3;
		foreach (vp_DamageHandler vp_DamageHandler in array5)
		{
			if (vp_DamageHandler.CreateTempRespawner())
			{
				num++;
			}
		}
		vp_DamageHandler[] array6 = array4;
		foreach (vp_DamageHandler vp_DamageHandler2 in array6)
		{
			if (vp_DamageHandler2.CreateTempRespawner())
			{
				num++;
			}
		}
		return num;
	}

	private void DisableOldParams()
	{
		Respawns = false;
		MinRespawnTime = -99999f;
		MaxRespawnTime = -99999f;
		RespawnCheckRadius = -99999f;
		RespawnSound = null;
	}

	private static void CreateRespawnerForDamageHandler(vp_DamageHandler damageHandler)
	{
		if ((bool)damageHandler.gameObject.GetComponent<vp_Respawner>() || (bool)damageHandler.gameObject.GetComponent<vp_PlayerRespawner>())
		{
			return;
		}
		vp_Respawner vp_Respawner = (!(damageHandler is vp_FPPlayerDamageHandler)) ? damageHandler.gameObject.AddComponent<vp_Respawner>() : damageHandler.gameObject.AddComponent<vp_PlayerRespawner>();
		if (!(vp_Respawner == null))
		{
			if (damageHandler.MinRespawnTime != -99999f)
			{
				vp_Respawner.MinRespawnTime = damageHandler.MinRespawnTime;
			}
			if (damageHandler.MaxRespawnTime != -99999f)
			{
				vp_Respawner.MaxRespawnTime = damageHandler.MaxRespawnTime;
			}
			if (damageHandler.RespawnCheckRadius != -99999f)
			{
				vp_Respawner.ObstructionRadius = damageHandler.RespawnCheckRadius;
			}
			if (damageHandler.RespawnSound != null)
			{
				vp_Respawner.SpawnSound = damageHandler.RespawnSound;
			}
		}
	}
}
