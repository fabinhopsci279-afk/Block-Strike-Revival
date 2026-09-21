using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000052 RID: 82
[ExecuteInEditMode]
[AddComponentMenu("NGUI/Interaction/Scrollable Popup List")]
public class UIScrollablePopupList : UIWidgetContainer
{
	// Token: 0x17000037 RID: 55
	// (get) Token: 0x060002AE RID: 686 RVA: 0x00004717 File Offset: 0x00002917
	// (set) Token: 0x060002AF RID: 687 RVA: 0x000290E8 File Offset: 0x000272E8
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

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x060002B0 RID: 688 RVA: 0x00004749 File Offset: 0x00002949
	// (set) Token: 0x060002B1 RID: 689 RVA: 0x00004751 File Offset: 0x00002951
	[Obsolete("Use EventDelegate.Add(popup.onChange, YourCallback) instead, and UIPopupList.current.value to determine the state")]
	public UIScrollablePopupList.LegacyEvent onSelectionChange
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

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x060002B2 RID: 690 RVA: 0x0000475A File Offset: 0x0000295A
	public bool isOpen
	{
		get
		{
			return this.mChild != null;
		}
	}

	// Token: 0x1700003A RID: 58
	// (get) Token: 0x060002B3 RID: 691 RVA: 0x00004768 File Offset: 0x00002968
	// (set) Token: 0x060002B4 RID: 692 RVA: 0x00004770 File Offset: 0x00002970
	public string value
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

	// Token: 0x1700003B RID: 59
	// (get) Token: 0x060002B5 RID: 693 RVA: 0x00004790 File Offset: 0x00002990
	// (set) Token: 0x060002B6 RID: 694 RVA: 0x00004798 File Offset: 0x00002998
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

	// Token: 0x1700003C RID: 60
	// (get) Token: 0x060002B7 RID: 695 RVA: 0x0002913C File Offset: 0x0002733C
	// (set) Token: 0x060002B8 RID: 696 RVA: 0x00029168 File Offset: 0x00027368
	private bool handleEvents
	{
		get
		{
			UIKeyNavigation component = base.GetComponent<UIKeyNavigation>();
			return component == null || !component.enabled;
		}
		set
		{
			UIKeyNavigation component = base.GetComponent<UIKeyNavigation>();
			if (component != null)
			{
				component.enabled = !value;
			}
		}
	}

	// Token: 0x1700003D RID: 61
	// (get) Token: 0x060002B9 RID: 697 RVA: 0x000047A1 File Offset: 0x000029A1
	private bool isValid
	{
		get
		{
			return this.bitmapFont != null || this.trueTypeFont != null;
		}
	}

	// Token: 0x1700003E RID: 62
	// (get) Token: 0x060002BA RID: 698 RVA: 0x000047C0 File Offset: 0x000029C0
	private int activeFontSize
	{
		get
		{
			return (this.trueTypeFont != null || this.bitmapFont == null) ? this.fontSize : this.bitmapFont.defaultSize;
		}
	}

	// Token: 0x1700003F RID: 63
	// (get) Token: 0x060002BB RID: 699 RVA: 0x000047F1 File Offset: 0x000029F1
	private float activeFontScale
	{
		get
		{
			return (this.trueTypeFont != null || this.bitmapFont == null) ? 1f : ((float)this.fontSize / (float)this.bitmapFont.defaultSize);
		}
	}

	// Token: 0x060002BC RID: 700 RVA: 0x00029190 File Offset: 0x00027390
	protected void TriggerCallbacks()
	{
		if (UIScrollablePopupList.current != this)
		{
			UIScrollablePopupList uiscrollablePopupList = UIScrollablePopupList.current;
			UIScrollablePopupList.current = this;
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
			UIScrollablePopupList.current = uiscrollablePopupList;
		}
	}

	// Token: 0x060002BD RID: 701 RVA: 0x00029224 File Offset: 0x00027424
	private void OnEnable()
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

	// Token: 0x060002BE RID: 702 RVA: 0x00029344 File Offset: 0x00027544
	private void OnValidate()
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

	// Token: 0x060002BF RID: 703 RVA: 0x000293FC File Offset: 0x000275FC
	private void Start()
	{
		if (this.textLabel != null)
		{
			EventDelegate.Add(this.onChange, new EventDelegate.Callback(this.textLabel.SetCurrentSelection));
			this.textLabel = null;
		}
		if (Application.isPlaying)
		{
			if (string.IsNullOrEmpty(this.mSelectedItem))
			{
				if (this.items.Count > 0)
				{
					this.value = this.items[0];
				}
			}
			else
			{
				string value = this.mSelectedItem;
				this.mSelectedItem = null;
				this.value = value;
			}
		}
	}

