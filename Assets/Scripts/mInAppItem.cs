using UnityEngine;

public class mInAppItem : MonoBehaviour
{
	public enum ItemList
	{
		Purchase,
		Consume
	}

	public ItemList Item;

	public string Sku;

	public UILabel PriceLabel;

	private bool isPurchase;

	private void Start()
	{
		PriceLabel.text = InAppManager.GetPrice(Sku);
		if (Item == ItemList.Purchase)
		{
			CheckPurchase();
			EventManager.AddListener("UpdateInApp", CheckPurchase);
		}
	}

	private void CheckPurchase()
	{
		if (InAppManager.GetPurchase(Sku))
		{
			UIWidget component = GetComponent<UIWidget>();
			component.alpha = 0.5f;
			isPurchase = true;
			PriceLabel.text = Localization.Get("You have already purchased this item");
		}
	}

	private void OnClick()
	{
		if (!isPurchase)
		{
			if (Item == ItemList.Purchase)
			{
				InAppManager.Purchase(Sku);
			}
			else
			{
				InAppManager.Consume(Sku);
			}
		}
	}
}
