using UnityEngine;

namespace _Development.Scripts.Shop.Data
{
	public abstract class RewardSo : ScriptableObject
	{
		public abstract void Rewarded();
		public abstract string GetDescription();
	}
}