using System;
using UnityEngine;

// Token: 0x02000021 RID: 33
[AddComponentMenu("NGUI/Interaction/Button Rotation")]
public class UIButtonRotation : MonoBehaviour
{
	// Token: 0x06000137 RID: 311 RVA: 0x000033A8 File Offset: 0x000015A8
	private void Start()
	{
		if (!this.mStarted)
		{
			this.mStarted = true;
			if (this.tweenTarget == null)
			{
				this.tweenTarget = base.transform;
			}
			this.mRot = this.tweenTarget.localRotation;
		}
	}

	// Token: 0x06000138 RID: 312 RVA: 0x000033E4 File Offset: 0x000015E4
	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00021568 File Offset: 0x0001F768
	private void OnDisable()
	{
		if (this.mStarted && this.tweenTarget != null)
		{
			TweenRotation component = this.tweenTarget.GetComponent<TweenRotation>();
			if (component != null)
			{
				component.value = this.mRot;
				component.enabled = false;
			}
		}
	}

	// Token: 0x0600013A RID: 314 RVA: 0x000215B4 File Offset: 0x0001F7B4
	private void OnPress(bool isPressed)
	{
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenRotation.Begin(this.tweenTarget.gameObject, this.duration, (!isPressed) ? ((!UICamera.IsHighlighted(base.gameObject)) ? this.mRot : (this.mRot * Quaternion.Euler(this.hover))) : (this.mRot * Quaternion.Euler(this.pressed))).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x0600013B RID: 315 RVA: 0x0002163C File Offset: 0x0001F83C
	private void OnHover(bool isOver)
	{
		if (base.enabled)
		{
			if (!this.mStarted)
			{
				this.Start();
			}
			TweenRotation.Begin(this.tweenTarget.gameObject, this.duration, (!isOver) ? this.mRot : (this.mRot * Quaternion.Euler(this.hover))).method = UITweener.Method.EaseInOut;
		}
	}

	// Token: 0x0600013C RID: 316 RVA: 0x000033FF File Offset: 0x000015FF
	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			this.OnHover(isSelected);
		}
	}

	// Token: 0x0400007F RID: 127
	public Transform tweenTarget;

	// Token: 0x04000080 RID: 128
	public Vector3 hover = Vector3.zero;

	// Token: 0x04000081 RID: 129
	public Vector3 pressed = Vector3.zero;

	// Token: 0x04000082 RID: 130
	public float duration = 0.2f;

	// Token: 0x04000083 RID: 131
	private Quaternion mRot;

	// Token: 0x04000084 RID: 132
	private bool mStarted;
}
