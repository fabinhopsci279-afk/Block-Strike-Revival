using System;
using UnityEngine;

// Token: 0x0200054F RID: 1359
public class UIScrollAlphaObject : MonoBehaviour
{
	// Token: 0x06002C2F RID: 11311 RVA: 0x000DDAF8 File Offset: 0x000DBCF8
	private void Start()
	{
		this.mTrans = base.transform;
		this.scale = this.mTrans.localScale;
		this.position = this.mTrans.localPosition;
		this.mParent = this.mTrans.parent;
		this.widget = base.GetComponent<UIWidget>();
	}

	// Token: 0x06002C30 RID: 11312 RVA: 0x000DDB50 File Offset: 0x000DBD50
	private void Update()
	{
		this.pos = this.mTrans.localPosition + this.mParent.localPosition;
		float num = Mathf.Clamp(Mathf.Abs(this.pos.x), 0f, this.cellWidth);
		this.mTrans.localScale = (this.cellWidth - num * this.downScale) / this.cellWidth * this.scale;
		this.mTrans.localPosition = num * this.downPosition / 100f + this.position;
		this.widget.alpha = 1f - this.downAlpha * num / 100f;
	}

	// Token: 0x04001C37 RID: 7223
	private UIWidget widget;

	// Token: 0x04001C38 RID: 7224
	private Transform mTrans;

	// Token: 0x04001C39 RID: 7225
	private Transform mParent;

	// Token: 0x04001C3A RID: 7226
	private Vector3 pos;

	// Token: 0x04001C3B RID: 7227
	private Vector3 scale;

	// Token: 0x04001C3C RID: 7228
	private Vector3 position;

	// Token: 0x04001C3D RID: 7229
	public float cellWidth = 160f;

	// Token: 0x04001C3E RID: 7230
	public Vector3 downPosition = new Vector3(0f, 20f, 0f);

	// Token: 0x04001C3F RID: 7231
	public float downAlpha = 0.5f;

	// Token: 0x04001C40 RID: 7232
	public float downScale = 0.5f;
}
