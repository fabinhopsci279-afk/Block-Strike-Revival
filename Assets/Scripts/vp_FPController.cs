using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(vp_FPPlayerEventHandler))]
public class vp_FPController : vp_Component
{
	private vp_PlayerEventHandler m_Player;

	private CharacterController m_CharacterController;

	protected Vector3 m_FixedPosition = Vector3.zero;

	protected Vector3 m_SmoothPosition = Vector3.zero;

	protected bool m_Grounded;

	protected bool m_HeadContact;

	protected RaycastHit m_GroundHit;

	protected RaycastHit m_LastGroundHit;

	protected RaycastHit m_CeilingHit;

	protected RaycastHit m_WallHit;

	protected float m_FallImpact;

	protected Terrain m_CurrentTerrain;

	protected vp_SurfaceIdentifier m_CurrentSurface;

	public float MotorAcceleration = 0.18f;

	public float MotorDamping = 0.17f;

	public float MotorBackwardsSpeed = 0.65f;

	public float MotorAirSpeed = 0.35f;

	public float MotorSlopeSpeedUp = 1f;

	public float MotorSlopeSpeedDown = 1f;

	public bool MotorFreeFly;

	protected Vector3 m_MoveDirection = Vector3.zero;

	protected float m_SlopeFactor = 1f;

	protected Vector3 m_MotorThrottle = Vector3.zero;

	protected float m_MotorAirSpeedModifier = 1f;

	protected float m_CurrentAntiBumpOffset;

	protected Vector2 m_MoveVector = Vector2.zero;

	public float MotorJumpForce = 0.18f;

	public float MotorJumpForceDamping = 0.08f;

	public float MotorJumpForceHold = 0.003f;

	public float MotorJumpForceHoldDamping = 0.5f;

	protected int m_MotorJumpForceHoldSkipFrames;

	protected float m_MotorJumpForceAcc;

	protected bool m_MotorJumpDone = true;

	protected float m_FallSpeed;

	protected float m_LastFallSpeed;

	protected float m_HighestFallSpeed;

	public float PhysicsForceDamping = 0.05f;

	public float PhysicsPushForce = 5f;

	public float PhysicsGravityModifier = 0.2f;

	public float PhysicsSlopeSlideLimit = 30f;

	public float PhysicsSlopeSlidiness = 0.15f;

	public float PhysicsWallBounce;

	public float PhysicsWallFriction;

	public bool PhysicsHasCollisionTrigger = true;

	protected GameObject m_Trigger;

	protected Vector3 m_ExternalForce = Vector3.zero;

	protected Vector3[] m_SmoothForceFrame = new Vector3[120];

	protected bool m_Slide;

	protected bool m_SlideFast;

	protected float m_SlideFallSpeed;

	protected float m_OnSteepGroundSince;

	protected float m_SlopeSlideSpeed;

	protected Vector3 m_PredictedPos = Vector3.zero;

	protected Vector3 m_PrevPos = Vector3.zero;

	protected Vector3 m_PrevDir = Vector3.zero;

	protected Vector3 m_NewDir = Vector3.zero;

	protected float m_ForceImpact;

	protected float m_ForceMultiplier;

	protected Vector3 CapsuleBottom = Vector3.zero;

	protected Vector3 CapsuleTop = Vector3.zero;

	protected float m_SkinWidth = 0.08f;

	protected Transform m_Platform;

	protected Vector3 m_PositionOnPlatform = Vector3.zero;

	protected float m_LastPlatformAngle;

	protected Vector3 m_LastPlatformPos = Vector3.zero;

	protected float m_NormalHeight;

	protected Vector3 m_NormalCenter = Vector3.zero;

	protected float m_CrouchHeight;

	protected Vector3 m_CrouchCenter = Vector3.zero;

	protected vp_PlayerEventHandler Player
	{
		get
		{
			if (m_Player == null && EventHandler != null)
			{
				m_Player = (vp_PlayerEventHandler)EventHandler;
			}
			return m_Player;
		}
	}

