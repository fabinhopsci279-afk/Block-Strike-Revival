using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioListener))]
[RequireComponent(typeof(Camera))]
public class vp_FPCamera : vp_Component
{
	public delegate void BobStepDelegate();

	public vp_FPController FPController;

	public Vector2 MouseSensitivity = new Vector2(5f, 5f);

	public int MouseSmoothSteps = 10;

	public float MouseSmoothWeight = 0.5f;

	public bool MouseAcceleration;

	public float MouseAccelerationThreshold = 0.4f;

	protected Vector2 m_MouseMove = Vector2.zero;

	protected List<Vector2> m_MouseSmoothBuffer = new List<Vector2>();

	public float RenderingFieldOfView = 60f;

	public float RenderingZoomDamping = 0.2f;

	protected float m_FinalZoomTime;

	public Vector3 PositionOffset = new Vector3(0f, 1.75f, 0.1f);

	public float PositionGroundLimit = 0.1f;

	public float PositionSpringStiffness = 0.01f;

	public float PositionSpringDamping = 0.25f;

	public float PositionSpring2Stiffness = 0.95f;

	public float PositionSpring2Damping = 0.25f;

	public float PositionKneeling = 0.025f;

	public int PositionKneelingSoftness = 1;

	public float PositionEarthQuakeFactor = 1f;

	protected vp_Spring m_PositionSpring;

	protected vp_Spring m_PositionSpring2;

	protected bool m_DrawCameraCollisionDebugLine;

	public Vector2 RotationPitchLimit = new Vector2(90f, -90f);

	public Vector2 RotationYawLimit = new Vector2(-360f, 360f);

	public float RotationSpringStiffness = 0.01f;

	public float RotationSpringDamping = 0.25f;

	public float RotationKneeling = 0.025f;

	public int RotationKneelingSoftness = 1;

	public float RotationStrafeRoll = 0.01f;

	public float RotationEarthQuakeFactor;

	protected float m_Pitch;

	protected float m_Yaw;

	protected vp_Spring m_RotationSpring;

	protected Vector2 m_InitialRotation = Vector2.zero;

	public float ShakeSpeed;

	public Vector3 ShakeAmplitude = new Vector3(10f, 10f, 0f);

	protected Vector3 m_Shake = Vector3.zero;

	public Vector4 BobRate = new Vector4(0f, 1.4f, 0f, 0.7f);

	public Vector4 BobAmplitude = new Vector4(0f, 0.25f, 0f, 0.5f);

	public float BobInputVelocityScale = 1f;

	public float BobMaxInputVelocity = 100f;

	public bool BobRequireGroundContact = true;

	protected float m_LastBobSpeed;

	protected Vector4 m_CurrentBobAmp = Vector4.zero;

	protected Vector4 m_CurrentBobVal = Vector4.zero;

	protected float m_BobSpeed;

	public BobStepDelegate BobStepCallback;

	public float BobStepThreshold = 10f;

	protected float m_LastUpBob;

	protected bool m_BobWasElevating;

	protected Vector3 m_CameraCollisionStartPos = Vector3.zero;

	protected Vector3 m_CameraCollisionEndPos = Vector3.zero;

	protected RaycastHit m_CameraHit;

	private vp_FPPlayerEventHandler m_Player;

	public bool DrawCameraCollisionDebugLine
	{
		get
		{
			return m_DrawCameraCollisionDebugLine;
		}
		set
		{
			m_DrawCameraCollisionDebugLine = value;
		}
	}

	private vp_FPPlayerEventHandler Player
	{
		get
		{
			if (m_Player == null && EventHandler != null)
			{
				m_Player = (vp_FPPlayerEventHandler)EventHandler;
			}
			return m_Player;
		}
	}

	public Vector2 Angle
	{
		get
		{
			return new Vector2(m_Pitch, m_Yaw);
		}
		set
		{
			Pitch = value.x;
			Yaw = value.y;
		}
	}

	public Vector3 Forward => m_Transform.forward;

	public float Pitch
	{
		get
		{
			return m_Pitch;
		}
		set
		{
			if (value > 90f)
			{
				value -= 360f;
			}
			m_Pitch = value;
		}
	}

	public float Yaw
	{
		get
		{
			return m_Yaw;
		}
		set
		{
			m_InitialRotation = Vector2.zero;
			m_Yaw = value;
		}
	}

	protected virtual Vector2 OnValue_Rotation
	{
		get
		{
			return Angle;
		}
		set
		{
			Angle = value;
		}
	}

	protected virtual Vector3 OnValue_Forward => Forward;

