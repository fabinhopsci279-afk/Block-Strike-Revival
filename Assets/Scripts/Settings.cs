using System;
using UnityEngine;

// Token: 0x02000457 RID: 1111
public class Settings
{
	// Token: 0x06002510 RID: 9488 RVA: 0x000C10B4 File Offset: 0x000BF2B4
	public static void Load()
	{
		Settings.Font = Settings.GetInt("Font", 0);
		Settings.FPSMeter = Settings.GetBool("FPSMeter", false);
		Settings.Console = Settings.GetBool("Console", false);
		Settings.Chat = Settings.GetBool("Chat", true);
		Settings.ShowDamage = Settings.GetBool("ShowDamage", false);
		Settings.BulletHole = Settings.GetBool("BulletHole", true);
		Settings.Blood = Settings.GetBool("Blood", true);
		Settings.HitMarker = Settings.GetBool("HitMarker", true);
		Settings.ColorCrosshair = Settings.GetInt("ColorCrosshair", 0);
		Settings.RenderDistance = Settings.GetFloat("RenderDistance", 0.2f);
		Settings.Sensitivity = Settings.GetFloat("Sensitivity", 0.2f);
		Settings.Volume = Settings.GetFloat("Volume", 0.8f);
		Settings.Sound = Settings.GetBool("Sound", true);
		Settings.AmbientSound = Settings.GetBool("AmbientSound", true);
		Settings.Lefty = Settings.GetBool("Lefty", false);
		Settings.ButtonAlpha = Mathf.Clamp(Settings.GetFloat("ButtonAlpha", 1f), 0.01f, 1f);
		Settings.HUD = Settings.GetBool("HUD", true);
		Settings.ShowWeapon = Settings.GetBool("ShowWeapon", true);
		Settings.Shell = Settings.GetBool("Shell", true);
		Settings.ProjectileEffect = Settings.GetBool("ProjectileEffect", true);
		Settings.SmokePlume = Settings.GetBool("SmokePlume", true);
		AudioListener.volume = Settings.Volume;
	}

	// Token: 0x06002511 RID: 9489 RVA: 0x000C123C File Offset: 0x000BF43C
	public static void Save()
	{
		Settings.SetInt("Font", Settings.Font);
		Settings.SetBool("FPSMeter", Settings.FPSMeter);
		Settings.SetBool("Console", Settings.Console);
		Settings.SetBool("Chat", Settings.Chat);
		Settings.SetBool("ShowDamage", Settings.ShowDamage);
		Settings.SetBool("BulletHole", Settings.BulletHole);
		Settings.SetBool("Blood", Settings.Blood);
		Settings.SetBool("HitMarker", Settings.HitMarker);
		Settings.SetInt("ColorCrosshair", Settings.ColorCrosshair);
		Settings.SetFloat("RenderDistance", Settings.RenderDistance);
		Settings.SetFloat("Sensitivity", Settings.Sensitivity);
		Settings.SetFloat("Volume", Settings.Volume);
		Settings.SetBool("Sound", Settings.Sound);
		Settings.SetBool("AmbientSound", Settings.AmbientSound);
		Settings.SetBool("Lefty", Settings.Lefty);
		Settings.SetFloat("ButtonAlpha", Mathf.Clamp(Settings.ButtonAlpha, 0.01f, 1f));
        Settings.SetBool("HUD", Settings.HUD);
		Settings.SetBool("ShowWeapon", Settings.ShowWeapon);
		Settings.SetBool("Shell", Settings.Shell);
		Settings.SetBool("ProjectileEffect", Settings.ProjectileEffect);
		Settings.SetBool("SmokePlume", Settings.SmokePlume);
		AudioListener.volume = Settings.Volume;
	}

	// Token: 0x06002512 RID: 9490 RVA: 0x0001961C File Offset: 0x0001781C
	private static bool GetBool(string key, bool defaultValue)
	{
		if (PlayerPrefs.HasKey(key))
		{
			return PlayerPrefs.GetInt(key) == 1;
		}
		return defaultValue;
	}

	// Token: 0x06002513 RID: 9491 RVA: 0x00019635 File Offset: 0x00017835
	private static int GetInt(string key, int defaultValue)
	{
		return PlayerPrefs.GetInt(key, defaultValue);
	}

	// Token: 0x06002514 RID: 9492 RVA: 0x0001963E File Offset: 0x0001783E
	private static float GetFloat(string key, float defaultValue)
	{
		return PlayerPrefs.GetFloat(key, defaultValue);
	}

	// Token: 0x06002515 RID: 9493 RVA: 0x00019647 File Offset: 0x00017847
	private static void SetBool(string key, bool value)
	{
		PlayerPrefs.SetInt(key, (!value) ? 0 : 1);
	}

	// Token: 0x06002516 RID: 9494 RVA: 0x00019656 File Offset: 0x00017856
	private static void SetInt(string key, int value)
	{
		PlayerPrefs.SetInt(key, value);
	}

	// Token: 0x06002517 RID: 9495 RVA: 0x0001965F File Offset: 0x0001785F
	private static void SetFloat(string key, float value)
	{
		PlayerPrefs.SetFloat(key, value);
	}

	// Token: 0x040016EF RID: 5871
	public static int Font;

	// Token: 0x040016F0 RID: 5872
	public static bool FPSMeter;

	// Token: 0x040016F1 RID: 5873
	public static bool Console;

	// Token: 0x040016F2 RID: 5874
	public static bool Chat = true;

	// Token: 0x040016F3 RID: 5875
	public static bool ShowDamage;

	// Token: 0x040016F4 RID: 5876
	public static bool BulletHole = true;

	// Token: 0x040016F5 RID: 5877
	public static bool Blood = true;

	// Token: 0x040016F6 RID: 5878
	public static bool HitMarker = true;

	// Token: 0x040016F7 RID: 5879
	public static int ColorCrosshair;

	// Token: 0x040016F8 RID: 5880
	public static float RenderDistance = 0.2f;

	// Token: 0x040016F9 RID: 5881
	public static float Sensitivity = 0.2f;

	// Token: 0x040016FA RID: 5882
	public static float Volume = 0.8f;

	// Token: 0x040016FB RID: 5883
	public static bool Sound = true;

	// Token: 0x040016FC RID: 5884
	public static bool AmbientSound = true;

	// Token: 0x040016FD RID: 5885
	public static bool Lefty;

	// Token: 0x040016FE RID: 5886
	public static float ButtonAlpha = 1f;

	// Token: 0x040016FF RID: 5887
	public static bool HUD = true;

	// Token: 0x04001700 RID: 5888
	public static bool ShowWeapon = true;

	// Token: 0x04001701 RID: 5889
	public static bool Shell = true;

	// Token: 0x04001702 RID: 5890
	public static bool ProjectileEffect = true;

	// Token: 0x04001703 RID: 5891
	public static bool SmokePlume = true;
}
