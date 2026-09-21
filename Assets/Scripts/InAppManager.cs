using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

public class InAppManager : MonoBehaviour
{
	private string DeveloperPayload;

	private bool isConsume;

	public static ObscuredBool NoAds;

	public static ObscuredBool AllWeapons;

	public static ObscuredBool AllSkins;

	private static InAppManager instance;

	private void Start()
	{
		instance = this;
		Init();
	}

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	public static void Init()
	{
		string text = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAkW8ls1E1B9/P6CXoZKl0tBNP+EUX6FIwbGbm/9LYW5iVOXASnng3+egvlSPnMekvcv9p/NoDRRY01pZhmlq1uShTlmaNT2pJipP2YXcyDvIPuO2rQSGKG8dIYmTAaIcVqqOl+22BQ321M8sSnCvhNOirCFbAEG5dCC0SoT3AOTFYB0GH5QbioLt0P+oV+a37c3GXbNJwlsMEmfeGxLEbgrpmrKfGT2E1JZta3JmcAXj+SVsxo5jOytiS7SluhBopirVU2nDDy+MawAVLCBGgOfeTfNBfFiwiFkLK7sS/wqRzl93/mlv2uZI62MNT/zHdfDWI+1szhUrckwvEwuGeHQIDAQAB";
	}

	private void billingSupportedEvent()
	{
		UpdateQueryInventory();
	}

	private void UpdateQueryInventory()
	{
		string[] array = new string[12]
		{
			"com.revival.blockstrikerevival.m5000",
			"com.revival.blockstrikerevival.m10000",
			"com.revival.blockstrikerevival.m20000",
			"com.revival.blockstrikerevival.m30000",
			"com.revival.blockstrikerevival.g100",
			"com.revival.blockstrikerevival.g250",
			"com.revival.blockstrikerevival.g600",
			"com.revival.blockstrikerevival.g1000",
			"com.revival.blockstrikerevival.ads",
			"com.revival.blockstrikerevival.allweapons",
			"com.revival.blockstrikerevival.allskins",
			"com.revival.blockstrikerevival.fullpack"
		};
	}

	private void billingNotSupportedEvent(string error)
	{
	}

	public static string GetPrice(string sku)
	{
		if (Application.isEditor)
		{
			return "0,00$";
		}
		return string.Empty;
	}

	public static bool GetPurchase(string sku)
	{
		if (Application.isEditor)
		{
			return false;
		}
		return false;
	}

	private void queryInventoryFailedEvent(string error)
	{
	}

	public static void Purchase(string sku)
	{
	}

	private void purchaseFailedEvent(string error, int response)
	{
		UIToast.Show(Localization.Get("Error"));
	}

	public static void Consume(string sku)
	{
	}

	private void consumePurchaseFailedEvent(string error)
	{
	}

	private void GenerateDeveloperPayload()
	{
		string text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
		string text2 = string.Empty;
		for (int i = 0; i < 10; i++)
		{
			text2 += text[Random.Range(0, text.Length)];
		}
		DeveloperPayload = text2;
	}
}
