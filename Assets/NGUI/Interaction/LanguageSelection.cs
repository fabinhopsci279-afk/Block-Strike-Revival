using System;
using UnityEngine;

// Token: 0x02000016 RID: 22
[RequireComponent(typeof(UIPopupList))]
[AddComponentMenu("NGUI/Interaction/Language Selection")]
public class LanguageSelection : MonoBehaviour
{
	// Token: 0x060000F2 RID: 242 RVA: 0x00002E62 File Offset: 0x00001062
	private void Awake()
	{
		this.mList = base.GetComponent<UIPopupList>();
		this.Refresh();
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x00002E76 File Offset: 0x00001076
	private void Start()
	{
		EventDelegate.Add(this.mList.onChange, delegate()
		{
			Localization.language = UIPopupList.current.value;
		});
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x000204B4 File Offset: 0x0001E6B4
	public void Refresh()
	{
		if (this.mList != null && Localization.knownLanguages != null)
		{
			this.mList.Clear();
			int i = 0;
			int num = Localization.knownLanguages.Length;
			while (i < num)
			{
				this.mList.items.Add(Localization.knownLanguages[i]);
				i++;
			}
			this.mList.value = Localization.language;
		}
	}

	// Token: 0x04000034 RID: 52
	private UIPopupList mList;
}
