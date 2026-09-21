using System;
using UnityEngine;

// Token: 0x0200003E RID: 62
[AddComponentMenu("NGUI/Interaction/Play Sound")]
public class UIPlaySound : MonoBehaviour
{
	// Token: 0x17000013 RID: 19
	// (get) Token: 0x06000202 RID: 514 RVA: 0x00024B00 File Offset: 0x00022D00
	private bool canPlay
	{
		get
		{
			if (!base.enabled)
			{
				return false;
			}
			UIButton component = base.GetComponent<UIButton>();
			return component == null || component.isEnabled;
		}
	}

	// Token: 0x06000203 RID: 515 RVA: 0x00003FCA File Offset: 0x000021CA
	private void OnEnable()
	{
		if (this.trigger == UIPlaySound.Trigger.OnEnable)
		{
			NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
		}
	}

	// Token: 0x06000204 RID: 516 RVA: 0x00003FED File Offset: 0x000021ED
	private void OnDisable()
	{
		if (this.trigger == UIPlaySound.Trigger.OnDisable)
		{
			NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
		}
	}

	// Token: 0x06000205 RID: 517 RVA: 0x00024B30 File Offset: 0x00022D30
	private void OnHover(bool isOver)
	{
		if (this.trigger == UIPlaySound.Trigger.OnMouseOver)
		{
			if (this.mIsOver == isOver)
			{
				return;
			}
			this.mIsOver = isOver;
		}
		if (this.canPlay && ((isOver && this.trigger == UIPlaySound.Trigger.OnMouseOver) || (!isOver && this.trigger == UIPlaySound.Trigger.OnMouseOut)))
		{
			NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
		}
	}

	// Token: 0x06000206 RID: 518 RVA: 0x00024B90 File Offset: 0x00022D90
	private void OnPress(bool isPressed)
	{
		if (this.trigger == UIPlaySound.Trigger.OnPress)
		{
			if (this.mIsOver == isPressed)
			{
				return;
			}
			this.mIsOver = isPressed;
		}
		if (this.canPlay && ((isPressed && this.trigger == UIPlaySound.Trigger.OnPress) || (!isPressed && this.trigger == UIPlaySound.Trigger.OnRelease)))
		{
			NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
		}
	}

	// Token: 0x06000207 RID: 519 RVA: 0x00004010 File Offset: 0x00002210
	private void OnClick()
	{
		if (this.canPlay && this.trigger == UIPlaySound.Trigger.OnClick)
		{
			NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
		}
	}

	// Token: 0x06000208 RID: 520 RVA: 0x0000403A File Offset: 0x0000223A
	private void OnSelect(bool isSelected)
	{
		if (this.canPlay && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			this.OnHover(isSelected);
		}
	}

	// Token: 0x06000209 RID: 521 RVA: 0x00004056 File Offset: 0x00002256
	public void Play()
	{
		NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
	}

	// Token: 0x0400014D RID: 333
	public AudioClip audioClip;

	// Token: 0x0400014E RID: 334
	public UIPlaySound.Trigger trigger;

	// Token: 0x0400014F RID: 335
	[Range(0f, 1f)]
	public float volume = 1f;

	// Token: 0x04000150 RID: 336
	[Range(0f, 2f)]
	public float pitch = 1f;

	// Token: 0x04000151 RID: 337
	private bool mIsOver;

	// Token: 0x0200003F RID: 63
	public enum Trigger
	{
		// Token: 0x04000153 RID: 339
		OnClick,
		// Token: 0x04000154 RID: 340
		OnMouseOver,
		// Token: 0x04000155 RID: 341
		OnMouseOut,
		// Token: 0x04000156 RID: 342
		OnPress,
		// Token: 0x04000157 RID: 343
		OnRelease,
		// Token: 0x04000158 RID: 344
		Custom,
		// Token: 0x04000159 RID: 345
		OnEnable,
		// Token: 0x0400015A RID: 346
		OnDisable
	}
}
