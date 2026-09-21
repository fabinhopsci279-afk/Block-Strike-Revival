using System.Collections.Generic;
using UnityEngine;

public class UISelectTeam : MonoBehaviour
{
	public UIGrid blueGrid;

	public UIGrid redGrid;

	public GameObject label;

	private vp_Timer.Handle Timer = new vp_Timer.Handle();

	private List<GameObject> List = new List<GameObject>();

	private static UISelectTeam instance;

	private void Awake()
	{
		instance = this;
	}

	public static void OnStart()
	{
		UIPanelManager.ShowPanel("SelectTeam");
		instance.UpdateList();
	}

	private void UpdateList()
	{
		ClearList();
		PhotonPlayer[] otherPlayers = PhotonNetwork.otherPlayers;
		for (int i = 0; i < otherPlayers.Length; i++)
		{
			GameObject gameObject = null;
			if (otherPlayers[i].GetTeam() == Team.Blue)
			{
				gameObject = NGUITools.AddChild(blueGrid.gameObject, label);
			}
			else if (otherPlayers[i].GetTeam() == Team.Red)
			{
				gameObject = NGUITools.AddChild(redGrid.gameObject, label);
			}
			if (gameObject != null)
			{
				gameObject.SetActive(value: true);
				gameObject.GetComponent<UILabel>().text = otherPlayers[i].name;
				List.Add(gameObject);
			}
		}
		blueGrid.repositionNow = true;
		redGrid.repositionNow = true;
		vp_Timer.In(3f, UpdateList, Timer);
	}

	private void ClearList()
	{
		for (int i = 0; i < List.Count; i++)
		{
			UnityEngine.Object.Destroy(List[i]);
		}
		List.Clear();
	}

	public void SelectTeam(int team)
	{
		int count = blueGrid.GetChildList().Count;
		int count2 = redGrid.GetChildList().Count;
		if ((team != 1 || count - count2 < 1) && (team != 2 || count2 - count < 1) && (team != 1 || PhotonNetwork.room.maxPlayers / 2 != count) && (team != 2 || PhotonNetwork.room.maxPlayers / 2 != count2))
		{
			Timer.Cancel();
			GameManager.OnSelectTeam((Team)team);
		}
	}
}
