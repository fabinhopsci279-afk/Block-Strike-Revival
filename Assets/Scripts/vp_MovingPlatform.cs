using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class vp_MovingPlatform : MonoBehaviour
{
	protected class WaypointComparer : IComparer
	{
		int IComparer.Compare(object x, object y)
		{
			return new CaseInsensitiveComparer().Compare(((Transform)x).name, ((Transform)y).name);
		}
	}

	public enum PathMoveType
	{
		PingPong,
		Loop,
		Target
	}

	public enum Direction
	{
		Forward,
		Backwards,
		Direct
	}

	public enum MovementInterpolationMode
	{
		EaseInOut,
		EaseIn,
		EaseOut,
		EaseOut2,
		Slerp,
		Lerp
	}

	public enum RotateInterpolationMode
	{
		SyncToMovement,
		EaseOut,
		CustomEaseOut,
		CustomRotate
	}

	protected Transform m_Transform;

	public PathMoveType PathType;

	public GameObject PathWaypoints;

	public Direction PathDirection;

	public int MoveAutoStartTarget = 1000;

	protected List<Transform> m_Waypoints = new List<Transform>();

	protected int m_NextWaypoint;

	protected Vector3 m_CurrentTargetPosition = Vector3.zero;

	protected Vector3 m_CurrentTargetAngle = Vector3.zero;

	protected int m_TargetedWayPoint;

	protected float m_TravelDistance;

	protected Vector3 m_OriginalAngle = Vector3.zero;

	protected int m_CurrentWaypoint;

	public float MoveSpeed = 0.1f;

	public float MoveReturnDelay;

	public float MoveCooldown;

	public MovementInterpolationMode MoveInterpolationMode;

	protected bool m_Moving;

	protected float m_NextAllowedMoveTime;

	protected float m_MoveTime;

	protected vp_Timer.Handle m_ReturnDelayTimer = new vp_Timer.Handle();

	protected Vector3 m_PrevPos = Vector3.zero;

	protected AnimationCurve m_EaseInOutCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	protected AnimationCurve m_LinearCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);

	public float RotationEaseAmount = 0.1f;

	public Vector3 RotationSpeed = Vector3.zero;

	public RotateInterpolationMode RotationInterpolationMode;

	protected Vector3 m_PrevAngle = Vector3.zero;

	public AudioClip SoundStart;

	public AudioClip SoundStop;

	public AudioClip SoundMove;

	public AudioClip SoundWaypoint;

	protected AudioSource m_Audio;

	public bool PhysicsSnapPlayerToTopOnIntersect = true;

	public float m_PhysicsPushForce = 2f;

	protected Rigidbody m_RigidBody;

	protected Collider m_Collider;

	protected Collider m_PlayerCollider;

	protected vp_PlayerEventHandler m_PlayerToPush;

	protected float m_PhysicsCurrentMoveVelocity;

	protected float m_PhysicsCurrentRotationVelocity;

	protected Dictionary<Collider, vp_PlayerEventHandler> m_KnownPlayers = new Dictionary<Collider, vp_PlayerEventHandler>();

	public int TargetedWaypoint => m_TargetedWayPoint;

	private void Start()
	{
		m_Transform = base.transform;
		m_Collider = GetComponentInChildren<Collider>();
		m_RigidBody = GetComponent<Rigidbody>();
		m_RigidBody.useGravity = false;
		m_RigidBody.isKinematic = true;
		m_NextWaypoint = 0;
		m_Audio = GetComponent<AudioSource>();
		m_Audio.loop = true;
		m_Audio.clip = SoundMove;
		if (PathWaypoints == null)
		{
			return;
		}
		base.gameObject.layer = 28;
		IEnumerator enumerator = PathWaypoints.transform.GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				object current = enumerator.Current;
				Transform transform = (Transform)current;
				if (vp_Utility.IsActive(transform.gameObject))
				{
					m_Waypoints.Add(transform);
					transform.gameObject.layer = 28;
				}
				if (transform.GetComponent<Renderer>() != null)
				{
					transform.GetComponent<Renderer>().enabled = false;
				}
				if (transform.GetComponent<Collider>() != null)
				{
					transform.GetComponent<Collider>().enabled = false;
				}
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
		IComparer comparer = new WaypointComparer();
		List<Transform> waypoints = m_Waypoints;
		IComparer comparer2 = comparer;
		waypoints.Sort(comparer2.Compare);
		if (m_Waypoints.Count > 0)
		{
			m_CurrentTargetPosition = m_Waypoints[m_NextWaypoint].position;
			m_CurrentTargetAngle = m_Waypoints[m_NextWaypoint].eulerAngles;
			m_Transform.position = m_CurrentTargetPosition;
			m_Transform.eulerAngles = m_CurrentTargetAngle;
			if (MoveAutoStartTarget > m_Waypoints.Count - 1)
			{
				MoveAutoStartTarget = m_Waypoints.Count - 1;
			}
		}
	}

	private void FixedUpdate()
	{
		UpdatePath();
		UpdateMovement();
		UpdateRotation();
		UpdateVelocity();
	}

	protected void UpdatePath()
	{
		if (m_Waypoints.Count < 2 || !(GetDistanceLeft() < 0.01f) || !(Time.time >= m_NextAllowedMoveTime))
		{
			return;
		}
		switch (PathType)
		{
		case PathMoveType.PingPong:
			if (PathDirection == Direction.Backwards)
			{
				if (m_NextWaypoint == 0)
				{
					PathDirection = Direction.Forward;
				}
			}
			else if (m_NextWaypoint == m_Waypoints.Count - 1)
			{
				PathDirection = Direction.Backwards;
			}
			OnArriveAtWaypoint();
			GoToNextWaypoint();
			break;
		case PathMoveType.Loop:
			OnArriveAtWaypoint();
			GoToNextWaypoint();
			break;
		case PathMoveType.Target:
			if (m_NextWaypoint == m_TargetedWayPoint)
			{
				if (m_Moving)
				{
					OnStop();
				}
				else if (m_NextWaypoint != 0)
				{
					OnArriveAtDestination();
				}
				break;
			}
			if (m_Moving)
			{
				if (m_PhysicsCurrentMoveVelocity == 0f)
				{
					OnStart();
				}
				else
				{
					OnArriveAtWaypoint();
				}
			}
			GoToNextWaypoint();
			break;
		}
	}

	protected void OnStart()
	{
		if (SoundStart != null)
		{
			m_Audio.PlayOneShot(SoundStart);
		}
	}

	protected void OnArriveAtWaypoint()
	{
		if (SoundWaypoint != null)
		{
			m_Audio.PlayOneShot(SoundWaypoint);
		}
	}

	protected void OnArriveAtDestination()
	{
		if (MoveReturnDelay > 0f && !m_ReturnDelayTimer.Active)
		{
			vp_Timer.In(MoveReturnDelay, delegate
			{
				GoTo(0);
			}, m_ReturnDelayTimer);
		}
	}

	protected void OnStop()
	{
		m_Audio.Stop();
		if (SoundStop != null)
		{
			m_Audio.PlayOneShot(SoundStop);
		}
		m_Transform.position = m_CurrentTargetPosition;
		m_Transform.eulerAngles = m_CurrentTargetAngle;
		m_Moving = false;
		if (m_NextWaypoint == 0)
		{
			m_NextAllowedMoveTime = Time.time + MoveCooldown;
		}
	}

	protected void UpdateMovement()
	{
		if (m_Waypoints.Count >= 2)
		{
			switch (MoveInterpolationMode)
			{
			case MovementInterpolationMode.EaseInOut:
				m_Transform.position = vp_MathUtility.NaNSafeVector3(Vector3.Lerp(m_Transform.position, m_CurrentTargetPosition, m_EaseInOutCurve.Evaluate(m_MoveTime)));
				break;
			case MovementInterpolationMode.EaseIn:
				m_Transform.position = vp_MathUtility.NaNSafeVector3(Vector3.MoveTowards(m_Transform.position, m_CurrentTargetPosition, m_MoveTime));
				break;
			case MovementInterpolationMode.EaseOut:
				m_Transform.position = vp_MathUtility.NaNSafeVector3(Vector3.Lerp(m_Transform.position, m_CurrentTargetPosition, m_LinearCurve.Evaluate(m_MoveTime)));
				break;
			case MovementInterpolationMode.EaseOut2:
				m_Transform.position = vp_MathUtility.NaNSafeVector3(Vector3.Lerp(m_Transform.position, m_CurrentTargetPosition, MoveSpeed * 0.25f));
				break;
			case MovementInterpolationMode.Slerp:
				m_Transform.position = vp_MathUtility.NaNSafeVector3(Vector3.Slerp(m_Transform.position, m_CurrentTargetPosition, m_LinearCurve.Evaluate(m_MoveTime)));
				break;
			case MovementInterpolationMode.Lerp:
				m_Transform.position = vp_MathUtility.NaNSafeVector3(Vector3.MoveTowards(m_Transform.position, m_CurrentTargetPosition, MoveSpeed));
				break;
			}
		}
	}

	protected void UpdateRotation()
	{
		switch (RotationInterpolationMode)
		{
		case RotateInterpolationMode.SyncToMovement:
			if (m_Moving)
			{
				m_Transform.eulerAngles = vp_MathUtility.NaNSafeVector3(new Vector3(Mathf.LerpAngle(m_OriginalAngle.x, m_CurrentTargetAngle.x, 1f - GetDistanceLeft() / m_TravelDistance), Mathf.LerpAngle(m_OriginalAngle.y, m_CurrentTargetAngle.y, 1f - GetDistanceLeft() / m_TravelDistance), Mathf.LerpAngle(m_OriginalAngle.z, m_CurrentTargetAngle.z, 1f - GetDistanceLeft() / m_TravelDistance)));
			}
			break;
		case RotateInterpolationMode.EaseOut:
		{
			Transform transform2 = m_Transform;
			Vector3 eulerAngles4 = m_Transform.eulerAngles;
			float x2 = Mathf.LerpAngle(eulerAngles4.x, m_CurrentTargetAngle.x, m_LinearCurve.Evaluate(m_MoveTime));
			Vector3 eulerAngles5 = m_Transform.eulerAngles;
			float y2 = Mathf.LerpAngle(eulerAngles5.y, m_CurrentTargetAngle.y, m_LinearCurve.Evaluate(m_MoveTime));
			Vector3 eulerAngles6 = m_Transform.eulerAngles;
			transform2.eulerAngles = vp_MathUtility.NaNSafeVector3(new Vector3(x2, y2, Mathf.LerpAngle(eulerAngles6.z, m_CurrentTargetAngle.z, m_LinearCurve.Evaluate(m_MoveTime))));
			break;
		}
		case RotateInterpolationMode.CustomEaseOut:
		{
			Transform transform = m_Transform;
			Vector3 eulerAngles = m_Transform.eulerAngles;
			float x = Mathf.LerpAngle(eulerAngles.x, m_CurrentTargetAngle.x, RotationEaseAmount);
			Vector3 eulerAngles2 = m_Transform.eulerAngles;
			float y = Mathf.LerpAngle(eulerAngles2.y, m_CurrentTargetAngle.y, RotationEaseAmount);
			Vector3 eulerAngles3 = m_Transform.eulerAngles;
			transform.eulerAngles = vp_MathUtility.NaNSafeVector3(new Vector3(x, y, Mathf.LerpAngle(eulerAngles3.z, m_CurrentTargetAngle.z, RotationEaseAmount)));
			break;
		}
		case RotateInterpolationMode.CustomRotate:
			m_Transform.Rotate(RotationSpeed);
			break;
		}
	}

	protected void UpdateVelocity()
	{
		m_MoveTime += MoveSpeed * 0.01f * vp_TimeUtility.AdjustedTimeScale;
		m_PhysicsCurrentMoveVelocity = (m_Transform.position - m_PrevPos).magnitude;
		m_PhysicsCurrentRotationVelocity = (m_Transform.eulerAngles - m_PrevAngle).magnitude;
		m_PrevPos = m_Transform.position;
		m_PrevAngle = m_Transform.eulerAngles;
	}

	public void GoTo(int targetWayPoint)
	{
		if (Time.time < m_NextAllowedMoveTime || PathType != PathMoveType.Target)
		{
			return;
		}
		m_TargetedWayPoint = GetValidWaypoint(targetWayPoint);
		if (targetWayPoint > m_NextWaypoint)
		{
			if (PathDirection != Direction.Direct)
			{
				PathDirection = Direction.Forward;
			}
		}
		else if (PathDirection != Direction.Direct)
		{
			PathDirection = Direction.Backwards;
		}
		m_Moving = true;
	}

	protected float GetDistanceLeft()
	{
		if (m_Waypoints.Count < 2)
		{
			return 0f;
		}
		return Vector3.Distance(m_Transform.position, m_Waypoints[m_NextWaypoint].position);
	}

	protected void GoToNextWaypoint()
	{
		if (m_Waypoints.Count >= 2)
		{
			m_MoveTime = 0f;
			if (!m_Audio.isPlaying)
			{
				m_Audio.Play();
			}
			m_CurrentWaypoint = m_NextWaypoint;
			switch (PathDirection)
			{
			case Direction.Forward:
				m_NextWaypoint = GetValidWaypoint(m_NextWaypoint + 1);
				break;
			case Direction.Backwards:
				m_NextWaypoint = GetValidWaypoint(m_NextWaypoint - 1);
				break;
			case Direction.Direct:
				m_NextWaypoint = m_TargetedWayPoint;
				break;
			}
			m_OriginalAngle = m_CurrentTargetAngle;
			m_CurrentTargetPosition = m_Waypoints[m_NextWaypoint].position;
			m_CurrentTargetAngle = m_Waypoints[m_NextWaypoint].eulerAngles;
			m_TravelDistance = GetDistanceLeft();
			m_Moving = true;
		}
	}

	protected int GetValidWaypoint(int wayPoint)
	{
		if (wayPoint < 0)
		{
			return m_Waypoints.Count - 1;
		}
		if (wayPoint > m_Waypoints.Count - 1)
		{
			return 0;
		}
		return wayPoint;
	}

	protected void OnTriggerEnter(Collider col)
	{
		if (GetPlayer(col))
		{
			TryPushPlayer();
			TryAutoStart();
		}
	}

	protected void OnTriggerStay(Collider col)
	{
		if (PhysicsSnapPlayerToTopOnIntersect && GetPlayer(col))
		{
			TrySnapPlayerToTop();
		}
	}

	protected bool GetPlayer(Collider col)
	{
		if (!m_KnownPlayers.ContainsKey(col))
		{
			if (col.gameObject.layer != 30)
			{
				return false;
			}
			vp_PlayerEventHandler component = col.transform.root.GetComponent<vp_PlayerEventHandler>();
			if (component == null)
			{
				return false;
			}
			m_KnownPlayers.Add(col, component);
		}
		if (!m_KnownPlayers.TryGetValue(col, out m_PlayerToPush))
		{
			return false;
		}
		m_PlayerCollider = col;
		return true;
	}

	protected void TryPushPlayer()
	{
		if (m_PlayerToPush == null || m_PlayerToPush.Platform == null)
		{
			return;
		}
		Vector3 vector = m_PlayerToPush.Position.Get();
		float y = vector.y;
		Vector3 max = m_Collider.bounds.max;
		if (!(y > max.y) && !(m_PlayerToPush.Platform.Get() == m_Transform))
		{
			float num = m_PhysicsCurrentMoveVelocity;
			if (num == 0f)
			{
				num = m_PhysicsCurrentRotationVelocity * 0.1f;
			}
			if (num > 0f)
			{
				m_PlayerToPush.ForceImpact.Send(vp_3DUtility.HorizontalVector(-(m_Transform.position - m_PlayerCollider.bounds.center).normalized * num * m_PhysicsPushForce));
			}
		}
	}

	protected void TrySnapPlayerToTop()
	{
		if (m_PlayerToPush == null || m_PlayerToPush.Platform == null)
		{
			return;
		}
		Vector3 vector = m_PlayerToPush.Position.Get();
		float y = vector.y;
		Vector3 max = m_Collider.bounds.max;
		if (y > max.y || m_PlayerToPush.Platform.Get() == m_Transform || RotationSpeed.x != 0f || RotationSpeed.z != 0f || m_CurrentTargetAngle.x != 0f || m_CurrentTargetAngle.z != 0f)
		{
			return;
		}
		Vector3 max2 = m_Collider.bounds.max;
		float x = max2.x;
		Vector3 max3 = m_PlayerCollider.bounds.max;
		if (x < max3.x)
		{
			return;
		}
		Vector3 max4 = m_Collider.bounds.max;
		float z = max4.z;
		Vector3 max5 = m_PlayerCollider.bounds.max;
		if (z < max5.z)
		{
			return;
		}
		Vector3 min = m_Collider.bounds.min;
		float x2 = min.x;
		Vector3 min2 = m_PlayerCollider.bounds.min;
		if (!(x2 > min2.x))
		{
			Vector3 min3 = m_Collider.bounds.min;
			float z2 = min3.z;
			Vector3 min4 = m_PlayerCollider.bounds.min;
			if (!(z2 > min4.z))
			{
				Vector3 o = m_PlayerToPush.Position.Get();
				Vector3 max6 = m_Collider.bounds.max;
				o.y = max6.y - 0.1f;
				m_PlayerToPush.Position.Set(o);
			}
		}
	}

	protected void TryAutoStart()
	{
		if (MoveAutoStartTarget != 0 && !(m_PhysicsCurrentMoveVelocity > 0f) && !m_Moving)
		{
			GoTo(MoveAutoStartTarget);
		}
	}
}
