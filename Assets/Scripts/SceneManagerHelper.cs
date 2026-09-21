using UnityEngine;

public class SceneManagerHelper
{
	public static string ActiveSceneName => UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

	public static int ActiveSceneBuildIndex => UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
}
