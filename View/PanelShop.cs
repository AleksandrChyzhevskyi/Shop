using System;
using _Development.Scripts.Observer;
using _Development.Scripts.ServiceLocator;
using _Development.Scripts.Shop.Data;
using _Development.Scripts.Shop.Interface;
using UnityEngine;

namespace _Development.Scripts.Shop.View
{
	public sealed class PanelShop : MonoBehaviour
	{
		private void OnEnable()
		{
			var purchaseService = Service<IPurchaseService>.Get();

			if (!purchaseService.IsInit())
			{
				gameObject.SetActive(false);
				return;
			}

			ObserverUI.Show("IAP Shop");
		}

		private void OnDisable() => 
			GeneralEvents.Instance.OnClosedPanelShop();

		public void Reward(RewardSo rewardSo)
		{
			rewardSo.Rewarded();
		}
	}
}