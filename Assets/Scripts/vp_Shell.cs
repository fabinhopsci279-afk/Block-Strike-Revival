using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]
public class vp_Shell : MonoBehaviour
{
	public delegate void RestAngleFunc();

	private Transform m_Transform;

	private Rigidbody m_Rigidbody;

	private AudioSource m_Audio;

	public float LifeTime = 10f;

	protected float m_RemoveTime;

	public float m_Persistence = 1f;

	protected RestAngleFunc m_RestAngleFunc;

	protected float m_RestTime;

	public List<AudioClip> m_BounceSounds = new List<AudioClip>();

	private void Awake()
	{
		m_Transform = base.transform;
		m_Rigidbody = GetComponent<Rigidbody>();
		m_Audio = GetComponent<AudioSource>();
		GetComponent<AudioSource>().playOnAwake = false;
		GetComponent<AudioSource>().dopplerLevel = 0f;
	}

	private void OnEnable()
	{
		m_RestAngleFunc = null;
		m_RemoveTime = Time.time + LifeTime;
		m_RestTime = Time.time + LifeTime * 0.25f;
		m_Rigidbody.maxAngularVelocity = 100f;
		m_Rigidbody.velocity = Vector3.zero;
		m_Rigidbody.angularVelocity = Vector3.zero;
		m_Rigidbody.constraints = RigidbodyConstraints.None;
		GetComponent<Collider>().enabled = true;
	}

	private void Update()
	{
		if (m_RestAngleFunc == null)
		{
			if (Time.time > m_RestTime)
			{
				DecideRestAngle();
			}
		}
		else
		{
			m_RestAngleFunc();
		}
		if (Time.time > m_RemoveTime)
		{
			m_Transform.localScale = Vector3.Lerp(m_Transform.localScale, Vector3.zero, Time.deltaTime * 60f * 0.2f);
			if (Time.time > m_RemoveTime + 0.5f)
			{
				vp_Utility.Destroy(base.gameObject);
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (collision.relativeVelocity.magnitude > 2f)
		{
			if (UnityEngine.Random.value > 0.5f)
			{
				m_Rigidbody.AddRelativeTorque(-Random.rotation.eulerAngles * 0.15f);
			}
			else
			{
				m_Rigidbody.AddRelativeTorque(UnityEngine.Random.rotation.eulerAngles * 0.15f);
			}
			if (m_Audio != null && m_BounceSounds.Count > 0)
			{
				m_Audio.pitch = Time.timeScale;
				m_Audio.PlayOneShot(m_BounceSounds[Random.Range(0, m_BounceSounds.Count)]);
			}
		}
		else if (UnityEngine.Random.value > m_Persistence)
		{
			GetComponent<Collider>().enabled = false;
			m_RemoveTime = Time.time + 0.5f;
		}
	}

	protected void DecideRestAngle()
	{
		Vector3 eulerAngles = m_Transform.eulerAngles;
		float num = Mathf.Abs(eulerAngles.x - 270f);
		if (num < 55f)
		{
			Ray ray = new Ray(m_Transform.position, Vector3.down);
			if (Physics.Raycast(ray, out RaycastHit hitInfo, 1f) && hitInfo.normal == Vector3.up)
			{
				m_RestAngleFunc = UpRight;
				m_Rigidbody.constraints = (RigidbodyConstraints)80;
			}
		}
		else
		{
			m_RestAngleFunc = TippedOver;
		}
	}

	protected void UpRight()
	{
		Transform transform = m_Transform;
		Quaternion rotation = m_Transform.rotation;
		Quaternion rotation2 = m_Transform.rotation;
		float y = rotation2.y;
		Quaternion rotation3 = m_Transform.rotation;
		transform.rotation = Quaternion.Lerp(rotation, Quaternion.Euler(-90f, y, rotation3.z), Time.time * (Time.deltaTime * 60f * 0.05f));
	}

	protected void TippedOver()
	{
		Transform transform = m_Transform;
		Quaternion localRotation = m_Transform.localRotation;
		Vector3 localEulerAngles = m_Transform.localEulerAngles;
		float y = localEulerAngles.y;
		Vector3 localEulerAngles2 = m_Transform.localEulerAngles;
		transform.localRotation = Quaternion.Lerp(localRotation, Quaternion.Euler(0f, y, localEulerAngles2.z), Time.time * (Time.deltaTime * 60f * 0.005f));
	}
}
