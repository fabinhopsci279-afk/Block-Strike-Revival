using UnityEngine;

public class PlayerSkinRagdoll : TimerBehaviour
{
	public PlayerSkin m_PlayerSkin;

	public float Force = 100f;

	public Collider[] Colliders;

	public Rigidbody[] Rigidbodies;

	public Transform[] Transforms;

	public Vector3[] Positions;

	public Quaternion[] Rotations;

	private bool ActiveRagdoll;

	private bool isActive;

	private Vector3 ForceVector;

	private bool isRagdoll;

	private void Start()
	{
		for (int i = 0; i < Rigidbodies.Length; i++)
		{
			Rigidbodies[i].detectCollisions = true;
		}
		isRagdoll = SaveLoadManager.GetRagdoll();
		if (!isRagdoll)
		{
			CharacterJoint[] componentsInChildren = base.gameObject.GetComponentsInChildren<CharacterJoint>();
			CharacterJoint[] array = componentsInChildren;
			foreach (CharacterJoint obj in array)
			{
				UnityEngine.Object.DestroyImmediate(obj);
			}
			Rigidbody[] rigidbodies = Rigidbodies;
			foreach (Rigidbody obj2 in rigidbodies)
			{
				UnityEngine.Object.DestroyImmediate(obj2);
			}
			for (int l = 0; l < Colliders.Length; l++)
			{
				Colliders[l].isTrigger = true;
			}
		}
		else
		{
			ActiveCoroutine();
			DeactiveCoroutine();
		}
	}

	private void Update()
	{
		if (isActive && !m_PlayerSkin.PlayerRenderer.isVisible)
		{
			isActive = false;
			Deactive();
			m_PlayerSkin.SetPosition(new Vector3(0f, -100f, 0f));
			base.gameObject.SetActive(value: false);
		}
	}

	[ContextMenu("Active")]
	public void Active()
	{
		Active(Vector3.zero);
	}

	public void Active(Vector3 vector, bool headShot = false)
	{
		ForceVector = vector;
		ActiveCoroutine();
	}

	private void ActiveCoroutine()
	{
		if (isRagdoll)
		{
			if (ForceVector.magnitude < 0.5f)
			{
				ForceVector = new Vector3(0f, 0f, Random.Range(-1, 1));
			}
			m_PlayerSkin.PlayerAnimator.enabled = false;
			for (int i = 0; i < Colliders.Length; i++)
			{
				Colliders[i].isTrigger = false;
			}
			for (int j = 0; j < Rigidbodies.Length; j++)
			{
				Rigidbodies[j].detectCollisions = true;
				Rigidbodies[j].isKinematic = false;
				Rigidbodies[j].AddForce(ForceVector * Force);
			}
		}
		else
		{
			m_PlayerSkin.PlayerAnimator.SetBool("Dead", value: true);
		}
		if (!ActiveRagdoll)
		{
			vp_Timer.In(2f, delegate
			{
				if (ActiveRagdoll)
				{
					isActive = true;
				}
			}, GetTimer());
		}
		ActiveRagdoll = true;
	}

	[ContextMenu("Deactive")]
	public void Deactive()
	{
		DeactiveCoroutine();
	}

	private void DeactiveCoroutine()
	{
		ActiveRagdoll = false;
		isActive = false;
		if (isRagdoll)
		{
			m_PlayerSkin.PlayerRenderer.enabled = true;
			for (int i = 0; i < Rigidbodies.Length; i++)
			{
				Rigidbodies[i].detectCollisions = true;
				Rigidbodies[i].isKinematic = true;
			}
			for (int j = 0; j < Colliders.Length; j++)
			{
				Colliders[j].isTrigger = true;
			}
			for (int k = 0; k < Transforms.Length; k++)
			{
				Transforms[k].localPosition = Positions[k];
				Transforms[k].localRotation = Rotations[k];
			}
			m_PlayerSkin.PlayerAnimator.enabled = true;
		}
		else
		{
			m_PlayerSkin.PlayerAnimator.SetBool("Dead", value: false);
		}
	}

	[ContextMenu("Get Ragdoll")]
	private void GetRagdoll()
	{
		Colliders = base.gameObject.GetComponentsInChildren<Collider>();
		Rigidbodies = base.gameObject.GetComponentsInChildren<Rigidbody>();
		Transforms = new Transform[Rigidbodies.Length];
		Positions = new Vector3[Rigidbodies.Length];
		Rotations = new Quaternion[Rigidbodies.Length];
		for (int i = 0; i < Rigidbodies.Length; i++)
		{
			Transforms[i] = Rigidbodies[i].transform;
			Positions[i] = Transforms[i].localPosition;
			Rotations[i] = Transforms[i].localRotation;
		}
	}

	[ContextMenu("Destroy Ragdoll")]
	private void DestroyRagdoll()
	{
		CharacterJoint[] componentsInChildren = base.gameObject.GetComponentsInChildren<CharacterJoint>();
		CharacterJoint[] array = componentsInChildren;
		foreach (CharacterJoint obj in array)
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}
		Rigidbody[] rigidbodies = Rigidbodies;
		foreach (Rigidbody obj2 in rigidbodies)
		{
			UnityEngine.Object.DestroyImmediate(obj2);
		}
		Collider[] colliders = Colliders;
		foreach (Collider obj3 in colliders)
		{
			UnityEngine.Object.DestroyImmediate(obj3);
		}
	}
}
