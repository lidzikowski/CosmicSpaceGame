using CosmicSpaceCommunication.Game.Resources;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

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

            if(value == null)
            {
                Destroy(gameObject);
                return;
            }

            ItemNameText.text = Item.Item.Name;
            ItemTypeText.text = Item.Item.ItemType.ToString();

            // Damage
            if (Item.Item.LaserDamagePve == Item.Item.LaserDamagePvp && Item.Item.LaserDamagePvp > 0)
            {
                Property(GameSettings.UserLanguage.DAMAGE, Item.Item.LaserDamagePve);
            }
            else
            {
                Property(GameSettings.UserLanguage.DAMAGE_PVP, Item.Item.LaserDamagePvp);
                Property(GameSettings.UserLanguage.DAMAGE_PVE, Item.Item.LaserDamagePve);
            }

            // Shot Range
            Property(GameSettings.UserLanguage.SHOT_RANGE, Item.Item.LaserShotRange);

            // Shot Dispersion
            if (Item.Item.LaserShotDispersion > 0)
                Property(GameSettings.UserLanguage.SHOT_DISPERSION, $"{Item.Item.LaserShotDispersion * 100} %");



            // Speed
            Property(GameSettings.UserLanguage.SPEED, Item.Item.GeneratorSpeed);

            // Shield
            Property(GameSettings.UserLanguage.SHIELD, Item.Item.GeneratorShield);

            // Shield Division
            if (Item.Item.GeneratorShieldDivision > 0)
                Property(GameSettings.UserLanguage.SHIELD_DIVISION, $"{Item.Item.GeneratorShieldDivision * 100} %");

            // Shield Repair
            if (Item.Item.GeneratorShieldRepair > 0)
                Property(GameSettings.UserLanguage.SHIELD_REPAIR, $"{Item.Item.GeneratorShieldRepair} s.");



            transform.position = new Vector2(ItemInfo.transform.position.x, ItemInfo.transform.position.y - GetComponent<RectTransform>().rect.height);
        }
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
}