	protected override void Awake()
	{
		base.Awake();
		if (FPController == null)
		{
			FPController = base.Root.GetComponent<vp_FPController>();
		}
		Vector3 eulerAngles = base.Transform.eulerAngles;
		float y = eulerAngles.y;
		Vector3 eulerAngles2 = base.Transform.eulerAngles;
		m_InitialRotation = new Vector2(y, eulerAngles2.x);
		base.Parent.gameObject.layer = 30;
		IEnumerator enumerator = base.Parent.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				Transform transform = (Transform)current;
				transform.gameObject.layer = 30;
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		GetComponent<Camera>().cullingMask &= 1073741823;
		GetComponent<Camera>().depth = 0f;
		IEnumerator enumerator2 = base.Transform.GetEnumerator();
		try
		{
			while (enumerator2.MoveNext())
			{
				object current2 = enumerator2.Current;
				Transform transform2 = (Transform)current2;
				Camera camera = (Camera)transform2.GetComponent(typeof(Camera));
				if (camera != null)
				{
					camera.transform.localPosition = Vector3.zero;
					camera.transform.localEulerAngles = Vector3.zero;
					camera.clearFlags = CameraClearFlags.Depth;
					camera.cullingMask = int.MinValue;
					camera.depth = 1f;
					camera.farClipPlane = 100f;
					camera.nearClipPlane = 0.01f;
					camera.fieldOfView = 60f;
					break;
				}
			}
		}
		finally
		{
			IDisposable disposable2;
			if ((disposable2 = (enumerator2 as IDisposable)) != null)
			{
				disposable2.Dispose();
			}
		}
		m_PositionSpring = new vp_Spring(base.Transform, vp_Spring.UpdateMode.Position, autoUpdate: false);
		m_PositionSpring.MinVelocity = 1E-05f;
		m_PositionSpring.RestState = PositionOffset;
		m_PositionSpring2 = new vp_Spring(base.Transform, vp_Spring.UpdateMode.PositionAdditive, autoUpdate: false);
		m_PositionSpring2.MinVelocity = 1E-05f;
		m_RotationSpring = new vp_Spring(base.Transform, vp_Spring.UpdateMode.RotationAdditive, autoUpdate: false);
		m_RotationSpring.MinVelocity = 1E-05f;
	}

	protected override void OnEnable()
	{
		base.OnEnable();
	}

	protected override void OnDisable()
	{
		base.OnDisable();
	}

	protected override void Start()
	{
		base.Start();
		Refresh();
		SnapSprings();
		SnapZoom();
	}

	protected override void Init()
	{
		base.Init();
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (Time.timeScale != 0f)
		{
			UpdateZoom();
			UpdateSwaying();
			UpdateBob();
			UpdateEarthQuake();
			UpdateShakes();
			UpdateSprings();
		}
	}

	protected override void LateUpdate()
	{
		base.LateUpdate();
		if (Time.timeScale != 0f)
		{
			m_Transform.position = FPController.SmoothPosition;
			m_Transform.localPosition += m_PositionSpring.State + m_PositionSpring2.State;
			DoCameraCollision();
			Quaternion lhs = Quaternion.AngleAxis(m_Yaw + m_InitialRotation.x, Vector3.up);
			Quaternion rhs = Quaternion.AngleAxis(0f, Vector3.left);
			base.Parent.rotation = vp_MathUtility.NaNSafeQuaternion(lhs * rhs, base.Parent.rotation);
			rhs = Quaternion.AngleAxis(0f - m_Pitch - m_InitialRotation.y, Vector3.left);
			base.Transform.rotation = vp_MathUtility.NaNSafeQuaternion(lhs * rhs, base.Transform.rotation);
			base.Transform.localEulerAngles += vp_MathUtility.NaNSafeVector3(Vector3.forward * m_RotationSpring.State.z);
		}
	}

	protected virtual void DoCameraCollision()
	{
		m_CameraCollisionStartPos = FPController.Transform.TransformPoint(0f, PositionOffset.y, 0f);
		m_CameraCollisionEndPos = base.Transform.position + (base.Transform.position - m_CameraCollisionStartPos).normalized * FPController.CharacterController.radius;
		if (Physics.Linecast(m_CameraCollisionStartPos, m_CameraCollisionEndPos, out m_CameraHit, -1744830485) && !m_CameraHit.collider.isTrigger)
		{
			base.Transform.position = m_CameraHit.point - (m_CameraHit.point - m_CameraCollisionStartPos).normalized * FPController.CharacterController.radius;
		}
		Vector3 localPosition = base.Transform.localPosition;
		if (localPosition.y < PositionGroundLimit)
		{
			Transform transform = base.Transform;
			Vector3 localPosition2 = base.Transform.localPosition;
			float x = localPosition2.x;
			float positionGroundLimit = PositionGroundLimit;
			Vector3 localPosition3 = base.Transform.localPosition;
			transform.localPosition = new Vector3(x, positionGroundLimit, localPosition3.z);
		}
	}

