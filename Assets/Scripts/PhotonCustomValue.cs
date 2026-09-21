using ExitGames.Client.Photon;

internal static class PhotonCustomValue
{
	public static void ClearProperties(this PhotonPlayer player)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["team"] = Team.None;
		hashtable["deaths"] = 0;
		hashtable["kills"] = 0;
		hashtable["dead"] = true;
		player.SetCustomProperties(hashtable);
	}

	public static void SetTeam(this PhotonPlayer player, Team team)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["team"] = team;
		player.SetCustomProperties(hashtable);
	}

	public static Team GetTeam(this PhotonPlayer player)
	{
		if (player.customProperties.TryGetValue("team", out object value))
		{
			return (Team)(int)value;
		}
		return Team.None;
	}

	public static void SetLevel(this PhotonPlayer player, int level)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["level"] = level;
		player.SetCustomProperties(hashtable);
	}

	public static int GetLevel(this PhotonPlayer player)
	{
		if (player.customProperties.TryGetValue("level", out object value))
		{
			return (int)value;
		}
		return 1;
	}

	public static void UpdatePing(this PhotonPlayer player)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["ping"] = PhotonNetwork.GetPing();
		player.SetCustomProperties(hashtable);
	}

	public static int GetPing(this PhotonPlayer player)
	{
		if (player.customProperties.TryGetValue("ping", out object value))
		{
			return (int)value;
		}
		return 0;
	}

	public static void SetDeaths(this PhotonPlayer player, int deaths)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["deaths"] = deaths;
		player.SetCustomProperties(hashtable);
	}

	public static void SetDeaths1(this PhotonPlayer player)
	{
		int num = 0;
		if (player.customProperties.TryGetValue("deaths", out object value))
		{
			num = (int)value;
		}
		num++;
		Hashtable hashtable = new Hashtable();
		hashtable["deaths"] = num;
		player.SetCustomProperties(hashtable);
	}

	public static int GetDeaths(this PhotonPlayer player)
	{
		if (player.customProperties.TryGetValue("deaths", out object value))
		{
			return (int)value;
		}
		return 0;
	}

	public static void SetKills(this PhotonPlayer player, int kills)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["kills"] = kills;
		player.SetCustomProperties(hashtable);
	}

	public static void SetKills1(this PhotonPlayer player)
	{
		int num = 0;
		if (player.customProperties.TryGetValue("kills", out object value))
		{
			num = (int)value;
		}
		num++;
		Hashtable hashtable = new Hashtable();
		hashtable["kills"] = num;
		player.SetCustomProperties(hashtable);
	}

	public static int GetKills(this PhotonPlayer player)
	{
		if (player.customProperties.TryGetValue("kills", out object value))
		{
			return (int)value;
		}
		return 0;
	}

	public static void SetDead(this PhotonPlayer player, bool dead)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["dead"] = dead;
		player.SetCustomProperties(hashtable);
	}

	public static bool GetDead(this PhotonPlayer player)
	{
		object value;
		return player.customProperties.TryGetValue("dead", out value) && (bool)value;
	}

	public static void SetGameMode(this Room room, GameMode mode)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["mode"] = (int)mode;
		room.SetCustomProperties(hashtable);
	}

	public static GameMode GetGameMode(this Room room)
	{
		if (room.customProperties.TryGetValue("mode", out object value))
		{
			return (GameMode)(int)value;
		}
		return GameMode.TeamDeathmatch;
	}

	public static void SetMapName(this Room room, string mapName)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["mapName"] = mapName;
		room.SetCustomProperties(hashtable);
	}

	public static GameMode GetGameMode(this RoomInfo room)
	{
		if (room.customProperties.TryGetValue("mode", out object value))
		{
			return (GameMode)(int)value;
		}
		return GameMode.TeamDeathmatch;
	}

	public static string GetMapName(this RoomInfo room)
	{
		if (room.customProperties.TryGetValue("mapName", out object value))
		{
			return (string)value;
		}
		return string.Empty;
	}

	public static Hashtable CreateRoomHashtable(this RoomInfo photonNetwork, string mapName, string password, GameMode mode)
	{
		Hashtable hashtable = new Hashtable();
		hashtable["mapName"] = mapName;
		hashtable["password"] = password;
		hashtable["mode"] = (int)mode;
		return hashtable;
	}

	public static string GetPassword(this RoomInfo room)
	{
		if (room.customProperties.TryGetValue("password", out object value))
		{
			return (string)value;
		}
		return string.Empty;
	}
}
