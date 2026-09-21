using System.Collections.Generic;
using UnityEngine;

public class UIChat : MonoBehaviour
{
	public UILabel Label;

	public UIInput m_Input;

	private List<string> TextList = new List<string>();

	private static UIChat instance;

	private void Start()
	{
		instance = this;
	}

	private void Update()
	{
		if (InputManager.GetButtonDown("Chat") && !UICamera.inputHasFocus)
		{
			Label.text = string.Empty;
			UICamera.selectedObject = m_Input.gameObject;
		}
	}

	public void OnSubmit()
	{
		string value = m_Input.value;
		value = value.Replace("\n", string.Empty);
		if (!string.IsNullOrEmpty(value))
		{
			m_Input.value = string.Empty;
			m_Input.isSelected = false;
			GameManager.OnChat(value);
		}
	}

	public static void NewLine(string text)
	{
		if (SaveLoadManager.GetChat())
		{
			instance.Label.supportEncoding = true;
			instance.TextList.Add(text);
			instance.UpdateLabel(clear: true);
		}
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
			vp_Timer.In(8f, delegate
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
