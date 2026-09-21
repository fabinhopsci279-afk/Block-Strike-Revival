using Boomlagoon.JSON;
using System;
using System.Collections;
using UnityEngine;

public class cBlockPlacer : MonoBehaviour
{
	public vp_FPController player;

	public Camera playerCamera;

	public GameObject block;

	public Transform blockPreview;

	public int selectBlockSkin;

	public Material[] blockSkins;

	public UITexture selectBlockButton;

	public float visibleDistance = 20f;

	[NonSerialized]
	public GameObject mapRoot;

	private bool isBlockAsync;

	private float lastJumpTime;

	private void Start()
	{
		mapRoot = new GameObject("Map");
		float[] array = new float[32];
		array[8] = visibleDistance;
		playerCamera.layerCullDistances = array;
	}

	private void Update()
	{
		if (isBlockAsync)
		{
			return;
		}
		if (Input.GetMouseButtonDown(0))
		{
			AddBlock();
		}
		if (Input.GetMouseButtonDown(1))
		{
			DeleteBlock();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Q))
		{
			SelectBlock(selectBlockSkin - 1);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.E))
		{
			SelectBlock(selectBlockSkin + 1);
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.Space))
		{
			if (lastJumpTime + 0.3f >= Time.time)
			{
				if (player.MotorFreeFly)
				{
					player.PhysicsGravityModifier = 0.2f;
				}
				else
				{
					player.PhysicsGravityModifier = 0f;
					player.Stop();
					player.AddForce(0f, 0.1f, 0f);
				}
				player.MotorFreeFly = !player.MotorFreeFly;
			}
			else
			{
				lastJumpTime = Time.time;
			}
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.F4))
		{
			ClearMap();
		}
		Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		if (Physics.Raycast(ray, out RaycastHit hitInfo, 5f))
		{
			if (hitInfo.distance <= 2.2f)
			{
				blockPreview.position = Vector3.forward * 500f;
				return;
			}
			Vector3 position = hitInfo.point;
			if (hitInfo.transform.CompareTag("Block"))
			{
				position = hitInfo.transform.parent.localPosition + hitInfo.normal;
			}
			else
			{
				position.x = Mathf.Round(position.x);
				position.y = Mathf.Round(position.y);
				position.z = Mathf.Round(position.z);
			}
			blockPreview.position = position;
		}
		else
		{
			blockPreview.position = Vector3.forward * 500f;
		}
	}

	private void AddBlock()
	{
		Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		if (Physics.Raycast(ray, out RaycastHit hitInfo, 5f))
		{
			Vector3 position = hitInfo.point;
			if (hitInfo.transform.CompareTag("Block"))
			{
				position = hitInfo.transform.parent.localPosition + hitInfo.normal;
			}
			else
			{
				position.x = Mathf.Round(position.x);
				position.y = Mathf.Round(position.y);
				position.z = Mathf.Round(position.z);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(block, position, Quaternion.identity);
			cBlock component = gameObject.GetComponent<cBlock>();
			component.SetSkin(blockSkins[selectBlockSkin], selectBlockSkin);
			component.Check();
			gameObject.transform.SetParent(mapRoot.transform);
		}
	}

	public void AddBlock(Vector3 pos, int skin, bool check = true)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(block, pos, Quaternion.identity);
		cBlock component = gameObject.GetComponent<cBlock>();
		component.SetSkin(blockSkins[skin], skin);
		if (check)
		{
			component.Check();
		}
		gameObject.transform.SetParent(mapRoot.transform);
	}

	public IEnumerator AddBlockAsync(JSONArray jsonArray)
	{
		isBlockAsync = true;
		for (int i = 0; i < jsonArray.Length; i++)
		{
			yield return new WaitForSeconds(0.005f);
			JSONObject json = jsonArray[i].Obj;
			GameObject go = UnityEngine.Object.Instantiate(block, Utils.GetVector3(json.GetString("p")), Quaternion.identity);
			cBlock sc = go.GetComponent<cBlock>();
			int skin = (int)json.GetNumber("s");
			sc.SetSkin(blockSkins[skin], skin);
			sc.Check();
			go.transform.SetParent(mapRoot.transform);
		}
		isBlockAsync = false;
	}

	private void DeleteBlock()
	{
		Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
		if (Physics.Raycast(ray, out RaycastHit hitInfo, 5f) && hitInfo.transform.CompareTag("Block"))
		{
			hitInfo.transform.parent.SendMessage("DeleteBlock", SendMessageOptions.DontRequireReceiver);
		}
	}

	public void ClearMap()
	{
		Transform[] componentsInChildren = mapRoot.GetComponentsInChildren<Transform>();
		for (int i = 1; i < componentsInChildren.Length; i++)
		{
			UnityEngine.Object.Destroy(componentsInChildren[i].gameObject);
		}
	}

	public void SelectBlock(Transform skin)
	{
		SelectBlock(int.Parse(skin.name));
	}

	public void SelectBlock(int skin)
	{
		if (skin <= -1)
		{
			skin = blockSkins.Length - 1;
		}
		else if (skin >= blockSkins.Length)
		{
			skin = 0;
		}
		selectBlockSkin = skin;
		selectBlockButton.mainTexture = blockSkins[selectBlockSkin].mainTexture;
	}
}
