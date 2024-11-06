using System;
using _Development.Scripts.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Development.Scripts.Shop.View
{
    public class ViewBuySkin : MonoBehaviour
    {
        public event Action<int, SkinData> Clicked;

        public Image Promo;
        public TextMeshProUGUI TextDescription;
        public TextMeshProUGUI TextButton;
        public Button BuyButton;

        public int EffectID { get; private set; }
        public SkinData SkinDataButton { get; private set; }

        private void OnEnable() =>
            BuyButton.onClick.AddListener(OnClicked);

        private void OnDisable() =>
            BuyButton.onClick.RemoveListener(OnClicked);

        private void OnClicked() =>
            Clicked?.Invoke(EffectID, SkinDataButton);

        public void SetData(int effectID, SkinData skinData)
        {
            SkinDataButton = skinData;
            EffectID = effectID;

            SetViewElements(skinData);
        }

        private void SetViewElements(SkinData skinData)
        {
            Promo.sprite = skinData.Promo;
            TextDescription.text = skinData.Info;
            TextButton.text = $"<sprite name={skinData.Currency.entryName}> " + $"{skinData.Price:##,###,###}";
        }
    }
}