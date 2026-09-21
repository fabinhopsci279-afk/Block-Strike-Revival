using System;
using UnityEngine;

// Token: 0x02000056 RID: 86
[AddComponentMenu("NGUI/Interaction/NGUI Slider")]
[ExecuteInEditMode]
public class UISlider : UIProgressBar
{
	// Token: 0x17000040 RID: 64
	// (get) Token: 0x060002D7 RID: 727 RVA: 0x00023740 File Offset: 0x00021940
	public bool isColliderEnabled
	{
		get
		{
			Collider component = base.GetComponent<Collider>();
			if (component != null)
			{
				return component.enabled;
			}
			Collider2D component2 = base.GetComponent<Collider2D>();
			return component2 != null && component2.enabled;
		}
	}

	// Token: 0x17000041 RID: 65
	// (get) Token: 0x060002D8 RID: 728 RVA: 0x000045B3 File Offset: 0x000027B3
	// (set) Token: 0x060002D9 RID: 729 RVA: 0x000045BB File Offset: 0x000027BB
	[Obsolete("Use 'value' instead")]
	public float sliderValue
	{
		get
		{
			return base.value;
		}
		set
		{
			base.value = value;
		}
	}

	// Token: 0x17000042 RID: 66
	// (get) Token: 0x060002DA RID: 730 RVA: 0x000048EB File Offset: 0x00002AEB
	// (set) Token: 0x060002DB RID: 731 RVA: 0x0000362C File Offset: 0x0000182C
	[Obsolete("Use 'fillDirection' instead")]
	public bool inverted
	{
		get
		{
			return base.isInverted;
		}
		set
		{
		}
	}

	// Token: 0x060002DC RID: 732 RVA: 0x0002A52C File Offset: 0x0002872C
	protected override void Upgrade()
	{
		if (this.direction != UISlider.Direction.Upgraded)
		{
			this.mValue = this.rawValue;
			if (this.foreground != null)
			{
				this.mFG = this.foreground.GetComponent<UIWidget>();
			}
			if (this.direction == UISlider.Direction.Horizontal)
			{
				this.mFill = ((!this.mInverted) ? UIProgressBar.FillDirection.LeftToRight : UIProgressBar.FillDirection.RightToLeft);
			}
			else
			{
				this.mFill = ((!this.mInverted) ? UIProgressBar.FillDirection.BottomToTop : UIProgressBar.FillDirection.TopToBottom);
			}
			this.direction = UISlider.Direction.Upgraded;
		}
	}

	// Token: 0x060002DD RID: 733 RVA: 0x0002A5A4 File Offset: 0x000287A4
	protected override void OnStart()
	{
		GameObject go = (!(this.mBG != null) || (!(this.mBG.GetComponent<Collider>() != null) && !(this.mBG.GetComponent<Collider2D>() != null))) ? base.gameObject : this.mBG.gameObject;
		UIEventListener uieventListener = UIEventListener.Get(go);
		UIEventListener uieventListener2 = uieventListener;
		uieventListener2.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener2.onPress, new UIEventListener.BoolDelegate(this.OnPressBackground));
		UIEventListener uieventListener3 = uieventListener;
		uieventListener3.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uieventListener3.onDrag, new UIEventListener.VectorDelegate(this.OnDragBackground));
		if (this.thumb != null && (this.thumb.GetComponent<Collider>() != null || this.thumb.GetComponent<Collider2D>() != null) && (this.mFG == null || this.thumb != this.mFG.cachedTransform))
		{
			UIEventListener uieventListener4 = UIEventListener.Get(this.thumb.gameObject);
			UIEventListener uieventListener5 = uieventListener4;
			uieventListener5.onPress = (UIEventListener.BoolDelegate)Delegate.Combine(uieventListener5.onPress, new UIEventListener.BoolDelegate(this.OnPressForeground));
			UIEventListener uieventListener6 = uieventListener4;
			uieventListener6.onDrag = (UIEventListener.VectorDelegate)Delegate.Combine(uieventListener6.onDrag, new UIEventListener.VectorDelegate(this.OnDragForeground));
		}
	}

	// Token: 0x060002DE RID: 734 RVA: 0x000048F3 File Offset: 0x00002AF3
	protected void OnPressBackground(GameObject go, bool isPressed)
	{
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			return;
		}
		this.mCam = UICamera.currentCamera;
		base.value = base.ScreenToValue(UICamera.lastEventPosition);
		if (!isPressed && this.onDragFinished != null)
		{
			this.onDragFinished();
		}
	}

	// Token: 0x060002DF RID: 735 RVA: 0x00004930 File Offset: 0x00002B30
	protected void OnDragBackground(GameObject go, Vector2 delta)
	{
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			return;
		}
		this.mCam = UICamera.currentCamera;
		base.value = base.ScreenToValue(UICamera.lastEventPosition);
	}

	// Token: 0x060002E0 RID: 736 RVA: 0x0002A6F8 File Offset: 0x000288F8
	protected void OnPressForeground(GameObject go, bool isPressed)
	{
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			return;
		}
		this.mCam = UICamera.currentCamera;
		if (isPressed)
		{
			this.mOffset = ((!(this.mFG == null)) ? (base.value - base.ScreenToValue(UICamera.lastEventPosition)) : 0f);
		}
		else if (this.onDragFinished != null)
		{
			this.onDragFinished();
		}
	}

	// Token: 0x060002E1 RID: 737 RVA: 0x00004957 File Offset: 0x00002B57
	protected void OnDragForeground(GameObject go, Vector2 delta)
	{
		if (UICamera.currentScheme == UICamera.ControlScheme.Controller)
		{
			return;
		}
		this.mCam = UICamera.currentCamera;
		base.value = this.mOffset + base.ScreenToValue(UICamera.lastEventPosition);
	}

	// Token: 0x060002E2 RID: 738 RVA: 0x00004985 File Offset: 0x00002B85
	public override void OnPan(Vector2 delta)
	{
		if (base.enabled && this.isColliderEnabled)
		{
			base.OnPan(delta);
		}
	}

	// Token: 0x0400022F RID: 559
	[SerializeField]
	[HideInInspector]
	private Transform foreground;

	// Token: 0x04000230 RID: 560
	[SerializeField]
	[HideInInspector]
	private float rawValue = 1f;

	// Token: 0x04000231 RID: 561
	[HideInInspector]
	[SerializeField]
	private UISlider.Direction direction = UISlider.Direction.Upgraded;

	// Token: 0x04000232 RID: 562
	[HideInInspector]
	[SerializeField]
	protected bool mInverted;

	// Token: 0x02000057 RID: 87
	private enum Direction
	{
		// Token: 0x04000234 RID: 564
		Horizontal,
		// Token: 0x04000235 RID: 565
		Vertical,
		// Token: 0x04000236 RID: 566
		Upgraded
	}
}