	public CharacterController CharacterController
	{
		get
		{
			if (m_CharacterController == null)
			{
				m_CharacterController = base.gameObject.GetComponent<CharacterController>();
			}
			return m_CharacterController;
		}
	}

	public Vector3 SmoothPosition => m_SmoothPosition;

	public bool Grounded => m_Grounded;

	public bool HeadContact => m_HeadContact;

	public Vector3 GroundNormal => m_GroundHit.normal;

	public float GroundAngle => Vector3.Angle(m_GroundHit.normal, Vector3.up);

	public Transform GroundTransform => m_GroundHit.transform;

	protected virtual Vector3 OnValue_Position
	{
		get
		{
			return base.Transform.position;
		}
		set
		{
			SetPosition(value);
		}
	}

	protected virtual Vector2 OnValue_InputMoveVector
	{
		get
		{
			return m_MoveVector;
		}
		set
		{
			m_MoveVector = ((!(value.y >= 0f)) ? (value.normalized * MotorBackwardsSpeed) : value.normalized);
		}
	}

	protected virtual Vector3 OnValue_Velocity => CharacterController.velocity;

	protected virtual float OnValue_StepOffset => CharacterController.stepOffset;

	protected virtual float OnValue_SlopeLimit => CharacterController.slopeLimit;

	protected virtual float OnValue_Radius => CharacterController.radius;

	protected virtual float OnValue_Height => CharacterController.height;

	protected virtual Vector3 OnValue_MotorThrottle
	{
		get
		{
			return m_MotorThrottle;
		}
		set
		{
			m_MotorThrottle = value;
		}
	}

	protected virtual bool OnValue_MotorJumpDone => m_MotorJumpDone;

	protected virtual float OnValue_FallSpeed
	{
		get
		{
			return m_FallSpeed;
		}
		set
		{
			m_FallSpeed = value;
		}
	}

	protected virtual Transform OnValue_Platform
	{
		get
		{
			return m_Platform;
		}
		set
		{
			m_Platform = value;
		}
	}

	protected virtual Texture OnValue_GroundTexture
	{
		get
		{
			if (GroundTransform == null)
			{
				return null;
			}
			if (GroundTransform.GetComponent<Renderer>() == null && m_CurrentTerrain == null)
			{
				return null;
			}
			int num = -1;
			if (m_CurrentTerrain != null)
			{
				num = vp_FootstepManager.GetMainTerrainTexture(Player.Position.Get(), m_CurrentTerrain);
				if (num > m_CurrentTerrain.terrainData.splatPrototypes.Length - 1)
				{
					return null;
				}
			}
			return (m_CurrentTerrain == null) ? GroundTransform.GetComponent<Renderer>().material.mainTexture : m_CurrentTerrain.terrainData.splatPrototypes[num].texture;
		}
	}

	protected virtual vp_SurfaceIdentifier OnValue_SurfaceType => m_CurrentSurface;

	protected override void Awake()
	{
		base.Awake();
		m_NormalHeight = CharacterController.height;
		CharacterController.center = (m_NormalCenter = new Vector3(0f, m_NormalHeight * 0.5f, 0f));
		CharacterController.radius = m_NormalHeight * 0.25f;
		m_CrouchHeight = m_NormalHeight * 0.5f;
		m_CrouchCenter = m_NormalCenter * 0.5f;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
		vp_TargetEvent<Vector3>.Register(m_Transform, "ForceImpact", AddForce);
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		vp_TargetEvent<Vector3>.Unregister(m_Root, "ForceImpact", AddForce);
		m_Platform = null;
	}

