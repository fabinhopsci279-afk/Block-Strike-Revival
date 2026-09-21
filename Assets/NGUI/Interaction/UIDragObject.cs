using System;
using UnityEngine;

// Token: 0x0200002C RID: 44
[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Drag Object")]
public class UIDragObject : MonoBehaviour
{
	// Token: 0x1700000B RID: 11
	// (get) Token: 0x06000178 RID: 376 RVA: 0x0000373C File Offset: 0x0000193C
	// (set) Token: 0x06000179 RID: 377 RVA: 0x00003744 File Offset: 0x00001944
	public Vector3 dragMovement
	{
		get
		{
			return this.scale;
		}
		set
		{
			this.scale = value;
		}
	}

	// Token: 0x0600017A RID: 378 RVA: 0x00022648 File Offset: 0x00020848
	private void OnEnable()
	{
		if (this.scrollWheelFactor != 0f)
		{
			this.scrollMomentum = this.scale * this.scrollWheelFactor;
			this.scrollWheelFactor = 0f;
		}
		if (this.contentRect == null && this.target != null && Application.isPlaying)
		{
			UIWidget component = this.target.GetComponent<UIWidget>();
			if (component != null)
			{
				this.contentRect = component;
			}
		}
		this.mTargetPos = ((!(this.target != null)) ? Vector3.zero : this.target.position);
	}

	// Token: 0x0600017B RID: 379 RVA: 0x0000374D File Offset: 0x0000194D
	private void OnDisable()
	{
		this.mStarted = false;
	}

	// Token: 0x0600017C RID: 380 RVA: 0x000226EC File Offset: 0x000208EC
	private void FindPanel()
	{
		this.panelRegion = ((!(this.target != null)) ? null : UIPanel.Find(this.target.transform.parent));
		if (this.panelRegion == null)
		{
			this.restrictWithinPanel = false;
		}
	}

	// Token: 0x0600017D RID: 381 RVA: 0x0002273C File Offset: 0x0002093C
	private void UpdateBounds()
	{
		if (this.contentRect)
		{
			Transform cachedTransform = this.panelRegion.cachedTransform;
			Matrix4x4 worldToLocalMatrix = cachedTransform.worldToLocalMatrix;
			Vector3[] worldCorners = this.contentRect.worldCorners;
			for (int i = 0; i < 4; i++)
			{
				worldCorners[i] = worldToLocalMatrix.MultiplyPoint3x4(worldCorners[i]);
			}
			this.mBounds = new Bounds(worldCorners[0], Vector3.zero);
			for (int j = 1; j < 4; j++)
			{
				this.mBounds.Encapsulate(worldCorners[j]);
			}
		}
		else
		{
			this.mBounds = NGUIMath.CalculateRelativeWidgetBounds(this.panelRegion.cachedTransform, this.target);
		}
	}

	// Token: 0x0600017E RID: 382 RVA: 0x00022808 File Offset: 0x00020A08
	private void OnPress(bool pressed)
	{
		if (UICamera.currentTouchID != -2)
		{
			if (UICamera.currentTouchID != -3)
			{
				float timeScale = Time.timeScale;
				if (timeScale < 0.01f && timeScale != 0f)
				{
					return;
				}
				if (base.enabled && NGUITools.GetActive(base.gameObject) && this.target != null)
				{
					if (pressed)
					{
						if (!this.mPressed)
						{
							this.mTouchID = UICamera.currentTouchID;
							this.mPressed = true;
							this.mStarted = false;
							this.CancelMovement();
							if (this.restrictWithinPanel && this.panelRegion == null)
							{
								this.FindPanel();
							}
							if (this.restrictWithinPanel)
							{
								this.UpdateBounds();
							}
							this.CancelSpring();
							Transform transform = UICamera.currentCamera.transform;
							this.mPlane = new Plane(((!(this.panelRegion != null)) ? transform.rotation : this.panelRegion.cachedTransform.rotation) * Vector3.back, UICamera.lastWorldPosition);
						}
					}
					else if (this.mPressed && this.mTouchID == UICamera.currentTouchID)
					{
						this.mPressed = false;
						if (this.restrictWithinPanel && this.dragEffect == UIDragObject.DragEffect.MomentumAndSpring && this.panelRegion.ConstrainTargetToBounds(this.target, ref this.mBounds, false))
						{
							this.CancelMovement();
						}
					}
				}
				return;
			}
		}
	}

	// Token: 0x0600017F RID: 383 RVA: 0x00022970 File Offset: 0x00020B70
	private void OnDrag(Vector2 delta)
	{
		if (this.mPressed && this.mTouchID == UICamera.currentTouchID && base.enabled && NGUITools.GetActive(base.gameObject) && this.target != null)
		{
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			Ray ray = UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos);
			float distance = 0f;
			if (this.mPlane.Raycast(ray, out distance))
			{
				Vector3 point = ray.GetPoint(distance);
				Vector3 vector = point - this.mLastPos;
				this.mLastPos = point;
				if (!this.mStarted)
				{
					this.mStarted = true;
					vector = Vector3.zero;
				}
				if (vector.x != 0f || vector.y != 0f)
				{
					vector = this.target.InverseTransformDirection(vector);
					vector.Scale(this.scale);
					vector = this.target.TransformDirection(vector);
				}
				if (this.dragEffect != UIDragObject.DragEffect.None)
				{
					this.mMomentum = Vector3.Lerp(this.mMomentum, this.mMomentum + vector * (0.01f * this.momentumAmount), 0.67f);
				}
				Vector3 localPosition = this.target.localPosition;
				this.Move(vector);
				if (this.restrictWithinPanel)
				{
					this.mBounds.center = this.mBounds.center + (this.target.localPosition - localPosition);
					if (this.dragEffect != UIDragObject.DragEffect.MomentumAndSpring && this.panelRegion.ConstrainTargetToBounds(this.target, ref this.mBounds, true))
					{
						this.CancelMovement();
					}
				}
			}
		}
	}

	// Token: 0x06000180 RID: 384 RVA: 0x00022B24 File Offset: 0x00020D24
	private void Move(Vector3 worldDelta)
	{
		if (this.panelRegion != null)
		{
			this.mTargetPos += worldDelta;
			Transform parent = this.target.parent;
			Rigidbody component = this.target.GetComponent<Rigidbody>();
			if (parent != null)
			{
				Vector3 vector = parent.worldToLocalMatrix.MultiplyPoint3x4(this.mTargetPos);
				vector.x = Mathf.Round(vector.x);
				vector.y = Mathf.Round(vector.y);
				if (component != null)
				{
					vector = parent.localToWorldMatrix.MultiplyPoint3x4(vector);
					component.position = vector;
				}
				else
				{
					this.target.localPosition = vector;
				}
			}
			else if (component != null)
			{
				component.position = this.mTargetPos;
			}
			else
			{
				this.target.position = this.mTargetPos;
			}
			UIScrollView component2 = this.panelRegion.GetComponent<UIScrollView>();
			if (component2 != null)
			{
				component2.UpdateScrollbars(true);
			}
		}
		else
		{
			this.target.position += worldDelta;
		}
	}

	// Token: 0x06000181 RID: 385 RVA: 0x00022C40 File Offset: 0x00020E40
	private void LateUpdate()
	{
		if (this.target == null)
		{
			return;
		}
		float deltaTime = RealTime.deltaTime;
		this.mMomentum -= this.mScroll;
		this.mScroll = NGUIMath.SpringLerp(this.mScroll, Vector3.zero, 20f, deltaTime);
		if (this.mMomentum.magnitude < 0.0001f)
		{
			return;
		}
		if (!this.mPressed)
		{
			if (this.panelRegion == null)
			{
				this.FindPanel();
			}
			this.Move(NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime));
			if (this.restrictWithinPanel && this.panelRegion != null)
			{
				this.UpdateBounds();
				if (this.panelRegion.ConstrainTargetToBounds(this.target, ref this.mBounds, this.dragEffect == UIDragObject.DragEffect.None))
				{
					this.CancelMovement();
				}
				else
				{
					this.CancelSpring();
				}
			}
			NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
			if (this.mMomentum.magnitude < 0.0001f)
			{
				this.CancelMovement();
			}
		}
		else
		{
			NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
		}
	}

	// Token: 0x06000182 RID: 386 RVA: 0x00022D68 File Offset: 0x00020F68
	public void CancelMovement()
	{
		if (this.target != null)
		{
			Vector3 localPosition = this.target.localPosition;
			localPosition.x = (float)Mathf.RoundToInt(localPosition.x);
			localPosition.y = (float)Mathf.RoundToInt(localPosition.y);
			localPosition.z = (float)Mathf.RoundToInt(localPosition.z);
			this.target.localPosition = localPosition;
		}
		this.mTargetPos = ((!(this.target != null)) ? Vector3.zero : this.target.position);
		this.mMomentum = Vector3.zero;
		this.mScroll = Vector3.zero;
	}

	// Token: 0x06000183 RID: 387 RVA: 0x00022E14 File Offset: 0x00021014
	public void CancelSpring()
	{
		SpringPosition component = this.target.GetComponent<SpringPosition>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	// Token: 0x06000184 RID: 388 RVA: 0x00003756 File Offset: 0x00001956
	private void OnScroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject))
		{
			this.mScroll -= this.scrollMomentum * (delta * 0.05f);
		}
	}

	// Token: 0x040000AE RID: 174
	public Transform target;

	// Token: 0x040000AF RID: 175
	public UIPanel panelRegion;

	// Token: 0x040000B0 RID: 176
	public Vector3 scrollMomentum = Vector3.zero;

	// Token: 0x040000B1 RID: 177
	public bool restrictWithinPanel;

	// Token: 0x040000B2 RID: 178
	public UIRect contentRect;

	// Token: 0x040000B3 RID: 179
	public UIDragObject.DragEffect dragEffect = UIDragObject.DragEffect.MomentumAndSpring;

	// Token: 0x040000B4 RID: 180
	public float momentumAmount = 35f;

	// Token: 0x040000B5 RID: 181
	[SerializeField]
	protected Vector3 scale = new Vector3(1f, 1f, 0f);

	// Token: 0x040000B6 RID: 182
	[SerializeField]
	[HideInInspector]
	private float scrollWheelFactor;

	// Token: 0x040000B7 RID: 183
	private Plane mPlane;

	// Token: 0x040000B8 RID: 184
	private Vector3 mTargetPos;

	// Token: 0x040000B9 RID: 185
	private Vector3 mLastPos;

	// Token: 0x040000BA RID: 186
	private Vector3 mMomentum = Vector3.zero;

	// Token: 0x040000BB RID: 187
	private Vector3 mScroll = Vector3.zero;

	// Token: 0x040000BC RID: 188
	private Bounds mBounds;

	// Token: 0x040000BD RID: 189
	private int mTouchID;

	// Token: 0x040000BE RID: 190
	private bool mStarted;

	// Token: 0x040000BF RID: 191
	private bool mPressed;

	// Token: 0x0200002D RID: 45
	public enum DragEffect
	{
		// Token: 0x040000C1 RID: 193
		None,
		// Token: 0x040000C2 RID: 194
		Momentum,
		// Token: 0x040000C3 RID: 195
		MomentumAndSpring
	}
}
