using System;
using UnityEngine;

// Token: 0x0200002F RID: 47
[AddComponentMenu("NGUI/Interaction/Drag Scroll View")]
public class UIDragScrollView : MonoBehaviour
{
	// Token: 0x0600018A RID: 394 RVA: 0x0002301C File Offset: 0x0002121C
	private void OnEnable()
	{
		this.mTrans = base.transform;
		if (this.scrollView == null && this.draggablePanel != null)
		{
			this.scrollView = this.draggablePanel;
			this.draggablePanel = null;
		}
		if (this.mStarted && (this.mAutoFind || this.mScroll == null))
		{
			this.FindScrollView();
		}
	}

	// Token: 0x0600018B RID: 395 RVA: 0x000037CE File Offset: 0x000019CE
	private void Start()
	{
		this.mStarted = true;
		this.FindScrollView();
	}

	// Token: 0x0600018C RID: 396 RVA: 0x00023088 File Offset: 0x00021288
	private void FindScrollView()
	{
		UIScrollView uiscrollView = NGUITools.FindInParents<UIScrollView>(this.mTrans);
		if (!(this.scrollView == null) && (!this.mAutoFind || !(uiscrollView != this.scrollView)))
		{
			if (this.scrollView == uiscrollView)
			{
				this.mAutoFind = true;
			}
		}
		else
		{
			this.scrollView = uiscrollView;
			this.mAutoFind = true;
		}
		this.mScroll = this.scrollView;
	}

	// Token: 0x0600018D RID: 397 RVA: 0x000230F8 File Offset: 0x000212F8
	private void OnPress(bool pressed)
	{
		if (this.mAutoFind && this.mScroll != this.scrollView)
		{
			this.mScroll = this.scrollView;
			this.mAutoFind = false;
		}
		if (this.scrollView && base.enabled && NGUITools.GetActive(base.gameObject))
		{
			this.scrollView.Press(pressed);
			if (!pressed && this.mAutoFind)
			{
				this.scrollView = NGUITools.FindInParents<UIScrollView>(this.mTrans);
				this.mScroll = this.scrollView;
			}
		}
	}

	// Token: 0x0600018E RID: 398 RVA: 0x000037DD File Offset: 0x000019DD
	private void OnDrag(Vector2 delta)
	{
		if (this.scrollView && NGUITools.GetActive(this))
		{
			this.scrollView.Drag();
		}
	}

	// Token: 0x0600018F RID: 399 RVA: 0x000037FF File Offset: 0x000019FF
	private void OnScroll(float delta)
	{
		if (this.scrollView && NGUITools.GetActive(this))
		{
			this.scrollView.Scroll(delta);
		}
	}

	// Token: 0x06000190 RID: 400 RVA: 0x00003822 File Offset: 0x00001A22
	public void OnPan(Vector2 delta)
	{
		if (this.scrollView && NGUITools.GetActive(this))
		{
			this.scrollView.OnPan(delta);
		}
	}

	// Token: 0x040000D1 RID: 209
	public UIScrollView scrollView;

	// Token: 0x040000D2 RID: 210
	[SerializeField]
	[HideInInspector]
	private UIScrollView draggablePanel;

	// Token: 0x040000D3 RID: 211
	private Transform mTrans;

	// Token: 0x040000D4 RID: 212
	private UIScrollView mScroll;

	// Token: 0x040000D5 RID: 213
	private bool mAutoFind;

	// Token: 0x040000D6 RID: 214
	private bool mStarted;
}