	public virtual void AddForce(Vector3 force)
	{
		m_PositionSpring.AddForce(force);
	}

	public virtual void AddForce(float x, float y, float z)
	{
		AddForce(new Vector3(x, y, z));
	}

	public virtual void AddForce2(Vector3 force)
	{
		m_PositionSpring2.AddForce(force);
	}

	public void AddForce2(float x, float y, float z)
	{
		AddForce2(new Vector3(x, y, z));
	}

	public virtual void AddRollForce(float force)
	{
		m_RotationSpring.AddForce(Vector3.forward * force);
	}

	public virtual void AddRotationForce(Vector3 force)
	{
		m_RotationSpring.AddForce(force);
	}

	public void AddRotationForce(float x, float y, float z)
	{
		AddRotationForce(new Vector3(x, y, z));
	}

	public void UpdateLook(Vector2 look)
	{
		UpdateMouseLook(look);
	}

	protected virtual void UpdateMouseLook(Vector2 look)
	{
		m_MouseMove.x = look.x * Time.timeScale;
		m_MouseMove.y = look.y * Time.timeScale;
		MouseSmoothSteps = Mathf.Clamp(MouseSmoothSteps, 1, 20);
		MouseSmoothWeight = Mathf.Clamp01(MouseSmoothWeight);
		while (m_MouseSmoothBuffer.Count > MouseSmoothSteps)
		{
			m_MouseSmoothBuffer.RemoveAt(0);
		}
		m_MouseSmoothBuffer.Add(m_MouseMove);
		float num = 1f;
		Vector2 a = Vector2.zero;
		float num2 = 0f;
		for (int num3 = m_MouseSmoothBuffer.Count - 1; num3 > 0; num3--)
		{
			a += m_MouseSmoothBuffer[num3] * num;
			num2 += 1f * num;
			num *= MouseSmoothWeight / base.Delta;
		}
		num2 = Mathf.Max(1f, num2);
		Vector2 vector = vp_MathUtility.NaNSafeVector2(a / num2);
		float num4 = 0f;
		float num5 = Mathf.Abs(vector.x);
		float num6 = Mathf.Abs(vector.y);
		if (MouseAcceleration)
		{
			num4 = Mathf.Sqrt(num5 * num5 + num6 * num6) / base.Delta;
			num4 = ((!(num4 > MouseAccelerationThreshold)) ? 0f : num4);
		}
		m_Yaw += vector.x * (MouseSensitivity.x + num4);
		m_Pitch -= vector.y * (MouseSensitivity.y + num4);
		m_Yaw = ((!(m_Yaw >= -360f)) ? (m_Yaw += 360f) : m_Yaw);
		m_Yaw = ((!(m_Yaw <= 360f)) ? (m_Yaw -= 360f) : m_Yaw);
		m_Yaw = Mathf.Clamp(m_Yaw, RotationYawLimit.x, RotationYawLimit.y);
		m_Pitch = ((!(m_Pitch >= -360f)) ? (m_Pitch += 360f) : m_Pitch);
		m_Pitch = ((!(m_Pitch <= 360f)) ? (m_Pitch -= 360f) : m_Pitch);
		m_Pitch = Mathf.Clamp(m_Pitch, 0f - RotationPitchLimit.x, 0f - RotationPitchLimit.y);
	}

	protected virtual void UpdateZoom()
	{
		if (!(m_FinalZoomTime <= Time.time))
		{
			RenderingZoomDamping = Mathf.Max(RenderingZoomDamping, 0.01f);
			float t = 1f - (m_FinalZoomTime - Time.time) / RenderingZoomDamping;
			base.gameObject.GetComponent<Camera>().fieldOfView = Mathf.SmoothStep(base.gameObject.GetComponent<Camera>().fieldOfView, RenderingFieldOfView, t);
		}
	}

	public virtual void Zoom()
	{
		m_FinalZoomTime = Time.time + RenderingZoomDamping;
	}

	public virtual void SnapZoom()
	{
		base.gameObject.GetComponent<Camera>().fieldOfView = RenderingFieldOfView;
	}

