using System;
using _Development.Scripts.Shop.Data;
using _Development.Scripts.Upgrade.Initialization;
using UnityEngine.Purchasing;

namespace _Development.Scripts.Shop.Interface
{
	public interface IPurchaseService
	{
		public void Buy(Purchase purchase);
		public string GetPrice(Purchase purchase);
		public bool GetProduct(string id, out Product product);
		public bool IsInit();

		public event Action <Product> OnPurchaseComplete;
		public event Action <string> OnPurchaseFail;
	}
}