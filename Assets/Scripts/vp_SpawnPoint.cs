using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class vp_SpawnPoint : MonoBehaviour
{
	public bool RandomDirection;

	public float Radius;

	public float GroundSnapThreshold = 2.5f;

	public bool LockGroundSnapToRadius = true;

	protected static List<vp_SpawnPoint> m_MatchingSpawnPoints = new List<vp_SpawnPoint>(50);

	protected static List<vp_SpawnPoint> m_SpawnPoints = null;

	public static List<vp_SpawnPoint> SpawnPoints
	{
		get
		{
			if (m_SpawnPoints == null)
			{
				m_SpawnPoints = new List<vp_SpawnPoint>(UnityEngine.Object.FindObjectsOfType(typeof(vp_SpawnPoint)) as vp_SpawnPoint[]);
			}
			return m_SpawnPoints;
		}
	}

	public static vp_Placement GetRandomPlacement()
	{
		return GetRandomPlacement(0f, null);
	}

	public static vp_Placement GetRandomPlacement(float physicsCheckRadius)
	{
		return GetRandomPlacement(physicsCheckRadius);
	}

	public static vp_Placement GetRandomPlacement(string tag)
	{
		return GetRandomPlacement(0f, tag);
	}

	public static vp_Placement GetRandomPlacement(float physicsCheckRadius, string tag)
	{
		if (SpawnPoints == null || SpawnPoints.Count < 1)
		{
			return null;
		}
		vp_SpawnPoint vp_SpawnPoint = (!string.IsNullOrEmpty(tag)) ? GetRandomSpawnPoint(tag) : GetRandomSpawnPoint();
		if (vp_SpawnPoint == null)
		{
			UnityEngine.Debug.LogError("Error (vp_SpawnPoint --> GetRandomPlacement) Could not find a spawnpoint" + ((!string.IsNullOrEmpty(tag)) ? (" tagged '" + tag + "'.") : "."));
			return null;
		}
		vp_Placement vp_Placement = new vp_Placement();
		vp_Placement.Position = vp_SpawnPoint.transform.position;
		if (vp_SpawnPoint.Radius > 0f)
		{
			Vector3 vector = UnityEngine.Random.insideUnitSphere * vp_SpawnPoint.Radius;
			vp_Placement vp_Placement2 = vp_Placement;
			vp_Placement2.Position.x = vp_Placement2.Position.x + vector.x;
			vp_Placement vp_Placement3 = vp_Placement;
			vp_Placement3.Position.z = vp_Placement3.Position.z + vector.z;
		}
		if (physicsCheckRadius != 0f)
		{
			if (!vp_Placement.AdjustPosition(vp_Placement, physicsCheckRadius))
			{
				return null;
			}
			vp_Placement.SnapToGround(vp_Placement, physicsCheckRadius, vp_SpawnPoint.GroundSnapThreshold);
		}
		if (vp_SpawnPoint.RandomDirection)
		{
			vp_Placement.Rotation = Quaternion.Euler(Vector3.up * UnityEngine.Random.Range(0f, 360f));
		}
		else
		{
			vp_Placement.Rotation = vp_SpawnPoint.transform.rotation;
		}
		return vp_Placement;
	}

	public static vp_SpawnPoint GetRandomSpawnPoint()
	{
		return SpawnPoints[UnityEngine.Random.Range(0, SpawnPoints.Count)];
	}

	public static vp_SpawnPoint GetRandomSpawnPoint(string tag)
	{
		m_MatchingSpawnPoints.Clear();
		for (int i = 0; i < SpawnPoints.Count; i++)
		{
			if (m_SpawnPoints[i].tag == tag)
			{
				m_MatchingSpawnPoints.Add(m_SpawnPoints[i]);
			}
		}
		if (m_MatchingSpawnPoints.Count < 1)
		{
			return null;
		}
		if (m_MatchingSpawnPoints.Count == 1)
		{
			return m_MatchingSpawnPoints[0];
		}
		return m_MatchingSpawnPoints[UnityEngine.Random.Range(0, m_MatchingSpawnPoints.Count)];
	}

	protected virtual void OnLevelWasLoaded()
	{
		m_SpawnPoints = null;
	}
}
