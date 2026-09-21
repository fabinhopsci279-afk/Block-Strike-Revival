using System;
using UnityEngine;

// Token: 0x02000030 RID: 48
[RequireComponent(typeof(Camera))]
[AddComponentMenu("NGUI/Interaction/Draggable Camera")]
public class UIDraggableCamera : MonoBehaviour
{
	// Token: 0x1700000C RID: 12
	// (get) Token: 0x06000192 RID: 402 RVA: 0x0000387C File Offset: 0x00001A7C
	// (set) Token: 0x06000193 RID: 403 RVA: 0x00003884 File Offset: 0x00001A84
	public Vector2 currentMomentum
	{
		get
		{
			return this.mMomentum;
		}
		set
		{
			this.mMomentum = value;
		}
	}

	// Token: 0x06000194 RID: 404 RVA: 0x0002318C File Offset: 0x0002138C
	private void Start()
	{
		this.mCam = base.GetComponent<Camera>();
		this.mTrans = base.transform;
		this.mRoot = NGUITools.FindInParents<UIRoot>(base.gameObject);
		if (this.rootForBounds == null)
		{
			Debug.LogError(NGUITools.GetHierarchy(base.gameObject) + " needs the 'Root For Bounds' parameter to be set", this);
			base.enabled = false;
		}
	}

	// Token: 0x06000195 RID: 405 RVA: 0x000231F4 File Offset: 0x000213F4
	private Vector3 CalculateConstrainOffset()
	{
		if (!(this.rootForBounds == null) && this.rootForBounds.childCount != 0)
		{
			Vector3 vector = new Vector3(this.mCam.rect.xMin * (float)Screen.width, this.mCam.rect.yMin * (float)Screen.height, 0f);
			Vector3 vector2 = new Vector3(this.mCam.rect.xMax * (float)Screen.width, this.mCam.rect.yMax * (float)Screen.height, 0f);
			vector = this.mCam.ScreenToWorldPoint(vector);
			vector2 = this.mCam.ScreenToWorldPoint(vector2);
			Vector2 minRect = new Vector2(this.mBounds.min.x, this.mBounds.min.y);
			Vector2 maxRect = new Vector2(this.mBounds.max.x, this.mBounds.max.y);
			return NGUIMath.ConstrainRect(minRect, maxRect, vector, vector2);
		}
		return Vector3.zero;
	}

	// Token: 0x06000196 RID: 406 RVA: 0x0002333C File Offset: 0x0002153C
	public bool ConstrainToBounds(bool immediate)
	{
		if (this.mTrans != null && this.rootForBounds != null)
		{
			Vector3 b = this.CalculateConstrainOffset();
			if (b.sqrMagnitude > 0f)
			{
				if (immediate)
				{
					this.mTrans.position -= b;
				}
				else
				{
					SpringPosition springPosition = SpringPosition.Begin(base.gameObject, this.mTrans.position - b, 13f);
					springPosition.ignoreTimeScale = true;
					springPosition.worldSpace = true;
				}
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000197 RID: 407 RVA: 0x000233CC File Offset: 0x000215CC
	public void Press(bool isPressed)
	{
		if (isPressed)
		{
			this.mDragStarted = false;
		}
		if (this.rootForBounds != null)
		{
			this.mPressed = isPressed;
			if (isPressed)
			{
				this.mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(this.rootForBounds);
				this.mMomentum = Vector2.zero;
				this.mScroll = 0f;
				SpringPosition component = base.GetComponent<SpringPosition>();
				if (component != null)
				{
					component.enabled = false;
				}
			}
			else if (this.dragEffect == UIDragObject.DragEffect.MomentumAndSpring)
			{
				this.ConstrainToBounds(false);
			}
		}
	}

	// Token: 0x06000198 RID: 408 RVA: 0x0002344C File Offset: 0x0002164C
	public void Drag(Vector2 delta)
	{
		if (this.smoothDragStart && !this.mDragStarted)
		{
			this.mDragStarted = true;
			return;
		}
		UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
		if (this.mRoot != null)
		{
			delta *= this.mRoot.pixelSizeAdjustment;
		}
		Vector2 vector = Vector2.Scale(delta, -this.scale);
		this.mTrans.localPosition += (Vector3)vector;
		this.mMomentum = Vector2.Lerp(this.mMomentum, this.mMomentum + vector * (0.01f * this.momentumAmount), 0.67f);
		if (this.dragEffect != UIDragObject.DragEffect.MomentumAndSpring && this.ConstrainToBounds(true))
		{
			this.mMomentum = Vector2.zero;
			this.mScroll = 0f;
		}
	}

	// Token: 0x06000199 RID: 409 RVA: 0x00023528 File Offset: 0x00021728
	public void Scroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject))
		{
			if (Mathf.Sign(this.mScroll) != Mathf.Sign(delta))
			{
				this.mScroll = 0f;
			}
			this.mScroll += delta * this.scrollWheelFactor;
		}
	}

	// Token: 0x0600019A RID: 410 RVA: 0x00023580 File Offset: 0x00021780
	private void Update()
	{
		float deltaTime = RealTime.deltaTime;
		if (this.mPressed)
		{
			SpringPosition component = base.GetComponent<SpringPosition>();
			if (component != null)
			{
				component.enabled = false;
			}
			this.mScroll = 0f;
		}
		else
		{
			this.mMomentum += this.scale * (this.mScroll * 20f);
			this.mScroll = NGUIMath.SpringLerp(this.mScroll, 0f, 20f, deltaTime);
			if (this.mMomentum.magnitude > 0.01f)
			{
				this.mTrans.localPosition += (Vector3)NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
				this.mBounds = NGUIMath.CalculateAbsoluteWidgetBounds(this.rootForBounds);
				if (!this.ConstrainToBounds(this.dragEffect == UIDragObject.DragEffect.None))
				{
					SpringPosition component2 = base.GetComponent<SpringPosition>();
					if (component2 != null)
					{
						component2.enabled = false;
					}
				}
				return;
			}
			this.mScroll = 0f;
		}
		NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
	}

	// Token: 0x040000D7 RID: 215
	public Transform rootForBounds;

	// Token: 0x040000D8 RID: 216
	public Vector2 scale = Vector2.one;

	// Token: 0x040000D9 RID: 217
	public float scrollWheelFactor;

	// Token: 0x040000DA RID: 218
	public UIDragObject.DragEffect dragEffect = UIDragObject.DragEffect.MomentumAndSpring;

	// Token: 0x040000DB RID: 219
	public bool smoothDragStart = true;

	// Token: 0x040000DC RID: 220
	public float momentumAmount = 35f;

	// Token: 0x040000DD RID: 221
	private Camera mCam;

	// Token: 0x040000DE RID: 222
	private Transform mTrans;

	// Token: 0x040000DF RID: 223
	private bool mPressed;

	// Token: 0x040000E0 RID: 224
	private Vector2 mMomentum = Vector2.zero;

	// Token: 0x040000E1 RID: 225
	private Bounds mBounds;

	// Token: 0x040000E2 RID: 226
	private float mScroll;

	// Token: 0x040000E3 RID: 227
	private UIRoot mRoot;

	// Token: 0x040000E4 RID: 228
	private bool mDragStarted;
}