	protected override void Start()
	{
		base.Start();
		SetPosition(base.Transform.position);
		if (PhysicsHasCollisionTrigger)
		{
			m_Trigger = new GameObject("Trigger");
			m_Trigger.transform.parent = m_Transform;
			CapsuleCollider capsuleCollider = m_Trigger.AddComponent<CapsuleCollider>();
			capsuleCollider.isTrigger = true;
			capsuleCollider.radius = CharacterController.radius + m_SkinWidth;
			capsuleCollider.height = CharacterController.height + m_SkinWidth * 2f;
			capsuleCollider.center = CharacterController.center;
			m_Trigger.layer = 30;
			m_Trigger.transform.localPosition = Vector3.zero;
		}
	}

	protected override void Update()
	{
		base.Update();
		SmoothMove();
	}

	protected override void FixedUpdate()
	{
		UpdateMotor();
		UpdateJump();
		UpdateForces();
		FixedMove();
		UpdateCollisions();
		UpdatePlatformMove();
		m_PrevPos = base.Transform.position;
	}

	protected virtual void UpdateMotor()
	{
		if (!MotorFreeFly)
		{
			UpdateThrottleWalk();
		}
		else
		{
			UpdateThrottleFree();
		}
		m_MotorThrottle = vp_MathUtility.SnapToZero(m_MotorThrottle);
	}

	protected virtual void UpdateThrottleWalk()
	{
		UpdateSlopeFactor();
		m_MotorAirSpeedModifier = (m_Grounded ? 1f : MotorAirSpeed);
		m_MotorThrottle += m_MoveVector.y * (base.Transform.TransformDirection(Vector3.forward * (MotorAcceleration * 0.1f) * m_MotorAirSpeedModifier) * m_SlopeFactor);
		m_MotorThrottle += m_MoveVector.x * (base.Transform.TransformDirection(Vector3.right * (MotorAcceleration * 0.1f) * m_MotorAirSpeedModifier) * m_SlopeFactor);
		m_MotorThrottle.x /= 1f + MotorDamping * m_MotorAirSpeedModifier * Time.timeScale;
		m_MotorThrottle.z /= 1f + MotorDamping * m_MotorAirSpeedModifier * Time.timeScale;
	}

	protected virtual void UpdateThrottleFree()
	{
		m_MotorThrottle += m_MoveVector.y * base.Transform.TransformDirection(base.Transform.InverseTransformDirection(Player.Forward.Get()) * (MotorAcceleration * 0.1f));
		m_MotorThrottle += m_MoveVector.x * base.Transform.TransformDirection(Vector3.right * (MotorAcceleration * 0.1f));
		m_MotorThrottle.x /= 1f + MotorDamping * Time.timeScale;
		m_MotorThrottle.z /= 1f + MotorDamping * Time.timeScale;
	}

	protected virtual void UpdateJump()
	{
		if (m_HeadContact)
		{
			Player.Jump.Stop(1f);
		}
		if (!MotorFreeFly)
		{
			UpdateJumpForceWalk();
		}
		else
		{
			UpdateJumpForceFree();
		}
		m_MotorThrottle.y += m_MotorJumpForceAcc * Time.timeScale;
		m_MotorJumpForceAcc /= 1f + MotorJumpForceHoldDamping * Time.timeScale;
		m_MotorThrottle.y /= 1f + MotorJumpForceDamping * Time.timeScale;
	}

	protected virtual void UpdateJumpForceWalk()
	{
		if (!Player.Jump.Active || m_Grounded)
		{
			return;
		}
		if (m_MotorJumpForceHoldSkipFrames > 2)
		{
			Vector3 vector = Player.Velocity.Get();
			if (vector.y >= 0f)
			{
				m_MotorJumpForceAcc += MotorJumpForceHold;
			}
		}
		else
		{
			m_MotorJumpForceHoldSkipFrames++;
		}
	}

	protected virtual void UpdateJumpForceFree()
	{
		if (Player.Jump.Active && Player.Crouch.Active)
		{
			return;
		}
		if (Player.Jump.Active)
		{
			m_MotorJumpForceAcc += MotorJumpForceHold;
		}
		else if (Player.Crouch.Active)
		{
			m_MotorJumpForceAcc -= MotorJumpForceHold;
			if (Grounded && CharacterController.height == m_NormalHeight)
			{
				CharacterController.height = m_CrouchHeight;
				CharacterController.center = m_CrouchCenter;
			}
		}
	}

