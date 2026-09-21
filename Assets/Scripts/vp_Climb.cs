using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class vp_Climb : vp_Interactable
{
	[Serializable]
	public class vp_ClimbingSounds
	{
		public AudioSource AudioSource;

		public List<AudioClip> MountSounds = new List<AudioClip>();

		public List<AudioClip> DismountSounds = new List<AudioClip>();

		public float ClimbingSoundSpeed = 4f;

		public Vector2 ClimbingPitch = new Vector2(1f, 1.5f);

		public List<AudioClip> ClimbingSounds = new List<AudioClip>();
	}

	public float MinimumClimbSpeed = 3f;

	public float ClimbSpeed = 16f;

	public float MountSpeed = 5f;

	public float DistanceToClimbable = 1f;

	public float MinVelocityToClimb = 7f;

	public float ClimbAgainTimeout = 1f;

	public bool MountAutoRotatePitch;

	public bool SimpleClimb = true;

	public float DismountForce = 0.2f;

	public vp_ClimbingSounds Sounds;

	protected int m_LastWeaponEquipped;

	protected bool m_IsClimbing;

	protected float m_CanClimbAgain;

	protected Vector3 m_CachedDirection = Vector3.zero;

	protected Vector2 m_CachedRotation = Vector2.zero;

	protected vp_Timer.Handle m_ClimbingSoundTimer = new vp_Timer.Handle();

	protected AudioClip m_SoundToPlay;

	protected AudioClip m_LastPlayedSound;

	protected override void Start()
	{
		base.Start();
		m_CanClimbAgain = Time.time;
	}

	public override bool TryInteract(vp_FPPlayerEventHandler player)
	{
		if (!base.enabled)
		{
			return false;
		}
		if (Time.time < m_CanClimbAgain)
		{
			return false;
		}
		if (m_IsClimbing)
		{
			m_Player.Climb.TryStop();
			return false;
		}
		if (m_Player == null)
		{
			m_Player = player;
		}
		if (m_Player.Interactable.Get() != null)
		{
			return false;
		}
		if (m_Controller == null)
		{
			m_Controller = m_Player.GetComponent<vp_FPController>();
		}
		if (m_Player.Velocity.Get().magnitude > MinVelocityToClimb)
		{
			return false;
		}
		if (m_Camera == null)
		{
			m_Camera = m_Player.GetComponentInChildren<vp_FPCamera>();
		}
		float f = Vector3.Angle(-m_Transform.forward, m_Transform.position - m_Camera.Transform.position);
		if (Mathf.Abs(f) > 75f)
		{
			Vector3 position = m_Controller.Transform.position;
			if (position.y < GetTopOfCollider(m_Transform))
			{
				return false;
			}
		}
		if (Sounds.AudioSource == null)
		{
			Sounds.AudioSource = m_Player.GetComponent<AudioSource>();
		}
		m_Player.Register(this);
		m_Player.Interactable.Set(this);
		return m_Player.Climb.TryStart();
	}

	protected virtual void OnStart_Climb()
	{
		m_Controller.PhysicsGravityModifier = 0f;
		m_Camera.SetRotation(m_Camera.Transform.eulerAngles, stop: false);
		m_Player.Jump.Stop();
		m_Player.AllowGameplayInput.Set(o: false);
		m_Player.Stop.Send();
		m_LastWeaponEquipped = m_Player.CurrentWeaponIndex.Get();
		m_Player.SetWeapon.TryStart(0);
		m_Player.Interactable.Set(null);
		PlaySound(Sounds.MountSounds);
		if (m_Controller.Transform.GetComponent<Collider>().enabled && m_Transform.GetComponent<Collider>().enabled)
		{
			Physics.IgnoreCollision(m_Controller.Transform.GetComponent<Collider>(), m_Transform.GetComponent<Collider>(), ignore: true);
		}
		StartCoroutine("LineUp");
	}

	protected virtual void PlaySound(List<AudioClip> sounds)
	{
		if (Sounds.AudioSource == null || sounds == null || sounds.Count == 0)
		{
			return;
		}
		do
		{
			m_SoundToPlay = sounds[UnityEngine.Random.Range(0, sounds.Count)];
			if (m_SoundToPlay == null)
			{
				return;
			}
		}
		while (m_SoundToPlay == m_LastPlayedSound && sounds.Count > 1);
		if (sounds == Sounds.ClimbingSounds)
		{
			Sounds.AudioSource.pitch = UnityEngine.Random.Range(Sounds.ClimbingPitch.x, Sounds.ClimbingPitch.y) * Time.timeScale;
		}
		else
		{
			Sounds.AudioSource.pitch = 1f;
		}
		Sounds.AudioSource.PlayOneShot(m_SoundToPlay);
		m_LastPlayedSound = m_SoundToPlay;
	}

	protected virtual IEnumerator LineUp()
	{
		Vector3 startPosition = m_Player.Position.Get();
		Vector3 endPosition3 = GetNewPosition();
		Quaternion startingRotation = m_Camera.transform.rotation;
		Quaternion endRotation2 = Quaternion.LookRotation(-m_Transform.forward);
		Vector3 position = m_Controller.Transform.position;
		float y = position.y;
		Vector3 center = m_Transform.GetComponent<Collider>().bounds.center;
		bool fromTop = y > center.y;
		endPosition3 = ((!fromTop) ? (endPosition3 + m_Controller.Transform.up * (m_Controller.CharacterController.height / 2f)) : (endPosition3 + Vector3.down * m_Controller.CharacterController.height));
		if (fromTop)
		{
			Vector3 vector = m_Transform.InverseTransformDirection(m_Player.Forward.Get());
			if (vector.z > 0f)
			{
				Vector3 eulerAngles = endRotation2.eulerAngles;
				float y2 = eulerAngles.y;
				Vector3 eulerAngles2 = endRotation2.eulerAngles;
				endRotation2 = Quaternion.Euler(new Vector3(45f, y2, eulerAngles2.z));
				goto IL_0224;
			}
		}
		Vector3 eulerAngles3 = endRotation2.eulerAngles;
		float y3 = eulerAngles3.y;
		Vector3 eulerAngles4 = endRotation2.eulerAngles;
		endRotation2 = Quaternion.Euler(new Vector3(-45f, y3, eulerAngles4.z));
		goto IL_0224;
		IL_0224:
		Vector3 center2 = m_Transform.GetComponent<Collider>().bounds.center;
		float x = center2.x;
		float y4 = endPosition3.y;
		Vector3 center3 = m_Transform.GetComponent<Collider>().bounds.center;
		endPosition3 = new Vector3(x, y4, center3.z);
		endPosition3 += m_Transform.forward;
		float t = 0f;
		float duration = Vector3.Distance(m_Controller.Transform.position, endPosition3) / ((!fromTop) ? (MountSpeed / 1.25f) : MountSpeed);
		while (t < 1f)
		{
			t += Time.deltaTime / duration;
			Vector3 newPosition = Vector3.Lerp(startPosition, endPosition3, t);
			m_Player.Position.Set(newPosition);
			Quaternion newRotation = Quaternion.Slerp(startingRotation, endRotation2, t);
			vp_Value<Vector2>.Setter<Vector2> set = m_Player.Rotation.Set;
			float x2;
			if (!MountAutoRotatePitch)
			{
				Vector2 vector2 = m_Player.Rotation.Get();
				x2 = vector2.x;
			}
			else
			{
				Vector3 eulerAngles5 = newRotation.eulerAngles;
				x2 = eulerAngles5.x;
			}
			Vector3 eulerAngles6 = newRotation.eulerAngles;
			set(new Vector2(x2, eulerAngles6.y));
			yield return new WaitForEndOfFrame();
		}
		m_CachedDirection = m_Camera.Transform.forward;
		m_CachedRotation = m_Player.Rotation.Get();
		m_IsClimbing = true;
	}

	protected virtual void OnStop_Climb()
	{
		m_Player.Interactable.Set(null);
		m_Player.AllowGameplayInput.Set(o: true);
		m_Player.SetWeapon.TryStart(m_LastWeaponEquipped);
		m_Player.Unregister(this);
		m_CanClimbAgain = Time.time + ClimbAgainTimeout;
		if (m_Controller.Transform.GetComponent<Collider>().enabled && m_Transform.GetComponent<Collider>().enabled)
		{
			Physics.IgnoreCollision(m_Controller.Transform.GetComponent<Collider>(), m_Transform.GetComponent<Collider>(), ignore: false);
		}
		PlaySound(Sounds.DismountSounds);
		Vector3 vector = m_Controller.Transform.forward * DismountForce;
		Vector3 center = m_Transform.GetComponent<Collider>().bounds.center;
		float y = center.y;
		Vector3 vector2 = m_Player.Position.Get();
		if (y < vector2.y)
		{
			vector *= 2f;
			vector.y = DismountForce * 0.5f;
		}
		else
		{
			vector = -vector * 0.5f;
		}
		m_Player.Stop.Send();
		m_Controller.AddForce(vector);
		m_IsClimbing = false;
		m_Player.SetState("Default");
		StartCoroutine("RestorePitch");
	}

	protected virtual IEnumerator RestorePitch()
	{
		float t = 0f;
		while (t < 1f && vp_Input.GetAxisRaw("Mouse Y") == 0f)
		{
			t += Time.deltaTime;
			vp_Value<Vector2>.Setter<Vector2> set = m_Player.Rotation.Set;
			Vector2 a = m_Player.Rotation.Get();
			Vector2 vector = m_Player.Rotation.Get();
			set(Vector2.Lerp(a, new Vector2(0f, vector.y), t));
			yield return new WaitForEndOfFrame();
		}
	}

	protected virtual bool CanStart_Interact()
	{
		if (m_IsClimbing)
		{
			m_Player.Climb.TryStop();
		}
		return true;
	}

	protected virtual void FixedUpdate()
	{
		Climbing();
	}

	protected virtual void Update()
	{
		InputJump();
	}

	protected virtual void OnStart_Dead()
	{
		FinishInteraction();
	}

	public override void FinishInteraction()
	{
		if (m_IsClimbing)
		{
			m_Player.Climb.TryStop();
		}
	}

	protected virtual void Climbing()
	{
		if (m_Player == null || !m_IsClimbing)
		{
			return;
		}
		m_Controller.PhysicsGravityModifier = 0f;
		m_Camera.RotationYawLimit = new Vector2(m_CachedRotation.y - 90f, m_CachedRotation.y + 90f);
		m_Camera.RotationPitchLimit = new Vector2(90f, -90f);
		Vector3 newPosition = GetNewPosition();
		Vector3 a = Vector3.zero;
		Vector2 vector = m_Player.Rotation.Get();
		float num = vector.x / 90f;
		float num2 = MinimumClimbSpeed / ClimbSpeed;
		if (Mathf.Abs(num) < num2)
		{
			num = ((!(num <= 0f)) ? num2 : (num2 * -1f));
		}
		if (num < 0f)
		{
			a = Vector3.up * (0f - num);
		}
		else if (num > 0f)
		{
			a = Vector3.down * num;
		}
		float num3 = ClimbSpeed;
		Vector3 vector2 = a * vp_Input.GetAxisRaw("Vertical");
		float num4 = vector2.y;
		if (SimpleClimb)
		{
			a = Vector3.up;
			num3 *= 0.75f;
			num4 = vp_Input.GetAxisRaw("Vertical");
		}
		if ((num4 > 0f && newPosition.y > GetTopOfCollider(m_Transform) - m_Controller.CharacterController.height * 0.25f) || (num4 < 0f && m_Controller.Grounded && m_Controller.GroundTransform.GetInstanceID() != m_Transform.GetInstanceID()))
		{
			m_Player.Climb.TryStop();
			return;
		}
		if (vp_Input.GetAxisRaw("Vertical") == 0f)
		{
			m_ClimbingSoundTimer.Cancel();
		}
		if (vp_Input.GetAxisRaw("Vertical") != 0f && !m_ClimbingSoundTimer.Active && Sounds.ClimbingSounds.Count > 0)
		{
			float num5 = Mathf.Abs(5f / a.y * (Time.deltaTime * 5f) / Sounds.ClimbingSoundSpeed);
			vp_Timer.In(SimpleClimb ? (num5 * 3f) : num5, delegate
			{
				PlaySound(Sounds.ClimbingSounds);
			}, m_ClimbingSoundTimer);
		}
		newPosition += a * num3 * Time.deltaTime * vp_Input.GetAxisRaw("Vertical");
		m_Player.Position.Set(Vector3.Slerp(m_Controller.Transform.position, newPosition, Time.deltaTime * num3));
	}

	protected virtual Vector3 GetNewPosition()
	{
		Vector3 vector = m_Controller.Transform.position;
		Ray ray = new Ray(m_Controller.Transform.position, m_CachedDirection);
		Physics.Raycast(ray, out RaycastHit hitInfo, DistanceToClimbable * 4f);
		if (hitInfo.collider != null && hitInfo.transform.GetInstanceID() == m_Transform.GetInstanceID() && (hitInfo.distance > DistanceToClimbable || hitInfo.distance < DistanceToClimbable))
		{
			vector = (vector - hitInfo.point).normalized * DistanceToClimbable + hitInfo.point;
		}
		return vector;
	}

	protected virtual void InputJump()
	{
		if (m_IsClimbing && !(m_Player == null) && (vp_Input.GetButton("Jump") || vp_Input.GetButtonDown("Interact")))
		{
			m_Player.Climb.TryStop();
			if (vp_Input.GetButton("Jump"))
			{
				m_Controller.AddForce(-m_Controller.Transform.forward * m_Controller.MotorJumpForce);
			}
		}
	}

	public static float GetTopOfCollider(Transform t)
	{
		Vector3 position = t.position;
		float y = position.y;
		Vector3 size = t.GetComponent<Collider>().bounds.size;
		return y + size.y / 2f;
	}
}
