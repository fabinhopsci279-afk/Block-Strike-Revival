using UnityEngine;

public class Logo : MonoBehaviour
{
	public GameObject RexetPanel;

	public GameObject RedictPanel;

	private void Start()
	{
		Screen.sleepTimeout = -1;
		Utils.SetActiveConsole(SaveLoadManager.GetConsole());
		OnAnimation();
	}

	private void OnAnimation()
	{
		vp_Timer.In(0.5f, delegate
		{
			TweenAlpha.Begin(RexetPanel, 1f, 1f);
			vp_Timer.In(1.8f, delegate
			{
				TweenAlpha.Begin(RexetPanel, 1f, 0f);
				vp_Timer.In(1.5f, delegate
				{
					TweenAlpha.Begin(RedictPanel, 1f, 1f);
					vp_Timer.In(1.8f, delegate
					{
						TweenAlpha.Begin(RedictPanel, 1f, 0f);
						vp_Timer.In(1.2f, delegate
						{
							UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
						});
					});
				});
			});
		});
	}
}
