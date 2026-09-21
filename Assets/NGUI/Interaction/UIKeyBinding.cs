using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000038 RID: 56
[AddComponentMenu("NGUI/Interaction/Key Binding")]
public class UIKeyBinding : MonoBehaviour
{
	// Token: 0x060001D3 RID: 467 RVA: 0x00023E70 File Offset: 0x00022070
	public static bool IsBound(KeyCode key)
	{
		int i = 0;
		int count = UIKeyBinding.mList.Count;
		while (i < count)
		{
			UIKeyBinding uikeyBinding = UIKeyBinding.mList[i];
			if (uikeyBinding != null && uikeyBinding.keyCode == key)
			{
				return true;
			}
			i++;
		}
		return false;
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x00003D4F File Offset: 0x00001F4F
	protected virtual void OnEnable()
	{
		UIKeyBinding.mList.Add(this);
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x00003D5C File Offset: 0x00001F5C
	protected virtual void OnDisable()
	{
		UIKeyBinding.mList.Remove(this);
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x00023EB8 File Offset: 0x000220B8
	protected virtual void Start()
	{
		UIInput component = base.GetComponent<UIInput>();
		this.mIsInput = (component != null);
		if (component != null)
		{
			EventDelegate.Add(component.onSubmit, new EventDelegate.Callback(this.OnSubmit));
		}
	}

	// Token: 0x060001D7 RID: 471 RVA: 0x00003D6A File Offset: 0x00001F6A
	protected virtual void OnSubmit()
	{
		if (UICamera.currentKey == this.keyCode && this.IsModifierActive())
		{
			this.mIgnoreUp = true;
		}
	}

	// Token: 0x060001D8 RID: 472 RVA: 0x00023EFC File Offset: 0x000220FC
	protected virtual bool IsModifierActive()
	{
		if (this.modifier == UIKeyBinding.Modifier.Any)
		{
			return true;
		}
		if (this.modifier == UIKeyBinding.Modifier.Alt)
		{
			if (UICamera.GetKey(KeyCode.LeftAlt) || UICamera.GetKey(KeyCode.RightAlt))
			{
				return true;
			}
		}
		else if (this.modifier == UIKeyBinding.Modifier.Control)
		{
			if (UICamera.GetKey(KeyCode.LeftControl) || UICamera.GetKey(KeyCode.RightControl))
			{
				return true;
			}
		}
		else if (this.modifier == UIKeyBinding.Modifier.Shift)
		{
			if (UICamera.GetKey(KeyCode.LeftShift) || UICamera.GetKey(KeyCode.RightShift))
			{
				return true;
			}
		}
		else if (this.modifier == UIKeyBinding.Modifier.None)
		{
			return !UICamera.GetKey(KeyCode.LeftAlt) && !UICamera.GetKey(KeyCode.RightAlt) && !UICamera.GetKey(KeyCode.LeftControl) && !UICamera.GetKey(KeyCode.RightControl) && !UICamera.GetKey(KeyCode.LeftShift) && !UICamera.GetKey(KeyCode.RightShift);
		}
		return false;
	}

	// Token: 0x060001D9 RID: 473 RVA: 0x00024018 File Offset: 0x00022218
	protected virtual void Update()
	{
		if (UICamera.inputHasFocus)
		{
			return;
		}
		if (this.keyCode != KeyCode.None && this.IsModifierActive())
		{
			bool flag = UICamera.GetKeyDown(this.keyCode);
			bool flag2 = UICamera.GetKeyUp(this.keyCode);
			if (flag)
			{
				this.mPress = true;
			}
			if (this.action == UIKeyBinding.Action.PressAndClick || this.action == UIKeyBinding.Action.All)
			{
				if (flag)
				{
					UICamera.currentKey = this.keyCode;
					this.OnBindingPress(true);
				}
				if (this.mPress && flag2)
				{
					UICamera.currentKey = this.keyCode;
					this.OnBindingPress(false);
					this.OnBindingClick();
				}
			}
			if ((this.action == UIKeyBinding.Action.Select || this.action == UIKeyBinding.Action.All) && flag2)
			{
				if (this.mIsInput)
				{
					if (!this.mIgnoreUp && !UICamera.inputHasFocus && this.mPress)
					{
						UICamera.selectedObject = base.gameObject;
					}
					this.mIgnoreUp = false;
				}
				else if (this.mPress)
				{
					UICamera.hoveredObject = base.gameObject;
				}
			}
			if (flag2)
			{
				this.mPress = false;
			}
			return;
		}
	}

	// Token: 0x060001DA RID: 474 RVA: 0x00003D88 File Offset: 0x00001F88
	protected virtual void OnBindingPress(bool pressed)
	{
		UICamera.Notify(base.gameObject, "OnPress", pressed);
	}

	// Token: 0x060001DB RID: 475 RVA: 0x00003DA0 File Offset: 0x00001FA0
	protected virtual void OnBindingClick()
	{
		UICamera.Notify(base.gameObject, "OnClick", null);
	}

	// Token: 0x0400011C RID: 284
	private static List<UIKeyBinding> mList = new List<UIKeyBinding>();

	// Token: 0x0400011D RID: 285
	public KeyCode keyCode;

	// Token: 0x0400011E RID: 286
	public UIKeyBinding.Modifier modifier;

	// Token: 0x0400011F RID: 287
	public UIKeyBinding.Action action;

	// Token: 0x04000120 RID: 288
	[NonSerialized]
	private bool mIgnoreUp;

	// Token: 0x04000121 RID: 289
	[NonSerialized]
	private bool mIsInput;

	// Token: 0x04000122 RID: 290
	[NonSerialized]
	private bool mPress;

	// Token: 0x02000039 RID: 57
	public enum Action
	{
		// Token: 0x04000124 RID: 292
		PressAndClick,
		// Token: 0x04000125 RID: 293
		Select,
		// Token: 0x04000126 RID: 294
		All
	}

	// Token: 0x0200003A RID: 58
	public enum Modifier
	{
		// Token: 0x04000128 RID: 296
		Any,
		// Token: 0x04000129 RID: 297
		Shift,
		// Token: 0x0400012A RID: 298
		Control,
		// Token: 0x0400012B RID: 299
		Alt,
		// Token: 0x0400012C RID: 300
		None
	}
}
