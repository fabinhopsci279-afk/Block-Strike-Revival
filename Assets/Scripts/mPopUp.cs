using UnityEngine;

public class mPopUp : MonoBehaviour
{
	public GameObject mainPanel;

	public UILabel showTextLabel;

	public GameObject popUpPanel;

	public UILabel popUpLabel;

	public UILabel popUpTitle;

	public UILabel popUpButton1;

	public UILabel popUpButton2;

	public UIInput input;

	private vp_GlobalCallback button1Callback;

	private vp_GlobalCallback button2Callback;

	private vp_GlobalCallback inputCallback;

	public static bool isActive;

	private static mPopUp instance;

	private void Awake()
	{
		instance = this;
	}

	public static void ShowText(string text)
	{
		ShowText(text, 0f, string.Empty);
	}

	public static void ShowText(string text, float duration, string panel)
	{
		isActive = true;
		mPanelManager.HidePanels();
		instance.HideElements();
		instance.showTextLabel.gameObject.SetActive(value: true);
		instance.mainPanel.SetActive(value: true);
		instance.popUpPanel.SetActive(value: false);
		instance.showTextLabel.text = text;
		if (duration != 0f)
		{
			vp_Timer.In(duration, delegate
			{
				HidePopUp(panel);
			});
		}
	}

	public static void ShowPopUp(string text, string title, string button1Text, vp_GlobalCallback callbackButton1, string button2Text, vp_GlobalCallback callbackButton2)
	{
		isActive = true;
		instance.HideElements();
		instance.mainPanel.SetActive(value: true);
		instance.popUpPanel.SetActive(value: true);
		instance.popUpLabel.gameObject.SetActive(value: true);
		instance.popUpLabel.text = text;
		instance.popUpTitle.gameObject.SetActive(value: true);
		instance.popUpTitle.text = title;
		instance.popUpButton1.text = button1Text;
		instance.button1Callback = callbackButton1;
		instance.popUpButton2.text = button2Text;
		instance.button2Callback = callbackButton2;
	}

	public static void ShowInput(string inputText, string title, int limit, UIInput.KeyboardType keyboardType, vp_GlobalCallback callbackInput, string button1Text, vp_GlobalCallback callbackButton1, string button2Text, vp_GlobalCallback callbackButton2)
	{
		isActive = true;
		instance.HideElements();
		instance.mainPanel.SetActive(value: true);
		instance.popUpPanel.SetActive(value: true);
		instance.input.gameObject.SetActive(value: true);
		instance.popUpTitle.gameObject.SetActive(value: true);
		instance.input.value = inputText;
		instance.input.characterLimit = limit;
		instance.input.keyboardType = keyboardType;
		instance.popUpTitle.text = title;
		instance.inputCallback = callbackInput;
		instance.popUpButton1.text = button1Text;
		instance.button1Callback = callbackButton1;
		instance.popUpButton2.text = button2Text;
		instance.button2Callback = callbackButton2;
	}

	public static void HidePopUp(string panel = "")
	{
		isActive = false;
		instance.mainPanel.SetActive(value: false);
		if (!string.IsNullOrEmpty(panel))
		{
			mPanelManager.ShowPanel(panel);
		}
	}

	public void OnClickButton1()
	{
		if (button1Callback != null)
		{
			button1Callback();
		}
	}

	public void OnClickButton2()
	{
		if (button2Callback != null)
		{
			button2Callback();
		}
	}

	public void OnSubmitInput()
	{
		if (inputCallback != null)
		{
			inputCallback();
		}
	}

	public static string GetInputText()
	{
		return instance.input.value;
	}

	public static void SetInputText(string text)
	{
		instance.input.value = text;
	}

	private void HideElements()
	{
		showTextLabel.gameObject.SetActive(value: false);
		input.gameObject.SetActive(value: false);
		popUpPanel.SetActive(value: false);
		popUpLabel.gameObject.SetActive(value: false);
		input.gameObject.SetActive(value: false);
		button1Callback = null;
		button2Callback = null;
		inputCallback = null;
	}
}