	protected virtual void UpdateForces()
	{
		if (m_Grounded && m_FallSpeed <= 0f)
		{
			Vector3 gravity = Physics.gravity;
			m_FallSpeed = gravity.y * (PhysicsGravityModifier * 0.002f) * vp_TimeUtility.AdjustedTimeScale;
		}
		else
		{
			float fallSpeed = m_FallSpeed;
			Vector3 gravity2 = Physics.gravity;
			m_FallSpeed = fallSpeed + gravity2.y * (PhysicsGravityModifier * 0.002f) * vp_TimeUtility.AdjustedTimeScale;
		}
		if (m_FallSpeed < m_LastFallSpeed)
		{
			m_HighestFallSpeed = m_FallSpeed;
		}
		m_LastFallSpeed = m_FallSpeed;
		if (m_SmoothForceFrame[0] != Vector3.zero)
		{
			AddForceInternal(m_SmoothForceFrame[0]);
			for (int i = 0; i < 120; i++)
			{
				m_SmoothForceFrame[i] = ((i < 119) ? m_SmoothForceFrame[i + 1] : Vector3.zero);
				if (m_SmoothForceFrame[i] == Vector3.zero)
				{
					break;
				}
			}
		}
		m_ExternalForce /= 1f + PhysicsForceDamping * vp_TimeUtility.AdjustedTimeScale;
	}

	protected virtual void UpdateSliding()
	{
		bool slideFast = m_SlideFast;
		bool slide = m_Slide;
		m_Slide = false;
		if (!m_Grounded)
		{
			m_OnSteepGroundSince = 0f;
			m_SlideFast = false;
		}
		else if (GroundAngle > PhysicsSlopeSlideLimit)
		{
			m_Slide = true;
			if (GroundAngle <= Player.SlopeLimit.Get())
			{
				m_SlopeSlideSpeed = Mathf.Max(m_SlopeSlideSpeed, PhysicsSlopeSlidiness * 0.01f);
				m_OnSteepGroundSince = 0f;
				m_SlideFast = false;
				m_SlopeSlideSpeed = ((!(Mathf.Abs(m_SlopeSlideSpeed) >= 0.0001f)) ? 0f : (m_SlopeSlideSpeed / (1f + 0.05f * vp_TimeUtility.AdjustedTimeScale)));
			}
			else
			{
				if (m_SlopeSlideSpeed > 0.01f)
				{
					m_SlideFast = true;
				}
				if (m_OnSteepGroundSince == 0f)
				{
					m_OnSteepGroundSince = Time.time;
				}
				m_SlopeSlideSpeed += PhysicsSlopeSlidiness * 0.01f * ((Time.time - m_OnSteepGroundSince) * 0.125f) * vp_TimeUtility.AdjustedTimeScale;
				m_SlopeSlideSpeed = Mathf.Max(PhysicsSlopeSlidiness * 0.01f, m_SlopeSlideSpeed);
			}
			AddForce(Vector3.Cross(Vector3.Cross(GroundNormal, Vector3.down), GroundNormal) * m_SlopeSlideSpeed * vp_TimeUtility.AdjustedTimeScale);
		}
		else
		{
			m_OnSteepGroundSince = 0f;
			m_SlideFast = false;
			m_SlopeSlideSpeed = 0f;
		}
		if (m_MotorThrottle != Vector3.zero)
		{
			m_Slide = false;
		}
		if (m_SlideFast)
		{
			Vector3 position = base.Transform.position;
			m_SlideFallSpeed = position.y;
		}
		else if (slideFast && !Grounded)
		{
			Vector3 position2 = base.Transform.position;
			m_FallSpeed = position2.y - m_SlideFallSpeed;
		}
		if (slide != m_Slide)
		{
			Player.SetState("Slide", m_Slide);
		}
		if (slideFast != m_SlideFast)
		{
			Player.SetState("SlideFast", m_SlideFast);
		}
	}

