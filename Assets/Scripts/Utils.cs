using Boomlagoon.JSON;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class Utils : MonoBehaviour
{
	public static void SavePlayerDataInfo(string data)
	{
		JSONObject jSONObject = JSONObject.Parse(data);
		SaveLoadManager.SetPlayerName(jSONObject.GetString("Name"));
		SaveLoadManager.SetMoney((int)jSONObject.GetNumber("Money"));
		SaveLoadManager.SetGold((int)jSONObject.GetNumber("Gold"));
		SaveLoadManager.SetPlayerXP((int)jSONObject.GetNumber("XP"));
		SaveLoadManager.SetPlayerLevel((int)jSONObject.GetNumber("Level"));
		SaveLoadManager.SetOpenCase((int)jSONObject.GetNumber("OpenCase"));
		SaveLoadManager.SetDeaths((int)jSONObject.GetNumber("Deaths"));
		SaveLoadManager.SetKills((int)jSONObject.GetNumber("Kills"));
		SaveLoadManager.SetHeadshot((int)jSONObject.GetNumber("Headshot"));
		SaveLoadManager.SetPlayerSkinSelected((int)jSONObject.GetNumber("PlayerSkin"));
		SaveLoadManager.SetCaseTime(jSONObject.GetNumber("CaseTime"));
		SaveLoadManager.SetSensitivity((float)jSONObject.GetNumber("Sensitivity"));
		SaveLoadManager.SetSound(jSONObject.GetBoolean("Sound"));
		SaveLoadManager.SetFPSMeter(jSONObject.GetBoolean("FPSMeter"));
		SaveLoadManager.SetConsole(jSONObject.GetBoolean("Console"));
		SaveLoadManager.SetChat(jSONObject.GetBoolean("Chat"));
		SaveLoadManager.SetShowDamage(jSONObject.GetBoolean("ShowDamage"));
		SaveLoadManager.SetColorCrosshair((int)jSONObject.GetNumber("ColorCrosshair"));
	}

	public static PlayerDataInfo GetPlayerDataInfo(string data)
	{
		PlayerDataInfo playerDataInfo = new PlayerDataInfo();
		JSONObject jSONObject = JSONObject.Parse(data);
		playerDataInfo.Name = jSONObject.GetString("Name");
		playerDataInfo.Money = (int)jSONObject.GetNumber("Money");
		playerDataInfo.Gold = (int)jSONObject.GetNumber("Gold");
		playerDataInfo.XP = (int)jSONObject.GetNumber("XP");
		playerDataInfo.Level = (int)jSONObject.GetNumber("Level");
		playerDataInfo.OpenCase = (int)jSONObject.GetNumber("OpenCase");
		playerDataInfo.Deaths = (int)jSONObject.GetNumber("Deaths");
		playerDataInfo.Kills = (int)jSONObject.GetNumber("Kills");
		playerDataInfo.Headshot = (int)jSONObject.GetNumber("Headshot");
		playerDataInfo.PlayerSkin = (int)jSONObject.GetNumber("PlayerSkin");
		playerDataInfo.CaseTime = jSONObject.GetNumber("CaseTime");
		playerDataInfo.Sensitivity = (float)jSONObject.GetNumber("Sensitivity");
		playerDataInfo.Sound = jSONObject.GetBoolean("Sound");
		playerDataInfo.FPSMeter = jSONObject.GetBoolean("FPSMeter");
		playerDataInfo.Console = jSONObject.GetBoolean("Console");
		playerDataInfo.Chat = jSONObject.GetBoolean("Chat");
		playerDataInfo.ShowDamage = jSONObject.GetBoolean("ShowDamage");
		playerDataInfo.ColorCrosshair = (int)jSONObject.GetNumber("ColorCrosshair");
		return playerDataInfo;
	}

	public static string PlayerDataToJson(PlayerDataInfo info)
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.Add("Name", info.Name);
		jSONObject.Add("Money", info.Money);
		jSONObject.Add("Gold", info.Gold);
		jSONObject.Add("XP", info.XP);
		jSONObject.Add("Level", info.Level);
		jSONObject.Add("OpenCase", info.OpenCase);
		jSONObject.Add("Deaths", info.Deaths);
		jSONObject.Add("Kills", info.Kills);
		jSONObject.Add("Headshot", info.Headshot);
		jSONObject.Add("PlayerSkin", info.PlayerSkin);
		jSONObject.Add("CaseTime", info.CaseTime);
		jSONObject.Add("Sensitivity", info.Sensitivity);
		jSONObject.Add("Sound", info.Sound);
		jSONObject.Add("FPSMeter", info.FPSMeter);
		jSONObject.Add("Console", info.Console);
		jSONObject.Add("Chat", info.Chat);
		jSONObject.Add("ShowDamage", info.ShowDamage);
		jSONObject.Add("ColorCrosshair", info.ColorCrosshair);
		return jSONObject.ToString();
	}

	public static string GetPlayerDataJson()
	{
		JSONObject jSONObject = new JSONObject();
		jSONObject.Add("Name", SaveLoadManager.GetPlayerName());
		jSONObject.Add("Money", SaveLoadManager.GetMoney());
		jSONObject.Add("Gold", SaveLoadManager.GetGold());
		jSONObject.Add("XP", SaveLoadManager.GetPlayerXP());
		jSONObject.Add("Level", SaveLoadManager.GetPlayerLevel());
		jSONObject.Add("OpenCase", SaveLoadManager.GetOpenCase());
		jSONObject.Add("Deaths", SaveLoadManager.GetDeaths());
		jSONObject.Add("Kills", SaveLoadManager.GetKills());
		jSONObject.Add("Headshot", SaveLoadManager.GetHeadshot());
		jSONObject.Add("PlayerSkin", SaveLoadManager.GetPlayerSkinSelected());
		jSONObject.Add("CaseTime", SaveLoadManager.GetCaseTime());
		jSONObject.Add("Sensitivity", SaveLoadManager.GetSensitivity());
		jSONObject.Add("Sound", SaveLoadManager.GetSound());
		jSONObject.Add("FPSMeter", SaveLoadManager.GetFPSMeter());
		jSONObject.Add("Console", SaveLoadManager.GetConsole());
		jSONObject.Add("Chat", SaveLoadManager.GetChat());
		jSONObject.Add("ShowDamage", SaveLoadManager.GetShowDamage());
		jSONObject.Add("ColorCrosshair", SaveLoadManager.GetColorCrosshair());
		return jSONObject.ToString();
	}

	public static void SaveFile(string directory, string fileName, string data)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(data);
		SaveFile(directory, fileName, bytes);
	}

	public static void SaveFile(string directory, string fileName, byte[] data)
	{
		string path = GetProjectPath() + "/" + directory + "/" + fileName;
		if (!Directory.Exists(GetProjectPath() + "/" + directory))
		{
			Directory.CreateDirectory(GetProjectPath() + "/" + directory);
		}
		File.WriteAllBytes(path, data);
	}

	public static string LoadFile(string directory, string fileName)
	{
		return Encoding.UTF8.GetString(LoadFileBytes(directory, fileName));
	}

	public static byte[] LoadFileBytes(string directory, string fileName)
	{
		string path = GetProjectPath() + "/" + directory + "/" + fileName;
		return File.ReadAllBytes(path);
	}

	public static string GetProjectPath()
	{
		return Directory.GetParent(Application.dataPath).FullName;
	}

	public static string GetFileName(string path)
	{
		return Path.GetFileNameWithoutExtension(path);
	}

	public static string[] GetFiles(string directory, string searchPattern)
	{
		return Directory.GetFiles(GetProjectPath() + "/" + directory, searchPattern);
	}

	public static bool ExistsDirectory(string directory)
	{
		return Directory.Exists(GetProjectPath() + "/" + directory);
	}

	public static bool ExistsFile(string directory, string fileName)
	{
		return File.Exists(GetProjectPath() + "/" + directory + "/" + fileName);
	}

	public static string EncryptData(string toEncrypt, string key)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(key);
		byte[] bytes2 = Encoding.UTF8.GetBytes(toEncrypt);
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = bytes;
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateEncryptor();
		byte[] array = cryptoTransform.TransformFinalBlock(bytes2, 0, bytes2.Length);
		return Convert.ToBase64String(array, 0, array.Length);
	}

	public static string DecryptData(string toDecrypt, string key)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(key);
		byte[] array = Convert.FromBase64String(toDecrypt);
		RijndaelManaged rijndaelManaged = new RijndaelManaged();
		rijndaelManaged.Key = bytes;
		rijndaelManaged.Mode = CipherMode.ECB;
		rijndaelManaged.Padding = PaddingMode.PKCS7;
		ICryptoTransform cryptoTransform = rijndaelManaged.CreateDecryptor();
		byte[] bytes2 = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
		return Encoding.UTF8.GetString(bytes2);
	}

	public static Vector3 GetVector3(string vector)
	{
		string[] array = vector.Substring(1, vector.Length - 2).Split(',');
		float x = float.Parse(array[0]);
		float y = float.Parse(array[1]);
		float z = float.Parse(array[2]);
		return new Vector3(x, y, z);
	}

	public static Vector2 GetVector2(string vector)
	{
		string[] array = vector.Substring(1, vector.Length - 2).Split(',');
		float x = float.Parse(array[0]);
		float y = float.Parse(array[1]);
		return new Vector2(x, y);
	}

	public static GameObject AddChild(GameObject inst, GameObject parent, [Optional] Vector3 position, [Optional] Quaternion rotation)
	{
		return AddChild(inst, parent.transform, position, rotation);
	}

	public static GameObject AddChild(GameObject inst, Transform parent, [Optional] Vector3 position, [Optional] Quaternion rotation)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(inst, Vector3.zero, Quaternion.identity);
		gameObject.transform.SetParent(parent);
		gameObject.transform.localPosition = position;
		gameObject.transform.localRotation = rotation;
		gameObject.transform.localScale = Vector3.one;
		gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
		return gameObject;
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}

	public static bool CompareTeam(Transform player)
	{
		return player.tag == "Blue" || player.tag == "Red";
	}

	public static Vector2 RandomAccuracy(Vector2 accuracy)
	{
		accuracy.x = UnityEngine.Random.Range(0f - accuracy.x, accuracy.x);
		accuracy.y = UnityEngine.Random.Range(0f - accuracy.y, accuracy.y);
		return accuracy;
	}

	public static Vector3 GetRagdollForce(Vector3 playerPosition, Vector3 attackPosition)
	{
		Vector3 result = playerPosition - attackPosition;
		result.x = Mathf.Clamp(result.x, -1f, 1f);
		result.y = Mathf.Clamp(result.y, -1f, 1f);
		result.z = Mathf.Clamp(result.z, -1f, 1f);
		return result;
	}

	public static string GetTeamHexColor(PhotonPlayer player)
	{
		string text = player.name;
		if (player.GetTeam() == Team.Blue)
		{
			text = "[018def]" + text + "[-]";
		}
		else if (player.GetTeam() == Team.Red)
		{
			text = "[ff0000]" + text + "[-]";
		}
		return text;
	}

	public static void SetActiveConsole(bool active)
	{
		if (active)
		{
			qLogger.CreateGO(isStackTrace: true, KeyCode.Escape);
		}
		else
		{
			qLogger.DestroyGO();
		}
	}

	public static string KillerStatus(DamageInfo damageInfo)
	{
		string result = string.Empty;
		if (damageInfo.PlayerID != -1)
		{
			PhotonPlayer player = PhotonPlayer.Find(damageInfo.PlayerID);
			string teamHexColor = GetTeamHexColor(player);
			string text = WeaponManager.GetWeaponType(damageInfo.WeaponID).WeaponName;
			result = teamHexColor + " [ " + text + (damageInfo.HeadShot ? "+Head" : string.Empty) + " ] " + GetTeamHexColor(PhotonNetwork.player);
		}
		return result;
	}

	public static Color GetColor(int color)
	{
		switch (color)
		{
		case 0:
			return Color.white;
		case 1:
			return Color.red;
		case 2:
			return Color.yellow;
		case 3:
			return Color.green;
		case 4:
			return Color.cyan;
		case 5:
			return Color.blue;
		case 6:
			return Color.magenta;
		case 7:
			return Color.gray;
		case 8:
			return Color.black;
		case 9:
			return Color.clear;
		default:
			return Color.white;
		}
	}

	public static string FloatToTime(float time)
	{
		int num = (int)time / 60;
		int num2 = (int)time - num * 60;
		return $"{num:0}:{num2:00}";
	}
}
