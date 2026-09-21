using System;
using System.Collections.Generic;
using UnityEngine;

public class UINotification : MonoBehaviour
{
	public class Data
	{
		public string Title;

		public string Text;

		public string ButtonPositive;

		public string ButtonNegative;

		public Action PositiveAction;

		public Action NegativeAction;
	}

	public GameObject Button;

	private List<Data> Datas = new List<Data>();

	private bool isShow;

	private static UINotification instance;

	private void Start()
	{
		instance = this;
	}

	private void OnEnable()
	{
		UIPopUp.ClickButton = (Action)Delegate.Combine(UIPopUp.ClickButton, new Action(Close));
	}

	private void OnDisable()
	{
		UIPopUp.ClickButton = (Action)Delegate.Remove(UIPopUp.ClickButton, new Action(Close));
	}

	public static void AddNotification(string title, string text, string buttonPositive, string buttonNegative, Action positive, Action negative)
	{
		Data data = new Data();
		data.Title = title;
		data.Text = text;
		data.ButtonPositive = buttonPositive;
		data.ButtonNegative = buttonNegative;
		data.PositiveAction = positive;
		data.NegativeAction = negative;
		instance.Datas.Add(data);
		if (!instance.isShow)
		{
			instance.Button.SetActive(value: true);
		}
	}

	public void ShowNotification()
	{
		Button.SetActive(value: false);
		isShow = true;
		Data data = Datas[0];
		UIPopUp.ShowPopUp(data.Text, data.Title, data.ButtonPositive, data.PositiveAction, data.ButtonNegative, data.NegativeAction);
		Datas.Remove(data);
	}

	private void Close()
	{
		UIPanelManager.ShowPanel("Display");
		isShow = false;
		if (Datas.Count != 0)
		{
			Button.SetActive(value: true);
		}
	}
}
