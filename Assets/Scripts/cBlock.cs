using UnityEngine;

public class cBlock : MonoBehaviour
{
	public Transform[] list;

	public int selectSkin;

	public Vector3 pos;

	private void Start()
	{
		base.name = "Block " + base.transform.localPosition.ToString();
		pos = base.transform.localPosition;
	}

	public void SetSkin(Material skin, int s)
	{
		selectSkin = s;
		for (int i = 0; i < list.Length; i++)
		{
			list[i].GetComponent<Renderer>().material = skin;
		}
	}

	public void Check()
	{
		for (int i = 0; i < list.Length; i++)
		{
			CheckBlock(list[i]);
		}
	}

	private void CheckBlock(Transform v)
	{
		if (Physics.Raycast(base.transform.position, -v.forward, out RaycastHit hitInfo, 1.1f))
		{
			if (hitInfo.transform.CompareTag("Block"))
			{
				hitInfo.transform.GetComponent<Renderer>().enabled = false;
				v.GetComponent<Renderer>().enabled = false;
			}
			else
			{
				v.GetComponent<Renderer>().enabled = true;
			}
		}
		else
		{
			v.GetComponent<Renderer>().enabled = true;
		}
	}

	public void DeleteBlock()
	{
		for (int i = 0; i < list.Length; i++)
		{
			if (Physics.Raycast(base.transform.position, -list[i].forward, out RaycastHit hitInfo, 1.1f) && hitInfo.transform.CompareTag("Block"))
			{
				hitInfo.transform.GetComponent<Renderer>().enabled = true;
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}
}
