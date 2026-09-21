using UnityEngine;

public class MenuCamera : MonoBehaviour
{
	public float MoveSpeed;

	public float MoveAmplitude;

	public MeshAtlas[] AK47;

	public MeshAtlas[] M4A1;

	private Vector3 StartPosition;

	private Transform m_Transform;

	private void Start()
	{
		m_Transform = base.transform;
		StartPosition = m_Transform.position;
		vp_Timer.In(0.02f, delegate
		{
			string randomWeaponSkin = SkinsManager.GetRandomWeaponSkin(1);
			for (int i = 0; i < AK47.Length; i++)
			{
				AK47[i].spriteName = randomWeaponSkin;
			}
			string randomWeaponSkin2 = SkinsManager.GetRandomWeaponSkin(5);
			for (int j = 0; j < M4A1.Length; j++)
			{
				M4A1[j].spriteName = randomWeaponSkin2;
			}
		});
	}

	private void LateUpdate()
	{
		m_Transform.position = StartPosition + Vector3.right * Mathf.Cos(Time.time * MoveSpeed) * MoveAmplitude;
	}
}
