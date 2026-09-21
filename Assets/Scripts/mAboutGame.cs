using UnityEngine;

public class mAboutGame : MonoBehaviour
{
	public void OnOpenURL(string url)
	{
		Application.OpenURL(url);
	}

	public void OnShare()
	{
	}
}
