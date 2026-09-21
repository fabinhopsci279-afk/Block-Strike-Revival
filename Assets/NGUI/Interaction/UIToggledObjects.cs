using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000060 RID: 96
[AddComponentMenu("NGUI/Interaction/Toggled Objects")]
public class UIToggledObjects : MonoBehaviour
{
	// Token: 0x06000309 RID: 777 RVA: 0x0002B39C File Offset: 0x0002959C
	private void Awake()
	{
		if (this.target != null)
		{
			if (this.activate.Count == 0 && this.deactivate.Count == 0)
			{
				if (this.inverse)
				{
					this.deactivate.Add(this.target);
				}
				else
				{
					this.activate.Add(this.target);
				}
			}
			else
			{
				this.target = null;
			}
		}
		UIToggle component = base.GetComponent<UIToggle>();
		EventDelegate.Add(component.onChange, new EventDelegate.Callback(this.Toggle));
	}

	// Token: 0x0600030A RID: 778 RVA: 0x0002B428 File Offset: 0x00029628
	public void Toggle()
	{
		bool value = UIToggle.current.value;
		if (base.enabled)
		{
			for (int i = 0; i < this.activate.Count; i++)
			{
				this.Set(this.activate[i], value);
			}
			for (int j = 0; j < this.deactivate.Count; j++)
			{
				this.Set(this.deactivate[j], !value);
			}
		}
	}

	// Token: 0x0600030B RID: 779 RVA: 0x00004B21 File Offset: 0x00002D21
	private void Set(GameObject go, bool state)
	{
		if (go != null)
		{
			NGUITools.SetActive(go, state);
		}
	}

	// Token: 0x04000264 RID: 612
	public List<GameObject> activate;

	// Token: 0x04000265 RID: 613
	public List<GameObject> deactivate;

	// Token: 0x04000266 RID: 614
	[HideInInspector]
	[SerializeField]
	private GameObject target;

	// Token: 0x04000267 RID: 615
	[SerializeField]
	[HideInInspector]
	private bool inverse;
}
