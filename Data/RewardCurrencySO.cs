using System;
using BLINK.RPGBuilder.Managers;
using UnityEngine;

namespace _Development.Scripts.Shop.Data
{
	[CreateAssetMenu(menuName = "Development/Rewards/Currency)")]
	public class RewardCurrencySO : RewardSo
	{
		[Serializable]
		public struct Reward
		{
			public RPGCurrency value;
			public int count;
		}

		public Reward Rewards;
		public override void Rewarded()
		{
			InventoryManager.Instance.AddCurrency(Rewards.value.ID, Rewards.count);
			Analytics.instance.Reward(Rewards);
		}

		public override string GetDescription()
		{
			return $"<sprite name={Rewards.value.entryName}> "+ $"{Rewards.count:##,###,###}";
		}
	}
}