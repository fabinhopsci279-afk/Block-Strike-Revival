using UnityEngine;

public class ModeObject : MonoBehaviour
{
	public GameMode Mode;

	public GameObject[] Targets;

	private void Start()
	{
		if (PhotonNetwork.room.GetGameMode() == Mode)
		{
			for (int i = 0; i < Targets.Length; i++)
			{
				Targets[i].SetActive(value: true);
			}
		}
	}
}
