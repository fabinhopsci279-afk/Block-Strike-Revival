using System;
using UnityEngine;

// Token: 0x0200002E RID: 46
[AddComponentMenu("NGUI/Interaction/Drag-Resize Widget")]
public class UIDragResize : MonoBehaviour
{
	// Token: 0x06000186 RID: 390 RVA: 0x00022E40 File Offset: 0x00021040
	private void OnDragStart()
	{
		if (this.target != null)
		{
			Vector3[] worldCorners = this.target.worldCorners;
			this.mPlane = new Plane(worldCorners[0], worldCorners[1], worldCorners[3]);
			Ray currentRay = UICamera.currentRay;
			float distance;
			if (this.mPlane.Raycast(currentRay, out distance))
			{
				this.mRayPos = currentRay.GetPoint(distance);
				this.mLocalPos = this.target.cachedTransform.localPosition;
				this.mWidth = this.target.width;
				this.mHeight = this.target.height;
				this.mDragging = true;
			}
		}
	}

	// Token: 0x06000187 RID: 391 RVA: 0x00022EFC File Offset: 0x000210FC
	private void OnDrag(Vector2 delta)
	{
		if (this.mDragging && this.target != null)
		{
			Ray currentRay = UICamera.currentRay;
			float distance;
			if (this.mPlane.Raycast(currentRay, out distance))
			{
				Transform cachedTransform = this.target.cachedTransform;
				cachedTransform.localPosition = this.mLocalPos;
				this.target.width = this.mWidth;
				this.target.height = this.mHeight;
				Vector3 b = currentRay.GetPoint(distance) - this.mRayPos;
				cachedTransform.position += b;
				Vector3 vector = Quaternion.Inverse(cachedTransform.localRotation) * (cachedTransform.localPosition - this.mLocalPos);
				cachedTransform.localPosition = this.mLocalPos;
				NGUIMath.ResizeWidget(this.target, this.pivot, vector.x, vector.y, this.minWidth, this.minHeight, this.maxWidth, this.maxHeight);
				if (this.updateAnchors)
				{
					this.target.BroadcastMessage("UpdateAnchors");
				}
			}
		}
	}

	// Token: 0x06000188 RID: 392 RVA: 0x000037C5 File Offset: 0x000019C5
	private void OnDragEnd()
	{
		this.mDragging = false;
	}

	// Token: 0x040000C4 RID: 196
	public UIWidget target;

	// Token: 0x040000C5 RID: 197
	public UIWidget.Pivot pivot = UIWidget.Pivot.BottomRight;

	// Token: 0x040000C6 RID: 198
	public int minWidth = 100;

	// Token: 0x040000C7 RID: 199
	public int minHeight = 100;

	// Token: 0x040000C8 RID: 200
	public int maxWidth = 100000;

	// Token: 0x040000C9 RID: 201
	public int maxHeight = 100000;

	// Token: 0x040000CA RID: 202
	public bool updateAnchors;

	// Token: 0x040000CB RID: 203
	private Plane mPlane;

	// Token: 0x040000CC RID: 204
	private Vector3 mRayPos;

	// Token: 0x040000CD RID: 205
	private Vector3 mLocalPos;

	// Token: 0x040000CE RID: 206
	private int mWidth;

	// Token: 0x040000CF RID: 207
	private int mHeight;

	// Token: 0x040000D0 RID: 208
	private bool mDragging;
}
