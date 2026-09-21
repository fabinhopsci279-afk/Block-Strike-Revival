using UnityEngine;

public class mSelectElement : MonoBehaviour
{
	public enum ParseList
	{
		Int,
		String
	}

	public ParseList parse;

	public string defaultValue;

	[HideInInspector]
	public int valueInt;

	[HideInInspector]
	public string valueString;

	[HideInInspector]
	public GameObject selectElement;

	public GameObject selectUI;

	public float duration;

	private void Start()
	{
		if (parse == ParseList.Int)
		{
			valueInt = int.Parse(defaultValue);
		}
		else
		{
			valueString = defaultValue;
		}
	}

	public void SetElement(GameObject go)
	{
		TweenPosition.Begin(selectUI, duration, go.transform.localPosition);
		selectElement = go;
		if (parse == ParseList.Int)
		{
			valueInt = int.Parse(selectElement.name);
		}
		else
		{
			valueString = selectElement.name;
		}
	}
}
