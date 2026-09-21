using UnityEngine;

public class TrapEnable : MonoBehaviour
{
	[Range(1f, 20f)]
	public int Key = 1;

	public GameObject Target;

	public bool Value;

	public float DelayIn;

	public float DelayOut = 3f;

	private bool Activated;

	private void Start()
	{
		if (Target == null)
		{
			Target = base.gameObject;
		}
		EventManager.AddListener("StartRound", StartRound);
		EventManager.AddListener("WaitPlayer", StartRound);
		EventManager.AddListener("Button" + Key, ActiveTrap);
	}

	private void ActiveTrap()
	{
		if (!Activated)
		{
			Activated = true;
			vp_Timer.In(DelayIn, delegate
			{
				Target.SetActive(Value);
				if (DelayOut != 0f)
				{
					vp_Timer.In(DelayOut, delegate
					{
						if (Activated)
						{
							Target.SetActive(!Value);
						}
					});
				}
			});
		}
	}

	private void StartRound()
	{
		Target.SetActive(!Value);
		Activated = false;
	}
}
