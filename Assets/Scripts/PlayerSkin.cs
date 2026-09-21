using System.Collections.Generic;
using UnityEngine;

public class PlayerSkin : TimerBehaviour
{
	public bool isPlayerActive;

	public Team PlayerTeam;

	public ControllerManager Controller;

	public Renderer PlayerRenderer;

	public Animator PlayerAnimator;

	public PlayerSkinRagdoll PlayerRagdoll;

	public SkinnedMeshAtlas PlayerAtlas;

	public Transform PlayerWeaponRoot;

	public AudioClip[] PlayerFoosteps;

	public AudioSource m_AudioSource;

	private bool isSound = true;

	private bool ShowDamage;

	public float PhotonSpeed = 10f;

	[Disabled]
	public Vector3 PhotonPosition;

	[Disabled]
	public Quaternion PhotonRotation;

	[Disabled]
	public bool Dead;

	private Vector2 Move;

	private float Rotate;

	private float RotateLast;

	private bool Grounded = true;

	[HideInInspector]
	public int PlayerSkinID = -1;

	[HideInInspector]
	public TPWeaponShooter SelectWeapon;

	private List<TPWeaponShooter> WeaponsList = new List<TPWeaponShooter>();

	private Transform m_Transform;

	private PlayerAnimator animator;

	private void Start()
	{
		Controller = base.transform.root.GetComponent<ControllerManager>();
		m_Transform = base.transform;
		EventManager.AddListener("UpdateOptions", UpdateOptions);
		UpdateOptions();
		vp_Timer.In(0.5f, CheckPosition, -1, 0.2f, GetTimer());
		vp_Timer.In(0.1f, UpdateRotate, -1, 0.03f, GetTimer());
		vp_Timer.In(0.1f, UpdateMove, -1, 0.05f, GetTimer());
		vp_Timer.In(0.1f, UpdatePosition, -1, 0.01f, GetTimer());
		animator = GetComponent<PlayerAnimator>();
		animator.SetDefault();
	}

	private void UpdateOptions()
	{
		isSound = SaveLoadManager.GetSound();
		ShowDamage = SaveLoadManager.GetShowDamage();
	}

	private void OnEnable()
	{
		isPlayerActive = true;
		if (PlayerSkinID != -1)
		{
			UpdateSkin();
		}
	}

	private void OnDisable()
	{
		isPlayerActive = false;
	}

	private void CheckPosition()
	{
		if ((m_Transform.position - PhotonPosition).sqrMagnitude > 3f)
		{
			m_Transform.position = PhotonPosition;
		}
	}

	private Vector3 VectorLerp(Vector3 from, Vector3 to, float t)
	{
		return new Vector3(from.x + (to.x - from.x) * t, from.y + (to.y - from.y) * t, from.z + (to.z - from.z) * t);
	}

	private void UpdatePosition()
	{
		if (PlayerRenderer.isVisible)
		{
			m_Transform.position = VectorLerp(m_Transform.position, PhotonPosition, Time.deltaTime * PhotonSpeed);
			m_Transform.rotation = Quaternion.Lerp(m_Transform.rotation, PhotonRotation, Time.deltaTime * PhotonSpeed);
		}
		else
		{
			m_Transform.position = PhotonPosition;
			m_Transform.rotation = PhotonRotation;
		}
	}

	private void UpdateMove()
	{
		if (isPlayerActive)
		{
			if (Move.x < 0.1f && Move.x > -0.1f)
			{
				Move.x = 0f;
			}
			if (Move.y < 0.1f && Move.y > -0.1f)
			{
				Move.y = 0f;
			}
			PlayerAnimator.SetFloat("MoveX", Move.x);
			PlayerAnimator.SetFloat("MoveY", Move.y);
			animator.move = ((Move.y < 0f) ? (0f - new Vector2(Move.x, Move.y).magnitude) : new Vector2(Move.x, Move.y).magnitude);
		}
	}

