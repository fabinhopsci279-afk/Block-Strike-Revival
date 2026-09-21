using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Loom : MonoBehaviour
{
	public struct DelayedQueueItem
	{
		public float time;

		public Action action;
	}

	public static int maxThreads = 8;

	private static int numThreads;

	private static Loom _current;

	private int _count;

	private static bool initialized;

	private List<Action> _actions = new List<Action>();

	private List<DelayedQueueItem> _delayed = new List<DelayedQueueItem>();

	private List<DelayedQueueItem> _currentDelayed = new List<DelayedQueueItem>();

	private List<Action> _currentActions = new List<Action>();

	public static Loom Current
	{
		get
		{
			Initialize();
			return _current;
		}
	}

	private void Awake()
	{
		_current = this;
		initialized = true;
	}

	private static void Initialize()
	{
		if (!initialized && Application.isPlaying)
		{
			initialized = true;
			GameObject gameObject = new GameObject("Loom");
			_current = gameObject.AddComponent<Loom>();
		}
	}

	public static void QueueOnMainThread(Action action)
	{
		QueueOnMainThread(action, 0f);
	}

	public static void QueueOnMainThread(Action action, float time)
	{
		if (time != 0f)
		{
			List<DelayedQueueItem> delayed = Current._delayed;
			lock (delayed)
			{
				Current._delayed.Add(new DelayedQueueItem
				{
					time = Time.time + time,
					action = action
				});
			}
		}
		else
		{
			List<Action> actions = Current._actions;
			lock (actions)
			{
				Current._actions.Add(action);
			}
		}
	}

	public static Thread RunAsync(Action a)
	{
		Initialize();
		while (numThreads >= maxThreads)
		{
			Thread.Sleep(1);
		}
		Interlocked.Increment(ref numThreads);
		ThreadPool.QueueUserWorkItem(RunAction, a);
		return null;
	}

	private static void RunAction(object action)
	{
		try
		{
			((Action)action)();
		}
		catch
		{
		}
		finally
		{
			Interlocked.Decrement(ref numThreads);
		}
	}

	private void OnDisable()
	{
		if (_current == this)
		{
			_current = null;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
		List<Action> actions = _actions;
		lock (actions)
		{
			_currentActions.Clear();
			_currentActions.AddRange(_actions);
			_actions.Clear();
		}
		foreach (Action currentAction in _currentActions)
		{
			currentAction();
		}
		List<DelayedQueueItem> delayed = _delayed;
		lock (delayed)
		{
			_currentDelayed.Clear();
			_currentDelayed.AddRange(from d in _delayed
				where d.time <= Time.time
				select d);
			foreach (DelayedQueueItem item in _currentDelayed)
			{
				_delayed.Remove(item);
			}
		}
		foreach (DelayedQueueItem item2 in _currentDelayed)
		{
			DelayedQueueItem current3 = item2;
			current3.action();
		}
	}
}
