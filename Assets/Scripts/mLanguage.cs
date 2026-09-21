using System;
using UnityEngine;

public class mLanguage : MonoBehaviour
{
	[Serializable]
	public class LanguageClass
	{
		public string language;

		public Texture2D Texture;
	}

	public LanguageClass[] languages;

	public UITexture Button;

	private int selectLanguage;

	private void Awake()
	{
		if (PlayerPrefs.HasKey("Language"))
		{
			SetLanguage(PlayerPrefs.GetString("Language"));
		}
		else if (Application.systemLanguage == SystemLanguage.Russian || Application.systemLanguage == SystemLanguage.Ukrainian)
		{
			SetLanguage("Russia");
		}
		else if (Application.systemLanguage == SystemLanguage.English)
		{
			SetLanguage("English");
		}
		else if (Application.systemLanguage == SystemLanguage.Korean)
		{
			SetLanguage("Korean");
		}
		else if (Application.systemLanguage == SystemLanguage.Spanish)
		{
			SetLanguage("Spanish");
		}
		else if (Application.systemLanguage == SystemLanguage.Portuguese)
		{
			SetLanguage("Portuguese");
		}
		else if (Application.systemLanguage == SystemLanguage.French)
		{
			SetLanguage("French");
		}
		else if (Application.systemLanguage == SystemLanguage.Japanese)
		{
			SetLanguage("Japan");
		}
	}

	private void SetLanguage(string language)
	{
		int num = 0;
		while (true)
		{
			if (num < languages.Length)
			{
				if (languages[num].language == language)
				{
					break;
				}
				num++;
				continue;
			}
			return;
		}
		Localization.language = languages[num].language;
		Button.mainTexture = languages[num].Texture;
		selectLanguage = num;
	}

	public void SelectLanguage()
	{
		if (selectLanguage < languages.Length - 1)
		{
			selectLanguage++;
		}
		else
		{
			selectLanguage = 0;
		}
		LanguageClass languageClass = languages[selectLanguage];
		Localization.language = languageClass.language;
		Button.mainTexture = languageClass.Texture;
	}
}