	protected virtual void FixedMove()
	{
		m_MoveDirection = Vector3.zero;
		m_MoveDirection += m_ExternalForce;
		m_MoveDirection += m_MotorThrottle;
		m_MoveDirection.y += m_FallSpeed;
		m_CurrentAntiBumpOffset = 0f;
		if (m_Grounded && m_MotorThrottle.y <= 0.001f)
		{
			m_CurrentAntiBumpOffset = Mathf.Max(Player.StepOffset.Get(), Vector3.Scale(m_MoveDirection, Vector3.one - Vector3.up).magnitude);
			m_MoveDirection += m_CurrentAntiBumpOffset * Vector3.down;
		}
		m_PredictedPos = base.Transform.position + vp_MathUtility.NaNSafeVector3(m_MoveDirection * base.Delta * Time.timeScale);
		if (m_Platform != null && m_PositionOnPlatform != Vector3.zero)
		{
			Player.Move.Send(vp_MathUtility.NaNSafeVector3(m_Platform.TransformPoint(m_PositionOnPlatform) - m_Transform.position));
		}
		Player.Move.Send(vp_MathUtility.NaNSafeVector3(m_MoveDirection * base.Delta * Time.timeScale));
		if (Player.Dead.Active)
		{
			m_MoveVector = Vector2.zero;
			return;
		}
		Physics.SphereCast(new Ray(base.Transform.position + Vector3.up * Player.Radius.Get(), Vector3.down), Player.Radius.Get(), out m_GroundHit, m_SkinWidth + 0.001f, -1744830485);
		m_Grounded = (m_GroundHit.collider != null);
		m_HeadContact = false;
		if (m_GroundHit.transform == null && m_LastGroundHit.transform != null)
		{
			if (m_Platform != null && m_PositionOnPlatform != Vector3.zero)
			{
				AddForce(m_Platform.position - m_LastPlatformPos);
				m_Platform = null;
			}
			if (m_CurrentAntiBumpOffset != 0f)
			{
				Player.Move.Send(vp_MathUtility.NaNSafeVector3(m_CurrentAntiBumpOffset * Vector3.up) * base.Delta * Time.timeScale);
				m_PredictedPos += vp_MathUtility.NaNSafeVector3(m_CurrentAntiBumpOffset * Vector3.up) * base.Delta * Time.timeScale;
				m_MoveDirection += m_CurrentAntiBumpOffset * Vector3.up;
			}
		}
	}

	protected virtual void SmoothMove()
	{
		if (Time.timeScale == 0f)
		{
			return;
		}
		m_FixedPosition = base.Transform.position;
		base.Transform.position = m_SmoothPosition;
		Player.Move.Send(vp_MathUtility.NaNSafeVector3(m_MoveDirection * base.Delta * Time.timeScale));
		m_SmoothPosition = base.Transform.position;
		base.Transform.position = m_FixedPosition;
		if (Vector3.Distance(base.Transform.position, m_SmoothPosition) > Player.Radius.Get())
		{
			m_SmoothPosition = base.Transform.position;
		}
		if (m_Platform != null)
		{
			float y = m_LastPlatformPos.y;
			Vector3 position = m_Platform.position;
			if (!(y < position.y))
			{
				float y2 = m_LastPlatformPos.y;
				Vector3 position2 = m_Platform.position;
				if (!(y2 > position2.y))
				{
					goto IL_0148;
				}
			}
			ref Vector3 smoothPosition = ref m_SmoothPosition;
			Vector3 position3 = base.Transform.position;
			smoothPosition.y = position3.y;
		}
		goto IL_0148;
		IL_0148:
		m_SmoothPosition = Vector3.Lerp(m_SmoothPosition, base.Transform.position, Time.deltaTime);
	}

