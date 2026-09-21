using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000047 RID: 71
[AddComponentMenu("NGUI/Interaction/NGUI Progress Bar")]
[ExecuteInEditMode]
public class UIProgressBar : UIWidgetContainer
{
	// Token: 0x17000022 RID: 34
	// (get) Token: 0x06000259 RID: 601 RVA: 0x0000439E File Offset: 0x0000259E
	public Transform cachedTransform
	{
		get
		{
			if (this.mTrans == null)
			{
				this.mTrans = base.transform;
			}
			return this.mTrans;
		}
	}

	// Token: 0x17000023 RID: 35
	// (get) Token: 0x0600025A RID: 602 RVA: 0x000043C0 File Offset: 0x000025C0
	public Camera cachedCamera
	{
		get
		{
			if (this.mCam == null)
			{
				this.mCam = NGUITools.FindCameraForLayer(base.gameObject.layer);
			}
			return this.mCam;
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x0600025B RID: 603 RVA: 0x000043EC File Offset: 0x000025EC
	// (set) Token: 0x0600025C RID: 604 RVA: 0x000043F4 File Offset: 0x000025F4
	public UIWidget foregroundWidget
	{
		get
		{
			return this.mFG;
		}
		set
		{
			if (this.mFG != value)
			{
				this.mFG = value;
				this.mIsDirty = true;
			}
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x0600025D RID: 605 RVA: 0x00004412 File Offset: 0x00002612
	// (set) Token: 0x0600025E RID: 606 RVA: 0x0000441A File Offset: 0x0000261A
	public UIWidget backgroundWidget
	{
		get
		{
			return this.mBG;
		}
		set
		{
			if (this.mBG != value)
			{
				this.mBG = value;
				this.mIsDirty = true;
			}
		}
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x0600025F RID: 607 RVA: 0x00004438 File Offset: 0x00002638
	// (set) Token: 0x06000260 RID: 608 RVA: 0x00004440 File Offset: 0x00002640
	public UIProgressBar.FillDirection fillDirection
	{
		get
		{
			return this.mFill;
		}
		set
		{
			if (this.mFill != value)
			{
				this.mFill = value;
				this.ForceUpdate();
			}
		}
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x06000261 RID: 609 RVA: 0x00004458 File Offset: 0x00002658
	// (set) Token: 0x06000262 RID: 610 RVA: 0x00026810 File Offset: 0x00024A10
	public float value
	{
		get
		{
			if (this.numberOfSteps > 1)
			{
				return Mathf.Round(this.mValue * (float)(this.numberOfSteps - 1)) / (float)(this.numberOfSteps - 1);
			}
			return this.mValue;
		}
		set
		{
			float num = Mathf.Clamp01(value);
			if (this.mValue != num)
			{
				float value2 = this.value;
				this.mValue = num;
				if (value2 != this.value)
				{
					this.ForceUpdate();
					if (NGUITools.GetActive(this) && EventDelegate.IsValid(this.onChange))
					{
						UIProgressBar.current = this;
						EventDelegate.Execute(this.onChange);
						UIProgressBar.current = null;
					}
				}
			}
		}
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x06000263 RID: 611 RVA: 0x00004489 File Offset: 0x00002689
	// (set) Token: 0x06000264 RID: 612 RVA: 0x00026878 File Offset: 0x00024A78
	public float alpha
	{
		get
		{
			if (this.mFG != null)
			{
				return this.mFG.alpha;
			}
			if (this.mBG != null)
			{
				return this.mBG.alpha;
			}
			return 1f;
		}
		set
		{
			if (this.mFG != null)
			{
				this.mFG.alpha = value;
				if (this.mFG.GetComponent<Collider>() != null)
				{
					this.mFG.GetComponent<Collider>().enabled = (this.mFG.alpha > 0.001f);
				}
				else if (this.mFG.GetComponent<Collider2D>() != null)
				{
					this.mFG.GetComponent<Collider2D>().enabled = (this.mFG.alpha > 0.001f);
				}
			}
			if (this.mBG != null)
			{
				this.mBG.alpha = value;
				if (this.mBG.GetComponent<Collider>() != null)
				{
					this.mBG.GetComponent<Collider>().enabled = (this.mBG.alpha > 0.001f);
				}
				else if (this.mBG.GetComponent<Collider2D>() != null)
				{
					this.mBG.GetComponent<Collider2D>().enabled = (this.mBG.alpha > 0.001f);
				}
			}
			if (this.thumb != null)
			{
				UIWidget component = this.thumb.GetComponent<UIWidget>();
				if (component != null)
				{
					component.alpha = value;
					if (component.GetComponent<Collider>() != null)
					{
						component.GetComponent<Collider>().enabled = (component.alpha > 0.001f);
					}
					else if (component.GetComponent<Collider2D>() != null)
					{
						component.GetComponent<Collider2D>().enabled = (component.alpha > 0.001f);
					}
				}
			}
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x06000265 RID: 613 RVA: 0x000044C4 File Offset: 0x000026C4
	protected bool isHorizontal
	{
		get
		{
			return this.mFill == UIProgressBar.FillDirection.LeftToRight || this.mFill == UIProgressBar.FillDirection.RightToLeft;
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x06000266 RID: 614 RVA: 0x000044DA File Offset: 0x000026DA
	protected bool isInverted
	{
		get
		{
			return this.mFill == UIProgressBar.FillDirection.RightToLeft || this.mFill == UIProgressBar.FillDirection.TopToBottom;
		}
	}

	// Token: 0x06000267 RID: 615 RVA: 0x00026A0C File Offset: 0x00024C0C
	protected void Start()
	{
		this.Upgrade();
		if (Application.isPlaying)
		{
			if (this.mBG != null)
			{
				this.mBG.autoResizeBoxCollider = true;
			}
			this.OnStart();
			if (UIProgressBar.current == null && this.onChange != null)
			{
				UIProgressBar.current = this;
				EventDelegate.Execute(this.onChange);
				UIProgressBar.current = null;
			}
		}
		this.ForceUpdate();
	}

	// Token: 0x06000268 RID: 616 RVA: 0x0000362C File Offset: 0x0000182C
	protected virtual void Upgrade()
	{
	}

	// Token: 0x06000269 RID: 617 RVA: 0x0000362C File Offset: 0x0000182C
	protected virtual void OnStart()
	{
	}

	// Token: 0x0600026A RID: 618 RVA: 0x000044F1 File Offset: 0x000026F1
	protected void Update()
	{
		if (this.mIsDirty)
		{
			this.ForceUpdate();
		}
	}

	// Token: 0x0600026B RID: 619 RVA: 0x00026A78 File Offset: 0x00024C78
	protected void OnValidate()
	{
		if (NGUITools.GetActive(this))
		{
			this.Upgrade();
			this.mIsDirty = true;
			float num = Mathf.Clamp01(this.mValue);
			if (this.mValue != num)
			{
				this.mValue = num;
			}
			if (this.numberOfSteps < 0)
			{
				this.numberOfSteps = 0;
			}
			else if (this.numberOfSteps > 20)
			{
				this.numberOfSteps = 20;
			}
			this.ForceUpdate();
		}
		else
		{
			float num2 = Mathf.Clamp01(this.mValue);
			if (this.mValue != num2)
			{
				this.mValue = num2;
			}
			if (this.numberOfSteps < 0)
			{
				this.numberOfSteps = 0;
			}
			else if (this.numberOfSteps > 20)
			{
				this.numberOfSteps = 20;
			}
		}
	}

	// Token: 0x0600026C RID: 620 RVA: 0x00026B24 File Offset: 0x00024D24
	protected float ScreenToValue(Vector2 screenPos)
	{
		Transform cachedTransform = this.cachedTransform;
		Plane plane = new Plane(cachedTransform.rotation * Vector3.back, cachedTransform.position);
		Ray ray = this.cachedCamera.ScreenPointToRay(screenPos);
		float distance;
		if (!plane.Raycast(ray, out distance))
		{
			return this.value;
		}
		return this.LocalToValue(cachedTransform.InverseTransformPoint(ray.GetPoint(distance)));
	}

	// Token: 0x0600026D RID: 621 RVA: 0x00026B94 File Offset: 0x00024D94
	protected virtual float LocalToValue(Vector2 localPos)
	{
		if (!(this.mFG != null))
		{
			return this.value;
		}
		Vector3[] localCorners = this.mFG.localCorners;
		Vector3 vector = localCorners[2] - localCorners[0];
		if (this.isHorizontal)
		{
			float num = (localPos.x - localCorners[0].x) / vector.x;
			return (!this.isInverted) ? num : (1f - num);
		}
		float num2 = (localPos.y - localCorners[0].y) / vector.y;
		return (!this.isInverted) ? num2 : (1f - num2);
	}

	// Token: 0x0600026E RID: 622 RVA: 0x00026C4C File Offset: 0x00024E4C
	public virtual void ForceUpdate()
	{
		this.mIsDirty = false;
		bool flag = false;
		if (this.mFG != null)
		{
			UIBasicSprite uibasicSprite = this.mFG as UIBasicSprite;
			if (this.isHorizontal)
			{
				if (uibasicSprite != null && uibasicSprite.type == UIBasicSprite.Type.Filled)
				{
					if (uibasicSprite.fillDirection == UIBasicSprite.FillDirection.Horizontal || uibasicSprite.fillDirection == UIBasicSprite.FillDirection.Vertical)
					{
						uibasicSprite.fillDirection = UIBasicSprite.FillDirection.Horizontal;
						uibasicSprite.invert = this.isInverted;
					}
					uibasicSprite.fillAmount = this.value;
				}
				else
				{
					this.mFG.drawRegion = ((!this.isInverted) ? new Vector4(0f, 0f, this.value, 1f) : new Vector4(1f - this.value, 0f, 1f, 1f));
					this.mFG.enabled = true;
					flag = (this.value < 0.001f);
				}
			}
			else if (uibasicSprite != null && uibasicSprite.type == UIBasicSprite.Type.Filled)
			{
				if (uibasicSprite.fillDirection == UIBasicSprite.FillDirection.Horizontal || uibasicSprite.fillDirection == UIBasicSprite.FillDirection.Vertical)
				{
					uibasicSprite.fillDirection = UIBasicSprite.FillDirection.Vertical;
					uibasicSprite.invert = this.isInverted;
				}
				uibasicSprite.fillAmount = this.value;
			}
			else
			{
				this.mFG.drawRegion = ((!this.isInverted) ? new Vector4(0f, 0f, 1f, this.value) : new Vector4(0f, 1f - this.value, 1f, 1f));
				this.mFG.enabled = true;
				flag = (this.value < 0.001f);
			}
		}
		if (this.thumb != null && (this.mFG != null || this.mBG != null))
		{
			Vector3[] array = (!(this.mFG != null)) ? this.mBG.localCorners : this.mFG.localCorners;
			Vector4 vector = (!(this.mFG != null)) ? this.mBG.border : this.mFG.border;
			Vector3[] array2 = array;
			int num = 0;
			array2[num].x = array2[num].x + vector.x;
			Vector3[] array3 = array;
			int num2 = 1;
			array3[num2].x = array3[num2].x + vector.x;
			Vector3[] array4 = array;
			int num3 = 2;
			array4[num3].x = array4[num3].x - vector.z;
			Vector3[] array5 = array;
			int num4 = 3;
			array5[num4].x = array5[num4].x - vector.z;
			Vector3[] array6 = array;
			int num5 = 0;
			array6[num5].y = array6[num5].y + vector.y;
			Vector3[] array7 = array;
			int num6 = 1;
			array7[num6].y = array7[num6].y - vector.w;
			Vector3[] array8 = array;
			int num7 = 2;
			array8[num7].y = array8[num7].y - vector.w;
			Vector3[] array9 = array;
			int num8 = 3;
			array9[num8].y = array9[num8].y + vector.y;
			Transform transform = (!(this.mFG != null)) ? this.mBG.cachedTransform : this.mFG.cachedTransform;
			for (int i = 0; i < 4; i++)
			{
				array[i] = transform.TransformPoint(array[i]);
			}
			if (this.isHorizontal)
			{
				Vector3 from = Vector3.Lerp(array[0], array[1], 0.5f);
				Vector3 to = Vector3.Lerp(array[2], array[3], 0.5f);
				this.SetThumbPosition(Vector3.Lerp(from, to, (!this.isInverted) ? this.value : (1f - this.value)));
			}
			else
			{
				Vector3 from2 = Vector3.Lerp(array[0], array[3], 0.5f);
				Vector3 to2 = Vector3.Lerp(array[1], array[2], 0.5f);
				this.SetThumbPosition(Vector3.Lerp(from2, to2, (!this.isInverted) ? this.value : (1f - this.value)));
			}
		}
		if (flag)
		{
			this.mFG.enabled = false;
		}
	}

	// Token: 0x0600026F RID: 623 RVA: 0x00027094 File Offset: 0x00025294
	protected void SetThumbPosition(Vector3 worldPos)
	{
		Transform parent = this.thumb.parent;
		if (parent != null)
		{
			worldPos = parent.InverseTransformPoint(worldPos);
			worldPos.x = Mathf.Round(worldPos.x);
			worldPos.y = Mathf.Round(worldPos.y);
			worldPos.z = 0f;
			if (Vector3.Distance(this.thumb.localPosition, worldPos) > 0.001f)
			{
				this.thumb.localPosition = worldPos;
			}
		}
		else if (Vector3.Distance(this.thumb.position, worldPos) > 1E-05f)
		{
			this.thumb.position = worldPos;
		}
	}

	// Token: 0x06000270 RID: 624 RVA: 0x0002713C File Offset: 0x0002533C
	public virtual void OnPan(Vector2 delta)
	{
		if (base.enabled)
		{
			switch (this.mFill)
			{
			case UIProgressBar.FillDirection.LeftToRight:
			{
				float value = Mathf.Clamp01(this.mValue + delta.x);
				this.value = value;
				this.mValue = value;
				break;
			}
			case UIProgressBar.FillDirection.RightToLeft:
			{
				float value2 = Mathf.Clamp01(this.mValue - delta.x);
				this.value = value2;
				this.mValue = value2;
				break;
			}
			case UIProgressBar.FillDirection.BottomToTop:
			{
				float value3 = Mathf.Clamp01(this.mValue + delta.y);
				this.value = value3;
				this.mValue = value3;
				break;
			}
			case UIProgressBar.FillDirection.TopToBottom:
			{
				float value4 = Mathf.Clamp01(this.mValue - delta.y);
				this.value = value4;
				this.mValue = value4;
				break;
			}
			}
		}
	}

	// Token: 0x040001AA RID: 426
	public static UIProgressBar current;

	// Token: 0x040001AB RID: 427
	public UIProgressBar.OnDragFinished onDragFinished;

	// Token: 0x040001AC RID: 428
	public Transform thumb;

	// Token: 0x040001AD RID: 429
	[SerializeField]
	[HideInInspector]
	protected UIWidget mBG;

	// Token: 0x040001AE RID: 430
	[HideInInspector]
	[SerializeField]
	protected UIWidget mFG;

	// Token: 0x040001AF RID: 431
	[HideInInspector]
	[SerializeField]
	protected float mValue = 1f;

	// Token: 0x040001B0 RID: 432
	[SerializeField]
	[HideInInspector]
	protected UIProgressBar.FillDirection mFill;

	// Token: 0x040001B1 RID: 433
	protected Transform mTrans;

	// Token: 0x040001B2 RID: 434
	protected bool mIsDirty;

	// Token: 0x040001B3 RID: 435
	protected Camera mCam;

	// Token: 0x040001B4 RID: 436
	protected float mOffset;

	// Token: 0x040001B5 RID: 437
	public int numberOfSteps;

	// Token: 0x040001B6 RID: 438
	public List<EventDelegate> onChange = new List<EventDelegate>();

	// Token: 0x02000048 RID: 72
	public enum FillDirection
	{
		// Token: 0x040001B8 RID: 440
		LeftToRight,
		// Token: 0x040001B9 RID: 441
		RightToLeft,
		// Token: 0x040001BA RID: 442
		BottomToTop,
		// Token: 0x040001BB RID: 443
		TopToBottom
	}

	// Token: 0x02000049 RID: 73
	// (Invoke) Token: 0x06000272 RID: 626
	public delegate void OnDragFinished();
}
