using UnityEngine;

public class vp_Placement
{
	public Vector3 Position = Vector3.zero;

	public Quaternion Rotation = Quaternion.identity;

	public static bool AdjustPosition(vp_Placement p, float physicsRadius, int attempts = 1000)
	{
		attempts--;
		if (attempts > 0)
		{
			if (p.IsObstructed(physicsRadius))
			{
				Vector3 insideUnitSphere = Random.insideUnitSphere;
				p.Position.x = p.Position.x + insideUnitSphere.x;
				p.Position.z = p.Position.z + insideUnitSphere.z;
				AdjustPosition(p, physicsRadius, attempts);
			}
			return true;
		}
		UnityEngine.Debug.LogWarning("(vp_Placement.AdjustPosition) Failed to find valid placement.");
		return false;
	}

	public virtual bool IsObstructed(float physicsRadius = 1f)
	{
		return Physics.CheckSphere(Position, physicsRadius, 1342177280);
	}

	public static void SnapToGround(vp_Placement p, float radius, float snapDistance)
	{
		if (snapDistance != 0f)
		{
			Physics.SphereCast(new Ray(p.Position + Vector3.up * snapDistance, Vector3.down), radius, out RaycastHit hitInfo, snapDistance * 2f, -1744830485);
			if (hitInfo.collider != null)
			{
				ref Vector3 position = ref p.Position;
				Vector3 point = hitInfo.point;
				position.y = point.y + 0.05f;
			}
		}
	}
}
