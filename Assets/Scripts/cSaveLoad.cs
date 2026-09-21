using Boomlagoon.JSON;
using System.Collections.Generic;
using UnityEngine;

public class cSaveLoad : MonoBehaviour
{
	public cBlockPlacer blockPlacer;

	public UIScrollView scrollView;

	public GameObject scrollButton;

	private List<GameObject> mapList = new List<GameObject>();

	private string selectMap;

	private void Update()
	{
		if (UnityEngine.Input.GetKeyDown(KeyCode.F5))
		{
			Save();
		}
		if (UnityEngine.Input.GetKeyDown(KeyCode.F6))
		{
			Load();
		}
	}

	private void Save(string saveName = "AutoSave")
	{
		JSONArray jSONArray = new JSONArray();
		cBlock[] componentsInChildren = blockPlacer.mapRoot.GetComponentsInChildren<cBlock>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			jSONArray.Add(new JSONObject
			{
				{
					"p",
					componentsInChildren[i].pos.ToString("f0")
				},
				{
					"s",
					componentsInChildren[i].selectSkin
				}
			});
		}
		Utils.SaveFile("LevelCreator", saveName + ".map", jSONArray.ToString());
	}

	private void Load(string loadName = "AutoSave")
	{
		string jsonString = Utils.LoadFile("LevelCreator", loadName + ".map");
		JSONArray value = JSONArray.Parse(jsonString);
		blockPlacer.ClearMap();
		blockPlacer.StartCoroutine("AddBlockAsync", value);
	}

	public void LoadMapList()
	{
		if (Utils.ExistsDirectory("LevelCreator"))
		{
			string[] files = Utils.GetFiles("LevelCreator", "*.map");
			for (int i = 0; i < mapList.Count; i++)
			{
				UnityEngine.Object.Destroy(mapList[i]);
			}
			mapList.Clear();
			for (int j = 0; j < files.Length; j++)
			{
				GameObject gameObject = NGUITools.AddChild(scrollView.gameObject, scrollButton);
				gameObject.SetActive(value: true);
				gameObject.transform.localPosition = Vector3.up * (130 - 30 * j);
				gameObject.name = Utils.GetFileName(files[j]);
				gameObject.GetComponent<UILabel>().text = gameObject.name;
				mapList.Add(gameObject);
			}
		}
	}

	public void LoadMap(Transform t)
	{
		selectMap = t.name;
	}

	private void LoadClick()
	{
		Load(selectMap);
		mPopUp.HidePopUp("Save/Load");
	}

	private void CancelLoadClick()
	{
		mPopUp.HidePopUp("Save/Load");
	}

	public void SaveMap()
	{
		mPopUp.ShowInput(string.Empty, "Введите название карты", 10, UIInput.KeyboardType.Default, SubmitClick, "Назад", CancelSaveClick, "Сохранить", SaveClick);
	}

	private void SubmitClick()
	{
		if (Utils.ExistsFile("LevelCreator", mPopUp.GetInputText() + ".map"))
		{
			UIToast.Show("Название уже занято");
		}
	}

	private void SaveClick()
	{
		if (Utils.ExistsFile("LevelCreator", mPopUp.GetInputText() + ".map"))
		{
			UIToast.Show("Название уже занято");
			return;
		}
		Save(mPopUp.GetInputText());
		mPopUp.HidePopUp("Save/Load");
		LoadMapList();
	}

	private void CancelSaveClick()
	{
		mPopUp.HidePopUp("Save/Load");
	}
}
