using System;
using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

// Token: 0x0200003D RID: 61
[AddComponentMenu("NGUI/Interaction/Play Animation")]
[ExecuteInEditMode]
public class UIPlayAnimation : MonoBehaviour
{
	// Token: 0x17000012 RID: 18
	// (get) Token: 0x060001EE RID: 494 RVA: 0x00003E46 File Offset: 0x00002046
	private bool dualState
	{
		get
		{
			return this.trigger == Trigger.OnPress || this.trigger == Trigger.OnHover;
		}
	}

	// Token: 0x060001EF RID: 495 RVA: 0x00024688 File Offset: 0x00022888
	private void Awake()
	{
		UIButton component = base.GetComponent<UIButton>();
		if (component != null)
		{
			this.dragHighlight = component.dragHighlight;
		}
		if (this.eventReceiver != null && EventDelegate.IsValid(this.onFinished))
		{
			this.eventReceiver = null;
			this.callWhenFinished = null;
		}
	}

	// Token: 0x060001F0 RID: 496 RVA: 0x000246DC File Offset: 0x000228DC
	private void Start()
	{
		this.mStarted = true;
		if (this.target == null && this.animator == null)
		{
			this.animator = base.GetComponentInChildren<Animator>();
		}
		if (this.animator != null)
		{
			if (this.animator.enabled)
			{
				this.animator.enabled = false;
			}
			return;
		}
		if (this.target == null)
		{
			this.target = base.GetComponentInChildren<Animation>();
		}
		if (this.target != null && this.target.enabled)
		{
			this.target.enabled = false;
		}
	}

