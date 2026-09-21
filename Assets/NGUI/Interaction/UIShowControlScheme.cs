using System;
using UnityEngine;

// Token: 0x02000055 RID: 85
public class UIShowControlScheme : MonoBehaviour
{
	// Token: 0x060002D3 RID: 723 RVA: 0x00004887 File Offset: 0x00002A87
	private void OnEnable()
	{
		UICamera.onSchemeChange = (UICamera.OnSchemeChange)Delegate.Combine(UICamera.onSchemeChange, new UICamera.OnSchemeChange(this.OnScheme));
		this.OnScheme();
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x000048AF File Offset: 0x00002AAF
	private void OnDisable()
	{
		UICamera.onSchemeChange = (UICamera.OnSchemeChange)Delegate.Remove(UICamera.onSchemeChange, new UICamera.OnSchemeChange(this.OnScheme));
	}

	// Token: 0x060002D5 RID: 725 RVA: 0x0002A4C8 File Offset: 0x000286C8
	private void OnScheme()
	{
		if (this.target != null)
		{
			UICamera.ControlScheme currentScheme = UICamera.currentScheme;
			if (currentScheme == UICamera.ControlScheme.Mouse)
			{
				this.target.SetActive(this.mouse);
			}
			else if (currentScheme == UICamera.ControlScheme.Touch)
			{
				this.target.SetActive(this.touch);
			}
			else if (currentScheme == UICamera.ControlScheme.Controller)
			{
				this.target.SetActive(this.controller);
			}
		}
	}

	// Token: 0x0400022B RID: 555
	public GameObject target;

	// Token: 0x0400022C RID: 556
	public bool mouse;

	// Token: 0x0400022D RID: 557
	public bool touch;

	// Token: 0x0400022E RID: 558
	public bool controller = true;
}