	protected virtual void UpdateCollisions()
	{
		if (m_GroundHit.transform != null && m_GroundHit.transform != m_LastGroundHit.transform)
		{
			if (!MotorFreeFly)
			{
				m_FallImpact = (0f - m_HighestFallSpeed) * Time.timeScale;
			}
			else
			{
				Vector3 velocity = CharacterController.velocity;
				m_FallImpact = (0f - velocity.y * 0.01f) * Time.timeScale;
			}
			ref Vector3 smoothPosition = ref m_SmoothPosition;
			Vector3 position = base.Transform.position;
			smoothPosition.y = position.y;
			DeflectDownForce();
			m_HighestFallSpeed = 0f;
			Player.FallImpact.Send(m_FallImpact);
			m_MotorThrottle.y = 0f;
			m_MotorJumpForceAcc = 0f;
			m_MotorJumpForceHoldSkipFrames = 0;
			if (m_GroundHit.collider.gameObject.layer == 28)
			{
				m_Platform = m_GroundHit.transform;
				Vector3 eulerAngles = m_Platform.eulerAngles;
				m_LastPlatformAngle = eulerAngles.y;
			}
			else
			{
				m_Platform = null;
			}
			vp_SurfaceIdentifier component = m_GroundHit.transform.GetComponent<vp_SurfaceIdentifier>();
			if (component != null)
			{
				m_CurrentSurface = component;
			}
			else
			{
				m_CurrentSurface = null;
			}
		}
		else
		{
			m_FallImpact = 0f;
		}
		m_LastGroundHit = m_GroundHit;
		float y = m_PredictedPos.y;
		Vector3 position2 = base.Transform.position;
		if (y > position2.y && (m_ExternalForce.y > 0f || m_MotorThrottle.y > 0f))
		{
			DeflectUpForce();
		}
		float x = m_PredictedPos.x;
		Vector3 position3 = base.Transform.position;
		if (x == position3.x)
		{
			float z = m_PredictedPos.z;
			Vector3 position4 = base.Transform.position;
			if (z == position4.z || !(m_ExternalForce != Vector3.zero))
			{
				return;
			}
		}
		DeflectHorizontalForce();
	}

	protected virtual void UpdateSlopeFactor()
	{
		if (!m_Grounded)
		{
			m_SlopeFactor = 1f;
			return;
		}
		m_SlopeFactor = 1f + (1f - Vector3.Angle(m_GroundHit.normal, m_MotorThrottle) / 90f);
		if (Mathf.Abs(1f - m_SlopeFactor) < 0.01f)
		{
			m_SlopeFactor = 1f;
		}
		else if (m_SlopeFactor > 1f)
		{
			if (MotorSlopeSpeedDown == 1f)
			{
				m_SlopeFactor = 1f / m_SlopeFactor;
				m_SlopeFactor *= 1.2f;
			}
			else
			{
				m_SlopeFactor *= MotorSlopeSpeedDown;
			}
		}
		else
		{
			if (MotorSlopeSpeedUp == 1f)
			{
				m_SlopeFactor *= 1.2f;
			}
			else
			{
				m_SlopeFactor *= MotorSlopeSpeedUp;
			}
			m_SlopeFactor = ((!(GroundAngle <= Player.SlopeLimit.Get())) ? 0f : m_SlopeFactor);
		}
	}

	protected virtual void UpdatePlatformMove()
	{
		if (!(m_Platform == null))
		{
			m_PositionOnPlatform = m_Platform.InverseTransformPoint(m_Transform.position);
			vp_Value<Vector2>.Setter<Vector2> set = Player.Rotation.Set;
			Vector2 vector = Player.Rotation.Get();
			float x = vector.x;
			Vector2 vector2 = Player.Rotation.Get();
			float y = vector2.y;
			Vector3 eulerAngles = m_Platform.eulerAngles;
			set(new Vector2(x, y - Mathf.DeltaAngle(eulerAngles.y, m_LastPlatformAngle)));
			Vector3 eulerAngles2 = m_Platform.eulerAngles;
			m_LastPlatformAngle = eulerAngles2.y;
			m_LastPlatformPos = m_Platform.position;
			m_SmoothPosition = base.Transform.position;
		}
	}