	// Token: 0x060002C0 RID: 704 RVA: 0x0000482A File Offset: 0x00002A2A
	private void OnLocalize()
	{
		if (this.isLocalized)
		{
			this.TriggerCallbacks();
		}
	}

	// Token: 0x060002C1 RID: 705 RVA: 0x00029488 File Offset: 0x00027688
	private void Highlight(UILabel lbl, bool instant)
	{
		if (this.mHighlight != null)
		{
			TweenPosition component = lbl.GetComponent<TweenPosition>();
			if (component != null && component.enabled)
			{
				return;
			}
			this.mHighlightedLabel = lbl;
			UISpriteData atlasSprite = this.mHighlight.GetAtlasSprite();
			if (atlasSprite == null)
			{
				return;
			}
			float pixelSize = this.atlas.pixelSize;
			float num = (float)atlasSprite.borderLeft * pixelSize;
			float y = (float)atlasSprite.borderTop * pixelSize;
			Vector3 vector = lbl.cachedTransform.localPosition + new Vector3(-num, y, 1f);
			if (!instant && this.isAnimated)
			{
				TweenPosition.Begin(this.mHighlight.gameObject, 0.1f, vector).method = UITweener.Method.EaseOut;
			}
			else
			{
				this.mHighlight.cachedTransform.localPosition = vector;
			}
		}
	}

	// Token: 0x060002C2 RID: 706 RVA: 0x00029554 File Offset: 0x00027754
	private void OnItemHover(GameObject go, bool isOver)
	{
		if (isOver)
		{
			UILabel component = go.GetComponent<UILabel>();
			this.Highlight(component, false);
		}
	}

	// Token: 0x060002C3 RID: 707 RVA: 0x00029574 File Offset: 0x00027774
	private void Select(UILabel lbl, bool instant)
	{
		this.Highlight(lbl, instant);
		UIEventListener component = lbl.gameObject.GetComponent<UIEventListener>();
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
		this.Close();
	}

	// Token: 0x060002C4 RID: 708 RVA: 0x0000483A File Offset: 0x00002A3A
	private void OnItemPress(GameObject go, bool isPressed)
	{
		if (isPressed)
		{
			this.Select(go.GetComponent<UILabel>(), true);
		}
	}

	// Token: 0x060002C5 RID: 709 RVA: 0x0000484C File Offset: 0x00002A4C
	private void OnItemClick(GameObject go)
	{
		this.Select(go.GetComponent<UILabel>(), true);
	}

