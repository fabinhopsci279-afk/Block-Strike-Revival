using CodeStage.AntiCheat.ObscuredTypes;
using System;

[Serializable]
public class PlayerSkinShopData
{
	public ObscuredInt SkinID;

	public ObscuredString SkinName;

	public MoneyType Money;

	public ObscuredInt SkinPrice;
}
