using Boomlagoon.JSON;
using UnityEngine;

public class DamageInfo
{
	public int Damage;

	public Vector3 AttackPosition;

	public Team AttackerTeam;

	public int WeaponID;

	public int PlayerID;

	public bool HeadShot;

	public static DamageInfo Create(int damage, Vector3 attackPosition, Team attackerTeam, int weaponID, int playerID)
	{
		DamageInfo damageInfo = new DamageInfo();
		damageInfo.Damage = damage;
		damageInfo.AttackPosition = attackPosition;
		damageInfo.AttackerTeam = attackerTeam;
		damageInfo.WeaponID = weaponID;
		damageInfo.PlayerID = playerID;
		return damageInfo;
	}

	public static string Deserialize(DamageInfo damageInfo)
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.Add("1", damageInfo.Damage);
		jSONObject.Add("2", damageInfo.AttackPosition.ToString("f0"));
		jSONObject.Add("3", (double)damageInfo.AttackerTeam);
		jSONObject.Add("4", damageInfo.WeaponID);
		jSONObject.Add("5", damageInfo.PlayerID);
		jSONObject.Add("6", damageInfo.HeadShot ? 1 : 0);
		return jSONObject.ToString();
	}

	public static DamageInfo Serialize(string text)
	{
		DamageInfo damageInfo = new DamageInfo();
		JSONObject jSONObject = JSONObject.Parse(text);
		damageInfo.Damage = (int)jSONObject.GetNumber("1");
		damageInfo.AttackPosition = Utils.GetVector3(jSONObject.GetString("2"));
		damageInfo.AttackerTeam = (Team)jSONObject.GetNumber("3");
		damageInfo.WeaponID = (int)jSONObject.GetNumber("4");
		damageInfo.PlayerID = (int)jSONObject.GetNumber("5");
		damageInfo.HeadShot = (jSONObject.GetNumber("6") == 1.0);
		return damageInfo;
	}
}
