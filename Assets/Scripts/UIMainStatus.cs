using UnityEngine;

public class UIMainStatus : MonoBehaviour
{
	public UILabel label;

	private vp_Timer.Handle Timer = new vp_Timer.Handle();

	private static UIMainStatus instance;

	private void Awake()
	{
		instance = this;
	}

	public static void ShowText(string text)
	{
		ShowText(text, 5f);
	}

	public static void ShowText(string text, float duration)
	{
		instance.label.text = text;
		if (instance.Timer.Active)
		{
			instance.Timer.Cancel();
		}
		vp_Timer.In(duration, delegate
		{
			HideText();
		});
	}

	private static void HideText()
	{
		instance.label.text = string.Empty;
	}
}
