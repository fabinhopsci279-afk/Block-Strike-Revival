using System;
using System.Collections.Generic;
using UnityEngine;

public class qLogger : MonoBehaviour
{
	public class LoggerClass
	{
		public string time;

		public string message;

		public string stackTrace;

		public LogType logType;
	}

	public KeyCode toogleKey;

	public bool openAwake;

	public bool showStackTrace = true;

	private List<LoggerClass> logList = new List<LoggerClass>();

	private bool isInfo = true;

	private bool isWarring = true;

	private bool isError = true;

	private bool isStackTrace;

	private bool isTime;

	private bool isShow;

	private Vector2 scrollPos;

	private static qLogger instance;

	private void Awake()
	{
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		isShow = openAwake;
		instance = this;
	}

	public static void CreateGO()
	{
		CreateGO(isStackTrace: true, KeyCode.None);
	}

	public static void CreateGO(bool isStackTrace, KeyCode toogleKey)
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject("qLogger");
			qLogger qLogger = gameObject.AddComponent<qLogger>();
			qLogger.showStackTrace = isStackTrace;
			qLogger.toogleKey = toogleKey;
			if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				instance.isShow = true;
			}
		}
	}

	public static void DestroyGO()
	{
		if (instance != null)
		{
			UnityEngine.Object.Destroy(instance.gameObject);
		}
	}

	public static void OnOpenLogger()
	{
		if (instance != null)
		{
			instance.isShow = true;
		}
	}

	public static void OnCloseLogger()
	{
		if (instance != null)
		{
			instance.isShow = false;
		}
	}

	private void OnEnable()
	{
		Application.logMessageReceived += HandleLog;
	}

	private void OnDisable()
	{
		Application.logMessageReceived -= HandleLog;
	}

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(toogleKey))
		{
			isShow = !isShow;
		}
		UpdateScroll();
	}

	private void UpdateScroll()
	{
		if (UnityEngine.Input.touchCount == 1)
		{
			Touch touch = Input.touches[0];
			if (touch.phase == TouchPhase.Moved)
			{
				scrollPos += touch.deltaPosition;
			}
		}
	}

	private void OnGUI()
	{
		if (isShow)
		{
			GUILayout.Window(28, new Rect(20f, 20f, Screen.width - 40, Screen.height - 40), ConsoleWindow, "Console");
		}
	}

	private void ConsoleWindow(int windowID)
	{
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.normal.textColor = Color.white;
		gUIStyle.fontSize = Screen.height / 24;
		scrollPos = GUILayout.BeginScrollView(scrollPos);
		for (int i = 0; i < logList.Count; i++)
		{
			if ((isInfo || logList[i].logType != LogType.Log) && (isWarring || logList[i].logType != LogType.Warning) && (isError || logList[i].logType != 0))
			{
				GUILayout.Label((isTime ? (logList[i].time + " ") : " ") + logList[i].message + (isStackTrace ? ("\n" + logList[i].stackTrace) : string.Empty), gUIStyle);
			}
		}
		GUI.contentColor = Color.white;
		GUILayout.EndScrollView();
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Clear", GUILayout.MaxWidth((float)Screen.width / 9.6f), GUILayout.MaxHeight(Screen.height / 16)))
		{
			logList.Clear();
		}
		GUILayout.Space((float)Screen.width / 3.2f);
		if (GUILayout.Button(isInfo ? "Info" : "<color=red>Info</color>", GUILayout.MaxWidth((float)Screen.width / 9.6f), GUILayout.MaxHeight(Screen.height / 16)))
		{
			isInfo = !isInfo;
		}
		if (GUILayout.Button(isWarring ? "Warring" : "<color=red>Warring</color>", GUILayout.MaxWidth((float)Screen.width / 9.6f), GUILayout.MaxHeight(Screen.height / 16)))
		{
			isWarring = !isWarring;
		}
		if (GUILayout.Button(isError ? "Error" : "<color=red>Error</color>", GUILayout.MaxWidth((float)Screen.width / 9.6f), GUILayout.MaxHeight(Screen.height / 16)))
		{
			isError = !isError;
		}
		if (GUILayout.Button(isStackTrace ? "StackTrace" : "<color=red>StackTrace</color>", GUILayout.MaxWidth((float)Screen.width / 9.6f), GUILayout.MaxHeight(Screen.height / 16)))
		{
			isStackTrace = !isStackTrace;
		}
		if (GUILayout.Button(isTime ? "Time" : "<color=red>Time</color>", GUILayout.MaxWidth((float)Screen.width / 9.6f), GUILayout.MaxHeight(Screen.height / 16)))
		{
			isTime = !isTime;
		}
		GUILayout.EndHorizontal();
	}

	public static void SendLog(string message, string stackTrace, LogType type)
	{
		if (SaveLoadManager.GetConsole())
		{
			instance.HandleLog(message, stackTrace, type);
		}
	}

	public void HandleLog(string message, string stackTrace, LogType type)
	{
		LoggerClass loggerClass = new LoggerClass();
		switch (type)
		{
		case LogType.Error:
		case LogType.Exception:
			loggerClass.message = "<color=red>" + message + "</color>";
			loggerClass.stackTrace = "<color=red>" + stackTrace + "</color>";
			break;
		case LogType.Warning:
			loggerClass.message = "<color=yellow>" + message + "</color>";
			loggerClass.stackTrace = "<color=yellow>" + stackTrace + "</color>";
			break;
		case LogType.Log:
			loggerClass.message = "<color=white>" + message + "</color>";
			loggerClass.stackTrace = "<color=white>" + stackTrace + "</color>";
			break;
		}
		loggerClass.time = DateTime.Now.Hour.ToString() + ":" + DateTime.Now.Minute.ToString() + ":" + DateTime.Now.Second.ToString() + "." + DateTime.Now.Millisecond.ToString();
		loggerClass.logType = type;
		logList.Add(loggerClass);
	}
}
