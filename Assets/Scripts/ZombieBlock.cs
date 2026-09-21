using Photon;
using UnityEngine;

public class ZombieBlock : Photon.MonoBehaviour
{
	public Transform[] Planes;

	public int Health = 500;

	private Transform mTransform;

	private void Start()
	{
		mTransform = base.transform;
	}

	private void AddBlock(Vector3 pos)
	{
		mTransform.position = pos;
		for (int i = 0; i < Planes.Length; i++)
		{
			if (Physics.Raycast(mTransform.position, -Planes[i].forward, out RaycastHit hitInfo, 1.1f))
			{
				if (hitInfo.transform.CompareTag("Block"))
				{
					hitInfo.transform.GetComponent<Renderer>().enabled = false;
					Planes[i].GetComponent<Renderer>().enabled = false;
				}
				else
				{
					Planes[i].GetComponent<Renderer>().enabled = true;
				}
			}
			else
			{
				Planes[i].GetComponent<Renderer>().enabled = true;
			}
		}
	}

	private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			stream.SendNext(Health);
		}
		else
		{
			Health = (int)stream.ReceiveNext();
		}
	}
}
