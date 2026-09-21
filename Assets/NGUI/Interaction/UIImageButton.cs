using System;
using UnityEngine;

// Token: 0x02000037 RID: 55
[AddComponentMenu("NGUI/UI/Image Button")]
public class UIImageButton : MonoBehaviour
{
	// Token: 0x1700000F RID: 15
	// (get) Token: 0x060001C9 RID: 457 RVA: 0x00023CD8 File Offset: 0x00021ED8
	// (set) Token: 0x060001CA RID: 458 RVA: 0x00023D00 File Offset: 0x00021F00
	public bool isEnabled
	{
		get
		{
			Collider collider = base.GetComponent<Collider>();
			return collider && collider.enabled;
		}
		set
		{
			Collider collider = base.GetComponent<Collider>();
			if (!collider)
			{
				return;
			}
			if (collider.enabled != value)
			{
				collider.enabled = value;
				this.UpdateImage();
			}
		}
	}

	// Token: 0x060001CB RID: 459 RVA: 0x00003CD9 File Offset: 0x00001ED9
	private void OnEnable()
	{
		if (this.target == null)
		{
			this.target = base.GetComponentInChildren<UISprite>();
		}
		this.UpdateImage();
	}

	// Token: 0x060001CC RID: 460 RVA: 0x00023D34 File Offset: 0x00021F34
	private void OnValidate()
	{
		if (this.target != null)
		{
			if (string.IsNullOrEmpty(this.normalSprite))
			{
				this.normalSprite = this.target.spriteName;
			}
			if (string.IsNullOrEmpty(this.hoverSprite))
			{
				this.hoverSprite = this.target.spriteName;
			}
			if (string.IsNullOrEmpty(this.pressedSprite))
			{
				this.pressedSprite = this.target.spriteName;
			}
			if (string.IsNullOrEmpty(this.disabledSprite))
			{
				this.disabledSprite = this.target.spriteName;
			}
		}
	}

	// Token: 0x060001CD RID: 461 RVA: 0x00023DC8 File Offset: 0x00021FC8
	private void UpdateImage()
	{
		if (this.target != null)
		{
			if (this.isEnabled)
			{
				this.SetSprite((!UICamera.IsHighlighted(base.gameObject)) ? this.normalSprite : this.hoverSprite);
			}
			else
			{
				this.SetSprite(this.disabledSprite);
			}
		}
	}

	// Token: 0x060001CE RID: 462 RVA: 0x00003CFB File Offset: 0x00001EFB
	private void OnHover(bool isOver)
	{
		if (this.isEnabled && this.target != null)
		{
			this.SetSprite((!isOver) ? this.normalSprite : this.hoverSprite);
		}
	}

	// Token: 0x060001CF RID: 463 RVA: 0x00003D2A File Offset: 0x00001F2A
	private void OnPress(bool pressed)
	{
		if (pressed)
		{
			this.SetSprite(this.pressedSprite);
		}
		else
		{
			this.UpdateImage();
		}
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x00023E1C File Offset: 0x0002201C
	private void SetSprite(string sprite)
	{
		if (!(this.target.atlas == null) && this.target.atlas.GetSprite(sprite) != null)
		{
			this.target.spriteName = sprite;
			if (this.pixelSnap)
			{
				this.target.MakePixelPerfect();
			}
			return;
		}
	}

	// Token: 0x04000116 RID: 278
	public UISprite target;

	// Token: 0x04000117 RID: 279
	public string normalSprite;

	// Token: 0x04000118 RID: 280
	public string hoverSprite;

	// Token: 0x04000119 RID: 281
	public string pressedSprite;

	// Token: 0x0400011A RID: 282
	public string disabledSprite;

	// Token: 0x0400011B RID: 283
	public bool pixelSnap = true;
}
