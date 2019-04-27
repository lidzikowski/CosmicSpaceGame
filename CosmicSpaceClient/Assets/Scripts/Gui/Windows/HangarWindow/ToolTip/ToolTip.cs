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

            transform.position = new Vector2(-1000, -1000);

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
}