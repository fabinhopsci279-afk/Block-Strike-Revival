using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000028 RID: 40
[AddComponentMenu("NGUI/Interaction/Drag and Drop Item")]
public class UIDragDropItem : MonoBehaviour
{
	// Token: 0x0600015D RID: 349 RVA: 0x00003606 File Offset: 0x00001806
	protected virtual void Awake()
	{
		this.mTrans = base.transform;
		this.mCollider = base.GetComponent<Collider>();
		this.mCollider2D = base.GetComponent<Collider2D>();
	}

	// Token: 0x0600015E RID: 350 RVA: 0x0000362C File Offset: 0x0000182C
	protected virtual void OnEnable()
	{
	}

	// Token: 0x0600015F RID: 351 RVA: 0x0000362E File Offset: 0x0000182E
	protected virtual void OnDisable()
	{
		if (this.mDragging)
		{
			this.StopDragging(UICamera.hoveredObject);
		}
	}

	// Token: 0x06000160 RID: 352 RVA: 0x00003643 File Offset: 0x00001843
	protected virtual void Start()
	{
		this.mButton = base.GetComponent<UIButton>();
		this.mDragScrollView = base.GetComponent<UIDragScrollView>();
	}

	// Token: 0x06000161 RID: 353 RVA: 0x00021F4C File Offset: 0x0002014C
	protected virtual void OnPress(bool isPressed)
	{
		if (this.interactable && UICamera.currentTouchID != -2)
		{
			if (UICamera.currentTouchID != -3)
			{
				if (isPressed)
				{
					if (!this.mPressed)
					{
						this.mTouch = UICamera.currentTouch;
						this.mDragStartTime = RealTime.time + this.pressAndHoldDelay;
						this.mPressed = true;
					}
				}
				else if (this.mPressed && this.mTouch == UICamera.currentTouch)
				{
					this.mPressed = false;
					this.mTouch = null;
				}
				return;
			}
		}
	}

	// Token: 0x06000162 RID: 354 RVA: 0x0000365D File Offset: 0x0000185D
	protected virtual void Update()
	{
		if (this.restriction == UIDragDropItem.Restriction.PressAndHold && this.mPressed && !this.mDragging && this.mDragStartTime < RealTime.time)
		{
			this.StartDragging();
		}
	}

	// Token: 0x06000163 RID: 355 RVA: 0x00021FCC File Offset: 0x000201CC
	protected virtual void OnDragStart()
	{
		if (!this.interactable)
		{
			return;
		}
		if (base.enabled)
		{
			if (this.mTouch == UICamera.currentTouch)
			{
				if (this.restriction != UIDragDropItem.Restriction.None)
				{
					if (this.restriction == UIDragDropItem.Restriction.Horizontal)
					{
						Vector2 totalDelta = this.mTouch.totalDelta;
						if (Mathf.Abs(totalDelta.x) < Mathf.Abs(totalDelta.y))
						{
							return;
						}
					}
					else if (this.restriction == UIDragDropItem.Restriction.Vertical)
					{
						Vector2 totalDelta2 = this.mTouch.totalDelta;
						if (Mathf.Abs(totalDelta2.x) > Mathf.Abs(totalDelta2.y))
						{
							return;
						}
					}
					else if (this.restriction == UIDragDropItem.Restriction.PressAndHold)
					{
						return;
					}
				}
				this.StartDragging();
				return;
			}
		}
	}

	// Token: 0x06000164 RID: 356 RVA: 0x00022078 File Offset: 0x00020278
	public virtual void StartDragging()
	{
		if (!this.interactable)
		{
			return;
		}
		if (!this.mDragging)
		{
			if (this.cloneOnDrag)
			{
				this.mPressed = false;
				GameObject gameObject = NGUITools.AddChild(base.transform.parent.gameObject, base.gameObject);
				gameObject.transform.localPosition = base.transform.localPosition;
				gameObject.transform.localRotation = base.transform.localRotation;
				gameObject.transform.localScale = base.transform.localScale;
				UIButtonColor component = gameObject.GetComponent<UIButtonColor>();
				if (component != null)
				{
					component.defaultColor = base.GetComponent<UIButtonColor>().defaultColor;
				}
				if (this.mTouch != null && this.mTouch.pressed == base.gameObject)
				{
					this.mTouch.current = gameObject;
					this.mTouch.pressed = gameObject;
					this.mTouch.dragged = gameObject;
					this.mTouch.last = gameObject;
				}
				UIDragDropItem component2 = gameObject.GetComponent<UIDragDropItem>();
				component2.mTouch = this.mTouch;
				component2.mPressed = true;
				component2.mDragging = true;
				component2.Start();
				component2.OnClone(base.gameObject);
				component2.OnDragDropStart();
				if (UICamera.currentTouch == null)
				{
					UICamera.currentTouch = this.mTouch;
				}
				this.mTouch = null;
				UICamera.Notify(base.gameObject, "OnPress", false);
				UICamera.Notify(base.gameObject, "OnHover", false);
			}
			else
			{
				this.mDragging = true;
				this.OnDragDropStart();
			}
		}
	}

