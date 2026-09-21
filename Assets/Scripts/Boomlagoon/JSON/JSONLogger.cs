using UnityEngine;

namespace Boomlagoon.JSON
{
	internal static class JSONLogger
	{
		public static void Log(string str)
		{
			UnityEngine.Debug.Log(str);
		}

		public static void Error(string str)
		{
			UnityEngine.Debug.LogError(str);
		}
	}
}
