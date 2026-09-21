using System;
using UnityEngine;

public class DrawElements : MonoBehaviour
{
	public enum ElementType
	{
		Cube,
		WireCube
	}

	public ElementType Element;

	public Color m_Color = Color.red;

	public Vector3 Size = Vector3.one;

	[NonSerialized]
	public Transform m_Transform;

	private void Start()
	{
		m_Transform = base.transform;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = m_Color;
		Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.forward * 5f);
		switch (Element)
		{
		case ElementType.WireCube:
			Gizmos.DrawWireCube(base.transform.position, Size);
			break;
		case ElementType.Cube:
			Gizmos.DrawCube(base.transform.position, Size);
			break;
		}
	}

	public Vector3 GetSpawnPosition()
	{
		Vector3 position = m_Transform.position;
		position.x += UnityEngine.Random.Range((0f - Size.x) / 2f, Size.x / 2f);
		position.z += UnityEngine.Random.Range((0f - Size.z) / 2f, Size.z / 2f);
		position.y += 1f;
		return position;
	}
}
