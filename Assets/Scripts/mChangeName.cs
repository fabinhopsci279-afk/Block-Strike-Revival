using UnityEngine;

public class mChangeName : MonoBehaviour
{
	private void Start()
	{
		EventManager.AddListener("NextGameEvent2", FirstName);
	}

	private void FirstName()
	{
		mPopUp.ShowInput(SaveLoadManager.GetPlayerName(), Localization.Get("ChangeName"), 12, UIInput.KeyboardType.Default, OnFirstSubmit, Localization.Get("Back"), null, "Ok", OnSaveName);
	}

	private void OnSaveName()
	{
		SaveLoadManager.SetPlayerName(mPopUp.GetInputText());
		EventManager.Dispatch("UpdatePlayerName");
		mPopUp.HidePopUp("Menu");
	}

	private void OnFirstSubmit()
	{
		string text = mPopUp.GetInputText();
		if (text.Length <= 3)
		{
			text = "Player " + UnityEngine.Random.Range(0, 9999);
		}
		text = NGUIText.StripSymbols(text);
		mPopUp.SetInputText(text);
	}

	public void ChangeName()
	{
		if (PlayerLevelManager.GetPlayerLevel() < 10)
		{
			UIToast.Show(Localization.Get("Requires Level") + " 10");
		}
		else
		{
			mPopUp.ShowPopUp(Localization.Get("Cost of change name 100 gold"), Localization.Get("ChangeName"), Localization.Get("Back"), ChangeNameCancel, "Ok", ChangeNameInput);
		}
	}

	private void ChangeNameInput()
	{
		if (SaveLoadManager.GetGold() < 100)
		{
			UIToast.Show(Localization.Get("Not enough money"));
		}
		else
		{
			mPopUp.ShowInput(SaveLoadManager.GetPlayerName(), Localization.Get("ChangeName"), 12, UIInput.KeyboardType.Default, OnSubmit, Localization.Get("Back"), ChangeNameCancel, "Ok", OnYes);
		}
	}

	private void ChangeNameCancel()
	{
		mPopUp.HidePopUp(string.Empty);
	}

	private void OnSubmit()
	{
		string text = mPopUp.GetInputText();
		if (text.Length <= 3)
		{
			text = "Player " + UnityEngine.Random.Range(0, 9999);
		}
		text = NGUIText.StripSymbols(text);
		mPopUp.SetInputText(text);
	}

	private void OnYes()
	{
		SaveLoadManager.SetPlayerName(mPopUp.GetInputText());
		SaveLoadManager.SetGold1(-100);
		EventManager.Dispatch("UpdatePlayerName");
		EventManager.Dispatch("UpdateMoney");
		mPopUp.HidePopUp(string.Empty);
	}
}
