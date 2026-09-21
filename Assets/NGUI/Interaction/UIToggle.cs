using System;
using System.Collections.Generic;
using AnimationOrTween;
using UnityEngine;

// Token: 0x0200005D RID: 93
[AddComponentMenu("NGUI/Interaction/Toggle")]
[ExecuteInEditMode]
public class UIToggle : UIWidgetContainer
{
	// Token: 0x17000044 RID: 68
	// (get) Token: 0x060002F6 RID: 758 RVA: 0x00004A7E File Offset: 0x00002C7E
	// (set) Token: 0x060002F7 RID: 759 RVA: 0x00004A96 File Offset: 0x00002C96
	public bool value
	{
		get
		{
			return (!this.mStarted) ? this.startsActive : this.mIsActive;
		}
		set
		{
			if (!this.mStarted)
			{
				this.startsActive = value;
			}
			else if (this.group == 0 || value || this.optionCanBeNone || !this.mStarted)
			{
				this.Set(value);
			}
		}
	}

	// Token: 0x17000045 RID: 69
	// (get) Token: 0x060002F8 RID: 760 RVA: 0x00023740 File Offset: 0x00021940
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

	// Token: 0x17000046 RID: 70
	// (get) Token: 0x060002F9 RID: 761 RVA: 0x00004ACB File Offset: 0x00002CCB
	// (set) Token: 0x060002FA RID: 762 RVA: 0x00004AD3 File Offset: 0x00002CD3
	[Obsolete("Use 'value' instead")]
	public bool isChecked
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	// Token: 0x060002FB RID: 763 RVA: 0x0002AE0C File Offset: 0x0002900C
	public static UIToggle GetActiveToggle(int group)
	{
		for (int i = 0; i < UIToggle.list.size; i++)
		{
			UIToggle uitoggle = UIToggle.list[i];
			if (uitoggle != null && uitoggle.group == group && uitoggle.mIsActive)
			{
				return uitoggle;
			}
		}
		return null;
	}

	// Token: 0x060002FC RID: 764 RVA: 0x00004ADC File Offset: 0x00002CDC
	private void OnEnable()
	{
		UIToggle.list.Add(this);
	}

	// Token: 0x060002FD RID: 765 RVA: 0x00004AE9 File Offset: 0x00002CE9
	private void OnDisable()
	{
		UIToggle.list.Remove(this);
	}

	// Token: 0x060002FE RID: 766 RVA: 0x0002AE58 File Offset: 0x00029058
	private void Start()
	{
		if (this.startsChecked)
		{
			this.startsChecked = false;
			this.startsActive = true;
		}
		if (!Application.isPlaying)
		{
			if (this.checkSprite != null && this.activeSprite == null)
			{
				this.activeSprite = this.checkSprite;
				this.checkSprite = null;
			}
			if (this.checkAnimation != null && this.activeAnimation == null)
			{
				this.activeAnimation = this.checkAnimation;
				this.checkAnimation = null;
			}
			if (Application.isPlaying && this.activeSprite != null)
			{
				this.activeSprite.alpha = ((!this.startsActive) ? 0f : 1f);
			}
			if (EventDelegate.IsValid(this.onChange))
			{
				this.eventReceiver = null;
				this.functionName = null;
			}
		}
		else
		{
			this.mIsActive = !this.startsActive;
			this.mStarted = true;
			bool flag = this.instantTween;
			this.instantTween = true;
			this.Set(this.startsActive);
			this.instantTween = flag;
		}
	}

	// Token: 0x060002FF RID: 767 RVA: 0x00004AF7 File Offset: 0x00002CF7
	private void OnClick()
	{
		if (base.enabled && this.isColliderEnabled && UICamera.currentTouchID != -2)
		{
			this.value = !this.value;
		}
	}

