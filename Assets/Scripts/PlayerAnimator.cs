using System;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
	[Serializable]
	public class LegsAnimData
	{
		public Key[] defaultValue;

		public Key[] groundedValue;

		public Keys[] moveValue;

		public int moveIndex;

		public void SetDefault()
		{
			for (int i = 0; i < defaultValue.Length; i++)
			{
				defaultValue[i].target.localPosition = defaultValue[i].pos;
				defaultValue[i].target.localRotation = defaultValue[i].rot;
			}
		}

		public void SetGrounded(bool grounded)
		{
			if (!grounded)
			{
				for (int i = 0; i < defaultValue.Length; i++)
				{
					groundedValue[i].target.localPosition = groundedValue[i].pos;
					groundedValue[i].target.localRotation = groundedValue[i].rot;
				}
			}
			else
			{
				SetDefault();
			}
		}

		public void UpdateMove(float speed)
		{
			if (speed >= 0f)
			{
				moveIndex++;
				if (moveIndex >= moveValue[0].pos.Length)
				{
					moveIndex = 0;
				}
			}
			else
			{
				moveIndex--;
				if (moveIndex <= 0)
				{
					moveIndex = moveValue[0].pos.Length - 1;
				}
				speed = Mathf.Abs(speed);
			}
			for (int i = 0; i < moveValue.Length; i++)
			{
				moveValue[i].target.localPosition = Vector3.Lerp(defaultValue[i].pos, moveValue[i].pos[moveIndex], speed);
				moveValue[i].target.localRotation = Quaternion.Lerp(defaultValue[i].rot, moveValue[i].rot[moveIndex], speed);
			}
		}
	}

	[Serializable]
	public class Key
	{
		public Transform target;

		public Vector3 pos;

		public Quaternion rot;
	}

	[Serializable]
	public class Keys
	{
		public Transform target;

		public Vector3[] pos;

		public Quaternion[] rot;
	}

	public LegsAnimData legs;

	public Transform root;

	private float cachedRotate;

	private bool cachedGrounded;

	private float cachedMove;

	public bool grounded
	{
		get
		{
			return cachedGrounded;
		}
		set
		{
			if (cachedGrounded != value)
			{
				cachedGrounded = value;
				legs.SetGrounded(value);
			}
		}
	}

	public float move
	{
		get
		{
			return cachedMove;
		}
		set
		{
			if (value == 0f)
			{
				if (cachedMove != 0f && cachedGrounded)
				{
					legs.UpdateMove(value);
					cachedMove = value;
				}
			}
			else
			{
				cachedMove = value;
			}
		}
	}

	private void FixedUpdate()
	{
		if (cachedMove != 0f && cachedGrounded)
		{
			legs.UpdateMove(cachedMove);
		}
	}

	public void SetDefault()
	{
		root.localPosition = Vector3.zero;
		root.localEulerAngles = Vector3.zero;
		cachedRotate = 0f;
		cachedMove = 0f;
		cachedGrounded = true;
		legs.SetGrounded(grounded);
	}
}
