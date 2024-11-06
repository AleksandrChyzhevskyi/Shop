using System;
using _Development.Scripts.Data;
using _Development.Scripts.Shop.Data;
using _Development.Scripts.Shop.Interface;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
using UnityEngine.Purchasing.Security;

namespace _Development.Scripts.Shop.Service
{
	public sealed class UnityPurchaseService : IPurchaseService, IDetailedStoreListener
	{
		private IStoreController m_StoreController;
		private IGooglePlayStoreExtensions m_GooglePlayStoreExtensions;
		private Purchase _currentPurchase;
		private StaticData _staticData;

		public UnityPurchaseService(StaticData staticData)
		{
			_staticData = staticData;
			InitializePurchasing();
		}
		private void InitializePurchasing()
		{
			var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
			builder.Configure<IGooglePlayConfiguration>().SetDeferredPurchaseListener(OnDeferredPurchase);

			var catalog = JsonUtility.FromJson<ProductCatalog>(_staticData.Configure.text);

			foreach (var s in catalog.allProducts)
			{
				builder.AddProduct(s.id, s.type);
			}

			UnityPurchasing.Initialize(this, builder);
		}
		private void OnDeferredPurchase(Product product)
		{
			Debug.Log($"Purchase of {product.definition.id} is deferred");
		}
		public void OnInitializeFailed(InitializationFailureReason error)
		{
		}

		public void OnInitializeFailed(InitializationFailureReason error, string message)
		{
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
		{
			var product = args.purchasedProduct;

			Debug.Log($"Processing Purchase: {product.definition.id}");

			if (m_GooglePlayStoreExtensions.IsPurchasedProductDeferred(product))
			{
				//The purchase is Deferred.
				//Therefore, we do not unlock the content or complete the transaction.
				//ProcessPurchase will be called again once the purchase is Purchased.
				return PurchaseProcessingResult.Pending;
			}
			
			bool validPurchase = true; // Presume valid for platforms with no R.V.

			// Unity IAP's validation logic is only included on these platforms.
			if (Application.platform is RuntimePlatform.Android)
			{
				// Prepare the validator with the secrets we prepared in the Editor
				// obfuscation window.
				var validator = new CrossPlatformValidator(GooglePlayTangle.Data(),
					AppleTangle.Data(), Application.identifier);

				try {
					// On Google Play, result has a single product ID.
					// On Apple stores, receipts contain multiple products.
					var result = validator.Validate(args.purchasedProduct.receipt);
					// For informational purposes, we list the receipt(s)
					Debug.Log("Receipt is valid. Contents:");
					foreach (IPurchaseReceipt productReceipt in result) {
						Debug.Log(productReceipt.productID);
						Debug.Log(productReceipt.purchaseDate);
						Debug.Log(productReceipt.transactionID);
					}
				} catch (IAPSecurityException) {
					Debug.Log("Invalid receipt, not unlocking content");
					validPurchase = false;
					OnPurchaseFail?.Invoke("Invalid receipt.");
				}
			}

			if (validPurchase)
				UnlockContent(product);

			return PurchaseProcessingResult.Complete;
		}


		public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription)
		{
			Debug.Log("Purchase fail : " + failureDescription);
		}
		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			Debug.Log("Purchase fail : " + failureReason);
		}

		void UnlockContent(Product product)
		{
			Debug.Log($"Unlock Content: {product.definition.id}");

			OnPurchaseComplete?.Invoke(product);

			if (product.definition.id.StartsWith("removeads"))
			{
				PlayerPrefs.SetInt("PlayerBuyRemoveAds", 1);
			}
			else if (product.definition.id == _currentPurchase.ID)
			{
				foreach (var currentPurchaseReward in _currentPurchase.Rewards)
				{
					currentPurchaseReward.Rewarded();
				}
			}
		}
		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			Debug.Log("In-App Purchasing successfully initialized");

			m_StoreController = controller;
			m_GooglePlayStoreExtensions = extensions.GetExtension<IGooglePlayStoreExtensions>();
		}

		public void Buy(Purchase purchase)
		{
			_currentPurchase = purchase;

			m_StoreController.InitiatePurchase(_currentPurchase.ID);
		}

		public bool GetProduct(string id, out Product product)
		{
			product = m_StoreController.products.WithID(id);

			if (product.metadata == null)
			{
				return false;
			}
			
			return true;
		}
		public string GetPrice(Purchase purchase)
		{
			Debug.Log(m_StoreController.products.WithID(purchase.ID).metadata.localizedPriceString);

			if (m_StoreController.products.WithID(purchase.ID).metadata == null)
			{
				return "";
			}

			var price = m_StoreController.products.WithID(purchase.ID).metadata.localizedPriceString;
			return $"{price}";
		}

		public bool IsInit()
		{
			return m_StoreController != null;
		}

		public event Action<Product> OnPurchaseComplete;
		public event Action<string> OnPurchaseFail;
	}
}