using System;
using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

// Token: 0x02000040 RID: 64
[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Play Tween")]
public class UIPlayTween : MonoBehaviour
{
	// Token: 0x0600020B RID: 523 RVA: 0x0000408A File Offset: 0x0000228A
	private void Awake()
	{
		if (this.eventReceiver != null && EventDelegate.IsValid(this.onFinished))
		{
			this.eventReceiver = null;
			this.callWhenFinished = null;
		}
	}

	// Token: 0x0600020C RID: 524 RVA: 0x000040B5 File Offset: 0x000022B5
	private void Start()
	{
		this.mStarted = true;
		if (this.tweenTarget == null)
		{
			this.tweenTarget = base.gameObject;
		}
	}

	// Token: 0x0600020D RID: 525 RVA: 0x00024BF0 File Offset: 0x00022DF0
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

	// Token: 0x0600020E RID: 526 RVA: 0x00024CA0 File Offset: 0x00022EA0
	private void OnDisable()
	{
		UIToggle component = base.GetComponent<UIToggle>();
		if (component != null)
		{
			EventDelegate.Remove(component.onChange, new EventDelegate.Callback(this.OnToggle));
		}
	}

	// Token: 0x0600020F RID: 527 RVA: 0x000040D8 File Offset: 0x000022D8
	private void OnDragOver()
	{
		if (this.trigger == Trigger.OnHover)
		{
			this.OnHover(true);
		}
	}

	// Token: 0x06000210 RID: 528 RVA: 0x00024CD8 File Offset: 0x00022ED8
	private void OnHover(bool isOver)
	{
		if (base.enabled && (this.trigger == Trigger.OnHover || (this.trigger == Trigger.OnHoverTrue && isOver) || (this.trigger == Trigger.OnHoverFalse && !isOver)))
		{
			this.mActivated = (isOver && this.trigger == Trigger.OnHover);
			this.Play(isOver);
		}
	}

	// Token: 0x06000211 RID: 529 RVA: 0x000040EA File Offset: 0x000022EA
	private void OnDragOut()
	{
		if (base.enabled && this.mActivated)
		{
			this.mActivated = false;
			this.Play(false);
		}
	}

	// Token: 0x06000212 RID: 530 RVA: 0x00024D2C File Offset: 0x00022F2C
	private void OnPress(bool isPressed)
	{
		if (base.enabled && (this.trigger == Trigger.OnPress || (this.trigger == Trigger.OnPressTrue && isPressed) || (this.trigger == Trigger.OnPressFalse && !isPressed)))
		{
			this.mActivated = (isPressed && this.trigger == Trigger.OnPress);
			this.Play(isPressed);
		}
	}

	// Token: 0x06000213 RID: 531 RVA: 0x0000410A File Offset: 0x0000230A
	private void OnClick()
	{
		if (base.enabled && this.trigger == Trigger.OnClick)
		{
			this.Play(true);
		}
	}

	// Token: 0x06000214 RID: 532 RVA: 0x00004123 File Offset: 0x00002323
	private void OnDoubleClick()
	{
		if (base.enabled && this.trigger == Trigger.OnDoubleClick)
		{
			this.Play(true);
		}
	}

	// Token: 0x06000215 RID: 533 RVA: 0x00024D80 File Offset: 0x00022F80
	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (this.trigger == Trigger.OnSelect || (this.trigger == Trigger.OnSelectTrue && isSelected) || (this.trigger == Trigger.OnSelectFalse && !isSelected)))
		{
			this.mActivated = (isSelected && this.trigger == Trigger.OnSelect);
			this.Play(isSelected);
		}
	}

	// Token: 0x06000216 RID: 534 RVA: 0x00024DD8 File Offset: 0x00022FD8
	private void OnToggle()
	{
		if (base.enabled && !(UIToggle.current == null))
		{
			if (this.trigger == Trigger.OnActivate || (this.trigger == Trigger.OnActivateTrue && UIToggle.current.value) || (this.trigger == Trigger.OnActivateFalse && !UIToggle.current.value))
			{
				this.Play(UIToggle.current.value);
			}
			return;
		}
	}

	// Token: 0x06000217 RID: 535 RVA: 0x00024E40 File Offset: 0x00023040
	private void Update()
	{
		if (this.disableWhenFinished != DisableCondition.DoNotDisable && this.mTweens != null)
		{
			bool flag = true;
			bool flag2 = true;
			int i = 0;
			int num = this.mTweens.Length;
			while (i < num)
			{
				UITweener uitweener = this.mTweens[i];
				if (uitweener.tweenGroup == this.tweenGroup)
				{
					if (!uitweener.enabled)
					{
						if (uitweener.direction != (Direction)this.disableWhenFinished)
						{
							flag2 = false;
						}
					}
					else
					{
						flag = false;
						IL_62:
						if (flag)
						{
							if (flag2)
							{
								NGUITools.SetActive(this.tweenTarget, false);
							}
							this.mTweens = null;
							return;
						}
						return;
					}
				}
				i++;
			}
	
		}
	}

	// Token: 0x06000218 RID: 536 RVA: 0x00024EC8 File Offset: 0x000230C8
	public void Play(bool forward)
	{
		this.mActive = 0;
		GameObject gameObject = (!(this.tweenTarget == null)) ? this.tweenTarget : base.gameObject;
		if (!NGUITools.GetActive(gameObject))
		{
			if (this.ifDisabledOnPlay != EnableCondition.EnableThenPlay)
			{
				return;
			}
			NGUITools.SetActive(gameObject, true);
		}
		this.mTweens = ((!this.includeChildren) ? gameObject.GetComponents<UITweener>() : gameObject.GetComponentsInChildren<UITweener>());
		if (this.mTweens.Length == 0)
		{
			if (this.disableWhenFinished != DisableCondition.DoNotDisable)
			{
				NGUITools.SetActive(this.tweenTarget, false);
			}
		}
		else
		{
			bool flag = false;
			if (this.playDirection == Direction.Reverse)
			{
				forward = !forward;
			}
			int i = 0;
			int num = this.mTweens.Length;
			while (i < num)
			{
				UITweener uitweener = this.mTweens[i];
				if (uitweener.tweenGroup == this.tweenGroup)
				{
					if (!flag && !NGUITools.GetActive(gameObject))
					{
						flag = true;
						NGUITools.SetActive(gameObject, true);
					}
					this.mActive++;
					if (this.playDirection == Direction.Toggle)
					{
						EventDelegate.Add(uitweener.onFinished, new EventDelegate.Callback(this.OnFinished), true);
						uitweener.Toggle();
					}
					else
					{
						if (this.resetOnPlay || (this.resetIfDisabled && !uitweener.enabled))
						{
							uitweener.Play(forward);
							uitweener.ResetToBeginning();
						}
						EventDelegate.Add(uitweener.onFinished, new EventDelegate.Callback(this.OnFinished), true);
						uitweener.Play(forward);
					}
				}
				i++;
			}
		}
	}

	// Token: 0x06000219 RID: 537 RVA: 0x00025034 File Offset: 0x00023234
	private void OnFinished()
	{
		if (--this.mActive == 0 && UIPlayTween.current == null)
		{
			UIPlayTween.current = this;
			EventDelegate.Execute(this.onFinished);
			if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
			{
				this.eventReceiver.SendMessage(this.callWhenFinished, SendMessageOptions.DontRequireReceiver);
			}
			this.eventReceiver = null;
			UIPlayTween.current = null;
		}
	}

	// Token: 0x0400015B RID: 347
	public static UIPlayTween current;

	// Token: 0x0400015C RID: 348
	public GameObject tweenTarget;

	// Token: 0x0400015D RID: 349
	public int tweenGroup;

	// Token: 0x0400015E RID: 350
	public Trigger trigger;

	// Token: 0x0400015F RID: 351
	public Direction playDirection = Direction.Forward;

	// Token: 0x04000160 RID: 352
	public bool resetOnPlay;

	// Token: 0x04000161 RID: 353
	public bool resetIfDisabled;

	// Token: 0x04000162 RID: 354
	public EnableCondition ifDisabledOnPlay;

	// Token: 0x04000163 RID: 355
	public DisableCondition disableWhenFinished;

	// Token: 0x04000164 RID: 356
	public bool includeChildren;

	// Token: 0x04000165 RID: 357
	public List<EventDelegate> onFinished = new List<EventDelegate>();

	// Token: 0x04000166 RID: 358
	[SerializeField]
	[HideInInspector]
	private GameObject eventReceiver;

	// Token: 0x04000167 RID: 359
	[HideInInspector]
	[SerializeField]
	private string callWhenFinished;

	// Token: 0x04000168 RID: 360
	private UITweener[] mTweens;

	// Token: 0x04000169 RID: 361
	private bool mStarted;

	// Token: 0x0400016A RID: 362
	private int mActive;

	// Token: 0x0400016B RID: 363
	private bool mActivated;
}
