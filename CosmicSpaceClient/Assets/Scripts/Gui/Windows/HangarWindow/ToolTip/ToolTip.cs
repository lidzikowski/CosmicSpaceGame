﻿using CosmicSpaceCommunication.Game.Resources;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public delegate void LanguageFunction(string name, object value);

public class ToolTip : MonoBehaviour
{
    public Text ItemNameText;
    public Text ItemTypeText;
    public Transform ItemProperties;
    public GameObject PropertyPrefab;



    ItemPilot Item => itemInfo?.ItemPilot;

    private ItemHandler itemInfo;
    public ItemHandler ItemInfo
    {
        get => itemInfo;
        set
        {
            if (itemInfo == value)
                return;

            itemInfo = value;

            if (value == null)
            {
                Destroy(gameObject);
                return;
            }

            transform.position = new Vector2(-1000, -1000);

            ItemNameText.text = Item.Item.Name;
            ItemTypeText.text = Item.Item.ItemType.ToString();

            FindLanguageToProperties(Item.Item, Property);

            StartCoroutine(Position());
        }
    }

    IEnumerator Position()
    {
        yield return new WaitForEndOfFrame();
        transform.position = new Vector2(ItemInfo.transform.position.x, ItemInfo.transform.position.y - GetComponent<RectTransform>().rect.height - 5);
    }



    private void Property(string name, object value)
    {
        if (!string.IsNullOrWhiteSpace(value?.ToString()))
        {
            Instantiate(PropertyPrefab, ItemProperties).GetComponent<ToolTipProperty>().SetProperty(name, value.ToString());
        }
    }

    private void Awake()
    {
        ItemInfo = null;
    }

    public static void FindLanguageToProperties(IShopItem shopItem, LanguageFunction languageFunction = null)
    {
        bool duplicateLaserDamage = false;
        foreach (var item in shopItem.ItemDescription)
        {
            if (item.Value == null)
                continue;

            string language = string.Empty;
            object value = item.Value;

            switch (item.Key)
            {
                case ItemProperty.LaserDamagePvp:
                case ItemProperty.LaserDamagePve:
                    if (duplicateLaserDamage)
                        continue;

                    if (shopItem.ItemDescription.ContainsKey(ItemProperty.LaserDamagePvp) && shopItem.ItemDescription.ContainsKey(ItemProperty.LaserDamagePve) && shopItem.ItemDescription[ItemProperty.LaserDamagePvp].Equals(shopItem.ItemDescription[ItemProperty.LaserDamagePve]))
                    {
                        language = GameSettings.UserLanguage.DAMAGE;
                        duplicateLaserDamage = true;
                    }
                    else
                    {
                        if (item.Key == ItemProperty.LaserDamagePvp)
                            language = GameSettings.UserLanguage.DAMAGE_PVP;
                        else
                            language = GameSettings.UserLanguage.DAMAGE_PVE;
                    }
                    break;

                case ItemProperty.LaserShotRange:
                    language = GameSettings.UserLanguage.SHOT_RANGE;
                    break;

                case ItemProperty.LaserShotDispersion:
                    language = GameSettings.UserLanguage.SHOT_DISPERSION;
                    value = $"{(float)value * 100} %";
                    break;


                case ItemProperty.GeneratorSpeed:
                    language = GameSettings.UserLanguage.SPEED;
                    break;

                case ItemProperty.GeneratorShield:
                    language = GameSettings.UserLanguage.SHIELD;
                    break;

                case ItemProperty.GeneratorShieldDivision:
                    language = GameSettings.UserLanguage.SHIELD_DIVISION;
                    value = $"{(float)value * 100} %";
                    break;

                case ItemProperty.GeneratorShieldRepair:
                    language = GameSettings.UserLanguage.SHIELD_REPAIR;
                    value = $"{value} s.";
                    break;


                case ItemProperty.Lasers:
                    language = GameSettings.UserLanguage.LASERS;
                    break;

                case ItemProperty.Generators:
                    language = GameSettings.UserLanguage.GENERATORS;
                    break;

                case ItemProperty.Extras:
                    language = GameSettings.UserLanguage.EXTRAS;
                    break;

                case ItemProperty.Speed:
                    language = GameSettings.UserLanguage.SPEED;
                    value = (int)((float)value * 8);
                    break;

                case ItemProperty.Cargo:
                    language = GameSettings.UserLanguage.CARGO;
                    break;

                case ItemProperty.Hitpoints:
                    language = GameSettings.UserLanguage.HITPOINTS;
                    break;

                case ItemProperty.RequiredLevel:
                    language = GameSettings.UserLanguage.REQUIRED_LEVEL;
                    break;
            }

            languageFunction?.Invoke(language, value);
        }
    }
}