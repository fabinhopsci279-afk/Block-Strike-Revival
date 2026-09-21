using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200005F RID: 95
[AddComponentMenu("NGUI/Interaction/Toggled Components")]
[ExecuteInEditMode]
[RequireComponent(typeof(UIToggle))]
public class UIToggledComponents : MonoBehaviour
{
	// Token: 0x06000306 RID: 774 RVA: 0x0002B290 File Offset: 0x00029490
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

	// Token: 0x06000307 RID: 775 RVA: 0x0002B31C File Offset: 0x0002951C
	public void Toggle()
	{
		if (base.enabled)
		{
			for (int i = 0; i < this.activate.Count; i++)
			{
				MonoBehaviour monoBehaviour = this.activate[i];
				monoBehaviour.enabled = UIToggle.current.value;
			}
			for (int j = 0; j < this.deactivate.Count; j++)
			{
				MonoBehaviour monoBehaviour2 = this.deactivate[j];
				monoBehaviour2.enabled = !UIToggle.current.value;
			}
		}
	}

	// Token: 0x04000260 RID: 608
	public List<MonoBehaviour> activate;

	// Token: 0x04000261 RID: 609
	public List<MonoBehaviour> deactivate;

	// Token: 0x04000262 RID: 610
	[HideInInspector]
	[SerializeField]
	private MonoBehaviour target;

	// Token: 0x04000263 RID: 611
	[HideInInspector]
	[SerializeField]
	private bool inverse;
}
