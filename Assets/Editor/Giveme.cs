using UnityEngine;
using UnityEditor;

public static class Giveme
{
    [MenuItem("Tools/vovichs/SetGold")]
    private static void GiveGold()
    {
        SaveLoadManager.SetGold(888888);
    }

    [MenuItem("Tools/vovichs/SetMoney")]
    private static void GiveMoney()
    {
        SaveLoadManager.SetMoney(888888);
    }
}
