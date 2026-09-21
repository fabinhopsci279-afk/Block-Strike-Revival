using System;
using UnityEngine;

// Token: 0x0200002B RID: 43
[AddComponentMenu("NGUI/Interaction/Drag and Drop Root")]
public class UIDragDropRoot : MonoBehaviour
{
	// Token: 0x06000175 RID: 373 RVA: 0x00003715 File Offset: 0x00001915
	private void OnEnable()
	{
		UIDragDropRoot.root = base.transform;
	}

	// Token: 0x06000176 RID: 374 RVA: 0x00003722 File Offset: 0x00001922
	private void OnDisable()
	{
		if (UIDragDropRoot.root == base.transform)
		{
			UIDragDropRoot.root = null;
		}
	}

	// Token: 0x040000AD RID: 173
	public static Transform root;
}
