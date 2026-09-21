using DG.Tweening;
using UnityEngine;

public class TrapRotation : MonoBehaviour
{
	[Range(1f, 20f)]
	public int Key = 1;

	public Transform Target;

	public Vector3 RotationIn;

	public Vector3 RotationOut;

	public RotateMode Mode;

	public float Duration;

	public float DelayIn;

	public float DelayOut = 3f;

	private Tweener Tween;

	public bool Activated;

	private void Start()
	{
		if (Target == null)
		{
			Target = base.transform;
		}
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", StartRound);
		EventManager.AddListener("Button" + Key, ActiveTrap);
	}

	[ContextMenu("Get Position")]
	private void GetValue()
	{
		RotationIn = Target.localEulerAngles;
		RotationOut = Target.localEulerAngles;
	}

	private void ActiveTrap()
	{
		if (!Activated)
		{
			Tween = Target.DORotate(RotationOut, Duration, Mode).OnComplete(ResetTrap).SetDelay(DelayIn);
			Activated = true;
		}
	}

	private void ResetTrap()
	{
		if (Activated && DelayOut != 0f)
		{
			Tween = Target.DORotate(RotationIn, Duration, Mode).SetDelay(DelayOut);
		}
	}

	private void StartRound()
	{
		if (Tween != null)
		{
			Tween.Kill();
		}
		Target.localEulerAngles = RotationIn;
		Activated = false;
	}
}
