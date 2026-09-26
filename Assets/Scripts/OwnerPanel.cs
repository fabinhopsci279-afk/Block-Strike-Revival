using System.Collections.Generic;
using UnityEngine;

public class OwnerPanel : MonoBehaviour
{
	public const string OwnerName = "fabinhopsci";

	private const string BanKey = "OwnerBanList";

	public static bool IsOwner()
	{
		return string.Equals(PhotonNetwork.playerName, OwnerName, System.StringComparison.OrdinalIgnoreCase);
	}

	public static List<string> GetBanList()
	{
		List<string> list = new List<string>();
		string @string = PlayerPrefs.GetString(BanKey, string.Empty);
		string[] array = @string.Split(new char[1] { '|' });
		for (int i = 0; i < array.Length; i++)
		{
			if (!string.IsNullOrEmpty(array[i]) && !list.Contains(array[i]))
			{
				list.Add(array[i]);
			}
		}
		return list;
	}

	public static bool IsBanned(string playerName)
	{
		if (string.IsNullOrEmpty(playerName))
		{
			return false;
		}
		List<string> banList = GetBanList();
		for (int i = 0; i < banList.Count; i++)
		{
			if (string.Equals(banList[i], playerName, System.StringComparison.OrdinalIgnoreCase))
			{
				return true;
			}
		}
		return false;
	}

	public static void BanPlayer(PhotonPlayer player)
	{
		if (!IsOwner() || player == null)
		{
			return;
		}
		List<string> banList = GetBanList();
		if (!banList.Contains(player.name))
		{
			banList.Add(player.name);
			PlayerPrefs.SetString(BanKey, string.Join("|", banList.ToArray()));
			PlayerPrefs.Save();
		}
		GameManager.SendKickPlayer(player, "hacker has got banned");
		UIToast.Show(player.name + ": hacker has got banned");
	}

	public static void UnbanPlayer(string playerName)
	{
		if (!IsOwner())
		{
			return;
		}
		List<string> banList = GetBanList();
		if (banList.Remove(playerName))
		{
			PlayerPrefs.SetString(BanKey, string.Join("|", banList.ToArray()));
			PlayerPrefs.Save();
		}
	}
}
