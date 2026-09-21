using System;
using UnityEngine;

// Token: 0x0200001A RID: 26
[AddComponentMenu("NGUI/Interaction/Button Activate")]
public class UIButtonActivate : MonoBehaviour
{
	// Token: 0x0600010D RID: 269 RVA: 0x00003049 File Offset: 0x00001249
	private void OnClick()
	{
		if (this.target != null)
		{
			NGUITools.SetActive(this.target, this.state);
		}
	}

	// Token: 0x04000056 RID: 86
	public GameObject target;

	// Token: 0x04000057 RID: 87
	public bool state = true;
}
