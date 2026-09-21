using ExitGames.Client.Photon;

public struct PhotonEventData
{
	public readonly byte id;

	public readonly PhotonEventTag tag;

	public readonly object[] parameters;

	public readonly int senderID;

	public readonly double timestamp;

	public PhotonEventData(Hashtable rpcEvent, int sender)
	{
		if (rpcEvent.ContainsKey(1))
		{
			tag = (PhotonEventTag)(byte)rpcEvent[1];
		}
		else
		{
			tag = PhotonEventTag.None;
		}
		if (rpcEvent.ContainsKey(2))
		{
			id = (byte)rpcEvent[2];
		}
		else
		{
			id = 0;
		}
		if (rpcEvent.ContainsKey(3))
		{
			uint num = (uint)(int)rpcEvent[3];
			double num2 = num;
			timestamp = num2 / 1000.0;
		}
		else
		{
			timestamp = -1.0;
		}
		if (rpcEvent.ContainsKey(4))
		{
			parameters = (object[])rpcEvent[4];
		}
		else
		{
			parameters = new object[0];
		}
		senderID = sender;
	}
}
