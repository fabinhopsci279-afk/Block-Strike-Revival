using System;
using UnityEngine;

// Token: 0x02000058 RID: 88
[AddComponentMenu("NGUI/Interaction/Sound Volume")]
[RequireComponent(typeof(UISlider))]
public class UISoundVolume : MonoBehaviour
{
	// Token: 0x060002E4 RID: 740 RVA: 0x0002A760 File Offset: 0x00028960
	private void Awake()
	{
		UISlider component = base.GetComponent<UISlider>();
		component.value = NGUITools.soundVolume;
		EventDelegate.Add(component.onChange, new EventDelegate.Callback(this.OnChange));
	}

	// Token: 0x060002E5 RID: 741 RVA: 0x0000499E File Offset: 0x00002B9E
	private void OnChange()
	{
		NGUITools.soundVolume = UIProgressBar.current.value;
	}
}
