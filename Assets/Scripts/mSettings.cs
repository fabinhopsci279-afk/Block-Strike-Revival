using UnityEngine;

public class mSettings : MonoBehaviour
{
	private void Start()
	{
		Utils.SetActiveConsole(SaveLoadManager.GetConsole());
	}

	public void UpdateData()
	{
		Utils.SetActiveConsole(SaveLoadManager.GetConsole());
	}
}
