using System.Collections.Generic;
using _Development.Scripts.Boot;
using _Development.Scripts.Roulette;
using _Development.Scripts.Roulette.Data;
using _Development.Scripts.Shop.Data;
using BLINK.RPGBuilder.Characters;
using BLINK.RPGBuilder.LogicMono;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Development.Scripts.Shop.View
{
    public class ShopGoldExchangeView : MonoBehaviour
    {
        [SerializeField] private Button _buyButton;
        [SerializeField] private TMP_Text _textPurchase;
        [SerializeField] private TMP_Text _textPrice;
        [SerializeField] private TMP_Text _textName;
        [SerializeField] private RewardCurrencySO _rewardScriptableObject;
        [SerializeField] private RPGCurrency _rewardCurrency;

        private List<PlayerRewardMultiplier> _iapCurrencyConversion;
        private int _rewardCount;
        private int _countCurrency;
        private int _price;
        private RPGCurrency _currency;

        private void OnEnable()
        {
            InitializeElement();
            CalculationAmountGold();

            if (_textPurchase != null)
                _textPurchase.text = $"<sprite name={_rewardCurrency.entryName}> " + $"{_rewardCount:##,###,###}";

            _textPrice.text = $"<sprite name={_currency.entryName}> " + $"{_price:##,###,###}";
            _buyButton.onClick.AddListener(TryBuy);
        }

        private void InitializeElement()
        {
            if (_currency != null)
                return;

            _currency = _rewardScriptableObject.Rewards.value;
            _price = _rewardScriptableObject.Rewards.count;
        }

        private void OnDisable() => 
            _buyButton.onClick.RemoveListener(TryBuy);

        private void TryBuy()
        {
            if (TryBuyElement() == false)
                return;

            WithdrawMoney();
            TransferMoney();
        }

        private void TransferMoney()
        {
            _countCurrency = Character.Instance.getCurrencyAmount(_rewardCurrency);
            _countCurrency += _rewardCount;
            RPGBuilderEssentials.Instance.RunnerElements.StartUpdateCurrency(_rewardCurrency, _countCurrency);
            EconomyUtilities.setCurrencyAmount(_rewardCurrency, _countCurrency);
        }

        private void WithdrawMoney()
        {
            _countCurrency = Character.Instance.getCurrencyAmount(_currency);
            _countCurrency -= _price;
            EconomyUtilities.setCurrencyAmount(_currency, _countCurrency);
            GeneralEvents.Instance.OnPlayerCurrencyChanged(_currency);
        }

        private bool TryBuyElement() =>
            Character.Instance.getCurrencyAmount(_currency) - _price >= 0;

        private void CalculationAmountGold()
        {
            foreach (PlayerRewardMultiplier playerRewardMultiplier in Game.instance.GetStaticData()
                         .IapCurrencyConversion)
            {
                if (Character.Instance.CharacterData.Level == (int)playerRewardMultiplier.Level)
                    _rewardCount = _rewardScriptableObject.Rewards.count * playerRewardMultiplier.Multiplier;
            }
        }
    }
}