using System;
using System.Collections.Generic;
using System.Linq;
using _Development.Scripts.Boot;
using _Development.Scripts.Data;
using _Development.Scripts.Shop.View;
using _Development.Scripts.SkinsPlayer.PlayerSkinsData;
using BLINK.RPGBuilder.Characters;
using BLINK.RPGBuilder.LogicMono;
using BLINK.RPGBuilder.Managers;
using UnityEngine;

namespace _Development.Scripts.Shop
{
    public class SkinsCarouselModel : MonoBehaviour
    {
        [SerializeField] private ViewBuySkin _prefabBuySkin;
        [SerializeField] private Transform _contents;
        [SerializeField] private Transform _objectForActivation;

        private List<ViewBuySkin> _listView = new();
        private List<StaticData.ShapeShiftingAbilityApply> _listSellSkins;

        private void Start()
        {
            CheckIsSell();
            CreateBuySkin();
        }

        private void OnDestroy()
        {
            foreach (ViewBuySkin viewBuySkin in _listView)
                viewBuySkin.Clicked -= OnButtonClick;
        }

        private void CreateBuySkin()
        {
            foreach (StaticData.ShapeShiftingAbilityApply shapeshiftingAbilityApply in _listSellSkins)
            {
                foreach (SkinData skinData in shapeshiftingAbilityApply.SkinDates)
                {
                    if (skinData.IsSell)
                    {
                        ViewBuySkin viewBuySkin = Instantiate(_prefabBuySkin, _contents);
                        viewBuySkin.SetData(shapeshiftingAbilityApply.ShapeshiftId, skinData);

                        viewBuySkin.Clicked += OnButtonClick;

                        _listView.Add(viewBuySkin);
                    }
                }
            }

            if (_listView.Count > 0)
                _objectForActivation.gameObject.SetActive(true);
        }

        private void CheckIsSell()
        {
            _listSellSkins = new List<StaticData.ShapeShiftingAbilityApply>();

            List<SkinData> skinDates = new List<SkinData>();

            foreach (StaticData.ShapeShiftingAbilityApply shapeShiftingAbilityApply in Game.instance.GetStaticData()
                         .ShapeshiftingAbilities)
            {
                PlayerSkinDataFile firstOrDefault =
                    Character.Instance.CharacterData.Skins.DataFiles.FirstOrDefault(x =>
                        x.EffectID == shapeShiftingAbilityApply.ShapeshiftId);

                if (firstOrDefault != null)
                {
                    foreach (SkinData skinData in shapeShiftingAbilityApply.SkinDates)
                    {
                        SkinIDDataFile skinIDDataFile =
                            firstOrDefault.Skinslist.FirstOrDefault(x => x.SkinIDData == skinData.ID);

                        if (skinIDDataFile != null)
                            continue;

                        skinDates.Add(skinData);
                    }

                    _listSellSkins.Add(CreateNewShapeShiftingAbilityApply(shapeShiftingAbilityApply, skinDates));
                    skinDates.Clear();
                }
                else
                    _listSellSkins.Add(shapeShiftingAbilityApply);
            }
        }

        private StaticData.ShapeShiftingAbilityApply CreateNewShapeShiftingAbilityApply(
            StaticData.ShapeShiftingAbilityApply shapeShiftingAbilityApply, List<SkinData> skinDates)
        {
            StaticData.ShapeShiftingAbilityApply abilityApply = new StaticData.ShapeShiftingAbilityApply
            {
                ShapeshiftId = shapeShiftingAbilityApply.ShapeshiftId,
                SkinDates = skinDates.ToArray()
            };

            return abilityApply;
        }

        private void OnButtonClick(int effectId, SkinData skinData)
        {
            if (Character.Instance.getCurrencyAmount(skinData.Currency) - skinData.Price < 0)
                return;

            BuySkin(skinData);

            if (GameState.playerEntity.IsShapeshifted())
                RPGBuilderEssentials.Instance.ModelShape.ApplyEffect(effectId, skinData);

            DestroyButton(effectId, skinData);

            RPGEffect effect = GameDatabase.Instance.GetEffects()[effectId];
            GeneralEvents.Instance.OnOpenedNewSkin(effect, skinData.ID);

            GetComponentInParent<PanelShop>().gameObject.SetActive(false);
        }

        private void BuySkin(SkinData skinData)
        {
            float currencyAmount = Character.Instance.getCurrencyAmount(skinData.Currency) - skinData.Price;
            EconomyUtilities.setCurrencyAmount(skinData.Currency, (int)currencyAmount);
            GeneralEvents.Instance.OnPlayerCurrencyChanged(skinData.Currency);
        }

        private void DestroyButton(int effectId, SkinData skinData)
        {
            ViewBuySkin buttonView = GetButtonView(effectId, skinData);

            buttonView.Clicked -= OnButtonClick;
            _listView.Remove(buttonView);

            if (_listView.Count <= 0)
                _objectForActivation.gameObject.SetActive(false);

            Destroy(buttonView.gameObject);
        }

        private ViewBuySkin GetButtonView(int effectId, SkinData skinData)
        {
            foreach (ViewBuySkin viewBuySkin in _listView)
            {
                if (effectId == viewBuySkin.EffectID && skinData.ID == viewBuySkin.SkinDataButton.ID)
                    return viewBuySkin;
            }

            return null;
        }
    }
}