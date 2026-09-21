using ExitGames.Client.Photon;
using System.IO;
using UnityEngine;

internal static class CustomTypes
{
	public static readonly byte[] memVector3 = new byte[12];

	public static readonly byte[] memVector2 = new byte[8];

	public static readonly byte[] memQuarternion = new byte[16];

	public static readonly byte[] memPlayer = new byte[4];

	internal static void Register()
	{
		PhotonPeer.RegisterType(typeof(Vector2), 87, SerializeVector2, DeserializeVector2);
		PhotonPeer.RegisterType(typeof(Vector3), 86, SerializeVector3, DeserializeVector3);
		PhotonPeer.RegisterType(typeof(Quaternion), 81, SerializeQuaternion, DeserializeQuaternion);
		PhotonPeer.RegisterType(typeof(PhotonPlayer), 80, SerializePhotonPlayer, DeserializePhotonPlayer);
	}

	private static short SerializeVector3(MemoryStream outStream, object customobject)
	{
		Vector3 vector = (Vector3)customobject;
		int targetOffset = 0;
		byte[] array = memVector3;
		lock (array)
		{
			byte[] array2 = memVector3;
			Protocol.Serialize(vector.x, array2, ref targetOffset);
			Protocol.Serialize(vector.y, array2, ref targetOffset);
			Protocol.Serialize(vector.z, array2, ref targetOffset);
			outStream.Write(array2, 0, 12);
		}
		return 12;
	}

	private static object DeserializeVector3(MemoryStream inStream, short length)
	{
		Vector3 vector = default(Vector3);
		byte[] array = memVector3;
		lock (array)
		{
			inStream.Read(memVector3, 0, 12);
			int offset = 0;
			Protocol.Deserialize(out vector.x, memVector3, ref offset);
			Protocol.Deserialize(out vector.y, memVector3, ref offset);
			Protocol.Deserialize(out vector.z, memVector3, ref offset);
		}
		return vector;
	}

	private static short SerializeVector2(MemoryStream outStream, object customobject)
	{
		Vector2 vector = (Vector2)customobject;
		byte[] array = memVector2;
		lock (array)
		{
			byte[] array2 = memVector2;
			int targetOffset = 0;
			Protocol.Serialize(vector.x, array2, ref targetOffset);
			Protocol.Serialize(vector.y, array2, ref targetOffset);
			outStream.Write(array2, 0, 8);
		}
		return 8;
	}

	private static object DeserializeVector2(MemoryStream inStream, short length)
	{
		Vector2 vector = default(Vector2);
		byte[] array = memVector2;
		lock (array)
		{
			inStream.Read(memVector2, 0, 8);
			int offset = 0;
			Protocol.Deserialize(out vector.x, memVector2, ref offset);
			Protocol.Deserialize(out vector.y, memVector2, ref offset);
		}
		return vector;
	}

	private static short SerializeQuaternion(MemoryStream outStream, object customobject)
	{
		Quaternion quaternion = (Quaternion)customobject;
		byte[] array = memQuarternion;
		lock (array)
		{
			byte[] array2 = memQuarternion;
			int targetOffset = 0;
			Protocol.Serialize(quaternion.w, array2, ref targetOffset);
			Protocol.Serialize(quaternion.x, array2, ref targetOffset);
			Protocol.Serialize(quaternion.y, array2, ref targetOffset);
			Protocol.Serialize(quaternion.z, array2, ref targetOffset);
			outStream.Write(array2, 0, 16);
		}
		return 16;
	}

	private static object DeserializeQuaternion(MemoryStream inStream, short length)
	{
		Quaternion quaternion = default(Quaternion);
		byte[] array = memQuarternion;
		lock (array)
		{
			inStream.Read(memQuarternion, 0, 16);
			int offset = 0;
			Protocol.Deserialize(out quaternion.w, memQuarternion, ref offset);
			Protocol.Deserialize(out quaternion.x, memQuarternion, ref offset);
			Protocol.Deserialize(out quaternion.y, memQuarternion, ref offset);
			Protocol.Deserialize(out quaternion.z, memQuarternion, ref offset);
		}
		return quaternion;
	}

	private static short SerializePhotonPlayer(MemoryStream outStream, object customobject)
	{
		int iD = ((PhotonPlayer)customobject).ID;
		byte[] array = memPlayer;
		lock (array)
		{
			byte[] array2 = memPlayer;
			int targetOffset = 0;
			Protocol.Serialize(iD, array2, ref targetOffset);
			outStream.Write(array2, 0, 4);
			return 4;
		}
	}

	private static object DeserializePhotonPlayer(MemoryStream inStream, short length)
	{
		byte[] array = memPlayer;
		int value;
		lock (array)
		{
			inStream.Read(memPlayer, 0, length);
			int offset = 0;
			Protocol.Deserialize(out value, memPlayer, ref offset);
		}
		if (PhotonNetwork.networkingPeer.mActors.ContainsKey(value))
		{
			return PhotonNetwork.networkingPeer.mActors[value];
		}
		return null;
	}
}