	public virtual void SetPosition(Vector3 position)
	{
		base.Transform.position = position;
		m_PrevPos = position;
		m_SmoothPosition = position;
	}

	protected virtual void AddForceInternal(Vector3 force)
	{
		m_ExternalForce += force;
	}

	public virtual void AddForce(float x, float y, float z)
	{
		AddForce(new Vector3(x, y, z));
	}

	public virtual void AddForce(Vector3 force)
	{
		if (Time.timeScale >= 1f)
		{
			AddForceInternal(force);
		}
		else
		{
			AddSoftForce(force, 1f);
		}
	}

	public virtual void AddSoftForce(Vector3 force, float frames)
	{
		force /= Time.timeScale;
		frames = Mathf.Clamp(frames, 1f, 120f);
		AddForceInternal(force / frames);
		for (int i = 0; i < Mathf.RoundToInt(frames) - 1; i++)
		{
			m_SmoothForceFrame[i] += force / frames;
		}
	}

	public virtual void StopSoftForce()
	{
		for (int i = 0; i < 120 && !(m_SmoothForceFrame[i] == Vector3.zero); i++)
		{
			m_SmoothForceFrame[i] = Vector3.zero;
		}
	}

	public virtual void Stop()
	{
		Player.Move.Send(Vector3.zero);
		m_MotorThrottle = Vector3.zero;
		m_ExternalForce = Vector3.zero;
		StopSoftForce();
		m_MoveVector = Vector2.zero;
		m_FallSpeed = 0f;
		m_LastFallSpeed = 0f;
		m_HighestFallSpeed = 0f;
		m_SmoothPosition = base.Transform.position;
	}

	public virtual void DeflectDownForce()
	{
		if (GroundAngle > PhysicsSlopeSlideLimit)
		{
			m_SlopeSlideSpeed = m_FallImpact * (0.25f * Time.timeScale);
		}
		if (GroundAngle > 85f)
		{
			m_MotorThrottle += vp_3DUtility.HorizontalVector(GroundNormal * m_FallImpact);
			m_Grounded = false;
		}
	}

	protected virtual void DeflectUpForce()
	{
		if (m_HeadContact)
		{
			m_NewDir = Vector3.Cross(Vector3.Cross(m_CeilingHit.normal, Vector3.up), m_CeilingHit.normal);
			m_ForceImpact = m_MotorThrottle.y + m_ExternalForce.y;
			Vector3 a = m_NewDir * (m_MotorThrottle.y + m_ExternalForce.y) * (1f - PhysicsWallFriction);
			m_ForceImpact -= a.magnitude;
			AddForce(a * Time.timeScale);
			m_MotorThrottle.y = 0f;
			m_ExternalForce.y = 0f;
			m_FallSpeed = 0f;
			ref Vector3 newDir = ref m_NewDir;
			Vector3 vector = base.Transform.InverseTransformDirection(m_NewDir);
			newDir.x = vector.x;
			Player.HeadImpact.Send((!(m_NewDir.x >= 0f) || (m_NewDir.x == 0f && !(Random.value >= 0.5f))) ? (0f - m_ForceImpact) : m_ForceImpact);
		}
	}

