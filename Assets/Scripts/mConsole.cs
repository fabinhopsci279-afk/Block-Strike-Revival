using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class mConsole : MonoBehaviour
{
	public GameObject Console;

	public UILabel Label;

	private bool Active;

	private static List<string> Message = new List<string>();

	private static List<string> StackTrace = new List<string>();

	private void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
	}

	private void OnDisable()
	{
		Application.logMessageReceived -= HandleLog;
	}

	private void HandleLog(string message, string stackTrace, LogType type)
	{
		switch (type)
		{
		case LogType.Error:
			Message.Add("[ff0000]" + message + "[-]");
			StackTrace.Add("[ff0000]" + stackTrace + "[-]");
			break;
		case LogType.Warning:
			Message.Add("[ffff00]" + message + "[-]");
			StackTrace.Add("[ffff00]" + stackTrace + "[-]");
			break;
		default:
			Message.Add(message);
			StackTrace.Add(stackTrace);
			break;
		}
	}

	public static string GetConsoleText(bool stackTrace)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < Message.Count; i++)
		{
			stringBuilder.AppendLine(Message[i]);
			if (stackTrace)
			{
				stringBuilder.AppendLine(StackTrace[i]);
			}
		}
		return stringBuilder.ToString();
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.Escape) && SaveLoadManager.GetConsole())
		{
			OnClick();
		}
	}

	public void OnClick()
	{
		Active = !Active;
		Console.SetActive(Active);
		if (Active)
		{
			Label.text = GetConsoleText(stackTrace: true);
		}
	}
}