	// Token: 0x060001F1 RID: 497 RVA: 0x00024784 File Offset: 0x00022984
	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
		if (UICamera.currentTouch != null)
		{
			if (this.trigger == Trigger.OnPress || this.trigger == Trigger.OnPressTrue)
			{
				this.mActivated = (UICamera.currentTouch.pressed == base.gameObject);
			}
			if (this.trigger == Trigger.OnHover || this.trigger == Trigger.OnHoverTrue)
			{
				this.mActivated = (UICamera.currentTouch.current == base.gameObject);
			}
		}
		UIToggle component = base.GetComponent<UIToggle>();
		if (component != null)
		{
			EventDelegate.Add(component.onChange, new EventDelegate.Callback(this.OnToggle));
		}
	}

	// Token: 0x060001F2 RID: 498 RVA: 0x00024834 File Offset: 0x00022A34
	private void OnDisable()
	{
		UIToggle component = base.GetComponent<UIToggle>();
		if (component != null)
		{
			EventDelegate.Remove(component.onChange, new EventDelegate.Callback(this.OnToggle));
		}
	}

	// Token: 0x060001F3 RID: 499 RVA: 0x00003E5D File Offset: 0x0000205D
	private void OnHover(bool isOver)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.trigger == Trigger.OnHover || (this.trigger == Trigger.OnHoverTrue && isOver) || (this.trigger == Trigger.OnHoverFalse && !isOver))
		{
			this.Play(isOver, this.dualState);
		}
	}

	// Token: 0x060001F4 RID: 500 RVA: 0x0002486C File Offset: 0x00022A6C
	private void OnPress(bool isPressed)
	{
		if (!base.enabled)
		{
			return;
		}
		if (UICamera.currentTouchID != -2)
		{
			if (UICamera.currentTouchID != -3)
			{
				if (this.trigger == Trigger.OnPress || (this.trigger == Trigger.OnPressTrue && isPressed) || (this.trigger == Trigger.OnPressFalse && !isPressed))
				{
					this.Play(isPressed, this.dualState);
				}
				return;
			}
		}
	}

	// Token: 0x060001F5 RID: 501 RVA: 0x00003E96 File Offset: 0x00002096
	private void OnClick()
	{
		if (UICamera.currentTouchID != -2)
		{
			if (UICamera.currentTouchID != -3)
			{
				if (base.enabled && this.trigger == Trigger.OnClick)
				{
					this.Play(true, false);
				}
				return;
			}
		}
	}

	// Token: 0x060001F6 RID: 502 RVA: 0x00003EC5 File Offset: 0x000020C5
	private void OnDoubleClick()
	{
		if (UICamera.currentTouchID != -2)
		{
			if (UICamera.currentTouchID != -3)
			{
				if (base.enabled && this.trigger == Trigger.OnDoubleClick)
				{
					this.Play(true, false);
				}
				return;
			}
		}
	}

	// Token: 0x060001F7 RID: 503 RVA: 0x00003EF6 File Offset: 0x000020F6
	private void OnSelect(bool isSelected)
	{
		if (!base.enabled)
		{
			return;
		}
		if (this.trigger == Trigger.OnSelect || (this.trigger == Trigger.OnSelectTrue && isSelected) || (this.trigger == Trigger.OnSelectFalse && !isSelected))
		{
			this.Play(isSelected, this.dualState);
		}
	}

	// Token: 0x060001F8 RID: 504 RVA: 0x000248C8 File Offset: 0x00022AC8
	private void OnToggle()
	{
		if (base.enabled && !(UIToggle.current == null))
		{
			if (this.trigger == Trigger.OnActivate || (this.trigger == Trigger.OnActivateTrue && UIToggle.current.value) || (this.trigger == Trigger.OnActivateFalse && !UIToggle.current.value))
			{
				this.Play(UIToggle.current.value, this.dualState);
			}
			return;
		}
	}

	// Token: 0x060001F9 RID: 505 RVA: 0x00024938 File Offset: 0x00022B38
	private void OnDragOver()
	{
		if (base.enabled && this.dualState)
		{
			if (UICamera.currentTouch.dragged == base.gameObject)
			{
				this.Play(true, true);
			}
			else if (this.dragHighlight && this.trigger == Trigger.OnPress)
			{
				this.Play(true, true);
			}
		}
	}

	// Token: 0x060001FA RID: 506 RVA: 0x00003F32 File Offset: 0x00002132
	private void OnDragOut()
	{
		if (base.enabled && this.dualState && UICamera.hoveredObject != base.gameObject)
		{
			this.Play(false, true);
		}
	}

	// Token: 0x060001FB RID: 507 RVA: 0x00003F5E File Offset: 0x0000215E
	private void OnDrop(GameObject go)
	{
		if (base.enabled && this.trigger == Trigger.OnPress && UICamera.currentTouch.dragged != base.gameObject)
		{
			this.Play(false, true);
		}
	}

	// Token: 0x060001FC RID: 508 RVA: 0x00003F90 File Offset: 0x00002190
	public void Play(bool forward)
	{
		this.Play(forward, true);
	}

	// Token: 0x060001FD RID: 509 RVA: 0x00024990 File Offset: 0x00022B90
	public void Play(bool forward, bool onlyIfDifferent)
	{
		if (this.target || this.animator)
		{
			if (onlyIfDifferent)
			{
				if (this.mActivated == forward)
				{
					return;
				}
				this.mActivated = forward;
			}
			if (this.clearSelection && UICamera.selectedObject == base.gameObject)
			{
				UICamera.selectedObject = null;
			}
			int num = (int)(-(int)this.playDirection);
			Direction direction = (Direction)((!forward) ? num : ((int)this.playDirection));
			ActiveAnimation activeAnimation = (!this.target) ? ActiveAnimation.Play(this.animator, this.clipName, direction, this.ifDisabledOnPlay, this.disableWhenFinished) : ActiveAnimation.Play(this.target, this.clipName, direction, this.ifDisabledOnPlay, this.disableWhenFinished);
			if (activeAnimation != null)
			{
				if (this.resetOnPlay)
				{
					activeAnimation.Reset();
				}
				for (int i = 0; i < this.onFinished.Count; i++)
				{
					EventDelegate.Add(activeAnimation.onFinished, new EventDelegate.Callback(this.OnFinished), true);
				}
			}
		}
	}

	// Token: 0x060001FE RID: 510 RVA: 0x00003F9A File Offset: 0x0000219A
	public void PlayForward()
	{
		this.Play(true);
	}

	// Token: 0x060001FF RID: 511 RVA: 0x00003FA3 File Offset: 0x000021A3
	public void PlayReverse()
	{
		this.Play(false);
	}

	// Token: 0x06000200 RID: 512 RVA: 0x00024A98 File Offset: 0x00022C98
	private void OnFinished()
	{
		if (UIPlayAnimation.current == null)
		{
			UIPlayAnimation.current = this;
			EventDelegate.Execute(this.onFinished);
			if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
			{
				this.eventReceiver.SendMessage(this.callWhenFinished, SendMessageOptions.DontRequireReceiver);
			}
			this.eventReceiver = null;
			UIPlayAnimation.current = null;
		}
	}

	// Token: 0x0400013D RID: 317
	public static UIPlayAnimation current;

	// Token: 0x0400013E RID: 318
	public Animation target;

	// Token: 0x0400013F RID: 319
	public Animator animator;

	// Token: 0x04000140 RID: 320
	public string clipName;

	// Token: 0x04000141 RID: 321
	public Trigger trigger;

	// Token: 0x04000142 RID: 322
	public Direction playDirection = Direction.Forward;

	// Token: 0x04000143 RID: 323
	public bool resetOnPlay;

	// Token: 0x04000144 RID: 324
	public bool clearSelection;

	// Token: 0x04000145 RID: 325
	public EnableCondition ifDisabledOnPlay;

	// Token: 0x04000146 RID: 326
	public DisableCondition disableWhenFinished;

	// Token: 0x04000147 RID: 327
	public List<EventDelegate> onFinished = new List<EventDelegate>();

	// Token: 0x04000148 RID: 328
	[SerializeField]
	[HideInInspector]
	private GameObject eventReceiver;

	// Token: 0x04000149 RID: 329
	[SerializeField]
	[HideInInspector]
	private string callWhenFinished;

	// Token: 0x0400014A RID: 330
	private bool mStarted;

	// Token: 0x0400014B RID: 331
	private bool mActivated;

	// Token: 0x0400014C RID: 332
	private bool dragHighlight;
}
