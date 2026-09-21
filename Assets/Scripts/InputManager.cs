using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public class InputButton
	{
		private bool pressed;

		private int lastPressedFrame = -5;

		private int lastReleasedFrame = -5;

		private KeyCode keyCode;

		public InputButton(KeyCode key)
		{
			keyCode = key;
		}

		public void Pressed()
		{
			if (!pressed)
			{
				pressed = true;
				lastPressedFrame = Time.frameCount;
			}
		}

		public void Released()
		{
			pressed = false;
			lastReleasedFrame = Time.frameCount;
		}

		public bool GetButton()
		{
			return pressed;
		}

		public bool GetButtonDown()
		{
			return lastPressedFrame - Time.frameCount == -1;
		}

		public bool GetButtonUp()
		{
			return lastReleasedFrame - Time.frameCount == -1;
		}
	}

	public class InputAxis
	{
		public float value;

		private string name;

		public InputAxis(string n)
		{
			name = n;
		}

		public void SetAxis(float v)
		{
			value = v;
		}

		public float GetAxis()
		{
			return value;
		}
	}

	private static Dictionary<string, InputButton> buttons = new Dictionary<string, InputButton>();

	private static Dictionary<string, InputAxis> axes = new Dictionary<string, InputAxis>();

	private static InputManager instance;

	private void Start()
	{
		instance = this;
		Object.DontDestroyOnLoad(base.gameObject);
	}

	public static void Init()
	{
		if (instance == null)
		{
			GameObject gameObject = new GameObject("InputManager");
			gameObject.AddComponent<InputManager>();
		}
	}

	public static void AddButton(string name, KeyCode key = KeyCode.None)
	{
		buttons.Add(name, new InputButton(key));
	}

	public static void SetButtonDown(string name)
	{
		if (!buttons.ContainsKey(name))
		{
			AddButton(name);
		}
		buttons[name].Pressed();
	}

	public static void SetButtonUp(string name)
	{
		if (!buttons.ContainsKey(name))
		{
			AddButton(name);
		}
		buttons[name].Released();
	}

	public static bool GetButton(string name)
	{
		if (!buttons.ContainsKey(name))
		{
			AddButton(name);
		}
		bool button = buttons[name].GetButton();
		if (!button)
		{
			if (name == "Jump" && UnityEngine.Input.GetKey(KeyCode.Space))
			{
				return true;
			}
			if (name == "Fire" && UnityEngine.Input.GetKey(KeyCode.Mouse0))
			{
				return true;
			}
			return false;
		}
		return button;
	}

	public static bool GetButtonDown(string name)
	{
		if (!buttons.ContainsKey(name))
		{
			AddButton(name);
		}
		bool buttonDown = buttons[name].GetButtonDown();
		if (!buttonDown)
		{
			if (name == "Fire" && UnityEngine.Input.GetKey(KeyCode.Mouse0))
			{
				return true;
			}
			if (name == "Jump" && UnityEngine.Input.GetKey(KeyCode.Space))
			{
				return true;
			}
			if (name == "Aim" && UnityEngine.Input.GetKeyDown(KeyCode.Mouse1))
			{
				return true;
			}
			if (name == "Reload" && UnityEngine.Input.GetKeyDown(KeyCode.R))
			{
				return true;
			}
			if (name == "Chat" && (UnityEngine.Input.GetKeyDown(KeyCode.T) || UnityEngine.Input.GetKeyDown(KeyCode.Y)))
			{
				return true;
			}
			if (name == "SelectWeapon" && Input.mouseScrollDelta.magnitude > 0.9f)
			{
				return true;
			}
			return false;
		}
		return buttonDown;
	}

	public static bool GetButtonUp(string name)
	{
		if (!buttons.ContainsKey(name))
		{
			AddButton(name);
		}
		return buttons[name].GetButtonUp();
	}

	public static void AddAxis(string name)
	{
		axes.Add(name, new InputAxis(name));
	}

	public static void SetAxis(string name, float value)
	{
		if (!axes.ContainsKey(name))
		{
			AddAxis(name);
		}
		axes[name].SetAxis(value);
	}

	public static float GetAxis(string name)
	{
		if (!axes.ContainsKey(name))
		{
			AddAxis(name);
		}
		return axes[name].GetAxis();
	}
}
