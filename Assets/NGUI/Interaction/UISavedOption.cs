using System;
using UnityEngine;

// Token: 0x0200004A RID: 74
[AddComponentMenu("NGUI/Interaction/Saved Option")]
public class UISavedOption : MonoBehaviour
{
	// Token: 0x1700002B RID: 43
	// (get) Token: 0x06000276 RID: 630 RVA: 0x00004501 File Offset: 0x00002701
	private string key
	{
		get
		{
			return (!string.IsNullOrEmpty(this.keyName)) ? this.keyName : ("NGUI State: " + base.name);
		}
	}

	// Token: 0x06000277 RID: 631 RVA: 0x00004528 File Offset: 0x00002728
	private void Awake()
	{
		this.mList = base.GetComponent<UIPopupList>();
		this.mCheck = base.GetComponent<UIToggle>();
		this.mSlider = base.GetComponent<UIProgressBar>();
	}

	// Token: 0x06000278 RID: 632 RVA: 0x00027208 File Offset: 0x00025408
	private void OnEnable()
	{
		if (this.mList != null)
		{
			EventDelegate.Add(this.mList.onChange, new EventDelegate.Callback(this.SaveSelection));
			string @string = PlayerPrefs.GetString(this.key);
			if (!string.IsNullOrEmpty(@string))
			{
				this.mList.value = @string;
			}
		}
		else if (this.mCheck != null)
		{
			EventDelegate.Add(this.mCheck.onChange, new EventDelegate.Callback(this.SaveState));
			this.mCheck.value = (PlayerPrefs.GetInt(this.key, (!this.mCheck.startsActive) ? 0 : 1) != 0);
		}
		else if (this.mSlider != null)
		{
			EventDelegate.Add(this.mSlider.onChange, new EventDelegate.Callback(this.SaveProgress));
			this.mSlider.value = PlayerPrefs.GetFloat(this.key, this.mSlider.value);
		}
		else
		{
			string string2 = PlayerPrefs.GetString(this.key);
			UIToggle[] componentsInChildren = base.GetComponentsInChildren<UIToggle>(true);
			int i = 0;
			int num = componentsInChildren.Length;
			while (i < num)
			{
				UIToggle uitoggle = componentsInChildren[i];
				uitoggle.value = (uitoggle.name == string2);
				i++;
			}
		}
	}

	// Token: 0x06000279 RID: 633 RVA: 0x00027354 File Offset: 0x00025554
	private void OnDisable()
	{
		if (this.mCheck != null)
		{
			EventDelegate.Remove(this.mCheck.onChange, new EventDelegate.Callback(this.SaveState));
		}
		else if (this.mList != null)
		{
			EventDelegate.Remove(this.mList.onChange, new EventDelegate.Callback(this.SaveSelection));
		}
		else if (this.mSlider != null)
		{
			EventDelegate.Remove(this.mSlider.onChange, new EventDelegate.Callback(this.SaveProgress));
		}
		else
		{
			UIToggle[] componentsInChildren = base.GetComponentsInChildren<UIToggle>(true);
			int i = 0;
			int num = componentsInChildren.Length;
			while (i < num)
			{
				UIToggle uitoggle = componentsInChildren[i];
				if (uitoggle.value)
				{
					PlayerPrefs.SetString(this.key, uitoggle.name);
					break;
				}
				i++;
			}
		}
	}

	// Token: 0x0600027A RID: 634 RVA: 0x0000454E File Offset: 0x0000274E
	public void SaveSelection()
	{
		PlayerPrefs.SetString(this.key, UIPopupList.current.value);
	}

	// Token: 0x0600027B RID: 635 RVA: 0x00004565 File Offset: 0x00002765
	public void SaveState()
	{
		PlayerPrefs.SetInt(this.key, (!UIToggle.current.value) ? 0 : 1);
	}

	// Token: 0x0600027C RID: 636 RVA: 0x00004582 File Offset: 0x00002782
	public void SaveProgress()
	{
		PlayerPrefs.SetFloat(this.key, UIProgressBar.current.value);
	}

	// Token: 0x040001BC RID: 444
	public string keyName;

	// Token: 0x040001BD RID: 445
	private UIPopupList mList;

	// Token: 0x040001BE RID: 446
	private UIToggle mCheck;

	// Token: 0x040001BF RID: 447
	private UIProgressBar mSlider;
}