	private void UpdateRotate()
	{
		if (isPlayerActive)
		{
			RotateLast = Mathf.Lerp(RotateLast, Rotate, Time.deltaTime * PhotonSpeed);
			PlayerAnimator.SetFloat("Rotate", RotateLast);
		}
	}

	public void SetMove(Vector2 move)
	{
		if (isPlayerActive)
		{
			Move = move;
		}
	}

	public void SetRotate(float rotate)
	{
		if (isPlayerActive)
		{
			Rotate = rotate;
		}
	}

	public void SetGrounded(bool grounded)
	{
		if (isPlayerActive && Grounded != grounded)
		{
			PlayerAnimator.SetBool("Grounded", grounded);
			Grounded = grounded;
			animator.grounded = grounded;
		}
	}

	public void SetWeapon(WeaponType weaponType, int skinID)
	{
		PlayerAnimator.SetInteger("Weapon", (int)weaponType.WeaponAnim);
		if (SelectWeapon != null && SelectWeapon.name == weaponType.WeaponName)
		{
			return;
		}
		if (SelectWeapon != null)
		{
			SelectWeapon.Deactive();
		}
		TPWeaponShooter tPWeaponShooter = ContainsWeapon(weaponType.WeaponName);
		if (tPWeaponShooter == null)
		{
			GameObject gameObject = Utils.AddChild(weaponType.TpsPrefab, PlayerWeaponRoot, weaponType.TpsPrefab.transform.position, weaponType.TpsPrefab.transform.rotation);
			SelectWeapon = gameObject.GetComponent<TPWeaponShooter>();
			SelectWeapon.name = weaponType.WeaponName;
			WeaponsList.Add(SelectWeapon);
		}
		else
		{
			SelectWeapon = tPWeaponShooter;
		}
		SelectWeapon.Active();
		string skin = weaponType.WeaponID + "-" + skinID;
		for (int i = 0; i < SelectWeapon.WeaponAtlas.Length; i++)
		{
			if (SelectWeapon.WeaponAtlas[i].mSpriteName != skin)
			{
				MeshAtlas atlas = SelectWeapon.WeaponAtlas[i];
				vp_Timer.In(0.01f, delegate
				{
					atlas.spriteName = skin;
				}, GetTimer());
			}
		}
	}

	private TPWeaponShooter ContainsWeapon(string weaponName)
	{
		for (int i = 0; i < WeaponsList.Count; i++)
		{
			if (weaponName == WeaponsList[i].name)
			{
				return WeaponsList[i];
			}
		}
		return null;
	}

	public void Fire()
	{
		if (SelectWeapon != null)
		{
			SelectWeapon.Fire(PlayerRenderer.isVisible);
		}
	}

	public void Damage(DamageInfo damageInfo)
	{
		if (PlayerTeam != damageInfo.AttackerTeam && !Dead)
		{
			if (ShowDamage)
			{
				UIToast.Show(Localization.Get("Damage") + ": " + damageInfo.Damage, 2f);
			}
			UICrosshair.Hit();
			Controller.Damage(damageInfo);
		}
	}

	public void UpdateFoosteps()
	{
		if (isSound && !isPlayerActive)
		{
			AudioClip clip = PlayerFoosteps[Random.Range(0, PlayerFoosteps.Length)];
			m_AudioSource.pitch = Random.Range(1f, 1.5f);
			m_AudioSource.clip = clip;
			m_AudioSource.Play();
		}
	}

	public void SetPosition(Vector3 pos)
	{
		PhotonPosition = pos;
		if (m_Transform == null)
		{
			m_Transform = base.transform;
		}
		m_Transform.position = PhotonPosition;
	}

	public void UpdateSkin()
	{
		string playerSkin = (int)PlayerTeam + "-" + PlayerSkinID;
		PlayerSkinRagdoll component = base.gameObject.GetComponent<PlayerSkinRagdoll>();
		if (PlayerAtlas.mSpriteName != playerSkin)
		{
			vp_Timer.In(0.01f, delegate
			{
				PlayerAtlas.spriteName = playerSkin;
			}, GetTimer());
		}
	}
}
