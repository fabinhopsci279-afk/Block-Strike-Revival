using System;
using UnityEngine;
using UnityEngine.Events;

// Token: 0x02000441 RID: 1089
public class UIEventClick : MonoBehaviour
{
	// Token: 0x0600247E RID: 9342 RVA: 0x00018F23 File Offset: 0x00017123
	private void OnClick()
	{
		if (this.onClick != null)
		{
			this.onClick.Invoke();
		}
	}

	// Token: 0x0400164A RID: 5706
	public UnityEvent onClick;
}
