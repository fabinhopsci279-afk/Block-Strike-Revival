using DG.Tweening;
using UnityEngine;

public class UICrosshair : MonoBehaviour
{
	public Transform LeftSprite;

	public Transform RightSprite;

	public Transform TopSprite;

	public Transform BottomSprite;

	public float TargetAccuracy;

	public float MaxAccuracy;

	public float Accuracy;

	public float Duration;

	private Vector2 ScreenSize;

	private Tweener Tween;

	[Header("Hit Settings")]
	public UIWidget HitSprite;

	public float HitDuration;

	private Tweener HitTweener;

	[Header("Rifle Scope")]
	public GameObject RifleScope;

	private bool isScope;

	private int SelectColor;

	private static UICrosshair instance;

	private void Awake()
	{
		instance = this;
		UpdateCrosshair();
		ScreenSize = new Vector2(Screen.width * 2, Screen.height * 2);
	}

	private void Start()
	{
		EventManager.AddListener("UpdateOptions", UpdateColor);
		UpdateColor();
	}

	public static void SetAccuracy(float accuracy)
	{
		instance.TargetAccuracy = accuracy;
		instance.Accuracy = accuracy;
		instance.UpdateCrosshair();
	}

	public static Vector2 Fire(float accuracy)
	{
		Vector2 result = Vector3.zero;
		if (!instance.isScope)
		{
			result = new Vector2(instance.Accuracy / instance.ScreenSize.x, instance.Accuracy / instance.ScreenSize.y);
			instance.Accuracy += accuracy;
			instance.Accuracy = Mathf.Min(instance.Accuracy, instance.MaxAccuracy);
			instance.UpdateCrosshair();
		}
		return result;
	}

	private void UpdateCrosshair()
	{
		if (Tween != null && Tween.IsActive())
		{
			Tween.Kill();
		}
		Tween = DOTween.To(() => Accuracy, delegate(float x)
		{
			Accuracy = x;
		}, TargetAccuracy, Duration).OnUpdate(delegate
		{
			LeftSprite.localPosition = Vector3.left * Accuracy;
			RightSprite.localPosition = Vector3.right * Accuracy;
			TopSprite.localPosition = Vector3.up * Accuracy;
			BottomSprite.localPosition = Vector3.down * Accuracy;
		});
	}

	public static void Hit()
	{
		if (instance.HitTweener != null && instance.HitTweener.IsActive())
		{
			instance.HitTweener.Kill();
		}
		instance.HitSprite.alpha = 1f;
		instance.HitTweener = DOTween.To(() => instance.HitSprite.alpha, delegate(float x)
		{
			instance.HitSprite.alpha = x;
		}, 0f, instance.HitDuration);
	}

	public static void SetActiveRifleScope(bool active)
	{
		instance.isScope = active;
		instance.RifleScope.SetActive(active);
		SetActiveCrosshair(!active);
	}

	public static void SetActiveCrosshair(bool active)
	{
		instance.LeftSprite.gameObject.SetActive(active);
		instance.RightSprite.gameObject.SetActive(active);
		instance.TopSprite.gameObject.SetActive(active);
		instance.BottomSprite.gameObject.SetActive(active);
	}

	private void UpdateColor()
	{
		int colorCrosshair = SaveLoadManager.GetColorCrosshair();
		if (SelectColor != colorCrosshair)
		{
			SelectColor = colorCrosshair;
			Color color = Utils.GetColor(colorCrosshair);
			LeftSprite.GetComponent<UISprite>().color = color;
			RightSprite.GetComponent<UISprite>().color = color;
			TopSprite.GetComponent<UISprite>().color = color;
			BottomSprite.GetComponent<UISprite>().color = color;
		}
	}
}
