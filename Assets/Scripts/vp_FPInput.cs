using UnityEngine;

public class vp_FPInput : MonoBehaviour
{
	public vp_FPPlayerEventHandler Player;

	protected vp_FPCamera m_FPCamera;

	public Rect[] MouseCursorZones;

	public bool ForceCursor;

	protected Vector2 m_MousePos = Vector2.zero;

	protected bool m_AllowGameplayInput = true;

	public bool AllowGameplayInput
	{
		get
		{
			return m_AllowGameplayInput;
		}
		set
		{
			m_AllowGameplayInput = value;
		}
	}

	public Vector2 MousePos => m_MousePos;

	public Vector2 MouseSensitivity
	{
		get
		{
			return m_FPCamera.MouseSensitivity;
		}
		set
		{
			m_FPCamera.MouseSensitivity = value;
		}
	}

	public int MouseSmoothSteps
	{
		get
		{
			return m_FPCamera.MouseSmoothSteps;
		}
		set
		{
			m_FPCamera.MouseSmoothSteps = (int)Mathf.Clamp(value, 1f, 10f);
		}
	}

	public float MouseSmoothWeight
	{
		get
		{
			return m_FPCamera.MouseSmoothWeight;
		}
		set
		{
			m_FPCamera.MouseSmoothWeight = Mathf.Clamp01(value);
		}
	}

	public bool MouseAcceleration
	{
		get
		{
			return m_FPCamera.MouseAcceleration;
		}
		set
		{
			m_FPCamera.MouseAcceleration = value;
		}
	}

	public float MouseAccelerationThreshold
	{
		get
		{
			return m_FPCamera.MouseAccelerationThreshold;
		}
		set
		{
			m_FPCamera.MouseAccelerationThreshold = value;
		}
	}

	protected virtual bool OnValue_AllowGameplayInput
	{
		get
		{
			return m_AllowGameplayInput;
		}
		set
		{
			m_AllowGameplayInput = value;
		}
	}

	protected virtual bool OnValue_Pause
	{
		get
		{
			return vp_TimeUtility.Paused;
		}
		set
		{
			vp_TimeUtility.Paused = value;
		}
	}

	protected virtual void Start()
	{
	}

	protected virtual void Update()
	{
		UpdateCursorLock();
		UpdatePause();
		if (!Player.Pause.Get() && m_AllowGameplayInput)
		{
			InputInteract();
			InputMove();
			InputRun();
			InputJump();
			InputCrouch();
			InputAttack();
			InputZoom();
			InputReload();
			InputSetWeapon();
		}
	}

	protected virtual void InputInteract()
	{
		if (vp_Input.GetButtonDown("Interact"))
		{
			Player.Interact.TryStart();
		}
		else
		{
			Player.Interact.TryStop();
		}
	}

	protected virtual void InputMove()
	{
		Player.InputMoveVector.Set(new Vector2(vp_Input.GetAxisRaw("Horizontal"), vp_Input.GetAxisRaw("Vertical")));
	}

	protected virtual void InputRun()
	{
		if (vp_Input.GetButton("Run"))
		{
			Player.Run.TryStart();
		}
		else
		{
			Player.Run.TryStop();
		}
	}

	protected virtual void InputJump()
	{
		if (vp_Input.GetButton("Jump"))
		{
			Player.Jump.TryStart();
		}
		else
		{
			Player.Jump.Stop();
		}
	}

	protected virtual void InputCrouch()
	{
		if (vp_Input.GetButton("Crouch"))
		{
			Player.Crouch.TryStart();
		}
		else
		{
			Player.Crouch.TryStop();
		}
	}

	protected virtual void InputZoom()
	{
		if (vp_Input.GetButton("Zoom"))
		{
			Player.Zoom.TryStart();
		}
		else
		{
			Player.Zoom.TryStop();
		}
	}

	protected virtual void InputAttack()
	{
		if (Screen.lockCursor)
		{
			if (vp_Input.GetButton("Attack"))
			{
				Player.Attack.TryStart();
			}
			else
			{
				Player.Attack.TryStop();
			}
		}
	}

	protected virtual void InputReload()
	{
		if (vp_Input.GetButtonDown("Reload"))
		{
			Player.Reload.TryStart();
		}
	}

	protected virtual void InputSetWeapon()
	{
		if (vp_Input.GetButtonDown("SetPrevWeapon"))
		{
			Player.SetPrevWeapon.Try();
		}
		if (vp_Input.GetButtonDown("SetNextWeapon"))
		{
			Player.SetNextWeapon.Try();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha1))
		{
			Player.SetWeapon.TryStart(1);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha2))
		{
			Player.SetWeapon.TryStart(2);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha3))
		{
			Player.SetWeapon.TryStart(3);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha4))
		{
			Player.SetWeapon.TryStart(4);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha5))
		{
			Player.SetWeapon.TryStart(5);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha6))
		{
			Player.SetWeapon.TryStart(6);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha7))
		{
			Player.SetWeapon.TryStart(7);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha8))
		{
			Player.SetWeapon.TryStart(8);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha9))
		{
			Player.SetWeapon.TryStart(9);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Alpha0))
		{
			Player.SetWeapon.TryStart(10);
		}
		if (vp_Input.GetButtonDown("ClearWeapon"))
		{
			Player.SetWeapon.TryStart(0);
		}
	}

	protected virtual void UpdatePause()
	{
		if (vp_Input.GetButtonDown("Pause"))
		{
			Player.Pause.Set(!Player.Pause.Get());
		}
	}

	protected virtual void UpdateCursorLock()
	{
		ref Vector2 mousePos = ref m_MousePos;
		Vector3 mousePosition = UnityEngine.Input.mousePosition;
		mousePos.x = mousePosition.x;
		ref Vector2 mousePos2 = ref m_MousePos;
		float num = Screen.height;
		Vector3 mousePosition2 = UnityEngine.Input.mousePosition;
		mousePos2.y = num - mousePosition2.y;
		if (ForceCursor)
		{
			Screen.lockCursor = false;
			return;
		}
		if (Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2))
		{
			if (MouseCursorZones.Length > 0)
			{
				Rect[] mouseCursorZones = MouseCursorZones;
				int num2 = 0;
				while (num2 < mouseCursorZones.Length)
				{
					Rect rect = mouseCursorZones[num2];
					if (!rect.Contains(m_MousePos))
					{
						num2++;
						continue;
					}
					goto IL_00a7;
				}
			}
			Screen.lockCursor = true;
		}
		goto IL_00c8;
		IL_00a7:
		Screen.lockCursor = false;
		goto IL_00c8;
		IL_00c8:
		if (vp_Input.GetButtonUp("Accept1") || vp_Input.GetButtonUp("Accept2"))
		{
			Screen.lockCursor = !Screen.lockCursor;
		}
	}

	protected virtual void Awake()
	{
		Player = (vp_FPPlayerEventHandler)base.transform.root.GetComponentInChildren(typeof(vp_FPPlayerEventHandler));
		m_FPCamera = GetComponentInChildren<vp_FPCamera>();
	}

	protected virtual void OnEnable()
	{
		if (Player != null)
		{
			Player.Register(this);
		}
	}

	protected virtual void OnDisable()
	{
		if (Player != null)
		{
			Player.Unregister(this);
		}
	}
}
