using UnityEngine;

public class PickupItem : MonoBehaviour
{
	public string id;

	public float amplitude;

	public float speed;

	public float rotation;

	private Vector3 startPosition;

	[HideInInspector]
	public Transform mTransform;

	public void SetPosition(Vector3 pos)
	{
		if (mTransform == null)
		{
			mTransform = base.transform;
		}
		mTransform.position = pos;
		startPosition = pos;
	}

	private void Update()
	{
		mTransform.position = startPosition + Vector3.up * Mathf.Cos(Time.time * speed) * amplitude;
		mTransform.Rotate(0f, rotation, 0f, Space.World);
	}

	private void OnTriggerEnter(Collider other)
	{
	}
}
