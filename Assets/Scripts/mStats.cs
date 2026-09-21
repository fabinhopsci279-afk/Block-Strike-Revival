using UnityEngine;

public class mStats : MonoBehaviour
{
	public UILabel NameLabel;

	public UILabel LevelLabel;

	public UILabel XPLabel;

	public UILabel DeathsLabel;

	public UILabel KillsLabel;

	public UILabel HeadshotKillsLabel;

	public UILabel OpenCaseLabel;

	public UILabel TotalSkinLabel;

	public UILabel LegendarySkinLabel;

	public UILabel ProfessionalSkinLabel;

	public UILabel BasicSkinLabel;

	public UILabel NormalSkinLabel;

	public void UpdateData()
	{
		NameLabel.text = SaveLoadManager.GetPlayerName();
		LevelLabel.text = SaveLoadManager.GetPlayerLevel().ToString();
		XPLabel.text = PlayerLevelManager.GetPlayerXP() + "/" + PlayerLevelManager.GetMaxPlayerXP();
		DeathsLabel.text = SaveLoadManager.GetDeaths().ToString();
		KillsLabel.text = SaveLoadManager.GetKills().ToString();
		HeadshotKillsLabel.text = SaveLoadManager.GetHeadshot().ToString();
		OpenCaseLabel.text = SaveLoadManager.GetOpenCase().ToString();
		TotalSkinLabel.text = GetOpenSkins();
		LegendarySkinLabel.text = GetOpenSkins(4);
		ProfessionalSkinLabel.text = GetOpenSkins(3);
		BasicSkinLabel.text = GetOpenSkins(2);
		NormalSkinLabel.text = GetOpenSkins(1);
	}

	private string GetOpenSkins(int rarity = 0)
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < GameSettings.instance.Weapons.Count; i++)
		{
			for (int j = 1; j < GameSettings.instance.WeaponsShop[i].Skins.Count; j++)
			{
				if ((int)GameSettings.instance.WeaponsShop[i].Skins[j].Rarity == rarity || rarity == 0)
				{
					num2++;
					if (SaveLoadManager.GetWeaponSkin(i + 1, j))
					{
						num++;
					}
				}
			}
		}
		return num + "/" + num2;
	}
}