	protected virtual void UpdateShakes()
	{
		if (ShakeSpeed != 0f)
		{
			m_Yaw -= m_Shake.y;
			m_Pitch -= m_Shake.x;
			m_Shake = Vector3.Scale(vp_SmoothRandom.GetVector3Centered(ShakeSpeed), ShakeAmplitude);
			m_Yaw += m_Shake.y;
			m_Pitch += m_Shake.x;
			m_RotationSpring.AddForce(Vector3.forward * m_Shake.z * Time.timeScale);
		}
	}

	protected virtual void UpdateBob()
	{
		if (!(BobAmplitude == Vector4.zero) && !(BobRate == Vector4.zero))
		{
			m_BobSpeed = ((BobRequireGroundContact && !FPController.Grounded) ? 0f : FPController.CharacterController.velocity.sqrMagnitude);
			m_BobSpeed = Mathf.Min(m_BobSpeed * BobInputVelocityScale, BobMaxInputVelocity);
			m_BobSpeed = Mathf.Round(m_BobSpeed * 1000f) / 1000f;
			if (m_BobSpeed == 0f)
			{
				m_BobSpeed = Mathf.Min(m_LastBobSpeed * 0.93f, BobMaxInputVelocity);
			}
			m_CurrentBobAmp.y = m_BobSpeed * (BobAmplitude.y * -0.0001f);
			m_CurrentBobVal.y = Mathf.Cos(Time.time * (BobRate.y * 10f)) * m_CurrentBobAmp.y;
			m_CurrentBobAmp.x = m_BobSpeed * (BobAmplitude.x * 0.0001f);
			m_CurrentBobVal.x = Mathf.Cos(Time.time * (BobRate.x * 10f)) * m_CurrentBobAmp.x;
			m_CurrentBobAmp.z = m_BobSpeed * (BobAmplitude.z * 0.0001f);
			m_CurrentBobVal.z = Mathf.Cos(Time.time * (BobRate.z * 10f)) * m_CurrentBobAmp.z;
			m_CurrentBobAmp.w = m_BobSpeed * (BobAmplitude.w * 0.0001f);
			m_CurrentBobVal.w = Mathf.Cos(Time.time * (BobRate.w * 10f)) * m_CurrentBobAmp.w;
			m_PositionSpring.AddForce(m_CurrentBobVal * Time.timeScale);
			AddRollForce(m_CurrentBobVal.w * Time.timeScale);
			m_LastBobSpeed = m_BobSpeed;
			DetectBobStep(m_BobSpeed, m_CurrentBobVal.y);
		}
	}

	protected virtual void DetectBobStep(float speed, float upBob)
	{
		if (BobStepCallback != null && !(speed < BobStepThreshold))
		{
			bool flag = m_LastUpBob < upBob;
			m_LastUpBob = upBob;
			if (flag && !m_BobWasElevating)
			{
				BobStepCallback();
			}
			m_BobWasElevating = flag;
		}
	}

	protected virtual void UpdateSwaying()
	{
		Vector3 vector = base.Transform.InverseTransformDirection(FPController.CharacterController.velocity * 0.016f) * Time.timeScale;
		AddRollForce(vector.x * RotationStrafeRoll);
	}

	protected virtual void UpdateEarthQuake()
	{
		if (!(Player == null) && Player.Earthquake.Active)
		{
			if (m_PositionSpring.State.y >= m_PositionSpring.RestState.y)
			{
				Vector3 o = Player.EarthQuakeForce.Get();
				o.y = 0f - o.y;
				Player.EarthQuakeForce.Set(o);
			}
			m_PositionSpring.AddForce(Player.EarthQuakeForce.Get() * PositionEarthQuakeFactor);
			vp_Spring rotationSpring = m_RotationSpring;
			Vector3 forward = Vector3.forward;
			Vector3 vector = Player.EarthQuakeForce.Get();
			rotationSpring.AddForce(forward * ((0f - vector.x) * 2f) * RotationEarthQuakeFactor);
		}
	}

	protected virtual void UpdateSprings()
	{
		m_PositionSpring.FixedUpdate();
		m_PositionSpring2.FixedUpdate();
		m_RotationSpring.FixedUpdate();
	}

	public virtual void DoBomb(Vector3 positionForce, float minRollForce, float maxRollForce)
	{
		AddForce2(positionForce);
		float num = UnityEngine.Random.Range(minRollForce, maxRollForce);
		if (UnityEngine.Random.value > 0.5f)
		{
			num = 0f - num;
		}
		AddRollForce(num);
	}

