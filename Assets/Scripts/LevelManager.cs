using Boomlagoon.JSON;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager
{
	private static Dictionary<GameMode, List<string>> SceneList = new Dictionary<GameMode, List<string>>();

	public static void LoadFile()
	{
		if (SceneList.Count != 0)
		{
			return;
		}
		TextAsset textAsset = Resources.Load("Others/SceneManager") as TextAsset;
		JSONArray jSONArray = JSONArray.Parse(textAsset.text);
		for (int i = 0; i < jSONArray.Length; i++)
		{
			GameMode key = (GameMode)(int)Enum.Parse(typeof(GameMode), jSONArray[i].Obj.GetString("GameMode"));
			List<string> list = new List<string>();
			JSONArray array = jSONArray[i].Obj.GetArray("Scenes");
			for (int j = 0; j < array.Length; j++)
			{
				list.Add(array[j].Str);
			}
			SceneList.Add(key, list);
		}
	}

	public static List<string> GetGameModeScenes(GameMode mode)
	{
		LoadFile();
		return SceneList[mode];
	}

	public static string GetNextScene(GameMode mode)
	{
		return GetNextScene(mode, GetSceneName());
	}

	public static string GetNextScene(GameMode mode, string scene)
	{
		LoadFile();
		List<string> list = SceneList[mode];
		for (int i = 0; i < list.Count; i++)
		{
			if (scene == list[i])
			{
				if (list.Count - 1 == i)
				{
					return list[0];
				}
				return list[i + 1];
			}
		}
		return string.Empty;
	}

	public static string GetSceneName()
	{
		return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
	}
}
