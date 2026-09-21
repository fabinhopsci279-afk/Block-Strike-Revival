using Photon;
using System;
using UnityEngine;

public class UIGameManager : PunBehaviour
{
	public UILabel NameLabel;

	[Header("Health")]
	public UILabel HealthLabel;

	public GameObject HealthPanel;

	[Header("Ammo")]
	public UILabel AmmoLabel;

	public GameObject AmmoPanel;

	[Header("Score")]
	public UILabel MaxScoreLabel;

	public UILabel BlueScoreLabel;

	public UILabel RedScoreLabel;

	[HideInInspector]
	public bool isScoreTimer;

	[HideInInspector]
	public float ScoreTimer;

	private Action ScoreTimerAction;

	[Header("FPS Meter")]
	public UILabel FPSMeterLabel;

	private bool isFPSMeter;

	private float FPSMeterAccum;

	private float FPSMeterFrames;

	[Header("Controller")]
	public GameObject ControllerPanel;

	public GameObject RifleScopeButton;

	public GameObject SelectWeaponButton;

	private bool isExitServer;

	private vp_Timer.Handle ExitServerTimer = new vp_Timer.Handle();

	public static UIGameManager instance;

	private void Awake()
	{
		instance = this;
		PhotonClassesManager.Add(this);
	}

	private void Start()
	{
		EventManager.AddListener("UpdateOptions", UpdateOptions);
		UpdateOptions();
	}

	private void Update()
	{
		if (InputManager.GetButtonDown("Pause"))
		{
			UIPanelManager.ShowPanel("Pause");
		}
		if (isFPSMeter)
		{
			FPSMeterAccum += Time.timeScale / Time.deltaTime;
			FPSMeterFrames += 1f;
		}
		if (!isScoreTimer)
		{
			return;
		}
		float num = ScoreTimer - Time.time;
		int num2 = (int)num / 60;
		int num3 = (int)num - num2 * 60;
		MaxScoreLabel.text = $"{num2:0}:{num3:00}";
		if (ScoreTimer <= Time.time)
		{
			isScoreTimer = false;
			if (ScoreTimerAction != null)
			{
				ScoreTimerAction();
			}
		}
	}

	private void UpdateOptions()
	{
		isFPSMeter = SaveLoadManager.GetFPSMeter();
		CancelInvoke("UpdateFPSMeter");
		if (isFPSMeter)
		{
			InvokeRepeating("UpdateFPSMeter", 1f, 1f);
		}
		FPSMeterLabel.gameObject.SetActive(isFPSMeter);
	}

	private void UpdateFPSMeter()
	{
		float num = FPSMeterAccum / FPSMeterFrames;
		string text = $"{num:F2} FPS";
		FPSMeterAccum = 0f;
		FPSMeterFrames = 0f;
		FPSMeterLabel.text = text;
	}

	public static void SetHealth(int health)
	{
		if (health == 0)
		{
			instance.HealthLabel.text = string.Empty;
			instance.HealthPanel.SetActive(value: false);
			instance.AmmoLabel.text = string.Empty;
			instance.AmmoPanel.SetActive(value: false);
		}
		else
		{
			if (!instance.HealthPanel.activeSelf)
			{
				instance.HealthPanel.SetActive(value: true);
			}
			instance.HealthLabel.text = "+" + health.ToString();
		}
	}

	public static void SetAmmo(int ammo, int maxAmmo)
	{
		if (maxAmmo == -1)
		{
			instance.AmmoLabel.text = string.Empty;
			instance.AmmoPanel.SetActive(value: false);
			return;
		}
		if (!instance.AmmoPanel.activeSelf)
		{
			instance.AmmoPanel.SetActive(value: true);
		}
		instance.AmmoLabel.text = ammo + "/" + maxAmmo;
	}

	public static void SetActiveScore(bool active, int maxScore)
	{
		instance.MaxScoreLabel.gameObject.SetActive(active);
		instance.MaxScoreLabel.text = maxScore.ToString();
	}

	public static void UpdateScore(int maxScore, int blueScore, int redScore)
	{
		instance.MaxScoreLabel.text = maxScore.ToString();
		instance.BlueScoreLabel.text = blueScore.ToString();
		instance.RedScoreLabel.text = redScore.ToString();
	}

	public static void StartScoreTimer(float time, Action finishAction)
	{
		instance.isScoreTimer = true;
		instance.ScoreTimer = time + Time.time;
		instance.ScoreTimerAction = finishAction;
	}

	public static void StopScoreTimer()
	{
		instance.isScoreTimer = false;
	}

	public static void SetActiveRifleScope(bool active)
	{
		instance.RifleScopeButton.SetActive(active);
	}

	public static void SetActiveSelectWeapon(bool active)
	{
		instance.SelectWeaponButton.SetActive(active);
	}

	public void OnExitServer()
	{
		isExitServer = true;
		PhotonNetwork.LeaveRoom();
	}

	private new void OnLeftRoom()
	{
		if (isExitServer)
		{
			vp_Timer.In(0.2f, delegate
			{
				AdsManager.ShowVideo();
			}, ExitServerTimer);
			ExitServerTimer.CancelOnLoad = false;
		}
		UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
	}

	public void OnDefaultButton()
	{
		EventManager.Dispatch("DefaultButton");
	}

	public void OnSaveButton()
	{
		EventManager.Dispatch("SaveButton");
	}
}