	protected virtual void DeflectHorizontalForce()
	{
		ref Vector3 predictedPos = ref m_PredictedPos;
		Vector3 position = base.Transform.position;
		predictedPos.y = position.y;
		ref Vector3 prevPos = ref m_PrevPos;
		Vector3 position2 = base.Transform.position;
		prevPos.y = position2.y;
		m_PrevDir = (m_PredictedPos - m_PrevPos).normalized;
		CapsuleBottom = m_PrevPos + Vector3.up * Player.Radius.Get();
		CapsuleTop = CapsuleBottom + Vector3.up * (Player.Height.Get() - Player.Radius.Get() * 2f);
		if (Physics.CapsuleCast(CapsuleBottom, CapsuleTop, Player.Radius.Get(), m_PrevDir, out m_WallHit, Vector3.Distance(m_PrevPos, m_PredictedPos), -1744830485))
		{
			m_NewDir = Vector3.Cross(m_WallHit.normal, Vector3.up).normalized;
			if (Vector3.Dot(Vector3.Cross(m_WallHit.point - base.Transform.position, m_PrevPos - base.Transform.position), Vector3.up) > 0f)
			{
				m_NewDir = -m_NewDir;
			}
			m_ForceMultiplier = Mathf.Abs(Vector3.Dot(m_PrevDir, m_NewDir)) * (1f - PhysicsWallFriction);
			if (PhysicsWallBounce > 0f)
			{
				m_NewDir = Vector3.Lerp(m_NewDir, Vector3.Reflect(m_PrevDir, m_WallHit.normal), PhysicsWallBounce);
				m_ForceMultiplier = Mathf.Lerp(m_ForceMultiplier, 1f, PhysicsWallBounce * (1f - PhysicsWallFriction));
			}
			m_ForceImpact = 0f;
			float y = m_ExternalForce.y;
			m_ExternalForce.y = 0f;
			m_ForceImpact = m_ExternalForce.magnitude;
			m_ExternalForce = m_NewDir * m_ExternalForce.magnitude * m_ForceMultiplier;
			m_ForceImpact -= m_ExternalForce.magnitude;
			for (int i = 0; i < 120 && !(m_SmoothForceFrame[i] == Vector3.zero); i++)
			{
				m_SmoothForceFrame[i] = m_SmoothForceFrame[i].magnitude * m_NewDir * m_ForceMultiplier;
			}
			m_ExternalForce.y = y;
		}
	}

	protected virtual bool CanStart_Jump()
	{
		return MotorFreeFly || (m_Grounded && m_MotorJumpDone && GroundAngle <= Player.SlopeLimit.Get());
	}

	protected virtual bool CanStart_Run()
	{
		return !Player.Crouch.Active;
	}

	protected virtual void OnStart_Jump()
	{
		m_MotorJumpDone = false;
		if (!MotorFreeFly || Grounded)
		{
			m_MotorThrottle.y = MotorJumpForce / Time.timeScale;
			ref Vector3 smoothPosition = ref m_SmoothPosition;
			Vector3 position = base.Transform.position;
			smoothPosition.y = position.y;
		}
	}

	protected virtual void OnStop_Jump()
	{
		m_MotorJumpDone = true;
	}

	protected virtual bool CanStop_Crouch()
	{
		if (Physics.SphereCast(new Ray(base.Transform.position, Vector3.up), Player.Radius.Get(), m_NormalHeight - Player.Radius.Get() + 0.01f, -1744830485))
		{
			Player.Crouch.NextAllowedStopTime = Time.time + 1f;
			return false;
		}
		return true;
	}

	protected virtual void OnStart_Crouch()
	{
		if (!MotorFreeFly || Grounded)
		{
			CharacterController.height = m_CrouchHeight;
			CharacterController.center = m_CrouchCenter;
		}
	}

	protected virtual void OnStop_Crouch()
	{
		CharacterController.height = m_NormalHeight;
		CharacterController.center = m_NormalCenter;
	}

	protected virtual void OnMessage_ForceImpact(Vector3 force)
	{
		AddForce(force);
	}

	protected virtual void OnMessage_Stop()
	{
		Stop();
	}

	protected virtual void OnMessage_Move(Vector3 direction)
	{
		CharacterController.Move(direction);
	}
}
