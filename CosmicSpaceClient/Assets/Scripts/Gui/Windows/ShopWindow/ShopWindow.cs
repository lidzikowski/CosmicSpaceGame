using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Resources;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ShopWindow : GameWindow
{
    [Header("Buttons")]
    public Button ShipsButton;
    public Button LasersButton;
    public Button GeneratorsButton;
    public Button ExtrasButton;

    [Header("Buttons Text")]
    public Text ShipsText;
    public Text LasersText;
    public Text GeneratorsText;
    public Text ExtrasText;

    [Header("Items")]
    public Transform ItemsTransform;
    public GameObject ItemPrefab;

    [Header("Selected Item Panel")]
    public Transform SelectedItemTransform;
    public Text ItemNameText;
    public Transform ContentTransform;
    public GameObject PropertyPrefab;

    [Header("Selected Item Buttons")]
    public Button BuyScrapButton;
    public Text BuyScrapText;
    public Button BuyMetalButton;
    public Text BuyMetalText;



    public ShopItems ServerItems;



    public override void Start()
    {
        base.Start();

        ButtonListener(ShipsButton, () => Player.DestroyChilds(ItemsTransform));
        ButtonListener(LasersButton, () => Player.DestroyChilds(ItemsTransform));
        ButtonListener(GeneratorsButton, () => Player.DestroyChilds(ItemsTransform));
        ButtonListener(ExtrasButton, () => Player.DestroyChilds(ItemsTransform));

        ButtonListener(ShipsButton, ShipsButton_Clicked);
        ButtonListener(LasersButton, () => Buttons_Clicked(ItemTypes.Laser));
        ButtonListener(GeneratorsButton, () => Buttons_Clicked(ItemTypes.Generator));
        ButtonListener(ExtrasButton, () => Buttons_Clicked(ItemTypes.Extra));

        Player.DestroyChilds(ItemsTransform);
        ShipsButton_Clicked();
    }

    public override void Refresh()
    {
        base.Refresh();
    }

    public override void ChangeLanguage()
    {
        base.ChangeLanguage();

        SetText(ShipsText, GameSettings.UserLanguage.SHIPS);
        SetText(LasersText, GameSettings.UserLanguage.LASERS);
        SetText(GeneratorsText, GameSettings.UserLanguage.GENERATORS);
        SetText(ExtrasText, GameSettings.UserLanguage.EXTRAS);
    }



    private void ShipsButton_Clicked()
    {
        ShowInformation(null);
        foreach (IShopItem item in ServerItems.Ships.OrderBy(o => o.RequiredLevel))
        {
            CreateItem(item);
        }
    }

    private void Buttons_Clicked(ItemTypes itemType)
    {
        ShowInformation(null);
        foreach (IShopItem item in ServerItems.Items.Where(o => o.ItemType == itemType).OrderBy(o => o.Id))
        {
            CreateItem(item);
        }
    }

    private void CreateItem(IShopItem item)
    {
        GameObject go = Instantiate(ItemPrefab, ItemsTransform);
        go.GetComponent<ShopItem>().Item = item;
        go.GetComponent<ShopItem>().OnShowInformation = ShowInformation;
    }

    private void ShowInformation(IShopItem item)
    {
        Player.DestroyChilds(ContentTransform);
        if (item == null)
        {
            SelectedItemTransform.gameObject.SetActive(false);
            return;
        }
        SelectedItemTransform.gameObject.SetActive(true);

        SetText(ItemNameText, item.Name);

        if(item.ScrapPrice > 0)
        {
            SetText(BuyScrapText, $"{GameSettings.UserLanguage.BUY_FOR} Scrap{System.Environment.NewLine}{item.ScrapPrice}");
            BuyScrapButton.interactable = true;
            ButtonListener(BuyScrapButton, () => BuyItem(item, true), true);
        }
        else
        {
            SetText(BuyScrapText, GameSettings.UserLanguage.UNAVAILABLE);
            BuyScrapButton.interactable = false;
        }

        if (item.MetalPrice > 0)
        {
            SetText(BuyMetalText, $"{GameSettings.UserLanguage.BUY_FOR} Metal{System.Environment.NewLine}{item.MetalPrice}");
            BuyMetalButton.interactable = true;
            ButtonListener(BuyMetalButton, () => BuyItem(item, false), true);
        }
        else
        {
            SetText(BuyMetalText, GameSettings.UserLanguage.UNAVAILABLE);
            BuyMetalButton.interactable = false;
        }

        ToolTip.FindLanguageToProperties(item, Property);
    }

    private void Property(string name, object value)
    {
        if (!string.IsNullOrWhiteSpace(value?.ToString()))
        {
            Instantiate(PropertyPrefab, ContentTransform).GetComponent<ToolTipProperty>().SetProperty(name, value.ToString());
        }
    }

    private void BuyItem(IShopItem item, bool scrap)
    {
        Debug.Log($"Kup {item.Id} {item.Name} -> {scrap}");
    }
}