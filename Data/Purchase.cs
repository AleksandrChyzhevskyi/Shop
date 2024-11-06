using _Development.Scripts.Upgrade.Initialization;
using NaughtyAttributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Purchasing;

namespace _Development.Scripts.Shop.Data
{
	[CreateAssetMenu(menuName = "Development/Purchase")]
	public class Purchase : ScriptableObject
	{
		[field: SerializeField, Dropdown("Get")] public string ID { get; private set; }
		[field: SerializeField] public ProductType Type { get; private set; }
		[field: SerializeField] public RewardSo[] Rewards { get; private set; }

		public static StringBuilder _builder = new StringBuilder();
		public List<string> Get()
		{
			var list = new List<string>();


			foreach (var productCatalogItem in ProductCatalog.LoadDefaultCatalog().allProducts)
			{
				list.Add(productCatalogItem.id);
			}

			return list;
		}

		public string GetDescription()
		{
			_builder.Clear();
			foreach (var rewardSo in Rewards)
			{
				_builder.Append(rewardSo.GetDescription());
			}


			return _builder.ToString();
		}
	}
}