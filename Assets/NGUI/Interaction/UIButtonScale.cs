using System;
using UnityEngine;

// Token: 0x02000022 RID: 34
[AddComponentMenu("NGUI/Interaction/Button Scale")]
public class UIButtonScale : MonoBehaviour
{
	// Token: 0x0600013E RID: 318 RVA: 0x0000341B File Offset: 0x0000161B
	private void Start()
	{
		if (!this.mStarted)
		{
			this.mStarted = true;
			if (this.tweenTarget == null)
			{
				this.tweenTarget = base.transform;
			}
			this.mScale = this.tweenTarget.localScale;
		}
	}

	// Token: 0x0600013F RID: 319 RVA: 0x00003457 File Offset: 0x00001657
	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
	}

	// Token: 0x06000140 RID: 320 RVA: 0x000216F0 File Offset: 0x0001F8F0
	private void OnDisable()
	{
		if (this.mStarted && this.tweenTarget != null)
		{
			TweenScale component = this.tweenTarget.GetComponent<TweenScale>();
			if (component != null)
			{
				component.value = this.mScale;
				component.enabled = false;
			}
		}
	}

	// Token: 0x06000141 RID: 321 RVA: 0x0002173C File Offset: 0x0001F93C
	private void OnPress(bool isPressed)
	{
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenScale.Begin(this.tweenTarget.gameObject, this.duration, (!isPressed) ? ((!UICamera.IsHighlighted(base.gameObject)) ? this.mScale : Vector3.Scale(this.mScale, this.hover)) : Vector3.Scale(this.mScale, this.pressed)).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x06000142 RID: 322 RVA: 0x000217B8 File Offset: 0x0001F9B8
	private void OnHover(bool isOver)
	{
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenScale.Begin(this.tweenTarget.gameObject, this.duration, (!isOver) ? this.mScale : Vector3.Scale(this.mScale, this.hover)).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x06000143 RID: 323 RVA: 0x00003472 File Offset: 0x00001672
	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			this.OnHover(isSelected);
		}
	}

	// Token: 0x04000085 RID: 133
	public Transform tweenTarget;

	// Token: 0x04000086 RID: 134
	public Vector3 hover = new Vector3(1.1f, 1.1f, 1.1f);

	// Token: 0x04000087 RID: 135
	public Vector3 pressed = new Vector3(1.05f, 1.05f, 1.05f);

	// Token: 0x04000088 RID: 136
	public float duration = 0.2f;

	// Token: 0x04000089 RID: 137
	private Vector3 mScale;

	// Token: 0x0400008A RID: 138
	private bool mStarted;
}
