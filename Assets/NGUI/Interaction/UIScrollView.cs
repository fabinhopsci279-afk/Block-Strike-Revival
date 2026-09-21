using System;
using UnityEngine;

// Token: 0x0200004D RID: 77
[AddComponentMenu("NGUI/Interaction/Scroll View")]
[ExecuteInEditMode]
[RequireComponent(typeof(UIPanel))]
public class UIScrollView : MonoBehaviour
{
	// Token: 0x1700002E RID: 46
	// (get) Token: 0x06000288 RID: 648 RVA: 0x000045D8 File Offset: 0x000027D8
	public UIPanel panel
	{
		get
		{
			return this.mPanel;
		}
	}

	// Token: 0x1700002F RID: 47
	// (get) Token: 0x06000289 RID: 649 RVA: 0x000045E0 File Offset: 0x000027E0
	public bool isDragging
	{
		get
		{
			return this.mPressed && this.mDragStarted;
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x0600028A RID: 650 RVA: 0x000045F3 File Offset: 0x000027F3
	public virtual Bounds bounds
	{
		get
		{
			if (!this.mCalculatedBounds)
			{
				this.mCalculatedBounds = true;
				this.mTrans = base.transform;
				this.mBounds = NGUIMath.CalculateRelativeWidgetBounds(this.mTrans, this.mTrans);
			}
			return this.mBounds;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x0600028B RID: 651 RVA: 0x0000462D File Offset: 0x0000282D
	public bool canMoveHorizontally
	{
		get
		{
			return this.movement == UIScrollView.Movement.Horizontal || this.movement == UIScrollView.Movement.Unrestricted || (this.movement == UIScrollView.Movement.Custom && this.customMovement.x != 0f);
		}
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x0600028C RID: 652 RVA: 0x00004664 File Offset: 0x00002864
	public bool canMoveVertically
	{
		get
		{
			return this.movement == UIScrollView.Movement.Vertical || this.movement == UIScrollView.Movement.Unrestricted || (this.movement == UIScrollView.Movement.Custom && this.customMovement.y != 0f);
		}
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x0600028D RID: 653 RVA: 0x000278FC File Offset: 0x00025AFC
	public virtual bool shouldMoveHorizontally
	{
		get
		{
			float num = this.bounds.size.x;
			if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
			{
				num += this.mPanel.clipSoftness.x * 2f;
			}
			return Mathf.RoundToInt(num - this.mPanel.width) > 0;
		}
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x0600028E RID: 654 RVA: 0x00027960 File Offset: 0x00025B60
	public virtual bool shouldMoveVertically
	{
		get
		{
			float num = this.bounds.size.y;
			if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
			{
				num += this.mPanel.clipSoftness.y * 2f;
			}
			return Mathf.RoundToInt(num - this.mPanel.height) > 0;
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x0600028F RID: 655 RVA: 0x000279C4 File Offset: 0x00025BC4
	protected virtual bool shouldMove
	{
		get
		{
			if (!this.disableDragIfFits)
			{
				return true;
			}
			if (this.mPanel == null)
			{
				this.mPanel = base.GetComponent<UIPanel>();
			}
			Vector4 finalClipRegion = this.mPanel.finalClipRegion;
			Bounds bounds = this.bounds;
			float num = (finalClipRegion.z != 0f) ? (finalClipRegion.z * 0.5f) : ((float)Screen.width);
			float num2 = (finalClipRegion.w != 0f) ? (finalClipRegion.w * 0.5f) : ((float)Screen.height);
			if (this.canMoveHorizontally)
			{
				if (bounds.min.x < finalClipRegion.x - num)
				{
					return true;
				}
				if (bounds.max.x > finalClipRegion.x + num)
				{
					return true;
				}
			}
			if (this.canMoveVertically)
			{
				if (bounds.min.y < finalClipRegion.y - num2)
				{
					return true;
				}
				if (bounds.max.y > finalClipRegion.y + num2)
				{
					return true;
				}
			}
			return false;
		}
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x06000290 RID: 656 RVA: 0x0000469C File Offset: 0x0000289C
	// (set) Token: 0x06000291 RID: 657 RVA: 0x000046A4 File Offset: 0x000028A4
	public Vector3 currentMomentum
	{
		get
		{
			return this.mMomentum;
		}
		set
		{
			this.mMomentum = value;
			this.mShouldMove = true;
		}
	}

	// Token: 0x06000292 RID: 658 RVA: 0x00027AD8 File Offset: 0x00025CD8
	private void Awake()
	{
		this.mTrans = base.transform;
		this.mPanel = base.GetComponent<UIPanel>();
		if (this.mPanel.clipping == UIDrawCall.Clipping.None)
		{
			this.mPanel.clipping = UIDrawCall.Clipping.ConstrainButDontClip;
		}
		if (this.movement != UIScrollView.Movement.Custom && this.scale.sqrMagnitude > 0.001f)
		{
			if (this.scale.x == 1f && this.scale.y == 0f)
			{
				this.movement = UIScrollView.Movement.Horizontal;
			}
			else if (this.scale.x == 0f && this.scale.y == 1f)
			{
				this.movement = UIScrollView.Movement.Vertical;
			}
			else if (this.scale.x == 1f && this.scale.y == 1f)
			{
				this.movement = UIScrollView.Movement.Unrestricted;
			}
			else
			{
				this.movement = UIScrollView.Movement.Custom;
				this.customMovement.x = this.scale.x;
				this.customMovement.y = this.scale.y;
			}
			this.scale = Vector3.zero;
		}
		if (this.contentPivot == UIWidget.Pivot.TopLeft && this.relativePositionOnReset != Vector2.zero)
		{
			this.contentPivot = NGUIMath.GetPivot(new Vector2(this.relativePositionOnReset.x, 1f - this.relativePositionOnReset.y));
			this.relativePositionOnReset = Vector2.zero;
		}
	}

	// Token: 0x06000293 RID: 659 RVA: 0x000046B4 File Offset: 0x000028B4
	private void OnEnable()
	{
		UIScrollView.list.Add(this);
		if (this.mStarted && Application.isPlaying)
		{
			this.CheckScrollbars();
		}
	}

	// Token: 0x06000294 RID: 660 RVA: 0x000046D6 File Offset: 0x000028D6
	private void Start()
	{
		this.mStarted = true;
		if (Application.isPlaying)
		{
			this.CheckScrollbars();
		}
	}

	// Token: 0x06000295 RID: 661 RVA: 0x00027C50 File Offset: 0x00025E50
	private void CheckScrollbars()
	{
		if (this.horizontalScrollBar != null)
		{
			EventDelegate.Add(this.horizontalScrollBar.onChange, new EventDelegate.Callback(this.OnScrollBar));
			this.horizontalScrollBar.BroadcastMessage("CacheDefaultColor", SendMessageOptions.DontRequireReceiver);
			this.horizontalScrollBar.alpha = ((this.showScrollBars == UIScrollView.ShowCondition.Always || this.shouldMoveHorizontally) ? 1f : 0f);
		}
		if (this.verticalScrollBar != null)
		{
			EventDelegate.Add(this.verticalScrollBar.onChange, new EventDelegate.Callback(this.OnScrollBar));
			this.verticalScrollBar.BroadcastMessage("CacheDefaultColor", SendMessageOptions.DontRequireReceiver);
			this.verticalScrollBar.alpha = ((this.showScrollBars == UIScrollView.ShowCondition.Always || this.shouldMoveVertically) ? 1f : 0f);
		}
	}

	// Token: 0x06000296 RID: 662 RVA: 0x000046EC File Offset: 0x000028EC
	private void OnDisable()
	{
		UIScrollView.list.Remove(this);
	}

	// Token: 0x06000297 RID: 663 RVA: 0x000046FA File Offset: 0x000028FA
	public bool RestrictWithinBounds(bool instant)
	{
		return this.RestrictWithinBounds(instant, true, true);
	}

	// Token: 0x06000298 RID: 664 RVA: 0x00027D24 File Offset: 0x00025F24
	public bool RestrictWithinBounds(bool instant, bool horizontal, bool vertical)
	{
		if (this.mPanel == null)
		{
			return false;
		}
		Bounds bounds = this.bounds;
		Vector3 vector = this.mPanel.CalculateConstrainOffset(bounds.min, bounds.max);
		if (!horizontal)
		{
			vector.x = 0f;
		}
		if (!vertical)
		{
			vector.y = 0f;
		}
		if (vector.sqrMagnitude > 0.1f)
		{
			if (!instant && this.dragEffect == UIScrollView.DragEffect.MomentumAndSpring)
			{
				Vector3 pos = this.mTrans.localPosition + vector;
				pos.x = Mathf.Round(pos.x);
				pos.y = Mathf.Round(pos.y);
				SpringPanel.Begin(this.mPanel.gameObject, pos, 13f).strength = 8f;
			}
			else
			{
				this.MoveRelative(vector);
				if (Mathf.Abs(vector.x) > 0.01f)
				{
					this.mMomentum.x = 0f;
				}
				if (Mathf.Abs(vector.y) > 0.01f)
				{
					this.mMomentum.y = 0f;
				}
				if (Mathf.Abs(vector.z) > 0.01f)
				{
					this.mMomentum.z = 0f;
				}
				this.mScroll = 0f;
			}
			return true;
		}
		return false;
	}

	// Token: 0x06000299 RID: 665 RVA: 0x00027E80 File Offset: 0x00026080
	public void DisableSpring()
	{
		SpringPanel component = base.GetComponent<SpringPanel>();
		if (component != null)
		{
			component.enabled = false;
		}
	}

	// Token: 0x0600029A RID: 666 RVA: 0x00004705 File Offset: 0x00002905
	public void UpdateScrollbars()
	{
		this.UpdateScrollbars(true);
	}

	// Token: 0x0600029B RID: 667 RVA: 0x00027EA4 File Offset: 0x000260A4
	public virtual void UpdateScrollbars(bool recalculateBounds)
	{
		if (this.mPanel == null)
		{
			return;
		}
		if (!(this.horizontalScrollBar != null) && !(this.verticalScrollBar != null))
		{
			if (recalculateBounds)
			{
				this.mCalculatedBounds = false;
			}
		}
		else
		{
			if (recalculateBounds)
			{
				this.mCalculatedBounds = false;
				this.mShouldMove = this.shouldMove;
			}
			Bounds bounds = this.bounds;
			Vector2 vector = bounds.min;
			Vector2 vector2 = bounds.max;
			if (this.horizontalScrollBar != null && vector2.x > vector.x)
			{
				Vector4 finalClipRegion = this.mPanel.finalClipRegion;
				int num = Mathf.RoundToInt(finalClipRegion.z);
				if ((num & 1) != 0)
				{
					num--;
				}
				float num2 = (float)num * 0.5f;
				num2 = Mathf.Round(num2);
				if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
				{
					num2 -= this.mPanel.clipSoftness.x;
				}
				float contentSize = vector2.x - vector.x;
				float viewSize = num2 * 2f;
				float num3 = vector.x;
				float num4 = vector2.x;
				float num5 = finalClipRegion.x - num2;
				float num6 = finalClipRegion.x + num2;
				num3 = num5 - num3;
				num4 -= num6;
				this.UpdateScrollbars(this.horizontalScrollBar, num3, num4, contentSize, viewSize, false);
			}
			if (this.verticalScrollBar != null && vector2.y > vector.y)
			{
				Vector4 finalClipRegion2 = this.mPanel.finalClipRegion;
				int num7 = Mathf.RoundToInt(finalClipRegion2.w);
				if ((num7 & 1) != 0)
				{
					num7--;
				}
				float num8 = (float)num7 * 0.5f;
				num8 = Mathf.Round(num8);
				if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
				{
					num8 -= this.mPanel.clipSoftness.y;
				}
				float contentSize2 = vector2.y - vector.y;
				float viewSize2 = num8 * 2f;
				float num9 = vector.y;
				float num10 = vector2.y;
				float num11 = finalClipRegion2.y - num8;
				float num12 = finalClipRegion2.y + num8;
				num9 = num11 - num9;
				num10 -= num12;
				this.UpdateScrollbars(this.verticalScrollBar, num9, num10, contentSize2, viewSize2, true);
			}
		}
	}

	// Token: 0x0600029C RID: 668 RVA: 0x00028100 File Offset: 0x00026300
	protected void UpdateScrollbars(UIProgressBar slider, float contentMin, float contentMax, float contentSize, float viewSize, bool inverted)
	{
		if (slider == null)
		{
			return;
		}
		this.mIgnoreCallbacks = true;
		float num;
		if (viewSize < contentSize)
		{
			contentMin = Mathf.Clamp01(contentMin / contentSize);
			contentMax = Mathf.Clamp01(contentMax / contentSize);
			num = contentMin + contentMax;
			slider.value = ((!inverted) ? ((num <= 0.001f) ? 1f : (contentMin / num)) : ((num <= 0.001f) ? 0f : (1f - contentMin / num)));
		}
		else
		{
			contentMin = Mathf.Clamp01(-contentMin / contentSize);
			contentMax = Mathf.Clamp01(-contentMax / contentSize);
			num = contentMin + contentMax;
			slider.value = ((!inverted) ? ((num <= 0.001f) ? 1f : (contentMin / num)) : ((num <= 0.001f) ? 0f : (1f - contentMin / num)));
			if (contentSize > 0f)
			{
				contentMin = Mathf.Clamp01(contentMin / contentSize);
				contentMax = Mathf.Clamp01(contentMax / contentSize);
				num = contentMin + contentMax;
			}
		}
		UIScrollBar uiscrollBar = slider as UIScrollBar;
		if (uiscrollBar != null)
		{
			uiscrollBar.barSize = 1f - num;
		}
		this.mIgnoreCallbacks = false;
	}

	// Token: 0x0600029D RID: 669 RVA: 0x00028210 File Offset: 0x00026410
	public virtual void SetDragAmount(float x, float y, bool updateScrollbars)
	{
		if (this.mPanel == null)
		{
			this.mPanel = base.GetComponent<UIPanel>();
		}
		this.DisableSpring();
		Bounds bounds = this.bounds;
		if (bounds.min.x != bounds.max.x)
		{
			if (bounds.min.y != bounds.max.y)
			{
				Vector4 finalClipRegion = this.mPanel.finalClipRegion;
				float num = finalClipRegion.z * 0.5f;
				float num2 = finalClipRegion.w * 0.5f;
				float num3 = bounds.min.x + num;
				float num4 = bounds.max.x - num;
				float num5 = bounds.min.y + num2;
				float num6 = bounds.max.y - num2;
				if (this.mPanel.clipping == UIDrawCall.Clipping.SoftClip)
				{
					num3 -= this.mPanel.clipSoftness.x;
					num4 += this.mPanel.clipSoftness.x;
					num5 -= this.mPanel.clipSoftness.y;
					num6 += this.mPanel.clipSoftness.y;
				}
				float num7 = Mathf.Lerp(num3, num4, x);
				float num8 = Mathf.Lerp(num6, num5, y);
				if (!updateScrollbars)
				{
					Vector3 localPosition = this.mTrans.localPosition;
					if (this.canMoveHorizontally)
					{
						localPosition.x += finalClipRegion.x - num7;
					}
					if (this.canMoveVertically)
					{
						localPosition.y += finalClipRegion.y - num8;
					}
					this.mTrans.localPosition = localPosition;
				}
				if (this.canMoveHorizontally)
				{
					finalClipRegion.x = num7;
				}
				if (this.canMoveVertically)
				{
					finalClipRegion.y = num8;
				}
				Vector4 baseClipRegion = this.mPanel.baseClipRegion;
				this.mPanel.clipOffset = new Vector2(finalClipRegion.x - baseClipRegion.x, finalClipRegion.y - baseClipRegion.y);
				if (updateScrollbars)
				{
					this.UpdateScrollbars(this.mDragID == -10);
				}
				return;
			}
		}
	}

	// Token: 0x0600029E RID: 670 RVA: 0x0000470E File Offset: 0x0000290E
	public void InvalidateBounds()
	{
		this.mCalculatedBounds = false;
	}

	// Token: 0x0600029F RID: 671 RVA: 0x00028460 File Offset: 0x00026660
	[ContextMenu("Reset Clipping Position")]
	public void ResetPosition()
	{
		if (NGUITools.GetActive(this))
		{
			this.mCalculatedBounds = false;
			Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.contentPivot);
			this.SetDragAmount(pivotOffset.x, 1f - pivotOffset.y, false);
			this.SetDragAmount(pivotOffset.x, 1f - pivotOffset.y, true);
		}
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x000284C0 File Offset: 0x000266C0
	public void UpdatePosition()
	{
		if (!this.mIgnoreCallbacks && (this.horizontalScrollBar != null || this.verticalScrollBar != null))
		{
			this.mIgnoreCallbacks = true;
			this.mCalculatedBounds = false;
			Vector2 pivotOffset = NGUIMath.GetPivotOffset(this.contentPivot);
			float x = (!(this.horizontalScrollBar != null)) ? pivotOffset.x : this.horizontalScrollBar.value;
			float y = (!(this.verticalScrollBar != null)) ? (1f - pivotOffset.y) : this.verticalScrollBar.value;
			this.SetDragAmount(x, y, false);
			this.UpdateScrollbars(true);
			this.mIgnoreCallbacks = false;
		}
	}

	// Token: 0x060002A1 RID: 673 RVA: 0x00028574 File Offset: 0x00026774
	public void OnScrollBar()
	{
		if (!this.mIgnoreCallbacks)
		{
			this.mIgnoreCallbacks = true;
			float x = (!(this.horizontalScrollBar != null)) ? 0f : this.horizontalScrollBar.value;
			float y = (!(this.verticalScrollBar != null)) ? 0f : this.verticalScrollBar.value;
			this.SetDragAmount(x, y, false);
			this.mIgnoreCallbacks = false;
		}
	}

	// Token: 0x060002A2 RID: 674 RVA: 0x000285E4 File Offset: 0x000267E4
	public virtual void MoveRelative(Vector3 relative)
	{
		this.mTrans.localPosition += relative;
		Vector2 clipOffset = this.mPanel.clipOffset;
		clipOffset.x -= relative.x;
		clipOffset.y -= relative.y;
		this.mPanel.clipOffset = clipOffset;
		this.UpdateScrollbars(false);
	}

	// Token: 0x060002A3 RID: 675 RVA: 0x00028654 File Offset: 0x00026854
	public void MoveAbsolute(Vector3 absolute)
	{
		Vector3 a = this.mTrans.InverseTransformPoint(absolute);
		Vector3 b = this.mTrans.InverseTransformPoint(Vector3.zero);
		this.MoveRelative(a - b);
	}

	// Token: 0x060002A4 RID: 676 RVA: 0x0002868C File Offset: 0x0002688C
	public void Press(bool pressed)
	{
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			return;
		}
		if (this.smoothDragStart && pressed)
		{
			this.mDragStarted = false;
			this.mDragStartOffset = Vector2.zero;
		}
		if (base.enabled && NGUITools.GetActive(base.gameObject))
		{
			if (!pressed && this.mDragID == UICamera.currentTouchID)
			{
				this.mDragID = -10;
			}
			this.mCalculatedBounds = false;
			this.mShouldMove = this.shouldMove;
			if (!this.mShouldMove)
			{
				return;
			}
			this.mPressed = pressed;
			if (pressed)
			{
				this.mMomentum = Vector3.zero;
				this.mScroll = 0f;
				this.DisableSpring();
				this.mLastPos = UICamera.lastWorldPosition;
				this.mPlane = new Plane(this.mTrans.rotation * Vector3.back, this.mLastPos);
				Vector2 clipOffset = this.mPanel.clipOffset;
				clipOffset.x = Mathf.Round(clipOffset.x);
				clipOffset.y = Mathf.Round(clipOffset.y);
				this.mPanel.clipOffset = clipOffset;
				Vector3 localPosition = this.mTrans.localPosition;
				localPosition.x = Mathf.Round(localPosition.x);
				localPosition.y = Mathf.Round(localPosition.y);
				this.mTrans.localPosition = localPosition;
				if (!this.smoothDragStart)
				{
					this.mDragStarted = true;
					this.mDragStartOffset = Vector2.zero;
					if (this.onDragStarted != null)
					{
						this.onDragStarted();
					}
				}
			}
			else if (this.centerOnChild)
			{
				this.centerOnChild.Recenter();
			}
			else
			{
				if (this.restrictWithinPanel && this.mPanel.clipping != UIDrawCall.Clipping.None)
				{
					this.RestrictWithinBounds(this.dragEffect == UIScrollView.DragEffect.None, this.canMoveHorizontally, this.canMoveVertically);
				}
				if (this.mDragStarted && this.onDragFinished != null)
				{
					this.onDragFinished();
				}
				if (!this.mShouldMove && this.onStoppedMoving != null)
				{
					this.onStoppedMoving();
				}
			}
		}
	}

	// Token: 0x060002A5 RID: 677 RVA: 0x000288A0 File Offset: 0x00026AA0
	public void Drag()
	{
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			return;
		}
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.mShouldMove)
		{
			if (this.mDragID == -10)
			{
				this.mDragID = UICamera.currentTouchID;
			}
			UICamera.currentTouch.clickNotification = UICamera.ClickNotification.BasedOnDelta;
			if (this.smoothDragStart && !this.mDragStarted)
			{
				this.mDragStarted = true;
				this.mDragStartOffset = UICamera.currentTouch.totalDelta;
				if (this.onDragStarted != null)
				{
					this.onDragStarted();
				}
			}
			Ray ray = (!this.smoothDragStart) ? UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos) : UICamera.currentCamera.ScreenPointToRay(UICamera.currentTouch.pos - this.mDragStartOffset);
			float distance = 0f;
			if (this.mPlane.Raycast(ray, out distance))
			{
				Vector3 point = ray.GetPoint(distance);
				Vector3 vector = point - this.mLastPos;
				this.mLastPos = point;
				if (vector.x != 0f || vector.y != 0f || vector.z != 0f)
				{
					vector = this.mTrans.InverseTransformDirection(vector);
					if (this.movement == UIScrollView.Movement.Horizontal)
					{
						vector.y = 0f;
						vector.z = 0f;
					}
					else if (this.movement == UIScrollView.Movement.Vertical)
					{
						vector.x = 0f;
						vector.z = 0f;
					}
					else if (this.movement == UIScrollView.Movement.Unrestricted)
					{
						vector.z = 0f;
					}
					else
					{
						vector.Scale(this.customMovement);
					}
					vector = this.mTrans.TransformDirection(vector);
				}
				if (this.dragEffect == UIScrollView.DragEffect.None)
				{
					this.mMomentum = Vector3.zero;
				}
				else
				{
					this.mMomentum = Vector3.Lerp(this.mMomentum, this.mMomentum + vector * (0.01f * this.momentumAmount), 0.67f);
				}
				if (this.iOSDragEmulation)
				{
					if (this.dragEffect == UIScrollView.DragEffect.MomentumAndSpring)
					{
						if (this.mPanel.CalculateConstrainOffset(this.bounds.min, this.bounds.max).magnitude > 1f)
						{
							this.MoveAbsolute(vector * 0.5f);
							this.mMomentum *= 0.5f;
							goto IL_296;
						}
						this.MoveAbsolute(vector);
						goto IL_296;
					}
				}
				this.MoveAbsolute(vector);
				IL_296:
				if (this.restrictWithinPanel && this.mPanel.clipping != UIDrawCall.Clipping.None && this.dragEffect != UIScrollView.DragEffect.MomentumAndSpring)
				{
					this.RestrictWithinBounds(true, this.canMoveHorizontally, this.canMoveVertically);
				}
			}
		}
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x00028B78 File Offset: 0x00026D78
	public void Scroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.scrollWheelFactor != 0f)
		{
			this.DisableSpring();
			this.mShouldMove |= this.shouldMove;
			if (Mathf.Sign(this.mScroll) != Mathf.Sign(delta))
			{
				this.mScroll = 0f;
			}
			this.mScroll += delta * this.scrollWheelFactor;
		}
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x00028BF4 File Offset: 0x00026DF4
	private void LateUpdate()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		float deltaTime = RealTime.deltaTime;
		if (this.showScrollBars != UIScrollView.ShowCondition.Always && (this.verticalScrollBar || this.horizontalScrollBar))
		{
			bool flag = false;
			bool flag2 = false;
			if (this.showScrollBars != UIScrollView.ShowCondition.WhenDragging || this.mDragID != -10 || this.mMomentum.magnitude > 0.01f)
			{
				flag = this.shouldMoveVertically;
				flag2 = this.shouldMoveHorizontally;
			}
			if (this.verticalScrollBar)
			{
				float num = this.verticalScrollBar.alpha;
				num += ((!flag) ? (-deltaTime * 3f) : (deltaTime * 6f));
				num = Mathf.Clamp01(num);
				if (this.verticalScrollBar.alpha != num)
				{
					this.verticalScrollBar.alpha = num;
				}
			}
			if (this.horizontalScrollBar)
			{
				float num2 = this.horizontalScrollBar.alpha;
				num2 += ((!flag2) ? (-deltaTime * 3f) : (deltaTime * 6f));
				num2 = Mathf.Clamp01(num2);
				if (this.horizontalScrollBar.alpha != num2)
				{
					this.horizontalScrollBar.alpha = num2;
				}
			}
		}
		if (!this.mShouldMove)
		{
			return;
		}
		if (!this.mPressed)
		{
			if (this.mMomentum.magnitude <= 0.0001f)
			{
				if (this.mScroll == 0f)
				{
					this.mScroll = 0f;
					this.mMomentum = Vector3.zero;
					SpringPanel component = base.GetComponent<SpringPanel>();
					if (component != null && component.enabled)
					{
						return;
					}
					this.mShouldMove = false;
					if (this.onStoppedMoving != null)
					{
						this.onStoppedMoving();
						return;
					}
					return;
				}
			}
			if (this.movement == UIScrollView.Movement.Horizontal)
			{
				this.mMomentum -= this.mTrans.TransformDirection(new Vector3(this.mScroll * 0.05f, 0f, 0f));
			}
			else if (this.movement == UIScrollView.Movement.Vertical)
			{
				this.mMomentum -= this.mTrans.TransformDirection(new Vector3(0f, this.mScroll * 0.05f, 0f));
			}
			else if (this.movement == UIScrollView.Movement.Unrestricted)
			{
				this.mMomentum -= this.mTrans.TransformDirection(new Vector3(this.mScroll * 0.05f, this.mScroll * 0.05f, 0f));
			}
			else
			{
				this.mMomentum -= this.mTrans.TransformDirection(new Vector3(this.mScroll * this.customMovement.x * 0.05f, this.mScroll * this.customMovement.y * 0.05f, 0f));
			}
			this.mScroll = NGUIMath.SpringLerp(this.mScroll, 0f, 20f, deltaTime);
			Vector3 absolute = NGUIMath.SpringDampen(ref this.mMomentum, this.dampenStrength, deltaTime);
			this.MoveAbsolute(absolute);
			if (this.restrictWithinPanel && this.mPanel.clipping != UIDrawCall.Clipping.None)
			{
				if (NGUITools.GetActive(this.centerOnChild))
				{
					if (this.centerOnChild.nextPageThreshold != 0f)
					{
						this.mMomentum = Vector3.zero;
						this.mScroll = 0f;
					}
					else
					{
						this.centerOnChild.Recenter();
					}
				}
				else
				{
					this.RestrictWithinBounds(false, this.canMoveHorizontally, this.canMoveVertically);
				}
			}
			if (this.onMomentumMove != null)
			{
				this.onMomentumMove();
			}
		}
		else
		{
			this.mScroll = 0f;
			NGUIMath.SpringDampen(ref this.mMomentum, 9f, deltaTime);
		}
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x00028FA0 File Offset: 0x000271A0
	public void OnPan(Vector2 delta)
	{
		if (this.horizontalScrollBar != null)
		{
			this.horizontalScrollBar.OnPan(delta);
		}
		if (this.verticalScrollBar != null)
		{
			this.verticalScrollBar.OnPan(delta);
		}
		if (this.horizontalScrollBar == null && this.verticalScrollBar == null)
		{
			if (this.scale.x != 0f)
			{
				this.Scroll(delta.x);
			}
			else if (this.scale.y != 0f)
			{
				this.Scroll(delta.y);
			}
		}
	}

	// Token: 0x040001C7 RID: 455
	public static BetterList<UIScrollView> list = new BetterList<UIScrollView>();

	// Token: 0x040001C8 RID: 456
	public UIScrollView.Movement movement;

	// Token: 0x040001C9 RID: 457
	public UIScrollView.DragEffect dragEffect = UIScrollView.DragEffect.MomentumAndSpring;

	// Token: 0x040001CA RID: 458
	public bool restrictWithinPanel = true;

	// Token: 0x040001CB RID: 459
	public bool disableDragIfFits;

	// Token: 0x040001CC RID: 460
	public bool smoothDragStart = true;

	// Token: 0x040001CD RID: 461
	public bool iOSDragEmulation = true;

	// Token: 0x040001CE RID: 462
	public float scrollWheelFactor = 0.25f;

	// Token: 0x040001CF RID: 463
	public float momentumAmount = 35f;

	// Token: 0x040001D0 RID: 464
	public float dampenStrength = 9f;

	// Token: 0x040001D1 RID: 465
	public UIProgressBar horizontalScrollBar;

	// Token: 0x040001D2 RID: 466
	public UIProgressBar verticalScrollBar;

	// Token: 0x040001D3 RID: 467
	public UIScrollView.ShowCondition showScrollBars = UIScrollView.ShowCondition.OnlyIfNeeded;

	// Token: 0x040001D4 RID: 468
	public Vector2 customMovement = new Vector2(1f, 0f);

	// Token: 0x040001D5 RID: 469
	public UIWidget.Pivot contentPivot;

	// Token: 0x040001D6 RID: 470
	public UIScrollView.OnDragNotification onDragStarted;

	// Token: 0x040001D7 RID: 471
	public UIScrollView.OnDragNotification onDragFinished;

	// Token: 0x040001D8 RID: 472
	public UIScrollView.OnDragNotification onMomentumMove;

	// Token: 0x040001D9 RID: 473
	public UIScrollView.OnDragNotification onStoppedMoving;

	// Token: 0x040001DA RID: 474
	[SerializeField]
	[HideInInspector]
	private Vector3 scale = new Vector3(1f, 0f, 0f);

	// Token: 0x040001DB RID: 475
	[HideInInspector]
	[SerializeField]
	private Vector2 relativePositionOnReset = Vector2.zero;

	// Token: 0x040001DC RID: 476
	protected Transform mTrans;

	// Token: 0x040001DD RID: 477
	protected UIPanel mPanel;

	// Token: 0x040001DE RID: 478
	protected Plane mPlane;

	// Token: 0x040001DF RID: 479
	protected Vector3 mLastPos;

	// Token: 0x040001E0 RID: 480
	protected bool mPressed;

	// Token: 0x040001E1 RID: 481
	protected Vector3 mMomentum = Vector3.zero;

	// Token: 0x040001E2 RID: 482
	protected float mScroll;

	// Token: 0x040001E3 RID: 483
	protected Bounds mBounds;

	// Token: 0x040001E4 RID: 484
	protected bool mCalculatedBounds;

	// Token: 0x040001E5 RID: 485
	protected bool mShouldMove;

	// Token: 0x040001E6 RID: 486
	protected bool mIgnoreCallbacks;

	// Token: 0x040001E7 RID: 487
	protected int mDragID = -10;

	// Token: 0x040001E8 RID: 488
	protected Vector2 mDragStartOffset = Vector2.zero;

	// Token: 0x040001E9 RID: 489
	protected bool mDragStarted;

	// Token: 0x040001EA RID: 490
	[NonSerialized]
	private bool mStarted;

	// Token: 0x040001EB RID: 491
	[HideInInspector]
	public UICenterOnChild centerOnChild;

	// Token: 0x0200004E RID: 78
	public enum Movement
	{
		// Token: 0x040001ED RID: 493
		Horizontal,
		// Token: 0x040001EE RID: 494
		Vertical,
		// Token: 0x040001EF RID: 495
		Unrestricted,
		// Token: 0x040001F0 RID: 496
		Custom
	}

	// Token: 0x0200004F RID: 79
	public enum DragEffect
	{
		// Token: 0x040001F2 RID: 498
		None,
		// Token: 0x040001F3 RID: 499
		Momentum,
		// Token: 0x040001F4 RID: 500
		MomentumAndSpring
	}

	// Token: 0x02000050 RID: 80
	public enum ShowCondition
	{
		// Token: 0x040001F6 RID: 502
		Always,
		// Token: 0x040001F7 RID: 503
		OnlyIfNeeded,
		// Token: 0x040001F8 RID: 504
		WhenDragging
	}

	// Token: 0x02000051 RID: 81
	// (Invoke) Token: 0x060002AA RID: 682
	public delegate void OnDragNotification();
}
