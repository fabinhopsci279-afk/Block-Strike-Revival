using UnityEngine;

public class InputJoystick : MonoBehaviour
{
	public Camera uiCamera;

	public GameObject stick;

	public GameObject background;

	public float distance = 0.3f;

	private float positionMultiplier;

	private bool move;

	private int id = -1;

	private bool activeJoystick = true;

	private Rect touchZone;

	private Vector2 value;

	private void Start()
	{
#if UNITY_STANDALONE
		base.gameObject.SetActive(value: false);
		return;
#else
		positionMultiplier = 1f / distance;
		touchZone = new Rect(0f, 0f, Screen.width / 2, Screen.height / 2);
		Hide();
#endif
	}

	private void OnDisable()
	{
		Hide();
		move = false;
		id = -1;
		UpdateValue(Vector2.zero);
	}

	private void Update()
	{
		if (!activeJoystick)
		{
			return;
		}
		for (int i = 0; i < UnityEngine.Input.touchCount; i++)
		{
			Touch touch = UnityEngine.Input.GetTouch(i);
			if (touch.phase == TouchPhase.Began && touchZone.Contains(touch.position))
			{
				id = touch.fingerId;
				if (!move)
				{
					move = true;
				}
				Vector2 position = touch.position;
				position.x = Mathf.Clamp01(position.x / (float)Screen.width);
				position.y = Mathf.Clamp01(position.y / (float)Screen.height);
				background.transform.position = uiCamera.ViewportToWorldPoint(position);
				stick.transform.position = uiCamera.ViewportToWorldPoint(position);
				Show();
			}
			if ((touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary) && id == touch.fingerId)
			{
				Vector3 position2 = touch.position;
				position2.x = Mathf.Clamp01(position2.x / (float)Screen.width);
				position2.y = Mathf.Clamp01(position2.y / (float)Screen.height);
				stick.transform.position = uiCamera.ViewportToWorldPoint(position2);
				stick.transform.position = Clamp(background.transform.position, stick.transform.position);
				UpdateValue((stick.transform.position - background.transform.position) * positionMultiplier);
			}
			if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && id == touch.fingerId)
			{
				move = false;
				id = -1;
				Hide();
				UpdateValue(Vector2.zero);
			}
		}
	}

	private Vector3 Clamp(Vector3 p, Vector3 y)
	{
		if (Vector3.Distance(p, y) > distance)
		{
			Vector3 a = y - p;
			a.Normalize();
			return a * distance + p;
		}
		return y;
	}

	private void Show()
	{
		stick.SetActive(value: true);
		background.SetActive(value: true);
	}

	private void Hide()
	{
		stick.SetActive(value: false);
		background.SetActive(value: false);
	}

	private void UpdateValue(Vector2 v)
	{
		value = v;
		InputManager.SetAxis("Horizontal", value.x);
		InputManager.SetAxis("Vertical", value.y);
	}

	public void SetActiveJoystick(bool active)
	{
		activeJoystick = active;
		if (!activeJoystick)
		{
			OnDisable();
		}
	}
}