	// Token: 0x060002C6 RID: 710 RVA: 0x000295EC File Offset: 0x000277EC
	private void OnKey(KeyCode key)
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.handleEvents)
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
			else if (key == KeyCode.DownArrow)
			{
				if (num + 1 < this.mLabelList.Count)
				{
					this.Select(this.mLabelList[num + 1], false);
				}
			}
			else if (key == KeyCode.Escape)
			{
				this.OnSelect(false);
			}
		}
	}

	// Token: 0x060002C7 RID: 711 RVA: 0x0000485B File Offset: 0x00002A5B
	private void OnSelect(bool isSelected)
	{
		if (!isSelected)
		{
			this.Close();
		}
	}

	// Token: 0x060002C8 RID: 712 RVA: 0x00029698 File Offset: 0x00027898
	public void Close()
	{
		if (this.mChild != null)
		{
			if (UICamera.hoveredObject && (UICamera.hoveredObject == this.scrollbarSprite.cachedGameObject || UICamera.hoveredObject == this.scrForegroundSprite.cachedGameObject || UICamera.hoveredObject == this.mBackground.cachedGameObject))
			{
				return;
			}
			this.mLabelList.Clear();
			this.handleEvents = false;
			if (this.isAnimated)
			{
				UIWidget[] componentsInChildren = this.mChild.GetComponentsInChildren<UIWidget>();
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
				Collider[] componentsInChildren2 = this.mChild.GetComponentsInChildren<Collider>();
				int j = 0;
				int num2 = componentsInChildren2.Length;
				while (j < num2)
				{
					componentsInChildren2[j].enabled = false;
					j++;
				}
				UnityEngine.Object.Destroy(this.mChild, 0.15f);
			}
			else
			{
				UnityEngine.Object.Destroy(this.mChild);
			}
			this.mBackground = null;
			this.mHighlight = null;
			this.mChild = null;
			this.mClippingPanel = null;
			this.scrollbarSprite = null;
			this.scrForegroundSprite = null;
		}
	}

	// Token: 0x060002C9 RID: 713 RVA: 0x00025970 File Offset: 0x00023B70
	private void AnimateColor(UIWidget widget)
	{
		Color color = widget.color;
		widget.color = new Color(color.r, color.g, color.b, 0f);
		TweenColor.Begin(widget.gameObject, 0.15f, color).method = UITweener.Method.EaseOut;
	}

	// Token: 0x060002CA RID: 714 RVA: 0x000259C0 File Offset: 0x00023BC0
	private void AnimatePosition(UIWidget widget, bool placeAbove, float bottom)
	{
		Vector3 localPosition = widget.cachedTransform.localPosition;
		Vector3 localPosition2 = (!placeAbove) ? new Vector3(localPosition.x, 0f, localPosition.z) : new Vector3(localPosition.x, bottom, localPosition.z);
		widget.cachedTransform.localPosition = localPosition2;
		GameObject gameObject = widget.gameObject;
		TweenPosition.Begin(gameObject, 0.15f, localPosition).method = UITweener.Method.EaseOut;
	}

	// Token: 0x060002CB RID: 715 RVA: 0x000297E8 File Offset: 0x000279E8
	private void AnimateScale(UIWidget widget, bool placeAbove, float bottom)
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

	// Token: 0x060002CC RID: 716 RVA: 0x00004866 File Offset: 0x00002A66
	private void Animate(UIWidget widget, bool placeAbove, float bottom)
	{
		this.AnimateColor(widget);
		this.AnimatePosition(widget, placeAbove, bottom);
	}

	// Token: 0x060002CD RID: 717 RVA: 0x0002989C File Offset: 0x00027A9C
	private void OnClick()
	{
		if (base.enabled && NGUITools.GetActive(base.gameObject) && this.mChild == null && this.atlas != null && this.isValid && this.items.Count > 0)
		{
			this.mLabelList.Clear();
			if (this.mPanel == null)
			{
				this.mPanel = UIPanel.Find(base.transform);
				if (this.mPanel == null)
				{
					return;
				}
			}
			this.handleEvents = true;
			Transform transform = base.transform;
			Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(transform.parent, transform);
			this.mChild = new GameObject("Drop-down List");
			this.mChild.layer = base.gameObject.layer;
			Transform transform2 = this.mChild.transform;
			transform2.parent = transform.parent;
			transform2.localPosition = bounds.min;
			transform2.localRotation = Quaternion.identity;
			transform2.localScale = Vector3.one;
			this.mBackground = NGUITools.AddSprite(this.mChild, this.atlas, this.backgroundSprite);
			this.mBackground.pivot = UIWidget.Pivot.TopLeft;
			this.mBackground.depth = NGUITools.CalculateNextDepth(this.mPanel.gameObject);
			this.mBackground.color = this.backgroundColor;
			this.mBackground.gameObject.name = "SpriteBackground";
			NGUITools.AddWidgetCollider(this.mBackground.cachedGameObject);
			UIDragScrollView uidragScrollView = this.mBackground.gameObject.AddComponent<UIDragScrollView>();
			GameObject gameObject = new GameObject("Panel");
			gameObject.layer = base.gameObject.layer;
			Transform transform3 = gameObject.transform;
			transform3.parent = this.mChild.transform;
			transform3.localPosition = Vector3.zero;
			transform3.localRotation = Quaternion.identity;
			transform3.localScale = Vector3.one;
			this.mClippingPanel = gameObject.AddComponent<UIPanel>();
			this.mClippingPanel.clipping = UIDrawCall.Clipping.SoftClip;
			this.mClippingPanel.leftAnchor.target = this.mBackground.transform;
			this.mClippingPanel.rightAnchor.target = this.mBackground.transform;
			this.mClippingPanel.topAnchor.target = this.mBackground.transform;
			this.mClippingPanel.bottomAnchor.target = this.mBackground.transform;
			this.mClippingPanel.ResetAnchors();
			this.mClippingPanel.UpdateAnchors();
			this.mClippingPanel.depth = NGUITools.CalculateNextDepth(this.mPanel.gameObject);
			UIScrollView uiscrollView = gameObject.AddComponent<UIScrollView>();
			uiscrollView.contentPivot = UIWidget.Pivot.TopLeft;
			uiscrollView.movement = UIScrollView.Movement.Vertical;
			uiscrollView.scrollWheelFactor = 0.25f;
			uiscrollView.disableDragIfFits = true;
			uiscrollView.dragEffect = UIScrollView.DragEffect.None;
			uiscrollView.ResetPosition();
			uiscrollView.UpdatePosition();
			uiscrollView.RestrictWithinBounds(true);
			uidragScrollView.scrollView = uiscrollView;
			Vector4 border = this.mBackground.border;
			this.mBgBorder = border.y;
			this.mBackground.cachedTransform.localPosition = new Vector3(0f, border.y, 0f);
			this.mHighlight = NGUITools.AddSprite(gameObject, this.atlas, this.highlightSprite);
			this.mHighlight.pivot = UIWidget.Pivot.TopLeft;
			this.mHighlight.color = this.highlightColor;
			this.mHighlight.gameObject.name = "Highlighter";
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
			int num6 = (!(this.bitmapFont != null)) ? this.fontSize : this.bitmapFont.defaultSize;
			List<UILabel> list = new List<UILabel>();
			int i = 0;
			int count = this.items.Count;
			while (i < count)
			{
				string text = this.items[i];
				UILabel uilabel = NGUITools.AddWidget<UILabel>(gameObject, int.MaxValue);
				uilabel.pivot = UIWidget.Pivot.TopLeft;
				uilabel.bitmapFont = this.bitmapFont;
				uilabel.trueTypeFont = this.trueTypeFont;
				uilabel.fontSize = num6;
				uilabel.fontStyle = this.fontStyle;
				uilabel.text = ((!this.isLocalized) ? text : Localization.Get(text));
				uilabel.color = this.textColor;
				uilabel.cachedTransform.localPosition = new Vector3(border.x + this.padding.x, num5, -1f);
				uilabel.overflowMethod = UILabel.Overflow.ResizeFreely;
				uilabel.MakePixelPerfect();
				uilabel.gameObject.AddComponent<UIDragScrollView>();
				if (activeFontScale != 1f)
				{
					uilabel.cachedTransform.localScale = Vector3.one * activeFontScale;
				}
				list.Add(uilabel);
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
			num4 = Mathf.Max(num4, bounds.size.x * activeFontScale - (border.x + this.padding.x) * 2f);
			float num7 = num4 / activeFontScale;
			Vector3 center = new Vector3(num7 * 0.5f, -num2 * 0.5f, 0f);
			Vector3 size = new Vector3(num7, (num3 + this.padding.y) / activeFontScale, 1f);
			int j = 0;
			int count2 = list.Count;
			while (j < count2)
			{
				UILabel uilabel2 = list[j];
				NGUITools.AddWidgetCollider(uilabel2.gameObject);
				BoxCollider component = uilabel2.GetComponent<BoxCollider>();
				center.z = component.center.z;
				component.center = center;
				component.size = size;
				j++;
			}
			num4 += (border.x + this.padding.x) * 2f;
			num5 -= border.y;
			this.mBackground.width = Mathf.RoundToInt(num4);
			int num8 = Mathf.RoundToInt(-num5 + border.y);
			if (this.maxHeight == 0)
			{
				this.maxHeight = num8;
			}
			this.mBackground.height = ((num8 <= this.maxHeight) ? num8 : this.maxHeight);
			uiscrollView.ResetPosition();
			uiscrollView.UpdatePosition();
			uiscrollView.RestrictWithinBounds(true, true, true);
			transform3.localPosition = Vector3.zero;
			float num9 = 2f * this.atlas.pixelSize;
			float f = num4 - (border.x + this.padding.x) * 2f + (float)atlasSprite.borderLeft * num9;
			float f2 = num3 + num * num9;
			this.mHighlight.width = Mathf.RoundToInt(f) - 5;
			this.mHighlight.height = Mathf.RoundToInt(f2);
			bool flag = this.position == UIScrollablePopupList.Position.Above;
			if (this.position == UIScrollablePopupList.Position.Auto)
			{
				UICamera uicamera = UICamera.FindCameraForLayer(base.gameObject.layer);
				if (uicamera != null)
				{
					flag = (uicamera.cachedCamera.WorldToViewportPoint(transform.position).y < 0.5f);
				}
			}
			if (this.isAnimated)
			{
				float bottom = num5 + num3;
				this.Animate(this.mHighlight, flag, bottom);
				int k = 0;
				int count3 = list.Count;
				while (k < count3)
				{
					this.Animate(list[k], flag, bottom);
					k++;
				}
				this.AnimateColor(this.mBackground);
				this.AnimateScale(this.mBackground, flag, bottom);
			}
			if (flag)
			{
				transform2.localPosition = new Vector3(bounds.min.x, bounds.min.y + (float)this.mBackground.height + (float)transform.GetComponent<UISprite>().height - border.y, bounds.min.z);
			}
			this.scrollbarSprite = NGUITools.AddSprite(this.mBackground.gameObject, this.scrollbarAtlas, this.scrollbarSpriteName);
			this.scrollbarSprite.depth = NGUITools.CalculateNextDepth(this.mPanel.gameObject);
			this.scrollbarSprite.color = this.scrollbarBgDefColour;
			this.scrollbarSprite.gameObject.name = "Scrollbar";
			UIButtonColor uibuttonColor = this.scrollbarSprite.gameObject.AddComponent<UIButtonColor>();
			uibuttonColor.defaultColor = this.scrollbarSprite.color;
			uibuttonColor.hover = this.scrollbarBgHovColour;
			uibuttonColor.pressed = this.scrollbarBgPrsColour;
			NGUITools.AddWidgetCollider(this.scrollbarSprite.gameObject);
			this.scrollbarSprite.leftAnchor.target = this.mBackground.transform;
			this.scrollbarSprite.leftAnchor.relative = 1f;
			this.scrollbarSprite.leftAnchor.absolute = -11;
			this.scrollbarSprite.rightAnchor.target = this.mBackground.transform;
			this.scrollbarSprite.rightAnchor.relative = 1f;
			this.scrollbarSprite.rightAnchor.absolute = -1;
			this.scrollbarSprite.topAnchor.target = this.mBackground.transform;
			this.scrollbarSprite.topAnchor.relative = 1f;
			this.scrollbarSprite.topAnchor.absolute = 0;
			this.scrollbarSprite.bottomAnchor.target = this.mBackground.transform;
			this.scrollbarSprite.bottomAnchor.relative = 0f;
			this.scrollbarSprite.bottomAnchor.absolute = 0;
			this.scrollbarSprite.ResetAnchors();
			this.scrollbarSprite.UpdateAnchors();
			this.scrForegroundSprite = NGUITools.AddSprite(this.scrollbarSprite.gameObject, this.scrollbarAtlas, this.scrollbarForegroundName);
			this.scrForegroundSprite.depth = NGUITools.CalculateNextDepth(this.mPanel.gameObject);
			this.scrForegroundSprite.color = this.scrollbarFgDefColour;
			this.scrForegroundSprite.gameObject.name = "Foreground";
			UIButtonColor uibuttonColor2 = this.scrForegroundSprite.gameObject.AddComponent<UIButtonColor>();
			uibuttonColor2.defaultColor = this.scrForegroundSprite.color;
			uibuttonColor2.hover = this.scrollbarFgHovColour;
			uibuttonColor2.pressed = this.scrollbarFgPrsColour;
			NGUITools.AddWidgetCollider(this.scrForegroundSprite.gameObject);
			this.scrForegroundSprite.leftAnchor.target = this.scrollbarSprite.transform;
			this.scrForegroundSprite.rightAnchor.target = this.scrollbarSprite.transform;
			this.scrForegroundSprite.topAnchor.target = this.scrollbarSprite.transform;
			this.scrForegroundSprite.bottomAnchor.target = this.scrollbarSprite.transform;
			this.scrForegroundSprite.ResetAnchors();
			this.scrForegroundSprite.UpdateAnchors();
			UIScrollBar uiscrollBar = this.scrollbarSprite.gameObject.AddComponent<UIScrollBar>();
			uiscrollBar.fillDirection = UIProgressBar.FillDirection.TopToBottom;
			uiscrollBar.backgroundWidget = this.scrollbarSprite;
			uiscrollBar.foregroundWidget = this.scrForegroundSprite;
			uiscrollView.verticalScrollBar = uiscrollBar;
			uiscrollView.ResetPosition();
			uiscrollBar.value = 0f;
		}
	}

	// Token: 0x040001F9 RID: 505
	private const float animSpeed = 0.15f;

	// Token: 0x040001FA RID: 506
	public static UIScrollablePopupList current;

	// Token: 0x040001FB RID: 507
	public UIAtlas atlas;

	// Token: 0x040001FC RID: 508
	public UIAtlas scrollbarAtlas;

	// Token: 0x040001FD RID: 509
	public string scrollbarSpriteName;

	// Token: 0x040001FE RID: 510
	public string scrollbarForegroundName;

	// Token: 0x040001FF RID: 511
	public UIFont bitmapFont;

	// Token: 0x04000200 RID: 512
	public Font trueTypeFont;

	// Token: 0x04000201 RID: 513
	public int fontSize = 16;

	// Token: 0x04000202 RID: 514
	public FontStyle fontStyle;

	// Token: 0x04000203 RID: 515
	public string backgroundSprite;

	// Token: 0x04000204 RID: 516
	public string highlightSprite;

	// Token: 0x04000205 RID: 517
	public UIScrollablePopupList.Position position;

	// Token: 0x04000206 RID: 518
	public List<string> items = new List<string>();

	// Token: 0x04000207 RID: 519
	public Vector2 padding = new Vector3(4f, 4f);

	// Token: 0x04000208 RID: 520
	public Color textColor = Color.white;

	// Token: 0x04000209 RID: 521
	public Color backgroundColor = Color.white;

	// Token: 0x0400020A RID: 522
	public Color highlightColor = new Color(0.882352948f, 0.784313738f, 0.5882353f, 1f);

	// Token: 0x0400020B RID: 523
	public Color scrollbarBgDefColour;

	// Token: 0x0400020C RID: 524
	public Color scrollbarBgHovColour;

	// Token: 0x0400020D RID: 525
	public Color scrollbarBgPrsColour;

	// Token: 0x0400020E RID: 526
	public Color scrollbarFgDefColour;

	// Token: 0x0400020F RID: 527
	public Color scrollbarFgHovColour;

	// Token: 0x04000210 RID: 528
	public Color scrollbarFgPrsColour;

	// Token: 0x04000211 RID: 529
	public bool isAnimated = true;

	// Token: 0x04000212 RID: 530
	public bool isLocalized;

	// Token: 0x04000213 RID: 531
	public int maxHeight = 100;

	// Token: 0x04000214 RID: 532
	public List<EventDelegate> onChange = new List<EventDelegate>();

	// Token: 0x04000215 RID: 533
	[HideInInspector]
	[SerializeField]
	private string mSelectedItem;

	// Token: 0x04000216 RID: 534
	private UIPanel mPanel;

	// Token: 0x04000217 RID: 535
	private GameObject mChild;

	// Token: 0x04000218 RID: 536
	private UISprite mBackground;

	// Token: 0x04000219 RID: 537
	private UISprite mHighlight;

	// Token: 0x0400021A RID: 538
	private UILabel mHighlightedLabel;

	// Token: 0x0400021B RID: 539
	private List<UILabel> mLabelList = new List<UILabel>();

	// Token: 0x0400021C RID: 540
	private float mBgBorder;

	// Token: 0x0400021D RID: 541
	private UIPanel mClippingPanel;

	// Token: 0x0400021E RID: 542
	private UISprite scrollbarSprite;

	// Token: 0x0400021F RID: 543
	private UISprite scrForegroundSprite;

	// Token: 0x04000220 RID: 544
	[SerializeField]
	[HideInInspector]
	private GameObject eventReceiver;

	// Token: 0x04000221 RID: 545
	[SerializeField]
	[HideInInspector]
	private string functionName = "OnSelectionChange";

	// Token: 0x04000222 RID: 546
	[SerializeField]
	[HideInInspector]
	private float textScale;

	// Token: 0x04000223 RID: 547
	[HideInInspector]
	[SerializeField]
	private UIFont font;

	// Token: 0x04000224 RID: 548
	[SerializeField]
	[HideInInspector]
	private UILabel textLabel;

	// Token: 0x04000225 RID: 549
	private UIScrollablePopupList.LegacyEvent mLegacyEvent;

	// Token: 0x04000226 RID: 550
	private bool mUseDynamicFont;

	// Token: 0x02000053 RID: 83
	public enum Position
	{
		// Token: 0x04000228 RID: 552
		Auto,
		// Token: 0x04000229 RID: 553
		Above,
		// Token: 0x0400022A RID: 554
		Below
	}

	// Token: 0x02000054 RID: 84
	// (Invoke) Token: 0x060002CF RID: 719
	public delegate void LegacyEvent(string val);
}