	// Token: 0x06000165 RID: 357 RVA: 0x0000362C File Offset: 0x0000182C
	protected virtual void OnClone(GameObject original)
	{
	}

	// Token: 0x06000166 RID: 358 RVA: 0x00022208 File Offset: 0x00020408
	protected virtual void OnDrag(Vector2 delta)
	{
		if (!this.interactable)
		{
			return;
		}
		if (this.mDragging && base.enabled)
		{
			if (this.mTouch == UICamera.currentTouch)
			{
				this.OnDragDropMove(delta * this.mRoot.pixelSizeAdjustment);
				return;
			}
		}
	}

	// Token: 0x06000167 RID: 359 RVA: 0x0000368B File Offset: 0x0000188B
	protected virtual void OnDragEnd()
	{
		if (!this.interactable)
		{
			return;
		}
		if (base.enabled)
		{
			if (this.mTouch == UICamera.currentTouch)
			{
				this.StopDragging(UICamera.hoveredObject);
				return;
			}
		}
	}

	// Token: 0x06000168 RID: 360 RVA: 0x000036B9 File Offset: 0x000018B9
	public void StopDragging(GameObject go)
	{
		if (this.mDragging)
		{
			this.mDragging = false;
			this.OnDragDropRelease(go);
		}
	}

	// Token: 0x06000169 RID: 361 RVA: 0x00022258 File Offset: 0x00020458
	protected virtual void OnDragDropStart()
	{
		if (!UIDragDropItem.draggedItems.Contains(this))
		{
			UIDragDropItem.draggedItems.Add(this);
		}
		if (this.mDragScrollView != null)
		{
			this.mDragScrollView.enabled = false;
		}
		if (this.mButton != null)
		{
			this.mButton.isEnabled = false;
		}
		else if (this.mCollider != null)
		{
			this.mCollider.enabled = false;
		}
		else if (this.mCollider2D != null)
		{
			this.mCollider2D.enabled = false;
		}
		this.mParent = this.mTrans.parent;
		this.mRoot = NGUITools.FindInParents<UIRoot>(this.mParent);
		this.mGrid = NGUITools.FindInParents<UIGrid>(this.mParent);
		this.mTable = NGUITools.FindInParents<UITable>(this.mParent);
		if (UIDragDropRoot.root != null)
		{
			this.mTrans.parent = UIDragDropRoot.root;
		}
		Vector3 localPosition = this.mTrans.localPosition;
		localPosition.z = 0f;
		this.mTrans.localPosition = localPosition;
		TweenPosition component = base.GetComponent<TweenPosition>();
		if (component != null)
		{
			component.enabled = false;
		}
		SpringPosition component2 = base.GetComponent<SpringPosition>();
		if (component2 != null)
		{
			component2.enabled = false;
		}
		NGUITools.MarkParentAsChanged(base.gameObject);
		if (this.mTable != null)
		{
			this.mTable.repositionNow = true;
		}
		if (this.mGrid != null)
		{
			this.mGrid.repositionNow = true;
		}
	}

	// Token: 0x0600016A RID: 362 RVA: 0x000036D1 File Offset: 0x000018D1
	protected virtual void OnDragDropMove(Vector2 delta)
	{
		this.mTrans.localPosition += (Vector3)delta;
	}

