using _Development.Scripts.ServiceLocator;
using _Development.Scripts.Shop.Data;
using _Development.Scripts.Shop.Interface;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Development.Scripts.Shop.View
{
	public sealed class PurchaseView : MonoBehaviour
	{
		[SerializeField] private Button _buyButton;
		[SerializeField] private TMP_Text _textPurchase;
		[SerializeField] private TMP_Text _textPrice;
		[SerializeField] private TMP_Text _textName;
		[SerializeField] private Purchase _purchase;

		private IPurchaseService _purchaseService = Service<IPurchaseService>.Get();
		private void OnEnable()
		{
			_purchaseService = Service<IPurchaseService>.Get();
			_purchaseService.GetProduct(_purchase.ID, out var product);

			if (product.metadata != null)
			{
				if (product.definition.id.StartsWith("removeads"))
				{
					if (PlayerPrefs.GetInt("PlayerBuyRemoveAds", 0) == 1)
					{
						gameObject.SetActive(false);
						return;
					}
				}
				
				if (_textName != null) _textName.text = product.metadata.localizedTitle;
				if (_textPurchase != null) _textPurchase.text = _purchase.GetDescription();
				_textPrice.text = product.metadata.localizedPriceString;
				_buyButton.onClick.AddListener(TryBuy);
			}
		}
		private void OnDisable() => 
			_buyButton.onClick.RemoveListener(TryBuy);

		private void TryBuy() => 
			_purchaseService.Buy(_purchase);
	}
}