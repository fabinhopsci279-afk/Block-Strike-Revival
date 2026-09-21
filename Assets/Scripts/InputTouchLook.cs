using UnityEngine;

public class InputTouchLook : MonoBehaviour
{
	private Rect touchZone = new Rect(50f, 0f, 50f, 100f);

	private bool move;

	private int id = -1;

	private float dpi;

	private Vector2 value;

	private void Start()
	{
		dpi = Screen.dpi / 100f;
		if (dpi == 0f)
		{
			dpi = 1.6f;
		}
		touchZone = NewRect(touchZone);
	}

	private void OnDisable()
	{
		UpdateValue(Vector2.zero);
		id = -1;
		move = false;
	}

	private void Update()
	{
		for (int i = 0; i < UnityEngine.Input.touchCount; i++)
		{
			Touch touch = UnityEngine.Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began)
			{
				ref Rect reference = ref touchZone;
				Vector2 position = touch.position;
				float x = position.x;
				float num = Screen.height;
				Vector2 position2 = touch.position;
				if (reference.Contains(new Vector3(x, num - position2.y, 0f)))
				{
					move = true;
					id = touch.fingerId;
				}
			}
			if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && id == touch.fingerId && move)
			{
				Vector2 zero = Vector2.zero;
				zero = touch.deltaPosition / dpi;
				UpdateValue(zero);
			}
			if ((touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended) && id == touch.fingerId && move)
			{
				id = -1;
				move = false;
				UpdateValue(Vector2.zero);
			}
		}
	}

	private void UpdateValue(Vector2 v)
	{
		value = v;
		InputManager.SetAxis("Mouse X", value.x);
		InputManager.SetAxis("Mouse Y", value.y);
	}

	private Rect NewRect(Rect rect)
	{
		float x = (float)Screen.width * rect.x / 100f;
		float y = (float)Screen.height * rect.y / 100f;
		float width = (float)Screen.width * rect.width / 100f;
		float height = (float)Screen.height * rect.height / 100f;
		return new Rect(x, y, width, height);
	}
}
