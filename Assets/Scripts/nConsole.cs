using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class nConsole : MonoBehaviour
{
	public KeyCode toggleKey = KeyCode.Escape;

	public Color color1 = new Color(0.145f, 0.145f, 0.145f, 1f);

	public Color color2 = new Color(0.145f, 0.145f, 0.145f, 1f);

	private GUIStyle backgroundStyle = new GUIStyle();

	private GUIStyle style1 = new GUIStyle();

	private GUIStyle style2 = new GUIStyle();

	private GUIStyle buttonStyle = new GUIStyle();

	private Vector2 scrollPos;

	private List<string> message = new List<string>();

	private List<string> stackTrace = new List<string>();

	private List<bool> showStackTrace = new List<bool>();

	private string consoleText = string.Empty;

	private bool show;

	private List<string> nameCommand = new List<string>();

	private List<UnityAction> actionCommand = new List<UnityAction>();

	private static nConsole instance;

	private void Awake()
	{
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	private void Start()
	{
		backgroundStyle.normal.background = NewTexture(color1);
		style1.contentOffset = new Vector2(15f, 0f);
		style2.normal.background = NewTexture(color2);
		style2.contentOffset = new Vector2(15f, 0f);
		buttonStyle.normal.background = NewTexture(color2);
		buttonStyle.normal.textColor = Color.white;
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		style1.fontSize = Mathf.RoundToInt(13 * Screen.width / 480);
		style2.fontSize = Mathf.RoundToInt(13 * Screen.width / 480);
		buttonStyle.fontSize = Mathf.RoundToInt(13 * Screen.width / 480);
	}

	private void OnEnable()
	{
		Application.RegisterLogCallback(HandleLog);
	}

	private void OnDisable()
	{
		Application.RegisterLogCallback(null);
	}

	public static void CreateGO(KeyCode toggleKey)
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject("nConsole");
			gameObject.AddComponent<nConsole>().toggleKey = toggleKey;
		}
	}

	public static void DestroyGO()
	{
		if (instance != null)
		{
			UnityEngine.Object.Destroy(instance.gameObject);
		}
	}

	public static void AddCommand(string command, UnityAction action)
	{
		if ((bool)instance)
		{
			instance.nameCommand.Add(command);
			instance.actionCommand.Add(action);
		}
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(toggleKey))
		{
			show = !show;
		}
	}

	private void OnGUI()
	{
		if (!show)
		{
			return;
		}
		GUI.Box(NewRect(-1f, -1f, 102f, 102f), string.Empty, backgroundStyle);
		GUI.Window(28, NewRect(0f, 2f, 100f, 85f), ConsoleWindow, string.Empty, GUIStyle.none);
		if (GUI.Button(NewRect(3f, 91f, 20f, 8f), "Clear", buttonStyle))
		{
			message.Clear();
			stackTrace.Clear();
			showStackTrace.Clear();
		}
		consoleText = GUI.TextField(NewRect(47f, 92.5f, 50f, 6f), consoleText);
		Event current = Event.current;
		if (current.type != EventType.KeyDown || current.keyCode != KeyCode.Return || string.IsNullOrEmpty(consoleText))
		{
			return;
		}
		for (int i = 0; i < nameCommand.Count; i++)
		{
			if (consoleText == nameCommand[i])
			{
				actionCommand[i]();
				break;
			}
		}
		consoleText = string.Empty;
	}

	private void ConsoleWindow(int windowID)
	{
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		for (int i = 0; i < message.Count; i++)
		{
			GUIStyle style = (i % 2 != 0) ? style1 : style2;
			if (GUILayout.Button(message[i], style))
			{
				showStackTrace[i] = !showStackTrace[i];
			}
			if (showStackTrace[i] && GUILayout.Button(stackTrace[i], style))
			{
				showStackTrace[i] = !showStackTrace[i];
			}
		}
		GUILayout.EndScrollView();
	}

	private Rect NewRect(float x, float y, float width, float height)
	{
		x = (float)Screen.width * x / 100f;
		y = (float)Screen.height * y / 100f;
		width = (float)Screen.width * width / 100f;
		height = (float)Screen.height * height / 100f;
		return new Rect(x, y, width, height);
	}

	private Texture2D NewTexture(Color color, int width = 5, int height = 5)
	{
		Texture2D texture2D = new Texture2D(width, height);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				texture2D.SetPixel(i, j, color);
			}
		}
		texture2D.Apply();
		return texture2D;
	}

	private void HandleLog(string m, string s, LogType type)
	{
		switch (type)
		{
		case LogType.Error:
		case LogType.Exception:
			message.Add("<color=red>" + m + "</color>");
			stackTrace.Add("<color=red>" + s + "</color>");
			break;
		case LogType.Warning:
			message.Add("<color=yellow>" + m + "</color>");
			stackTrace.Add("<color=yellow>" + s + "</color>");
			break;
		case LogType.Log:
			message.Add("<color=white>" + m + "</color>");
			stackTrace.Add("<color=white>" + s + "</color>");
			break;
		}
		showStackTrace.Add(item: false);
	}
}
