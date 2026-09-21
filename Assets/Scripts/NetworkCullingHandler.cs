using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class NetworkCullingHandler : MonoBehaviour, IPunObservable
{
	private int orderIndex;

	private List<byte> previousActiveCells;

	private List<byte> activeCells;

	private PhotonView pView;

	private Vector3 lastPosition;

	private Vector3 currentPosition;

	private void OnEnable()
	{
		if (pView == null)
		{
			pView = GetComponent<PhotonView>();
			if (!pView.isMine)
			{
				return;
			}
		}
		previousActiveCells = new List<byte>(0);
		activeCells = new List<byte>(0);
		currentPosition = (lastPosition = base.transform.position);
	}

	private void Start()
	{
		if (pView.isMine && !PhotonNetwork.inRoom)
		{
		}
	}

	private void Update()
	{
		if (pView.isMine)
		{
			lastPosition = currentPosition;
			currentPosition = base.transform.position;
			if (currentPosition != lastPosition && HaveActiveCellsChanged())
			{
				UpdateInterestGroups();
			}
		}
	}

	private void OnGUI()
	{
		if (pView.isMine)
		{
			string str = "Inside cells:\n";
			string text = "Subscribed cells:\n";
			for (int i = 0; i < activeCells.Count; i++)
			{
				text = text + activeCells[i] + " | ";
			}
			GUI.Label(new Rect(20f, (float)Screen.height - 120f, 200f, 40f), "<color=white>PhotonView Group: " + pView.group + "</color>", new GUIStyle
			{
				alignment = TextAnchor.UpperLeft,
				fontSize = 16
			});
			GUI.Label(new Rect(20f, (float)Screen.height - 100f, 200f, 40f), "<color=white>" + str + "</color>", new GUIStyle
			{
				alignment = TextAnchor.UpperLeft,
				fontSize = 16
			});
			GUI.Label(new Rect(20f, (float)Screen.height - 60f, 200f, 40f), "<color=white>" + text + "</color>", new GUIStyle
			{
				alignment = TextAnchor.UpperLeft,
				fontSize = 16
			});
		}
	}

	private bool HaveActiveCellsChanged()
	{
		return false;
	}

	private void UpdateInterestGroups()
	{
		List<byte> list = new List<byte>(0);
		foreach (byte previousActiveCell in previousActiveCells)
		{
			if (!activeCells.Contains(previousActiveCell))
			{
				list.Add(previousActiveCell);
			}
		}
	}

	public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
	}
}
