using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Resources;
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


    PilotResource PilotResource => itemInfo?.PilotResource;
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

            transform.position = new Vector2(-10000, -10000);

            if (Item.Item != null)
            {
                ItemNameText.text = Item.Item.Name;
                ItemTypeText.text = Item.Item.ItemType.ToString();
                FindLanguageToProperties(Item.Item, Property);
            }
            else if (PilotResource != null)
            {
                ItemNameText.text = PilotResource.ColumnName;
                ItemTypeText.text = GameSettings.UserLanguage.RESOURCE;
                FindLanguageToProperties(Client.Pilot.ServerResources[PilotResource.AmmunitionId], Property);
                Property(GameSettings.UserLanguage.QUANTITY, PilotResource.Count);
            }

            StartCoroutine(Position());
        }
    }

    IEnumerator Position()
    {
        yield return new WaitForSeconds(0.2f);

        if (ItemInfo.transform.position.y - GetComponent<RectTransform>().rect.height < 50)
        {
            transform.position = new Vector2(ItemInfo.transform.position.x, ItemInfo.transform.position.y + (GetComponent<RectTransform>().rect.height / 2) + 30);
        }
        else
        {
            transform.position = new Vector2(ItemInfo.transform.position.x, ItemInfo.transform.position.y - (GetComponent<RectTransform>().rect.height / 2) - 30);
        }
    }



    private void Property(string name, object value)
    {
        if (!string.IsNullOrWhiteSpace(value?.ToString()))
        {
            Instantiate(PropertyPrefab, ItemProperties).GetComponent<ToolTipProperty>().SetProperty(name, value.ToString());
        }
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

                case ItemProperty.MultiplierPlayer:
                case ItemProperty.MultiplierEnemy:
                    if (duplicateLaserDamage)
                        continue;

                    if (shopItem.ItemDescription.ContainsKey(ItemProperty.MultiplierPlayer) && shopItem.ItemDescription.ContainsKey(ItemProperty.MultiplierEnemy) && shopItem.ItemDescription[ItemProperty.MultiplierPlayer].Equals(shopItem.ItemDescription[ItemProperty.MultiplierEnemy]))
                    {
                        language = GameSettings.UserLanguage.MULTIPLIER;
                        duplicateLaserDamage = true;
                    }
                    else
                    {
                        if (item.Key == ItemProperty.MultiplierPlayer)
                            language = GameSettings.UserLanguage.MULTIPLIER_PLAYER;
                        else
                            language = GameSettings.UserLanguage.MULTIPLIER_ENEMY;
                    }
                    break;

                case ItemProperty.IsAmmunition:
                    language = GameSettings.UserLanguage.IS_AMMO;
                    value = (bool)value ? GameSettings.UserLanguage.IS_AMMO_LASER : GameSettings.UserLanguage.IS_AMMO_ROCKET;
                    break;

                case ItemProperty.BaseDamage:
                    language = GameSettings.UserLanguage.BASE_DAMAGE;
                    break;
            }

            languageFunction?.Invoke(language, value);
        }
    }
}