public class AchievementsManager
{
	public static void UpdateLevel()
	{
		switch (PlayerLevelManager.GetPlayerLevel())
		{
		}
	}

	public static void UpdateMoney()
	{
		if (SaveLoadManager.GetMoney() > 10000)
		{
		}
		if (SaveLoadManager.GetGold() <= 1000)
		{
		}
	}

	public static void UpdateKills(DamageInfo damageInfo)
	{
		if (damageInfo.HeadShot)
		{
		}
		if (PlayerInput.instance.mCharacterController.isGrounded)
		{
		}
	}
}
