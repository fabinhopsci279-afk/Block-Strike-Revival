using CodeStage.AntiCheat.ObscuredTypes;
using DG.Tweening;
using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
	public ObscuredInt Health = 100;

	public Team PlayerTeam;

	public ObscuredFloat PlayerSpeed = 0.18f;

	[Disabled]
	public bool Dead = true;

	[Disabled]
	public bool NoDamage;

	[Disabled]
	public bool Move = true;

	[Disabled]
	public bool Zombie;

	[Disabled]
	public bool Climb;

	[Disabled]
	public bool Water;

	[Header("UFPS")]
	public vp_FPController FPController;

	public vp_FPPlayerEventHandler FPlayerEvent;

	public vp_FPCamera FPCamera;

	[Header("Player")]
	public CharacterController mCharacterController;

	public Transform PlayerTransform;

	public Camera PlayerCamera;

	public PlayerWeapons PlayerWeapon;

	public ControllerManager Controller;

	public AudioClip[] PlayerFoosteps;

	[Disabled]
	public Vector2 MoveVector;

	[Disabled]
	public float RotateCamera;

	[Header("Others")]
	public AudioSource m_AudioSource;

	private bool isSound = true;

	private Tweener ZombieMove;

	private bool isCursor = true;

	public static PlayerInput instance;

	[HideInInspector]
	public bool Grounded => !Water && mCharacterController.isGrounded;

	private void Start()
	{
		instance = this;
		Controller = base.transform.root.GetComponent<ControllerManager>();
		UIGameManager.SetHealth(Health);
		EventManager.AddListener("UpdateOptions", UpdateOptions);
		UpdateOptions();
	}

	private void OnEnable()
	{
		vp_FPCamera fPCamera = FPCamera;
		fPCamera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Combine(fPCamera.BobStepCallback, new vp_FPCamera.BobStepDelegate(PlayFoosteps));
		if (GameManager.isStartDamage())
		{
			StartNoDamage();
		}
		if (Climb)
		{
			SetClimb(active: false);
		}
		if (Water)
		{
			SetWater(active: false);
		}
	}

	private void OnDisable()
	{
		vp_FPCamera fPCamera = FPCamera;
		fPCamera.BobStepCallback = (vp_FPCamera.BobStepDelegate)Delegate.Remove(fPCamera.BobStepCallback, new vp_FPCamera.BobStepDelegate(PlayFoosteps));
	}

	private void Update()
	{
		UpdateMove();
		UpdateLook();
		UpdateJump();
		UpdateCursor();
	}

	private void UpdateCursor()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.P) || UnityEngine.Input.GetKeyDown(KeyCode.Tab))
		{
			isCursor = !isCursor;
		}
		if (Cursor.visible != isCursor)
		{
			Cursor.visible = isCursor;
		}
		if (isCursor)
		{
			Cursor.lockState = CursorLockMode.None;
		}
		else
		{
			Cursor.lockState = CursorLockMode.Locked;
		}
	}

	private void UpdateMove()
	{
		if (Move)
		{
			MoveVector = new Vector2(InputManager.GetAxis("Horizontal"), InputManager.GetAxis("Vertical"));
			float y = 0f;
			float x = 0f;
			if (UnityEngine.Input.GetKey(KeyCode.W))
			{
				y = 1f;
			}
			if (UnityEngine.Input.GetKey(KeyCode.S))
			{
				y = -1f;
			}
			if (UnityEngine.Input.GetKey(KeyCode.A))
			{
				x = -1f;
			}
			if (UnityEngine.Input.GetKey(KeyCode.D))
			{
				x = 1f;
			}
			MoveVector = new Vector2(x, y);
			if (Climb || Water)
			{
				MoveVector /= 2.5f;
			}
			FPlayerEvent.InputMoveVector.Set(MoveVector);
		}
	}

	private void UpdateLook()
	{
		Vector2 vector = new Vector2(InputManager.GetAxis("Mouse X"), InputManager.GetAxis("Mouse Y"));
		vector = new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y"));
		if (PlayerWeapon.isScope)
		{
			vector *= PlayerWeapon.GetSelectedWeaponData().RifleScopeSensitivity;
		}
		FPCamera.UpdateLook(vector);
		RotateCamera = 0f - (float)Math.Round(FPCamera.Pitch / 60f, 1);
	}

	private void UpdateJump()
	{
		if (InputManager.GetButton("Jump") && !Climb && !Water)
		{
			FPlayerEvent.Jump.TryStart();
		}
		else
		{
			FPlayerEvent.Jump.Stop();
		}
	}

	public void Damage(DamageInfo damageInfo)
	{
		if (Dead || NoDamage)
		{
			return;
		}
		Health = (int)Health - damageInfo.Damage;
		Health = Mathf.Clamp(Health, 0, Zombie ? 2000 : 100);
		UIGameManager.SetHealth(Health);
		if (damageInfo.AttackPosition != Vector3.zero)
		{
			UIDamage.Damage(damageInfo.AttackPosition, FPCamera.Transform);
		}
		if ((int)Health == 0)
		{
			GameManager.OnDeadPlayer(damageInfo);
			PlayerWeapon.DeactiveScope();
			return;
		}
		if (Zombie)
		{
			FPController.MotorAcceleration = 0.15f;
			if (ZombieMove != null && ZombieMove.IsActive())
			{
				ZombieMove.Kill();
			}
			ZombieMove = DOTween.To(() => FPController.MotorAcceleration, delegate(float x)
			{
				FPController.MotorAcceleration = x;
			}, 0.2f, 1.5f);
		}
		FPCamera.AddRollForce(UnityEngine.Random.Range(-2, 2));
	}

	private void PlayFoosteps()
	{
		if (!Water && !Climb)
		{
			Controller.UpdateFoosteps();
		}
	}

	public void UpdateFoosteps()
	{
		if (isSound)
		{
			AudioClip clip = PlayerFoosteps[UnityEngine.Random.Range(0, PlayerFoosteps.Length)];
			m_AudioSource.pitch = UnityEngine.Random.Range(1f, 1.5f);
			m_AudioSource.clip = clip;
			m_AudioSource.Play();
		}
	}

	public void StartNoDamage()
	{
		NoDamage = true;
		vp_Timer.In(GameManager.GetStartDamageTime(), delegate
		{
			NoDamage = false;
		});
	}

	private void UpdateOptions()
	{
		vp_Timer.In(0.1f, delegate
		{
			float num = SaveLoadManager.GetSensitivity() * 16f;
			FPCamera.MouseSensitivity = new Vector2(num, num);
			isSound = SaveLoadManager.GetSound();
		});
	}

	public void SetHealth(int health)
	{
		Health = health;
		UIGameManager.SetHealth(Health);
	}

	public void SetMove(bool move)
	{
		Move = move;
	}

	public void UpdatePlayerSpeed(float speed)
	{
		PlayerSpeed = speed;
		FPController.MotorAcceleration = PlayerSpeed;
	}

	public void SetPlayerSpeed(float mass)
	{
		FPController.MotorAcceleration = (float)PlayerSpeed - mass;
	}

	public void SetClimb(bool active)
	{
		Climb = active;
		if (Climb)
		{
			FPController.Stop();
			FPController.PhysicsGravityModifier = 0f;
			FPController.MotorFreeFly = true;
		}
		else
		{
			FPController.PhysicsGravityModifier = 0.2f;
			FPController.MotorFreeFly = false;
		}
	}

	public void SetWater(bool active)
	{
		Water = active;
		if (Water)
		{
			FPController.PhysicsGravityModifier = 0.01f;
			FPController.MotorFreeFly = true;
		}
		else
		{
			FPController.PhysicsGravityModifier = 0.2f;
			FPController.MotorFreeFly = false;
		}
	}
}
