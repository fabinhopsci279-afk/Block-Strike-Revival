using System;
using UnityEngine;

// Token: 0x02000020 RID: 32
[AddComponentMenu("NGUI/Interaction/Button Offset")]
public class UIButtonOffset : MonoBehaviour
{
	// Token: 0x0600012E RID: 302 RVA: 0x000032A9 File Offset: 0x000014A9
	private void Start()
	{
		if (!this.mStarted)
		{
			this.mStarted = true;
			if (this.tweenTarget == null)
			{
				this.tweenTarget = base.transform;
			}
			this.mPos = this.tweenTarget.localPosition;
		}
	}

	// Token: 0x0600012F RID: 303 RVA: 0x000032E5 File Offset: 0x000014E5
	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
	}

	// Token: 0x06000130 RID: 304 RVA: 0x0002143C File Offset: 0x0001F63C
	private void OnDisable()
	{
		if (this.mStarted && this.tweenTarget != null)
		{
			TweenPosition component = this.tweenTarget.GetComponent<TweenPosition>();
			if (component != null)
			{
				component.value = this.mPos;
				component.enabled = false;
			}
		}
	}

	// Token: 0x06000131 RID: 305 RVA: 0x00021488 File Offset: 0x0001F688
	private void OnPress(bool isPressed)
	{
		this.mPressed = isPressed;
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenPosition.Begin(this.tweenTarget.gameObject, this.duration, (!isPressed) ? ((!UICamera.IsHighlighted(base.gameObject)) ? this.mPos : (this.mPos + this.hover)) : (this.mPos + this.pressed)).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x06000132 RID: 306 RVA: 0x0002150C File Offset: 0x0001F70C
	private void OnHover(bool isOver)
	{
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenPosition.Begin(this.tweenTarget.gameObject, this.duration, (!isOver) ? this.mPos : (this.mPos + this.hover)).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x06000133 RID: 307 RVA: 0x00003300 File Offset: 0x00001500
	private void OnDragOver()
	{
		if (this.mPressed)
		{
			TweenPosition.Begin(this.tweenTarget.gameObject, this.duration, this.mPos + this.hover).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00003337 File Offset: 0x00001537
	private void OnDragOut()
	{
		if (this.mPressed)
		{
			TweenPosition.Begin(this.tweenTarget.gameObject, this.duration, this.mPos).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00003363 File Offset: 0x00001563
	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			this.OnHover(isSelected);
		}
	}

	// Token: 0x04000078 RID: 120
	public Transform tweenTarget;

	// Token: 0x04000079 RID: 121
	public Vector3 hover = Vector3.zero;

	// Token: 0x0400007A RID: 122
	public Vector3 pressed = new Vector3(2f, -2f);

	// Token: 0x0400007B RID: 123
	public float duration = 0.2f;

	// Token: 0x0400007C RID: 124
	[NonSerialized]
	private Vector3 mPos;

	// Token: 0x0400007D RID: 125
	[NonSerialized]
	private bool mStarted;

	// Token: 0x0400007E RID: 126
	[NonSerialized]
	private bool mPressed;
}
