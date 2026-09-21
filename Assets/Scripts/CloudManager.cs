using System;
using UnityEngine;

public class CloudManager : MonoBehaviour
{
	private static Action<string> LoadComplete;

	private static Action<string> LoadFailed;

	private static Action SaveComplete;

	private static Action<string> SaveFailed;

	private static CloudManager instance;

	private void Start()
	{
		instance = this;
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	public static void Load(Action<string> complete, Action<string> error)
	{
		vp_Timer.In(0.5f, delegate
		{
			error(string.Empty);
		});
	}

	private void loadSnapshotSucceededEvent(string data)
	{
		LoadComplete(data);
	}

	private void loadSnapshotFailedEvent(string error)
	{
		LoadFailed(error);
	}

	public static void Save(string data, Action complete, Action<string> error)
	{
		vp_Timer.In(0.5f, delegate
		{
			error(string.Empty);
		});
	}

	private void saveSnapshotSucceededEvent()
	{
		SaveComplete();
	}

	private void saveSnapshotFailedEvent(string error)
	{
		SaveFailed(error);
	}
}
