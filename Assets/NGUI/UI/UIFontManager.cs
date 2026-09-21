using System;
using UnityEngine;

// Token: 0x02000442 RID: 1090
public class UIFontManager : MonoBehaviour
{
	// Token: 0x06002480 RID: 9344 RVA: 0x00018F38 File Offset: 0x00017138
	private void Start()
	{
		UIFontManager.instance = this;
		UIFontManager.SetFont(Settings.Font);
	}

	// Token: 0x06002481 RID: 9345 RVA: 0x000BE6D8 File Offset: 0x000BC8D8
	public static void SetFont(int index)
	{
		for (int i = 0; i < UIFontManager.instance.labels.Length; i++)
		{
            if (UIFontManager.instance.labels[i] != null)
            {
                UIFontManager.instance.labels[i].trueTypeFont = UIFontManager.instance.fonts[index];
            }
		}
		for (int j = 0; j < UIFontManager.instance.popupLists.Length; j++)
		{
            if (UIFontManager.instance.popupLists[j] != null)
            {
                UIFontManager.instance.popupLists[j].trueTypeFont = UIFontManager.instance.fonts[index];
            }    
		}
	}

	// Token: 0x06002482 RID: 9346 RVA: 0x00018F4A File Offset: 0x0001714A
	public static Font[] GetFonts()
	{
		return UIFontManager.instance.fonts;
	}

	// Token: 0x0400164B RID: 5707
	public UILabel[] labels;

	// Token: 0x0400164C RID: 5708
	public UIPopupList[] popupLists;

	// Token: 0x0400164D RID: 5709
	public Font[] fonts;

	// Token: 0x0400164E RID: 5710
	private static UIFontManager instance;
}
