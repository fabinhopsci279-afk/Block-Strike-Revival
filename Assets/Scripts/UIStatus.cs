using System.Collections.Generic;
using UnityEngine;

public class UIStatus : MonoBehaviour
{
	public UILabel Label;

	private List<string> TextList = new List<string>();

	private static UIStatus instance;

	private void Awake()
	{
		instance = this;
	}

	public static void NewLine(string text)
	{
		instance.TextList.Add(text);
		instance.UpdateLabel(clear: true);
	}

	private void UpdateLabel(bool clear)
	{
		string text = string.Empty;
		for (int i = 0; i < TextList.Count; i++)
		{
			if (i > 0)
			{
				text += "\n";
			}
			text += TextList[TextList.Count - 1 - i];
		}
		Label.text = text;
		if (clear)
		{
			vp_Timer.In(5f, delegate
			{
				Remove();
			});
		}
	}

	private void Remove()
	{
		TextList.RemoveAt(0);
		UpdateLabel(clear: false);
	}
}
