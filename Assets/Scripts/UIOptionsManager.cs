using UnityEngine;

public class UIOptionsManager : MonoBehaviour
{
	public UISlider Sensitivity;

	public UIToggle FPSMeter;

	public UIToggle Console;

	public UIToggle Sound;

	public UIToggle ShowDamage;

	public UIToggle Chat;

	private void Start()
	{
		LoadData();
	}

	private void LoadData()
	{
		Sensitivity.value = SaveLoadManager.GetSensitivity();
		FPSMeter.value = SaveLoadManager.GetFPSMeter();
		Console.value = SaveLoadManager.GetConsole();
		Sound.value = SaveLoadManager.GetSound();
		ShowDamage.value = SaveLoadManager.GetShowDamage();
		Chat.value = SaveLoadManager.GetChat();
		Utils.SetActiveConsole(Console.value);
	}

	public void SaveData()
	{
		SaveLoadManager.SetSensitivity(Sensitivity.value);
		SaveLoadManager.SetFPSMeter(FPSMeter.value);
		SaveLoadManager.SetConsole(Console.value);
		SaveLoadManager.SetSound(Sound.value);
		SaveLoadManager.SetShowDamage(ShowDamage.value);
		SaveLoadManager.SetChat(Chat.value);
		Utils.SetActiveConsole(Console.value);
		EventManager.Dispatch("UpdateOptions");
	}

	public void DefaultData()
	{
		SaveLoadManager.SetSensitivity(0.2f);
		SaveLoadManager.SetFPSMeter(active: false);
		SaveLoadManager.SetConsole(active: false);
		SaveLoadManager.SetSound(active: true);
		SaveLoadManager.SetShowDamage(active: false);
		SaveLoadManager.SetChat(active: true);
		LoadData();
		EventManager.Dispatch("UpdateOptions");
	}
}
