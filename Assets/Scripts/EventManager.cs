using System;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
	public delegate void Callback();

	public delegate void Callback<T>(T arg1);

	public delegate void Callback<T, T2>(T arg1, T2 arg2);

	public delegate void Callback<T, T2, T3>(T arg1, T2 arg2, T3 arg3);

	private List<string> eventKey = new List<string>();

	private List<int> eventID = new List<int>();

	private List<Delegate> eventDelegate = new List<Delegate>();

	private static EventManager instance;

	private int nextID = -1;

	private static void Init()
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject("EventManager");
			instance = gameObject.AddComponent<EventManager>();
			UnityEngine.Object.DontDestroyOnLoad(gameObject);
		}
	}

	private void OnLevelWasLoaded(int level)
	{
		ClearAll();
	}

	public static int AddListener(string key, Callback e)
	{
		Init();
		return instance.OnListenerAdding(key, e);
	}

	public static int AddListener<T>(string key, Callback<T> e)
	{
		Init();
		return instance.OnListenerAdding(key, e);
	}

	public static int AddListener<T, T2>(string key, Callback<T, T2> e)
	{
		Init();
		return instance.OnListenerAdding(key, e);
	}

	public static int AddListener<T, T2, T3>(string key, Callback<T, T2, T3> e)
	{
		Init();
		return instance.OnListenerAdding(key, e);
	}

	private int OnListenerAdding(string key, Delegate e)
	{
		int num = GetNextID();
		eventKey.Add(key);
		eventID.Add(num);
		eventDelegate.Add(e);
		return num;
	}

	public static void Dispatch(string key)
	{
		Init();
		for (int i = 0; i < instance.eventKey.Count; i++)
		{
			if (instance.eventKey[i] == key)
			{
				(instance.eventDelegate[i] as Callback)?.Invoke();
			}
		}
	}

	public static void Dispatch(int id)
	{
		Init();
		int num = 0;
		while (true)
		{
			if (num < instance.eventKey.Count)
			{
				if (instance.eventID[num] == id)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		(instance.eventDelegate[num] as Callback)?.Invoke();
	}

	public static void Dispatch<T>(string key, T e)
	{
		Init();
		for (int i = 0; i < instance.eventKey.Count; i++)
		{
			if (instance.eventKey[i] == key)
			{
				(instance.eventDelegate[i] as Callback<T>)?.Invoke(e);
			}
		}
	}

	public static void Dispatch<T>(int id, T e)
	{
		Init();
		int num = 0;
		while (true)
		{
			if (num < instance.eventKey.Count)
			{
				if (instance.eventID[num] == id)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		(instance.eventDelegate[num] as Callback<T>)?.Invoke(e);
	}

	public static void Dispatch<T, T2>(string key, T e, T2 e2)
	{
		Init();
		for (int i = 0; i < instance.eventKey.Count; i++)
		{
			if (instance.eventKey[i] == key)
			{
				(instance.eventDelegate[i] as Callback<T, T2>)?.Invoke(e, e2);
			}
		}
	}

	public static void Dispatch<T, T2>(int id, T e, T2 e2)
	{
		Init();
		int num = 0;
		while (true)
		{
			if (num < instance.eventKey.Count)
			{
				if (instance.eventID[num] == id)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		(instance.eventDelegate[num] as Callback<T, T2>)?.Invoke(e, e2);
	}

	public static void Dispatch<T, T2, T3>(string key, T e, T2 e2, T3 e3)
	{
		Init();
		for (int i = 0; i < instance.eventKey.Count; i++)
		{
			if (instance.eventKey[i] == key)
			{
				(instance.eventDelegate[i] as Callback<T, T2, T3>)?.Invoke(e, e2, e3);
			}
		}
	}

	public static void Dispatch<T, T2, T3>(int id, T e, T2 e2, T3 e3)
	{
		Init();
		int num = 0;
		while (true)
		{
			if (num < instance.eventKey.Count)
			{
				if (instance.eventID[num] == id)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		(instance.eventDelegate[num] as Callback<T, T2, T3>)?.Invoke(e, e2, e3);
	}

	public static void Clear(string key)
	{
		Init();
		for (int i = 0; i < instance.eventKey.Count; i++)
		{
			if (instance.eventKey[i] == key)
			{
				instance.eventKey.RemoveAt(i);
				instance.eventID.RemoveAt(i);
				instance.eventDelegate.RemoveAt(i);
			}
		}
	}

	public static void Clear(int id)
	{
		Init();
		int num = 0;
		while (true)
		{
			if (num < instance.eventKey.Count)
			{
				if (instance.eventID[num] == id)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		instance.eventKey.RemoveAt(num);
		instance.eventID.RemoveAt(num);
		instance.eventDelegate.RemoveAt(num);
	}

	public static void ClearAll()
	{
		Init();
		instance.eventDelegate.Clear();
		instance.eventID.Clear();
		instance.eventKey.Clear();
		instance.nextID = -1;
	}

	private int GetNextID()
	{
		nextID++;
		return nextID;
	}
}
