using System;
using System.Collections;
using UnityEngine;

public sealed class vp_Layer
{
	public static class Mask
	{
		public const int BulletBlockers = -1828716565;

		public const int ExternalBlockers = -1744830485;

		public const int PhysicsBlockers = 1342177280;

		public const int IgnoreWalkThru = -738197525;
	}

	public const int Default = 0;

	public const int TransparentFX = 1;

	public const int IgnoreRaycast = 2;

	public const int Water = 4;

	public const int IgnoreBullets = 24;

	public const int Enemy = 25;

	public const int Pickup = 26;

	public const int Trigger = 27;

	public const int MovableObject = 28;

	public const int Debris = 29;

	public const int LocalPlayer = 30;

	public const int Weapon = 31;

	public static readonly vp_Layer instance;

	private vp_Layer()
	{
	}

	static vp_Layer()
	{
		instance = new vp_Layer();
		Physics.IgnoreLayerCollision(30, 29);
		Physics.IgnoreLayerCollision(29, 29);
	}

	public static void Set(GameObject obj, int layer, bool recursive = false)
	{
		if (layer < 0 || layer > 31)
		{
			UnityEngine.Debug.LogError("vp_Layer: Attempted to set layer id out of range [0-31].");
			return;
		}
		obj.layer = layer;
		if (recursive)
		{
			IEnumerator enumerator = obj.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object current = enumerator.Current;
					Transform transform = (Transform)current;
					Set(transform.gameObject, layer, recursive: true);
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
		}
	}

	public static bool IsInMask(int layer, int layerMask)
	{
		return (layerMask & (1 << layer)) == 0;
	}
}
