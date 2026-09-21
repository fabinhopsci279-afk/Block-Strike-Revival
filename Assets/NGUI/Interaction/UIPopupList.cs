using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Beebyte.Obfuscator;
using UnityEngine;

// Token: 0x02000041 RID: 65
[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Popup List")]
public class UIPopupList : UIWidgetContainer
{
	// Token: 0x17000014 RID: 20
	// (get) Token: 0x0600021B RID: 539 RVA: 0x0000413E File Offset: 0x0000233E
	// (set) Token: 0x0600021C RID: 540 RVA: 0x00025164 File Offset: 0x00023364
	public UnityEngine.Object ambigiousFont
	{
		get
		{
			if (this.trueTypeFont != null)
			{
				return this.trueTypeFont;
			}
			if (this.bitmapFont != null)
			{
				return this.bitmapFont;
			}
			return this.font;
		}
		set
		{
			if (value is Font)
			{
				this.trueTypeFont = (value as Font);
				this.bitmapFont = null;
				this.font = null;
			}
			else if (value is UIFont)
			{
				this.bitmapFont = (value as UIFont);
				this.trueTypeFont = null;
				this.font = null;
			}
		}
	}

	// Token: 0x17000015 RID: 21
	// (get) Token: 0x0600021D RID: 541 RVA: 0x00004170 File Offset: 0x00002370
	// (set) Token: 0x0600021E RID: 542 RVA: 0x00004178 File Offset: 0x00002378
	[Obsolete("Use EventDelegate.Add(popup.onChange, YourCallback) instead, and UIPopupList.current.value to determine the state")]
	public UIPopupList.LegacyEvent onSelectionChange
	{
		get
		{
			return this.mLegacyEvent;
		}
		set
		{
			this.mLegacyEvent = value;
		}
	}

	// Token: 0x17000016 RID: 22
	// (get) Token: 0x0600021F RID: 543 RVA: 0x00004181 File Offset: 0x00002381
	public static bool isOpen
	{
		get
		{
			return UIPopupList.current != null && (UIPopupList.mChild != null || UIPopupList.mFadeOutComplete > Time.unscaledTime);
		}
	}

	// Token: 0x17000017 RID: 23
	// (get) Token: 0x06000220 RID: 544 RVA: 0x000041AF File Offset: 0x000023AF
	// (set) Token: 0x06000221 RID: 545 RVA: 0x000041B7 File Offset: 0x000023B7
	public virtual string value
	{
		get
		{
			return this.mSelectedItem;
		}
		set
		{
			this.mSelectedItem = value;
			if (this.mSelectedItem == null)
			{
				return;
			}
			if (this.mSelectedItem != null)
			{
				this.TriggerCallbacks();
			}
		}
	}

	// Token: 0x17000018 RID: 24
	// (get) Token: 0x06000222 RID: 546 RVA: 0x000251B8 File Offset: 0x000233B8
	public virtual object data
	{
		get
		{
			int num = this.items.IndexOf(this.mSelectedItem);
			return (num <= -1 || num >= this.itemData.Count) ? null : this.itemData[num];
		}
	}

	// Token: 0x17000019 RID: 25
	// (get) Token: 0x06000223 RID: 547 RVA: 0x00023740 File Offset: 0x00021940
	public bool isColliderEnabled
	{
		get
		{
			Collider component = base.GetComponent<Collider>();
			if (component != null)
			{
				return component.enabled;
			}
			Collider2D component2 = base.GetComponent<Collider2D>();
			return component2 != null && component2.enabled;
		}
	}

	// Token: 0x1700001A RID: 26
	// (get) Token: 0x06000224 RID: 548 RVA: 0x000041D7 File Offset: 0x000023D7
	// (set) Token: 0x06000225 RID: 549 RVA: 0x000041DF File Offset: 0x000023DF
	[Obsolete("Use 'value' instead")]
	public string selection
	{
		get
		{
			return this.value;
		}
		set
		{
			this.value = value;
		}
	}

	// Token: 0x1700001B RID: 27
	// (get) Token: 0x06000226 RID: 550 RVA: 0x000041E8 File Offset: 0x000023E8
	private bool isValid
	{
		get
		{
			return this.bitmapFont != null || this.trueTypeFont != null;
		}
	}

	// Token: 0x1700001C RID: 28
	// (get) Token: 0x06000227 RID: 551 RVA: 0x00004207 File Offset: 0x00002407
	private int activeFontSize
	{
		get
		{
			return (this.trueTypeFont != null || this.bitmapFont == null) ? this.fontSize : this.bitmapFont.defaultSize;
		}
	}

	// Token: 0x1700001D RID: 29
	// (get) Token: 0x06000228 RID: 552 RVA: 0x00004238 File Offset: 0x00002438
	private float activeFontScale
	{
		get
		{
			return (this.trueTypeFont != null || this.bitmapFont == null) ? 1f : ((float)this.fontSize / (float)this.bitmapFont.defaultSize);
		}
	}

	// Token: 0x06000229 RID: 553 RVA: 0x00004271 File Offset: 0x00002471
	public virtual void Clear()
	{
		this.items.Clear();
		this.itemData.Clear();
	}

	// Token: 0x0600022A RID: 554 RVA: 0x00004289 File Offset: 0x00002489
	public virtual void AddItem(string text)
	{
		this.items.Add(text);
		this.itemData.Add(null);
	}

	// Token: 0x0600022B RID: 555 RVA: 0x000042A3 File Offset: 0x000024A3
	public virtual void AddItem(string text, object data)
	{
		this.items.Add(text);
		this.itemData.Add(data);
	}

	// Token: 0x0600022C RID: 556 RVA: 0x000251F8 File Offset: 0x000233F8
	public virtual void RemoveItem(string text)
	{
		int num = this.items.IndexOf(text);
		if (num != -1)
		{
			this.items.RemoveAt(num);
			this.itemData.RemoveAt(num);
		}
	}

	// Token: 0x0600022D RID: 557 RVA: 0x00025230 File Offset: 0x00023430
	public virtual void RemoveItemByData(object data)
	{
		int num = this.itemData.IndexOf(data);
		if (num != -1)
		{
			this.items.RemoveAt(num);
			this.itemData.RemoveAt(num);
		}
	}

	// Token: 0x0600022E RID: 558 RVA: 0x00025268 File Offset: 0x00023468
	protected void TriggerCallbacks()
	{
		if (!this.mExecuting)
		{
			this.mExecuting = true;
			UIPopupList uipopupList = UIPopupList.current;
			UIPopupList.current = this;
			if (this.mLegacyEvent != null)
			{
				this.mLegacyEvent(this.mSelectedItem);
			}
			if (EventDelegate.IsValid(this.onChange))
			{
				EventDelegate.Execute(this.onChange);
			}
			else if (this.eventReceiver != null && !string.IsNullOrEmpty(this.functionName))
			{
				this.eventReceiver.SendMessage(this.functionName, this.mSelectedItem, SendMessageOptions.DontRequireReceiver);
			}
			UIPopupList.current = uipopupList;
			this.mExecuting = false;
		}
	}

	// Token: 0x0600022F RID: 559 RVA: 0x00025308 File Offset: 0x00023508
	protected virtual void OnEnable()
	{
		if (EventDelegate.IsValid(this.onChange))
		{
			this.eventReceiver = null;
			this.functionName = null;
		}
		if (this.font != null)
		{
			if (this.font.isDynamic)
			{
				this.trueTypeFont = this.font.dynamicFont;
				this.fontStyle = this.font.dynamicFontStyle;
				this.mUseDynamicFont = true;
			}
			else if (this.bitmapFont == null)
			{
				this.bitmapFont = this.font;
				this.mUseDynamicFont = false;
			}
			this.font = null;
		}
		if (this.textScale != 0f)
		{
			this.fontSize = ((!(this.bitmapFont != null)) ? 16 : Mathf.RoundToInt((float)this.bitmapFont.defaultSize * this.textScale));
			this.textScale = 0f;
		}
		if (this.trueTypeFont == null && this.bitmapFont != null && this.bitmapFont.isDynamic)
		{
			this.trueTypeFont = this.bitmapFont.dynamicFont;
			this.bitmapFont = null;
		}
	}

	// Token: 0x06000230 RID: 560 RVA: 0x00025428 File Offset: 0x00023628
	protected virtual void OnValidate()
	{
		Font x = this.trueTypeFont;
		UIFont uifont = this.bitmapFont;
		this.bitmapFont = null;
		this.trueTypeFont = null;
		if (x != null && (uifont == null || !this.mUseDynamicFont))
		{
			this.bitmapFont = null;
			this.trueTypeFont = x;
			this.mUseDynamicFont = true;
		}
		else if (uifont != null)
		{
			if (uifont.isDynamic)
			{
				this.trueTypeFont = uifont.dynamicFont;
				this.fontStyle = uifont.dynamicFontStyle;
				this.fontSize = uifont.defaultSize;
				this.mUseDynamicFont = true;
			}
			else
			{
				this.bitmapFont = uifont;
				this.mUseDynamicFont = false;
			}
		}
		else
		{
			this.trueTypeFont = x;
			this.mUseDynamicFont = true;
		}
	}

	// Token: 0x06000231 RID: 561 RVA: 0x000254E0 File Offset: 0x000236E0
	protected virtual void Start()
	{
		if (this.textLabel != null)
		{
			EventDelegate.Add(this.onChange, new EventDelegate.Callback(this.textLabel.SetCurrentSelection));
			this.textLabel = null;
		}
		if (Application.isPlaying)
		{
			if (string.IsNullOrEmpty(this.mSelectedItem) && this.items.Count > 0)
			{
				this.mSelectedItem = this.items[0];
			}
			if (!string.IsNullOrEmpty(this.mSelectedItem))
			{
				this.TriggerCallbacks();
			}
		}
	}

	// Token: 0x06000232 RID: 562 RVA: 0x000042BD File Offset: 0x000024BD
	protected virtual void OnLocalize()
	{
		if (this.isLocalized)
		{
			this.TriggerCallbacks();
		}
	}

	// Token: 0x06000233 RID: 563 RVA: 0x00025568 File Offset: 0x00023768
	protected virtual void Highlight(UILabel lbl, bool instant)
	{
		if (this.mHighlight != null)
		{
			this.mHighlightedLabel = lbl;
			if (this.mHighlight.GetAtlasSprite() == null)
			{
				return;
			}
			Vector3 highlightPosition = this.GetHighlightPosition();
			if (!instant && this.isAnimated)
			{
				TweenPosition.Begin(this.mHighlight.gameObject, 0.1f, highlightPosition).method = UITweener.Method.EaseOut;
				if (!this.mTweening)
				{
					this.mTweening = true;
					base.StartCoroutine(this.UpdateTweenPosition());
				}
			}
			else
			{
				this.mHighlight.cachedTransform.localPosition = highlightPosition;
			}
		}
	}

	// Token: 0x06000234 RID: 564 RVA: 0x000255F8 File Offset: 0x000237F8
	protected virtual Vector3 GetHighlightPosition()
	{
		if (this.mHighlightedLabel == null || this.mHighlight == null)
		{
			return Vector3.zero;
		}
		UISpriteData atlasSprite = this.mHighlight.GetAtlasSprite();
		if (atlasSprite == null)
		{
			return Vector3.zero;
		}
		float pixelSize = this.atlas.pixelSize;
		float num = (float)atlasSprite.borderLeft * pixelSize;
		float y = (float)atlasSprite.borderTop * pixelSize;
		return this.mHighlightedLabel.cachedTransform.localPosition + new Vector3(-num, y, 1f);
	}

	// Token: 0x06000235 RID: 565 RVA: 0x00025680 File Offset: 0x00023880
	protected virtual IEnumerator UpdateTweenPosition()
	{
        if ((UnityEngine.Object)(object)mHighlight != (UnityEngine.Object)null && (UnityEngine.Object)(object)mHighlightedLabel != (UnityEngine.Object)null)
        {
            TweenPosition tp = ((Component)mHighlight).GetComponent<TweenPosition>();
            while ((UnityEngine.Object)(object)tp != (UnityEngine.Object)null && ((Behaviour)tp).enabled)
            {
                tp.to = GetHighlightPosition();
                yield return null;
            }
        }
        mTweening = false;
    }

	// Token: 0x06000236 RID: 566 RVA: 0x0002569C File Offset: 0x0002389C
	protected virtual void OnItemHover(GameObject go, bool isOver)
	{
		if (isOver)
		{
			UILabel component = go.GetComponent<UILabel>();
			this.Highlight(component, false);
		}
	}

	// Token: 0x06000237 RID: 567 RVA: 0x000256BC File Offset: 0x000238BC
	protected virtual void OnItemPress(GameObject go, bool isPressed)
	{
		if (isPressed)
		{
			this.Select(go.GetComponent<UILabel>(), true);
			UIEventListener component = go.GetComponent<UIEventListener>();
			this.value = (component.parameter as string);
			UIPlaySound[] components = base.GetComponents<UIPlaySound>();
			int i = 0;
			int num = components.Length;
			while (i < num)
			{
				UIPlaySound uiplaySound = components[i];
				if (uiplaySound.trigger == UIPlaySound.Trigger.OnClick)
				{
					NGUITools.PlaySound(uiplaySound.audioClip, uiplaySound.volume, 1f);
				}
				i++;
			}
			this.CloseSelf();
		}
	}

	// Token: 0x06000238 RID: 568 RVA: 0x00025738 File Offset: 0x00023938
	protected virtual void OnItemClick(GameObject go)
	{
		this.Select(go.GetComponent<UILabel>(), true);
		UIEventListener component = go.GetComponent<UIEventListener>();
		this.value = (component.parameter as string);
		UIPlaySound[] components = base.GetComponents<UIPlaySound>();
		int i = 0;
		int num = components.Length;
		while (i < num)
		{
			UIPlaySound uiplaySound = components[i];
			if (uiplaySound.trigger == UIPlaySound.Trigger.OnClick)
			{
				NGUITools.PlaySound(uiplaySound.audioClip, uiplaySound.volume, 1f);
			}
			i++;
		}
		this.CloseSelf();
	}

	// Token: 0x06000239 RID: 569 RVA: 0x000042CD File Offset: 0x000024CD
	private void Select(UILabel lbl, bool instant)
	{
		this.Highlight(lbl, instant);
	}

	// Token: 0x0600023A RID: 570 RVA: 0x000257B0 File Offset: 0x000239B0
	protected virtual void OnNavigate(KeyCode key)
	{
		if (base.enabled && UIPopupList.current == this)
		{
			int num = this.mLabelList.IndexOf(this.mHighlightedLabel);
			if (num == -1)
			{
				num = 0;
			}
			if (key == KeyCode.UpArrow)
			{
				if (num > 0)
				{
					this.Select(this.mLabelList[num - 1], false);
				}
			}
			else if (key == KeyCode.DownArrow && num + 1 < this.mLabelList.Count)
			{
				this.Select(this.mLabelList[num + 1], false);
			}
		}
	}

	// Token: 0x0600023B RID: 571 RVA: 0x000042D7 File Offset: 0x000024D7
	protected virtual void OnKey(KeyCode key)
	{
		if (base.enabled && UIPopupList.current == this && (key == UICamera.current.cancelKey0 || key == UICamera.current.cancelKey1))
		{
			this.OnSelect(false);
		}
	}

	// Token: 0x0600023C RID: 572 RVA: 0x0000430F File Offset: 0x0000250F
	protected virtual void OnDisable()
	{
		this.CloseSelf();
	}

	// Token: 0x0600023D RID: 573 RVA: 0x00004317 File Offset: 0x00002517
	protected virtual void OnSelect(bool isSelected)
	{
		if (isSelected)
		{
		}
	}

	// Token: 0x0600023E RID: 574 RVA: 0x0000431C File Offset: 0x0000251C
	public static void Close()
	{
		if (UIPopupList.current != null)
		{
			UIPopupList.current.CloseSelf();
			UIPopupList.current = null;
		}
	}

	// Token: 0x0600023F RID: 575 RVA: 0x00025840 File Offset: 0x00023A40
	public virtual void CloseSelf()
	{
		if (UIPopupList.mChild != null && UIPopupList.current == this)
		{
			base.StopCoroutine("CloseIfUnselected");
			this.mSelection = null;
			this.mLabelList.Clear();
			if (this.isAnimated)
			{
				UIWidget[] componentsInChildren = UIPopupList.mChild.GetComponentsInChildren<UIWidget>();
				int i = 0;
				int num = componentsInChildren.Length;
				while (i < num)
				{
					UIWidget uiwidget = componentsInChildren[i];
					Color color = uiwidget.color;
					color.a = 0f;
					TweenColor.Begin(uiwidget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
					i++;
				}
				Collider[] componentsInChildren2 = UIPopupList.mChild.GetComponentsInChildren<Collider>();
				int j = 0;
				int num2 = componentsInChildren2.Length;
				while (j < num2)
				{
					componentsInChildren2[j].enabled = false;
					j++;
				}
				UnityEngine.Object.Destroy(UIPopupList.mChild, 0.15f);
				UIPopupList.mFadeOutComplete = Time.unscaledTime + Mathf.Max(0.1f, 0.15f);
			}
			else
			{
				UnityEngine.Object.Destroy(UIPopupList.mChild);
				UIPopupList.mFadeOutComplete = Time.unscaledTime + 0.1f;
			}
			this.mBackground = null;
			this.mHighlight = null;
			UIPopupList.mChild = null;
			UIPopupList.current = null;
		}
	}

	// Token: 0x06000240 RID: 576 RVA: 0x00025970 File Offset: 0x00023B70
	protected virtual void AnimateColor(UIWidget widget)
	{
		Color color = widget.color;
		widget.color = new Color(color.r, color.g, color.b, 0f);
		TweenColor.Begin(widget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
	}

	// Token: 0x06000241 RID: 577 RVA: 0x000259C0 File Offset: 0x00023BC0
	protected virtual void AnimatePosition(UIWidget widget, bool placeAbove, float bottom)
	{
		Vector3 localPosition = widget.cachedTransform.localPosition;
		Vector3 localPosition2 = (!placeAbove) ? new Vector3(localPosition.x, 0f, localPosition.z) : new Vector3(localPosition.x, bottom, localPosition.z);
		widget.cachedTransform.localPosition = localPosition2;
		GameObject gameObject = widget.gameObject;
		TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
	}

	// Token: 0x06000242 RID: 578 RVA: 0x00025A30 File Offset: 0x00023C30
	protected virtual void AnimateScale(UIWidget widget, bool placeAbove, float bottom)
	{
		GameObject gameObject = widget.gameObject;
		Transform cachedTransform = widget.cachedTransform;
		float num = (float)this.activeFontSize * this.activeFontScale + this.mBgBorder * 2f;
		cachedTransform.localScale = new Vector3(1f, num / (float)widget.height, 1f);
		TweenScale.Begin(gameObject, 0.15f, Vector3.one).method = UITweener.Method.EaseOut;
		if (placeAbove)
		{
			Vector3 localPosition = cachedTransform.localPosition;
			cachedTransform.localPosition = new Vector3(localPosition.x, localPosition.y - (float)widget.height + num, localPosition.z);
			TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
		}
	}

	// Token: 0x06000243 RID: 579 RVA: 0x0000433B File Offset: 0x0000253B
	private void Animate(UIWidget widget, bool placeAbove, float bottom)
	{
		this.AnimateColor(widget);
		this.AnimatePosition(widget, placeAbove, bottom);
	}

	// Token: 0x06000244 RID: 580 RVA: 0x00025AE4 File Offset: 0x00023CE4
	protected virtual void OnClick()
	{
		if (this.mOpenFrame == Time.frameCount)
		{
			return;
		}
		if (UIPopupList.mChild == null)
		{
			if (this.openOn != UIPopupList.OpenOn.DoubleClick)
			{
				if (this.openOn != UIPopupList.OpenOn.Manual)
				{
					if (this.openOn == UIPopupList.OpenOn.RightClick && UICamera.currentTouchID != -2)
					{
						return;
					}
					this.Show();
					return;
				}
			}
			return;
		}
		if (this.mHighlightedLabel != null)
		{
			this.OnItemPress(this.mHighlightedLabel.gameObject, true);
		}
	}

	// Token: 0x06000245 RID: 581 RVA: 0x0000434D File Offset: 0x0000254D
	protected virtual void OnDoubleClick()
	{
		if (this.openOn == UIPopupList.OpenOn.DoubleClick)
		{
			this.Show();
		}
	}

	// Token: 0x06000246 RID: 582 RVA: 0x00025B5C File Offset: 0x00023D5C
	[DebuggerHidden]
	[ReplaceLiteralsWithName]
	private IEnumerator CloseIfUnselected()
	{
        while (true)
        {
            yield return null;
            if (!((UnityEngine.Object)(object)UICamera.selectedObject != (UnityEngine.Object)(object)mSelection) || !((UnityEngine.Object)(object)UICamera.selectedObject != (UnityEngine.Object)(object)mBackground.cachedGameObject) || !((UnityEngine.Object)(object)UICamera.selectedObject != (UnityEngine.Object)(object)mHighlight.cachedGameObject) || !((UnityEngine.Object)(object)UICamera.selectedObject != (UnityEngine.Object)(object)mHighlightedLabel.cachedGameObject))
            {
                continue;
            }
            bool equal = false;
            for (int i = 0; i < mLabelList.Count; i++)
            {
                if ((UnityEngine.Object)(object)UICamera.selectedObject == (UnityEngine.Object)(object)mLabelList[i].cachedGameObject)
                {
                    equal = true;
                    break;
                }
            }
            if (!equal)
            {
                break;
            }
        }
        CloseSelf();
    }

	// Token: 0x06000247 RID: 583 RVA: 0x00025B78 File Offset: 0x00023D78
	public virtual void Show()
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && UIPopupList.mChild == null && this.atlas != null && this.isValid && this.items.Count > 0)
		{
			this.mLabelList.Clear();
			base.StopCoroutine("CloseIfUnselected");
			UICamera.selectedObject = (UICamera.hoveredObject ?? base.gameObject);
			this.mSelection = UICamera.selectedObject;
			this.source = UICamera.selectedObject;
			if (this.source == null)
			{
				UnityEngine.Debug.LogError("Popup list needs a source object...");
				return;
			}
			this.mOpenFrame = Time.frameCount;
			if (this.mPanel == null)
			{
				this.mPanel = UIPanel.Find(base.transform);
				if (this.mPanel == null)
				{
					return;
				}
			}
			UIPopupList.mChild = new GameObject("Drop-down List");
			UIPopupList.mChild.layer = base.gameObject.layer;
			if (this.separatePanel)
			{
				if (base.GetComponent<Collider>() != null)
				{
					Rigidbody rigidbody = UIPopupList.mChild.AddComponent<Rigidbody>();
					rigidbody.isKinematic = true;
				}
				else if (base.GetComponent<Collider2D>() != null)
				{
					Rigidbody2D rigidbody2D = UIPopupList.mChild.AddComponent<Rigidbody2D>();
					rigidbody2D.isKinematic = true;
				}
				UIPopupList.mChild.AddComponent<UIPanel>().depth = 1000000;
			}
			UIScrollView uiscrollView = NGUITools.AddChild(UIPopupList.mChild).AddComponent<UIScrollView>();
			uiscrollView.gameObject.AddComponent<Rigidbody>().isKinematic = true;
			uiscrollView.movement = UIScrollView.Movement.Vertical;
			uiscrollView.panel.clipping = UIDrawCall.Clipping.SoftClip;
			uiscrollView.panel.depth = 1000001;
			uiscrollView.disableDragIfFits = true;
			UIPopupList.current = this;
			Transform transform = UIPopupList.mChild.transform;
			transform.parent = this.mPanel.cachedTransform;
			Vector3 localPosition;
			Vector3 vector;
			Vector3 v;
			if (this.openOn == UIPopupList.OpenOn.Manual && this.mSelection != base.gameObject)
			{
				localPosition = UICamera.lastEventPosition;
				vector = this.mPanel.cachedTransform.InverseTransformPoint(this.mPanel.anchorCamera.ScreenToWorldPoint(localPosition));
				v = vector;
				transform.localPosition = vector;
				localPosition = transform.position;
			}
			else
			{
				Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(this.mPanel.cachedTransform, base.transform, false, false);
				vector = bounds.min;
				v = bounds.max;
				transform.localPosition = vector;
				localPosition = transform.position;
			}
			base.StartCoroutine("CloseIfUnselected");
			transform.localRotation = Quaternion.identity;
			transform.localScale = Vector3.one;
			this.mBackground = NGUITools.AddSprite(UIPopupList.mChild, this.atlas, this.backgroundSprite);
			this.mBackground.pivot = UIWidget.Pivot.TopLeft;
			this.mBackground.color = this.backgroundColor;
			NGUITools.AddWidgetCollider(this.mBackground.cachedGameObject);
			this.mBackground.gameObject.AddComponent<UIDragScrollView>().scrollView = uiscrollView;
			uiscrollView.panel.SetAnchor(this.mBackground.cachedGameObject, 0, 0, 0, 0);
			Vector4 border = this.mBackground.border;
			this.mBgBorder = border.y;
			this.mBackground.cachedTransform.localPosition = new Vector3(0f, border.y, 0f);
			this.mHighlight = NGUITools.AddSprite(uiscrollView.gameObject, this.atlas, this.highlightSprite);
			this.mHighlight.pivot = UIWidget.Pivot.TopLeft;
			this.mHighlight.color = this.highlightColor;
			UISpriteData atlasSprite = this.mHighlight.GetAtlasSprite();
			if (atlasSprite == null)
			{
				return;
			}
			float num = (float)atlasSprite.borderTop;
			float num2 = (float)this.activeFontSize;
			float activeFontScale = this.activeFontScale;
			float num3 = num2 * activeFontScale;
			float num4 = 0f;
			float num5 = -this.padding.y;
			List<UILabel> list = new List<UILabel>();
			if (!this.items.Contains(this.mSelectedItem))
			{
				this.mSelectedItem = null;
			}
			int i = 0;
			int count = this.items.Count;
			while (i < count)
			{
				string text = this.items[i];
				UILabel uilabel = NGUITools.AddWidget<UILabel>(uiscrollView.gameObject, this.mBackground.depth + 2);
				uilabel.name = i.ToString();
				uilabel.pivot = UIWidget.Pivot.TopLeft;
				uilabel.bitmapFont = this.bitmapFont;
				uilabel.trueTypeFont = this.trueTypeFont;
				uilabel.fontSize = this.fontSize;
				uilabel.fontStyle = this.fontStyle;
				uilabel.text = ((!this.isLocalized) ? text : Localization.Get(text));
				uilabel.color = this.textColor;
				uilabel.cachedTransform.localPosition = new Vector3(border.x + this.padding.x - uilabel.pivotOffset.x, num5, -1f);
				uilabel.overflowMethod = UILabel.Overflow.ResizeFreely;
				uilabel.alignment = this.alignment;
				list.Add(uilabel);
				uilabel.gameObject.AddComponent<UIDragScrollView>().scrollView = uiscrollView;
				num5 -= num3;
				num5 -= this.padding.y;
				num4 = Mathf.Max(num4, uilabel.printedSize.x);
				UIEventListener uieventListener = UIEventListener.Get(uilabel.gameObject);
				uieventListener.onHover = new UIEventListener.BoolDelegate(this.OnItemHover);
				uieventListener.onClick = new UIEventListener.VoidDelegate(this.OnItemClick);
				uieventListener.parameter = text;
				if (this.mSelectedItem == text || (i == 0 && string.IsNullOrEmpty(this.mSelectedItem)))
				{
					this.Highlight(uilabel, true);
				}
				this.mLabelList.Add(uilabel);
				i++;
			}
			num4 = Mathf.Max(num4, v.x - vector.x - (border.x + this.padding.x) * 2f);
			float num6 = num4;
			Vector3 vector2 = new Vector3(num6 * 0.5f, -num3 * 0.5f, 0f);
			Vector3 vector3 = new Vector3(num6, num3 + this.padding.y, 1f);
			int j = 0;
			int count2 = list.Count;
			while (j < count2)
			{
				UILabel uilabel2 = list[j];
				NGUITools.AddWidgetCollider(uilabel2.gameObject);
				uilabel2.autoResizeBoxCollider = false;
				BoxCollider component = uilabel2.GetComponent<BoxCollider>();
				if (component != null)
				{
					vector2.z = component.center.z;
					component.center = vector2;
					component.size = vector3;
				}
				else
				{
					BoxCollider2D component2 = uilabel2.GetComponent<BoxCollider2D>();
					component2.offset = vector2;
					component2.size = vector3;
				}
				j++;
			}
			int width = Mathf.RoundToInt(num4);
			num4 += (border.x + this.padding.x) * 2f;
			num5 -= border.y;
			this.mBackground.width = Mathf.RoundToInt(num4);
			this.mBackground.height = Mathf.RoundToInt(-num5 + border.y);
			if (this.minHeight > 0)
			{
				this.mBackground.height = Mathf.Min(this.mBackground.height, this.minHeight);
			}
			int k = 0;
			int count3 = list.Count;
			while (k < count3)
			{
				UILabel uilabel3 = list[k];
				uilabel3.overflowMethod = UILabel.Overflow.ShrinkContent;
				uilabel3.width = width;
				k++;
			}
			float num7 = 2f * this.atlas.pixelSize;
			float f = num4 - (border.x + this.padding.x) * 2f + (float)atlasSprite.borderLeft * num7;
			float f2 = num3 + num * num7;
			this.mHighlight.width = Mathf.RoundToInt(f);
			this.mHighlight.height = Mathf.RoundToInt(f2);
			bool flag = this.position == UIPopupList.Position.Above;
			if (this.position == UIPopupList.Position.Auto)
			{
				UICamera uicamera = UICamera.FindCameraForLayer(this.mSelection.layer);
				if (uicamera != null)
				{
					flag = (uicamera.cachedCamera.WorldToViewportPoint(localPosition).y < 0.5f);
				}
			}
			if (this.isAnimated)
			{
				this.AnimateColor(this.mBackground);
				if (Time.timeScale == 0f || Time.timeScale >= 0.1f)
				{
					float bottom = num5 + num3;
					this.Animate(this.mHighlight, flag, bottom);
					int l = 0;
					int count4 = list.Count;
					while (l < count4)
					{
						this.Animate(list[l], flag, bottom);
						l++;
					}
					this.AnimateScale(this.mBackground, flag, bottom);
				}
			}
			if (flag)
			{
				vector.y = v.y - border.y;
				v.y = vector.y + (float)this.mBackground.height;
				v.x = vector.x + (float)this.mBackground.width;
				transform.localPosition = new Vector3(vector.x, v.y - border.y, vector.z);
			}
			else
			{
				v.y = vector.y + border.y;
				vector.y = v.y - (float)this.mBackground.height;
				v.x = vector.x + (float)this.mBackground.width;
			}
			Transform parent = this.mPanel.cachedTransform.parent;
			if (parent != null)
			{
				vector = this.mPanel.cachedTransform.TransformPoint(vector);
				v = this.mPanel.cachedTransform.TransformPoint(v);
				vector = parent.InverseTransformPoint(vector);
				v = parent.InverseTransformPoint(v);
			}
			Vector3 b = (!this.mPanel.hasClipping) ? this.mPanel.CalculateConstrainOffset(vector, v) : Vector3.zero;
			localPosition = transform.localPosition + b;
			localPosition.x = Mathf.Round(localPosition.x);
			localPosition.y = Mathf.Round(localPosition.y);
			transform.localPosition = localPosition;
		}
		else
		{
			this.OnSelect(false);
		}
	}

	// Token: 0x0400016C RID: 364
	private const float animSpeed = 0.15f;

	// Token: 0x0400016D RID: 365
	public int minHeight;

	// Token: 0x0400016E RID: 366
	public static UIPopupList current;

	// Token: 0x0400016F RID: 367
	private static GameObject mChild;

	// Token: 0x04000170 RID: 368
	private static float mFadeOutComplete;

	// Token: 0x04000171 RID: 369
	public UIAtlas atlas;

	// Token: 0x04000172 RID: 370
	public UIFont bitmapFont;

	// Token: 0x04000173 RID: 371
	public Font trueTypeFont;

	// Token: 0x04000174 RID: 372
	public int fontSize = 16;

	// Token: 0x04000175 RID: 373
	public FontStyle fontStyle;

	// Token: 0x04000176 RID: 374
	public string backgroundSprite;

	// Token: 0x04000177 RID: 375
	public string highlightSprite;

	// Token: 0x04000178 RID: 376
	public UIPopupList.Position position;

	// Token: 0x04000179 RID: 377
	public NGUIText.Alignment alignment = NGUIText.Alignment.Left;

	// Token: 0x0400017A RID: 378
	public List<string> items = new List<string>();

	// Token: 0x0400017B RID: 379
	public List<object> itemData = new List<object>();

	// Token: 0x0400017C RID: 380
	public Vector2 padding = new Vector3(4f, 4f);

	// Token: 0x0400017D RID: 381
	public Color textColor = Color.white;

	// Token: 0x0400017E RID: 382
	public Color backgroundColor = Color.white;

	// Token: 0x0400017F RID: 383
	public Color highlightColor = new Color(0.882352948f, 0.784313738f, 0.5882353f, 1f);

	// Token: 0x04000180 RID: 384
	public bool isAnimated = true;

	// Token: 0x04000181 RID: 385
	public bool isLocalized;

	// Token: 0x04000182 RID: 386
	public bool separatePanel = true;

	// Token: 0x04000183 RID: 387
	public UIPopupList.OpenOn openOn;

	// Token: 0x04000184 RID: 388
	public List<EventDelegate> onChange = new List<EventDelegate>();

	// Token: 0x04000185 RID: 389
	[SerializeField]
	[HideInInspector]
	protected string mSelectedItem;

	// Token: 0x04000186 RID: 390
	[SerializeField]
	[HideInInspector]
	protected UIPanel mPanel;

	// Token: 0x04000187 RID: 391
	[SerializeField]
	[HideInInspector]
	protected UISprite mBackground;

	// Token: 0x04000188 RID: 392
	[SerializeField]
	[HideInInspector]
	protected UISprite mHighlight;

	// Token: 0x04000189 RID: 393
	[SerializeField]
	[HideInInspector]
	protected UILabel mHighlightedLabel;

	// Token: 0x0400018A RID: 394
	[SerializeField]
	[HideInInspector]
	protected List<UILabel> mLabelList = new List<UILabel>();

	// Token: 0x0400018B RID: 395
	[HideInInspector]
	[SerializeField]
	protected float mBgBorder;

	// Token: 0x0400018C RID: 396
	[NonSerialized]
	protected GameObject mSelection;

	// Token: 0x0400018D RID: 397
	[NonSerialized]
	protected int mOpenFrame;

	// Token: 0x0400018E RID: 398
	[SerializeField]
	[HideInInspector]
	private GameObject eventReceiver;

	// Token: 0x0400018F RID: 399
	[HideInInspector]
	[SerializeField]
	private string functionName = "OnSelectionChange";

	// Token: 0x04000190 RID: 400
	[HideInInspector]
	[SerializeField]
	private float textScale;

	// Token: 0x04000191 RID: 401
	[HideInInspector]
	[SerializeField]
	private UIFont font;

	// Token: 0x04000192 RID: 402
	[SerializeField]
	[HideInInspector]
	private UILabel textLabel;

	// Token: 0x04000193 RID: 403
	private UIPopupList.LegacyEvent mLegacyEvent;

	// Token: 0x04000194 RID: 404
	[NonSerialized]
	protected bool mExecuting;

	// Token: 0x04000195 RID: 405
	protected bool mUseDynamicFont;

	// Token: 0x04000196 RID: 406
	protected bool mTweening;

	// Token: 0x04000197 RID: 407
	public GameObject source;

	// Token: 0x02000042 RID: 66
	public enum Position
	{
		// Token: 0x04000199 RID: 409
		Auto,
		// Token: 0x0400019A RID: 410
		Above,
		// Token: 0x0400019B RID: 411
		Below
	}

	// Token: 0x02000043 RID: 67
	public enum OpenOn
	{
		// Token: 0x0400019D RID: 413
		ClickOrTap,
		// Token: 0x0400019E RID: 414
		RightClick,
		// Token: 0x0400019F RID: 415
		DoubleClick,
		// Token: 0x040001A0 RID: 416
		Manual
	}

	// Token: 0x02000044 RID: 68
	// (Invoke) Token: 0x06000249 RID: 585
	public delegate void LegacyEvent(string val);
}