	public override void Refresh()
	{
		if (Application.isPlaying)
		{
			if (m_PositionSpring != null)
			{
				m_PositionSpring.Stiffness = new Vector3(PositionSpringStiffness, PositionSpringStiffness, PositionSpringStiffness);
				m_PositionSpring.Damping = Vector3.one - new Vector3(PositionSpringDamping, PositionSpringDamping, PositionSpringDamping);
				m_PositionSpring.MinState.y = PositionGroundLimit;
				m_PositionSpring.RestState = PositionOffset;
			}
			if (m_PositionSpring2 != null)
			{
				m_PositionSpring2.Stiffness = new Vector3(PositionSpring2Stiffness, PositionSpring2Stiffness, PositionSpring2Stiffness);
				m_PositionSpring2.Damping = Vector3.one - new Vector3(PositionSpring2Damping, PositionSpring2Damping, PositionSpring2Damping);
				m_PositionSpring2.MinState.y = 0f - PositionOffset.y + PositionGroundLimit;
			}
			if (m_RotationSpring != null)
			{
				m_RotationSpring.Stiffness = new Vector3(RotationSpringStiffness, RotationSpringStiffness, RotationSpringStiffness);
				m_RotationSpring.Damping = Vector3.one - new Vector3(RotationSpringDamping, RotationSpringDamping, RotationSpringDamping);
			}
			Zoom();
		}
	}

	public virtual void SnapSprings()
	{
		if (m_PositionSpring != null)
		{
			m_PositionSpring.RestState = PositionOffset;
			m_PositionSpring.State = PositionOffset;
			m_PositionSpring.Stop(includeSoftForce: true);
		}
		if (m_PositionSpring2 != null)
		{
			m_PositionSpring2.RestState = Vector3.zero;
			m_PositionSpring2.State = Vector3.zero;
			m_PositionSpring2.Stop(includeSoftForce: true);
		}
		if (m_RotationSpring != null)
		{
			m_RotationSpring.RestState = Vector3.zero;
			m_RotationSpring.State = Vector3.zero;
			m_RotationSpring.Stop(includeSoftForce: true);
		}
	}

	public virtual void StopSprings()
	{
		if (m_PositionSpring != null)
		{
			m_PositionSpring.Stop(includeSoftForce: true);
		}
		if (m_PositionSpring2 != null)
		{
			m_PositionSpring2.Stop(includeSoftForce: true);
		}
		if (m_RotationSpring != null)
		{
			m_RotationSpring.Stop(includeSoftForce: true);
		}
		m_BobSpeed = 0f;
		m_LastBobSpeed = 0f;
	}

	public virtual void Stop()
	{
		SnapSprings();
		SnapZoom();
		Refresh();
	}

	public virtual void SetRotation(Vector2 eulerAngles, bool stop = true, bool resetInitialRotation = true)
	{
		Angle = eulerAngles;
		if (stop)
		{
			Stop();
		}
		if (resetInitialRotation)
		{
			m_InitialRotation = Vector2.zero;
		}
	}

	protected virtual void OnMessage_FallImpact(float impact)
	{
		impact = Mathf.Abs(impact * 55f);
		float t = impact * PositionKneeling;
		float t2 = impact * RotationKneeling;
		t = Mathf.SmoothStep(0f, 1f, t);
		t2 = Mathf.SmoothStep(0f, 1f, t2);
		t2 = Mathf.SmoothStep(0f, 1f, t2);
		if (m_PositionSpring != null)
		{
			m_PositionSpring.AddSoftForce(Vector3.down * t, PositionKneelingSoftness);
		}
		if (m_RotationSpring != null)
		{
			float d = (!(UnityEngine.Random.value <= 0.5f)) ? (t2 * 2f) : (0f - t2 * 2f);
			m_RotationSpring.AddSoftForce(Vector3.forward * d, RotationKneelingSoftness);
		}
	}

	protected virtual void OnMessage_HeadImpact(float impact)
	{
		if (m_RotationSpring != null && Mathf.Abs(m_RotationSpring.State.z) < 30f)
		{
			m_RotationSpring.AddForce(Vector3.forward * (impact * 20f) * Time.timeScale);
		}
	}

	protected virtual void OnMessage_GroundStomp(float impact)
	{
		AddForce2(new Vector3(0f, -1f, 0f) * impact);
	}

	protected virtual void OnMessage_BombShake(float impact)
	{
		DoBomb(new Vector3(1f, -10f, 1f) * impact, 1f, 2f);
	}

	protected virtual void OnStart_Zoom()
	{
		if (!(Player == null))
		{
			Player.Run.Stop();
		}
	}

	protected virtual bool CanStart_Run()
	{
		return Player == null || !Player.Zoom.Active;
	}

	protected virtual void OnMessage_Stop()
	{
		Stop();
	}
}
