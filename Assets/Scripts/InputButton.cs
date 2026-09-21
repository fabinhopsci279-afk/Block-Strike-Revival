using UnityEngine;

public class InputButton : MonoBehaviour
{
	public string button;

	public float fadeAlpha = 0.5f;

	public float fadeDuration = 0.1f;

	private bool mPressed;

	private GameObject mGameObject;

	private void Start()
	{
		mGameObject = base.gameObject;
#if UNITY_STANDALONE
		mGameObject.SetActive(value: false);
		return;
#else
		EventManager.AddListener("SaveButton", OnSetPosition);
		OnSetPosition();
#endif
	}

	private void OnEnable()
	{
		if (fadeAlpha != 0f && mGameObject != null)
		{
			TweenAlpha.Begin(mGameObject, fadeDuration, 1f);
		}
	}

	private void OnDisable()
	{
		if (mPressed)
		{
			InputManager.SetButtonUp(button);
		}
	}

	private void OnPress(bool pressed)
	{
		mPressed = pressed;
		if (pressed)
		{
			if (fadeAlpha != 0f)
			{
				TweenAlpha.Begin(mGameObject, fadeDuration, fadeAlpha);
			}
			InputManager.SetButtonDown(button);
		}
		else
		{
			if (fadeAlpha != 0f)
			{
				TweenAlpha.Begin(mGameObject, fadeDuration, 1f);
			}
			InputManager.SetButtonUp(button);
		}
	}

	private void OnSetPosition()
	{
		vp_Timer.In(0.1f, delegate
		{
			if (PlayerPrefs.HasKey("Button" + button))
			{
				UISprite component = mGameObject.GetComponent<UISprite>();
				component.cachedTransform.localPosition = Utils.GetVector3(PlayerPrefs.GetString("Button" + button));
				int num2 = component.height = (component.width = PlayerPrefs.GetInt("ButtonSize" + button));
			}
		});
	}
}
