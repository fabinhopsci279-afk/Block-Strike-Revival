using System;
using UnityEngine;

// Token: 0x0200001E RID: 30
[AddComponentMenu("NGUI/Interaction/Button Message (Legacy)")]
public class UIButtonMessage : MonoBehaviour
{
	// Token: 0x06000125 RID: 293 RVA: 0x000031B5 File Offset: 0x000013B5
	private void Start()
	{
		this.mStarted = true;
	}

	// Token: 0x06000126 RID: 294 RVA: 0x000031BE File Offset: 0x000013BE
	private void OnEnable()
	{
		if (this.mStarted)
		{
			this.OnHover(UICamera.IsHighlighted(base.gameObject));
		}
	}

	// Token: 0x06000127 RID: 295 RVA: 0x000031D9 File Offset: 0x000013D9
	private void OnHover(bool isOver)
	{
		if (base.enabled && ((isOver && this.trigger == UIButtonMessage.Trigger.OnMouseOver) || (!isOver && this.trigger == UIButtonMessage.Trigger.OnMouseOut)))
		{
			this.Send();
		}
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00003201 File Offset: 0x00001401
	private void OnPress(bool isPressed)
	{
		if (base.enabled && ((isPressed && this.trigger == UIButtonMessage.Trigger.OnPress) || (!isPressed && this.trigger == UIButtonMessage.Trigger.OnRelease)))
		{
			this.Send();
		}
	}

	// Token: 0x06000129 RID: 297 RVA: 0x00003229 File Offset: 0x00001429
	private void OnSelect(bool isSelected)
	{
		if (base.enabled && (!isSelected || UICamera.currentScheme == UICamera.ControlScheme.Controller))
		{
			this.OnHover(isSelected);
		}
	}

	// Token: 0x0600012A RID: 298 RVA: 0x00003245 File Offset: 0x00001445
	private void OnClick()
	{
		if (base.enabled && this.trigger == UIButtonMessage.Trigger.OnClick)
		{
			this.Send();
		}
	}

	// Token: 0x0600012B RID: 299 RVA: 0x0000325D File Offset: 0x0000145D
	private void OnDoubleClick()
	{
		if (base.enabled && this.trigger == UIButtonMessage.Trigger.OnDoubleClick)
		{
			this.Send();
		}
	}

	// Token: 0x0600012C RID: 300 RVA: 0x000213AC File Offset: 0x0001F5AC
	private void Send()
	{
		if (string.IsNullOrEmpty(this.functionName))
		{
			return;
		}
		if (this.target == null)
		{
			this.target = base.gameObject;
		}
		if (this.includeChildren)
		{
			Transform[] componentsInChildren = this.target.GetComponentsInChildren<Transform>();
			int i = 0;
			int num = componentsInChildren.Length;
			while (i < num)
			{
				Transform transform = componentsInChildren[i];
				transform.gameObject.SendMessage(this.functionName, base.gameObject, SendMessageOptions.DontRequireReceiver);
				i++;
			}
		}
		else
		{
			this.target.SendMessage(this.functionName, base.gameObject, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Token: 0x0400006C RID: 108
	public GameObject target;

	// Token: 0x0400006D RID: 109
	public string functionName;

	// Token: 0x0400006E RID: 110
	public UIButtonMessage.Trigger trigger;

	// Token: 0x0400006F RID: 111
	public bool includeChildren;

	// Token: 0x04000070 RID: 112
	private bool mStarted;

	// Token: 0x0200001F RID: 31
	public enum Trigger
	{
		// Token: 0x04000072 RID: 114
		OnClick,
		// Token: 0x04000073 RID: 115
		OnMouseOver,
		// Token: 0x04000074 RID: 116
		OnMouseOut,
		// Token: 0x04000075 RID: 117
		OnPress,
		// Token: 0x04000076 RID: 118
		OnRelease,
		// Token: 0x04000077 RID: 119
		OnDoubleClick
	}
}
