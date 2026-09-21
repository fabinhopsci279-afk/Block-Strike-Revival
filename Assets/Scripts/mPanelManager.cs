using System.Collections.Generic;
using UnityEngine;

public class mPanelManager : MonoBehaviour
{
	public List<GameObject> PanelList = new List<GameObject>();

	public GameObject PanelUp;

	public GameObject InAppButton;

	private static mPanelManager instance;

	private void Awake()
	{
		instance = this;
	}

	public static void ShowPanel(string panelName, bool activePanelUp = true, bool activeInApp = true)
	{
		for (int i = 0; i < instance.PanelList.Count; i++)
		{
			if (instance.PanelList[i].name == panelName)
			{
				instance.PanelList[i].SetActive(value: true);
			}
			else
			{
				instance.PanelList[i].SetActive(value: false);
			}
		}
		instance.SetActivePanelUp(activePanelUp);
		instance.SetActiveInAppButton(activeInApp);
	}

	public void ShowPanel(GameObject panel)
	{
		ShowPanel(panel.name);
	}

	public static void HidePanels()
	{
		for (int i = 0; i < instance.PanelList.Count; i++)
		{
			instance.PanelList[i].SetActive(value: false);
		}
		instance.PanelUp.SetActive(value: false);
	}

	public void SetActivePanelUp(bool active)
	{
		PanelUp.SetActive(active);
	}

	public void SetActiveInAppButton(bool active)
	{
		InAppButton.SetActive(active);
	}
}
