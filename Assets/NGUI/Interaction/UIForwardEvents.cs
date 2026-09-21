using System;
using UnityEngine;

// Token: 0x02000032 RID: 50
[AddComponentMenu("NGUI/Interaction/Forward Events (Legacy)")]
public class UIForwardEvents : MonoBehaviour
{
	// Token: 0x060001A8 RID: 424 RVA: 0x00003A7B File Offset: 0x00001C7B
	private void OnHover(bool isOver)
	{
		if (this.onHover && this.target != null)
		{
			this.target.SendMessage("OnHover", isOver, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x00003AAA File Offset: 0x00001CAA
	private void OnPress(bool pressed)
	{
		if (this.onPress && this.target != null)
		{
			this.target.SendMessage("OnPress", pressed, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x060001AA RID: 426 RVA: 0x00003AD9 File Offset: 0x00001CD9
	private void OnClick()
	{
		if (this.onClick && this.target != null)
		{
			this.target.SendMessage("OnClick", SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x060001AB RID: 427 RVA: 0x00003B02 File Offset: 0x00001D02
	private void OnDoubleClick()
	{
		if (this.onDoubleClick && this.target != null)
		{
			this.target.SendMessage("OnDoubleClick", SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x060001AC RID: 428 RVA: 0x00003B2B File Offset: 0x00001D2B
	private void OnSelect(bool selected)
	{
		if (this.onSelect && this.target != null)
		{
			this.target.SendMessage("OnSelect", selected, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x060001AD RID: 429 RVA: 0x00003B5A File Offset: 0x00001D5A
	private void OnDrag(Vector2 delta)
	{
		if (this.onDrag && this.target != null)
		{
			this.target.SendMessage("OnDrag", delta, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x060001AE RID: 430 RVA: 0x00003B89 File Offset: 0x00001D89
	private void OnDrop(GameObject go)
	{
		if (this.onDrop && this.target != null)
		{
			this.target.SendMessage("OnDrop", go, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x060001AF RID: 431 RVA: 0x00003BB3 File Offset: 0x00001DB3
	private void OnSubmit()
	{
		if (this.onSubmit && this.target != null)
		{
			this.target.SendMessage("OnSubmit", SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x00003BDC File Offset: 0x00001DDC
	private void OnScroll(float delta)
	{
		if (this.onScroll && this.target != null)
		{
			this.target.SendMessage("OnScroll", delta, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x040000F3 RID: 243
	public GameObject target;

	// Token: 0x040000F4 RID: 244
	public bool onHover;

	// Token: 0x040000F5 RID: 245
	public bool onPress;

	// Token: 0x040000F6 RID: 246
	public bool onClick;

	// Token: 0x040000F7 RID: 247
	public bool onDoubleClick;

	// Token: 0x040000F8 RID: 248
	public bool onSelect;

	// Token: 0x040000F9 RID: 249
	public bool onDrag;

	// Token: 0x040000FA RID: 250
	public bool onDrop;

	// Token: 0x040000FB RID: 251
	public bool onSubmit;

	// Token: 0x040000FC RID: 252
	public bool onScroll;
}
