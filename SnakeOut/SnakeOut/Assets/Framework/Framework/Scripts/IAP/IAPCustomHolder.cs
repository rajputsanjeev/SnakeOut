using System;
using Framework;
using Framework.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IAPCustomHolder : MonoBehaviour
{
	public Action OnPurchaseComplete;

	public ProductKeyType ProductKeyType;
	[SerializeField] TextMeshProUGUI priceText;
	[SerializeField] Button purchaseButton;

	private void OnEnable()
	{
		IAPManager.PurchaseCompleted += PurchaseCompleted;
	}

	private void OnDisable()
	{
		IAPManager.PurchaseCompleted -= PurchaseCompleted;
	}

	private void Awake()
	{
		// Button event
		purchaseButton.onClick.AddListener(() => OnPurchaseButtonClicked());

		IAPManager.SubscribeOnPurchaseModuleInitted(() =>
		{
			ProductData productData = IAPManager.GetProductData(ProductKeyType);
			if (productData != null)
			{
				// Price of the product: USD 0.99
				priceText.text = productData.GetLocalPrice();
			}
		});
	}

	public void OnPurchaseButtonClicked()
	{
		IAPManager.BuyProduct(ProductKeyType);
	}

	private void PurchaseCompleted(ProductKeyType productKeyType)
	{
		if (productKeyType == ProductKeyType)
		{
			OnPurchaseComplete?.Invoke();
			// The NoAds product has been successfully purchased
		}
	}
}
