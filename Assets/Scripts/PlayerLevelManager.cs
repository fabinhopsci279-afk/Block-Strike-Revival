using UnityEngine;

public static class PlayerLevelManager
{
	public static void UpdatePlayerXP(int xp)
	{
		int playerLevel = GetPlayerLevel();
		int num = GetPlayerXP() + xp;
		int maxPlayerXP = GetMaxPlayerXP();
		if (num >= maxPlayerXP)
		{
			if (playerLevel == 100)
			{
				num = maxPlayerXP;
				maxPlayerXP = 150 + 150 * playerLevel;
			}
			else
			{
				playerLevel++;
				num -= maxPlayerXP;
				num = Mathf.Max(num, 0);
				maxPlayerXP = 150 + 150 * playerLevel;
				SaveLoadManager.SetPlayerLevel(playerLevel);
				EventManager.Dispatch("LevelUp");
			}
		}
		SaveLoadManager.SetPlayerXP(num);
	}

	public static int GetPlayerLevel()
	{
		return SaveLoadManager.GetPlayerLevel();
	}

	public static int GetPlayerXP()
	{
		return SaveLoadManager.GetPlayerXP();
	}

	public static int GetMaxPlayerXP()
	{
		return 150 + 150 * GetPlayerLevel();
	}

	public static float GetPlayerXPPercent()
	{
		return (float)GetPlayerXP() / (float)GetMaxPlayerXP();
	}
}
