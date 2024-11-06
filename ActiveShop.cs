using _Development.Scripts.Navigation;
using _Development.Scripts.Shop.View;
using BLINK.RPGBuilder.LogicMono;
using UnityEngine;

namespace _Development.Scripts.Shop
{
    public class ActiveShop : MonoBehaviour
    {
        private PanelShop _panelShop;

        private void Start() => 
            _panelShop = RPGBuilderEssentials.Instance.Shop;

        private void OnTriggerEnter(Collider other)
        {
            if(other.TryGetComponent(out PlayerNavigation _))
                _panelShop.gameObject.SetActive(true);
        }
    }
}