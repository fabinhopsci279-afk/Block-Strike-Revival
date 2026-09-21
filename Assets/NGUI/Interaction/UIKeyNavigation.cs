using System;
using UnityEngine;

// Token: 0x0200003B RID: 59
[AddComponentMenu("NGUI/Interaction/Key Navigation")]
public class UIKeyNavigation : MonoBehaviour
{
	// Token: 0x17000010 RID: 16
	// (get) Token: 0x060001DE RID: 478 RVA: 0x00024120 File Offset: 0x00022320
	public static UIKeyNavigation current
	{
		get
		{
			GameObject hoveredObject = UICamera.hoveredObject;
			if (hoveredObject == null)
			{
				return null;
			}
			return hoveredObject.GetComponent<UIKeyNavigation>();
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x060001DF RID: 479 RVA: 0x00024144 File Offset: 0x00022344
	public bool isColliderEnabled
	{
		get
		{
			if (!base.enabled || !base.gameObject.activeInHierarchy)
			{
				return false;
			}
			Collider component = base.GetComponent<Collider>();
			if (component != null)
			{
				return component.enabled;
			}
			Collider2D component2 = base.GetComponent<Collider2D>();
			return component2 != null && component2.enabled;
		}
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x00003DC5 File Offset: 0x00001FC5
	protected virtual void OnEnable()
	{
		UIKeyNavigation.list.Add(this);
		if (this.mStarted)
		{
			this.Start();
		}
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x00003DE0 File Offset: 0x00001FE0
	private void Start()
	{
		this.mStarted = true;
		if (this.startsSelected && this.isColliderEnabled)
		{
			UICamera.hoveredObject = base.gameObject;
		}
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x00003E04 File Offset: 0x00002004
	protected virtual void OnDisable()
	{
		UIKeyNavigation.list.Remove(this);
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x00024198 File Offset: 0x00022398
	private static bool IsActive(GameObject go)
	{
		if (!go || !go.activeInHierarchy)
		{
			return false;
		}
		Collider component = go.GetComponent<Collider>();
		if (component != null)
		{
			return component.enabled;
		}
		Collider2D component2 = go.GetComponent<Collider2D>();
		return component2 != null && component2.enabled;
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x000241E8 File Offset: 0x000223E8
	public GameObject GetLeft()
	{
		if (UIKeyNavigation.IsActive(this.onLeft))
		{
			return this.onLeft;
		}
		if (this.constraint != UIKeyNavigation.Constraint.Vertical)
		{
			if (this.constraint != UIKeyNavigation.Constraint.Explicit)
			{
				return this.Get(Vector3.left, 1f, 2f);
			}
		}
		return null;
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x00024234 File Offset: 0x00022434
	public GameObject GetRight()
	{
		if (UIKeyNavigation.IsActive(this.onRight))
		{
			return this.onRight;
		}
		if (this.constraint != UIKeyNavigation.Constraint.Vertical)
		{
			if (this.constraint != UIKeyNavigation.Constraint.Explicit)
			{
				return this.Get(Vector3.right, 1f, 2f);
			}
		}
		return null;
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x00024280 File Offset: 0x00022480
	public GameObject GetUp()
	{
		if (UIKeyNavigation.IsActive(this.onUp))
		{
			return this.onUp;
		}
		if (this.constraint != UIKeyNavigation.Constraint.Horizontal)
		{
			if (this.constraint != UIKeyNavigation.Constraint.Explicit)
			{
				return this.Get(Vector3.up, 2f, 1f);
			}
		}
		return null;
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x000242CC File Offset: 0x000224CC
	public GameObject GetDown()
	{
		if (UIKeyNavigation.IsActive(this.onDown))
		{
			return this.onDown;
		}
		if (this.constraint != UIKeyNavigation.Constraint.Horizontal)
		{
			if (this.constraint != UIKeyNavigation.Constraint.Explicit)
			{
				return this.Get(Vector3.down, 2f, 1f);
			}
		}
		return null;
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x00024318 File Offset: 0x00022518
	public GameObject Get(Vector3 myDir, float x = 1f, float y = 1f)
	{
		Transform transform = base.transform;
		myDir = transform.TransformDirection(myDir);
		Vector3 center = UIKeyNavigation.GetCenter(base.gameObject);
		float num = float.MaxValue;
		GameObject result = null;
		for (int i = 0; i < UIKeyNavigation.list.size; i++)
		{
			UIKeyNavigation uikeyNavigation = UIKeyNavigation.list[i];
			if (!(uikeyNavigation == this) && uikeyNavigation.constraint != UIKeyNavigation.Constraint.Explicit && uikeyNavigation.isColliderEnabled)
			{
				UIWidget component = uikeyNavigation.GetComponent<UIWidget>();
				if (!(component != null) || component.alpha != 0f)
				{
					Vector3 direction = UIKeyNavigation.GetCenter(uikeyNavigation.gameObject) - center;
					float num2 = Vector3.Dot(myDir, direction.normalized);
					if (num2 >= 0.707f)
					{
						direction = transform.InverseTransformDirection(direction);
						direction.x *= x;
						direction.y *= y;
						float sqrMagnitude = direction.sqrMagnitude;
						if (sqrMagnitude <= num)
						{
							result = uikeyNavigation.gameObject;
							num = sqrMagnitude;
						}
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x00024430 File Offset: 0x00022630
	protected static Vector3 GetCenter(GameObject go)
	{
		UIWidget component = go.GetComponent<UIWidget>();
		UICamera uicamera = UICamera.FindCameraForLayer(go.layer);
		if (uicamera != null)
		{
			Vector3 vector = go.transform.position;
			if (component != null)
			{
				Vector3[] worldCorners = component.worldCorners;
				vector = (worldCorners[0] + worldCorners[2]) * 0.5f;
			}
			vector = uicamera.cachedCamera.WorldToScreenPoint(vector);
			vector.z = 0f;
			return vector;
		}
		if (component != null)
		{
			Vector3[] worldCorners2 = component.worldCorners;
			return (worldCorners2[0] + worldCorners2[2]) * 0.5f;
		}
		return go.transform.position;
	}

	// Token: 0x060001EA RID: 490 RVA: 0x00024500 File Offset: 0x00022700
	public virtual void OnNavigate(KeyCode key)
	{
		if (UIPopupList.isOpen)
		{
			return;
		}
		if (UIKeyNavigation.mLastFrame == Time.frameCount)
		{
			return;
		}
		UIKeyNavigation.mLastFrame = Time.frameCount;
		GameObject gameObject = null;
		switch (key)
		{
		case KeyCode.UpArrow:
			gameObject = this.GetUp();
			break;
		case KeyCode.DownArrow:
			gameObject = this.GetDown();
			break;
		case KeyCode.RightArrow:
			gameObject = this.GetRight();
			break;
		case KeyCode.LeftArrow:
			gameObject = this.GetLeft();
			break;
		}
		if (gameObject != null)
		{
			UICamera.hoveredObject = gameObject;
		}
	}

	// Token: 0x060001EB RID: 491 RVA: 0x00024580 File Offset: 0x00022780
	public virtual void OnKey(KeyCode key)
	{
		if (UIPopupList.isOpen)
		{
			return;
		}
		if (UIKeyNavigation.mLastFrame == Time.frameCount)
		{
			return;
		}
		UIKeyNavigation.mLastFrame = Time.frameCount;
		if (key == KeyCode.Tab)
		{
			GameObject gameObject = this.onTab;
			if (gameObject == null)
			{
				if (!UICamera.GetKey(KeyCode.LeftShift) && !UICamera.GetKey(KeyCode.RightShift))
				{
					gameObject = this.GetRight();
					if (gameObject == null)
					{
						gameObject = this.GetDown();
					}
					if (gameObject == null)
					{
						gameObject = this.GetUp();
					}
					if (gameObject == null)
					{
						gameObject = this.GetLeft();
					}
				}
				else
				{
					gameObject = this.GetLeft();
					if (gameObject == null)
					{
						gameObject = this.GetUp();
					}
					if (gameObject == null)
					{
						gameObject = this.GetDown();
					}
					if (gameObject == null)
					{
						gameObject = this.GetRight();
					}
				}
			}
			if (gameObject != null)
			{
				UICamera.currentScheme = UICamera.ControlScheme.Controller;
				UICamera.hoveredObject = gameObject;
				UIInput component = gameObject.GetComponent<UIInput>();
				if (component != null)
				{
					component.isSelected = true;
				}
			}
		}
	}

	// Token: 0x060001EC RID: 492 RVA: 0x00003E12 File Offset: 0x00002012
	protected virtual void OnClick()
	{
		if (NGUITools.GetActive(this.onClick))
		{
			UICamera.hoveredObject = this.onClick;
		}
	}

	// Token: 0x0400012D RID: 301
	public static BetterList<UIKeyNavigation> list = new BetterList<UIKeyNavigation>();

	// Token: 0x0400012E RID: 302
	public UIKeyNavigation.Constraint constraint;

	// Token: 0x0400012F RID: 303
	public GameObject onUp;

	// Token: 0x04000130 RID: 304
	public GameObject onDown;

	// Token: 0x04000131 RID: 305
	public GameObject onLeft;

	// Token: 0x04000132 RID: 306
	public GameObject onRight;

	// Token: 0x04000133 RID: 307
	public GameObject onClick;

	// Token: 0x04000134 RID: 308
	public GameObject onTab;

	// Token: 0x04000135 RID: 309
	public bool startsSelected;

	// Token: 0x04000136 RID: 310
	[NonSerialized]
	private bool mStarted;

	// Token: 0x04000137 RID: 311
	public static int mLastFrame = 0;

	// Token: 0x0200003C RID: 60
	public enum Constraint
	{
		// Token: 0x04000139 RID: 313
		None,
		// Token: 0x0400013A RID: 314
		Vertical,
		// Token: 0x0400013B RID: 315
		Horizontal,
		// Token: 0x0400013C RID: 316
		Explicit
	}
}