	// Token: 0x06000300 RID: 768 RVA: 0x0002AF6C File Offset: 0x0002916C
	public void Set(bool state)
	{
		if (this.validator != null && !this.validator(state))
		{
			return;
		}
		if (!this.mStarted)
		{
			this.mIsActive = state;
			this.startsActive = state;
			if (this.activeSprite != null)
			{
				this.activeSprite.alpha = ((!state) ? 0f : 1f);
			}
		}
		else if (this.mIsActive != state)
		{
			if (this.group != 0 && state)
			{
				int i = 0;
				int size = UIToggle.list.size;
				while (i < size)
				{
					UIToggle uitoggle = UIToggle.list[i];
					if (uitoggle != this && uitoggle.group == this.group)
					{
						uitoggle.Set(false);
					}
					if (UIToggle.list.size != size)
					{
						size = UIToggle.list.size;
						i = 0;
					}
					else
					{
						i++;
					}
				}
			}
			this.mIsActive = state;
			if (this.activeSprite != null)
			{
				if (!this.instantTween && NGUITools.GetActive(this))
				{
					TweenAlpha.Begin(this.activeSprite.gameObject, 0.15f, (!this.mIsActive) ? 0f : 1f);
				}
				else
				{
					this.activeSprite.alpha = ((!this.mIsActive) ? 0f : 1f);
				}
			}
			if (UIToggle.current == null)
			{
				UIToggle uitoggle2 = UIToggle.current;
				UIToggle.current = this;
				if (EventDelegate.IsValid(this.onChange))
				{
					EventDelegate.Execute(this.onChange);
				}
				else if (this.eventReceiver != null && !string.IsNullOrEmpty(this.functionName))
				{
					this.eventReceiver.SendMessage(this.functionName, this.mIsActive, SendMessageOptions.DontRequireReceiver);
				}
				UIToggle.current = uitoggle2;
			}
			if (this.animator != null)
			{
				ActiveAnimation activeAnimation = ActiveAnimation.Play(this.animator, null, (!state) ? Direction.Reverse : Direction.Forward, EnableCondition.IgnoreDisabledState, DisableCondition.DoNotDisable);
				if (activeAnimation != null && (this.instantTween || !NGUITools.GetActive(this)))
				{
					activeAnimation.Finish();
				}
			}
			else if (this.activeAnimation != null)
			{
				ActiveAnimation activeAnimation2 = ActiveAnimation.Play(this.activeAnimation, null, (!state) ? Direction.Reverse : Direction.Forward, EnableCondition.IgnoreDisabledState, DisableCondition.DoNotDisable);
				if (activeAnimation2 != null && (this.instantTween || !NGUITools.GetActive(this)))
				{
					activeAnimation2.Finish();
				}
			}
			else if (this.tween != null)
			{
				bool active = NGUITools.GetActive(this);
				if (this.tween.tweenGroup != 0)
				{
					UITweener[] componentsInChildren = this.tween.GetComponentsInChildren<UITweener>();
					int j = 0;
					int num = componentsInChildren.Length;
					while (j < num)
					{
						UITweener uitweener = componentsInChildren[j];
						if (uitweener.tweenGroup == this.tween.tweenGroup)
						{
							uitweener.Play(state);
							if (this.instantTween || !active)
							{
								uitweener.tweenFactor = ((!state) ? 0f : 1f);
							}
						}
						j++;
					}
				}
				else
				{
					this.tween.Play(state);
					if (this.instantTween || !active)
					{
						this.tween.tweenFactor = ((!state) ? 0f : 1f);
					}
				}
			}
		}
	}

	// Token: 0x0400024D RID: 589
	public static BetterList<UIToggle> list = new BetterList<UIToggle>();

	// Token: 0x0400024E RID: 590
	public static UIToggle current;

	// Token: 0x0400024F RID: 591
	public int group;

	// Token: 0x04000250 RID: 592
	public UIWidget activeSprite;

	// Token: 0x04000251 RID: 593
	public Animation activeAnimation;

	// Token: 0x04000252 RID: 594
	public Animator animator;

	// Token: 0x04000253 RID: 595
	public UITweener tween;

	// Token: 0x04000254 RID: 596
	public bool startsActive;

	// Token: 0x04000255 RID: 597
	public bool instantTween;

	// Token: 0x04000256 RID: 598
	public bool optionCanBeNone;

	// Token: 0x04000257 RID: 599
	public List<EventDelegate> onChange = new List<EventDelegate>();

	// Token: 0x04000258 RID: 600
	public UIToggle.Validate validator;

	// Token: 0x04000259 RID: 601
	[HideInInspector]
	[SerializeField]
	private UISprite checkSprite;

	// Token: 0x0400025A RID: 602
	[HideInInspector]
	[SerializeField]
	private Animation checkAnimation;

	// Token: 0x0400025B RID: 603
	[SerializeField]
	[HideInInspector]
	private GameObject eventReceiver;

	// Token: 0x0400025C RID: 604
	[HideInInspector]
	[SerializeField]
	private string functionName = "OnActivate";

	// Token: 0x0400025D RID: 605
	[HideInInspector]
	[SerializeField]
	private bool startsChecked;

	// Token: 0x0400025E RID: 606
	private bool mIsActive = true;

	// Token: 0x0400025F RID: 607
	private bool mStarted;

	// Token: 0x0200005E RID: 94
	// (Invoke) Token: 0x06000302 RID: 770
	public delegate bool Validate(bool choice);
}
