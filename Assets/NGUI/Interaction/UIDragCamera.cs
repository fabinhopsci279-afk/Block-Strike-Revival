using System;
using UnityEngine;

// Token: 0x02000026 RID: 38
[AddComponentMenu("NGUI/Interaction/Drag Camera")]
[ExecuteInEditMode]
public class UIDragCamera : MonoBehaviour
{
	// Token: 0x06000155 RID: 341 RVA: 0x00003510 File Offset: 0x00001710
	private void Awake()
	{
		if (this.draggableCamera == null)
		{
			this.draggableCamera = NGUITools.FindInParents<UIDraggableCamera>(base.gameObject);
		}
	}

	// Token: 0x06000156 RID: 342 RVA: 0x00003531 File Offset: 0x00001731
	private void OnPress(bool isPressed)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggableCamera != null)
		{
			this.draggableCamera.Press(isPressed);
		}
	}

	// Token: 0x06000157 RID: 343 RVA: 0x00003562 File Offset: 0x00001762
	private void OnDrag(Vector2 delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggableCamera != null)
		{
			this.draggableCamera.Drag(delta);
		}
	}

	// Token: 0x06000158 RID: 344 RVA: 0x00003593 File Offset: 0x00001793
	private void OnScroll(float delta)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.draggableCamera != null)
		{
			this.draggableCamera.Scroll(delta);
		}
	}

	// Token: 0x04000091 RID: 145
	public UIDraggableCamera draggableCamera;
}