	// Token: 0x0600016B RID: 363 RVA: 0x000223DC File Offset: 0x000205DC
	protected virtual void OnDragDropRelease(GameObject surface)
	{
		if (!this.cloneOnDrag)
		{
			if (this.mButton != null)
			{
				this.mButton.isEnabled = true;
			}
			else if (this.mCollider != null)
			{
				this.mCollider.enabled = true;
			}
			else if (this.mCollider2D != null)
			{
				this.mCollider2D.enabled = true;
			}
			UIDragDropContainer uidragDropContainer = (!surface) ? null : NGUITools.FindInParents<UIDragDropContainer>(surface);
			if (uidragDropContainer != null)
			{
				this.mTrans.parent = ((!(uidragDropContainer.reparentTarget != null)) ? uidragDropContainer.transform : uidragDropContainer.reparentTarget);
				Vector3 localPosition = this.mTrans.localPosition;
				localPosition.z = 0f;
				this.mTrans.localPosition = localPosition;
			}
			else
			{
				this.mTrans.parent = this.mParent;
			}
			this.mParent = this.mTrans.parent;
			this.mGrid = NGUITools.FindInParents<UIGrid>(this.mParent);
			this.mTable = NGUITools.FindInParents<UITable>(this.mParent);
			if (this.mDragScrollView != null)
			{
				base.StartCoroutine(this.EnableDragScrollView());
			}
			NGUITools.MarkParentAsChanged(base.gameObject);
			if (this.mTable != null)
			{
				this.mTable.repositionNow = true;
			}
			if (this.mGrid != null)
			{
				this.mGrid.repositionNow = true;
			}
		}
		else
		{
			NGUITools.Destroy(base.gameObject);
		}
		this.OnDragDropEnd();
	}

	// Token: 0x0600016C RID: 364 RVA: 0x000036EF File Offset: 0x000018EF
	protected virtual void OnDragDropEnd()
	{
		UIDragDropItem.draggedItems.Remove(this);
	}

	// Token: 0x0600016D RID: 365 RVA: 0x00022560 File Offset: 0x00020760
	protected IEnumerator EnableDragScrollView()
	{
		yield return new WaitForEndOfFrame();
		//return 1;
		if (!(this.mDragScrollView != null))
		{
			goto IL_56;
		}
		this.mDragScrollView.enabled = true;
		IL_56:
		yield break;
	}

	// Token: 0x04000093 RID: 147
	public UIDragDropItem.Restriction restriction;

	// Token: 0x04000094 RID: 148
	public bool cloneOnDrag;

	// Token: 0x04000095 RID: 149
	[HideInInspector]
	public float pressAndHoldDelay = 1f;

	// Token: 0x04000096 RID: 150
	public bool interactable = true;

	// Token: 0x04000097 RID: 151
	[NonSerialized]
	protected Transform mTrans;

	// Token: 0x04000098 RID: 152
	[NonSerialized]
	protected Transform mParent;

	// Token: 0x04000099 RID: 153
	[NonSerialized]
	protected Collider mCollider;

	// Token: 0x0400009A RID: 154
	[NonSerialized]
	protected Collider2D mCollider2D;

	// Token: 0x0400009B RID: 155
	[NonSerialized]
	protected UIButton mButton;

	// Token: 0x0400009C RID: 156
	[NonSerialized]
	protected UIRoot mRoot;

	// Token: 0x0400009D RID: 157
	[NonSerialized]
	protected UIGrid mGrid;

	// Token: 0x0400009E RID: 158
	[NonSerialized]
	protected UITable mTable;

	// Token: 0x0400009F RID: 159
	[NonSerialized]
	protected float mDragStartTime;

	// Token: 0x040000A0 RID: 160
	[NonSerialized]
	protected UIDragScrollView mDragScrollView;

	// Token: 0x040000A1 RID: 161
	[NonSerialized]
	protected bool mPressed;

	// Token: 0x040000A2 RID: 162
	[NonSerialized]
	protected bool mDragging;

	// Token: 0x040000A3 RID: 163
	[NonSerialized]
	protected UICamera.MouseOrTouch mTouch;

	// Token: 0x040000A4 RID: 164
	public static List<UIDragDropItem> draggedItems = new List<UIDragDropItem>();

	// Token: 0x02000029 RID: 41
	public enum Restriction
	{
		// Token: 0x040000A6 RID: 166
		None,
		// Token: 0x040000A7 RID: 167
		Horizontal,
		// Token: 0x040000A8 RID: 168
		Vertical,
		// Token: 0x040000A9 RID: 169
		PressAndHold
	}
}
