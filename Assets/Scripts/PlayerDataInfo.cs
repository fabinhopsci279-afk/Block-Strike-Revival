using System.Collections.Generic;

public class PlayerDataInfo
{
	public class Weapon
	{
		public int ID;

		public bool Selected;

		public bool Buy;

		public int Upgrade;

		public List<int> Skins = new List<int>();

		public int SelectedSkin;
	}

	public PhotonPlayer photonData;

	public string Name;

	public int Money;

	public int Gold;

	public int XP;

	public int Level;

	public int OpenCase;

	public int Deaths;

	public int Kills;

	public int Headshot;

	public int PlayerSkin;

	public double CaseTime;

	public float Sensitivity;

	public bool Sound;

	public bool FPSMeter;

	public bool Console;

	public bool Chat;

	public bool ShowDamage;

	public int ColorCrosshair;

	public List<Weapon> Weapons = new List<Weapon>();
}
