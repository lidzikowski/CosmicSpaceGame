using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;
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
    public Button ResourcesButton;

    [Header("Buttons Text")]
    public Text ShipsText;
    public Text LasersText;
    public Text GeneratorsText;
    public Text ExtrasText;
    public Text ResourcesText;

    [Header("Items")]
    public Transform ItemsTransform;
    public GameObject ItemPrefab;

    [Header("Selected Item Panel")]
    public Transform SelectedItemTransform;
    public Text ItemNameText;
    public Transform ContentTransform;
    public GameObject PropertyPrefab;
    public RawImage ItemRawImage;
    public RenderTexture ItemRenderTexture;

    [Header("Selected Item Buttons")]
    public Button BuyScrapButton;
    public Text BuyScrapText;
    public Button BuyMetalButton;
    public Text BuyMetalText;
    public InputField QuantityInputField;
    public Text QuantityText;



    public ShopItems ServerItems;



    public override void Start()
    {
        base.Start();

        ButtonListener(ShipsButton, () => Player.DestroyChilds(ItemsTransform));
        ButtonListener(LasersButton, () => Player.DestroyChilds(ItemsTransform));
        ButtonListener(GeneratorsButton, () => Player.DestroyChilds(ItemsTransform));
        ButtonListener(ExtrasButton, () => Player.DestroyChilds(ItemsTransform));
        ButtonListener(ResourcesButton, () => Player.DestroyChilds(ItemsTransform));

        ButtonListener(ShipsButton, ShipsButton_Clicked);
        ButtonListener(LasersButton, () => Buttons_Clicked(ItemTypes.Laser));
        ButtonListener(GeneratorsButton, () => Buttons_Clicked(ItemTypes.Generator));
        ButtonListener(ExtrasButton, () => Buttons_Clicked(ItemTypes.Extra));
        ButtonListener(ResourcesButton, () => Buttons_Clicked(ItemTypes.Ammunition));

        QuantityInputField.onValueChanged.AddListener((s) => QuantityInputField_Changed(s));

        ResourcesUI.Instance.LoadImages();

        Player.DestroyChilds(ItemsTransform);
        ShipsButton_Clicked();
    }

    public override void Refresh()
    {
        base.Refresh();

        if (LastShowShopItem == null)
            return;

        if (LastShowShopItem.ScrapPrice > 0)
        {
            SetText(BuyScrapText, $"{GameSettings.UserLanguage.BUY_FOR} Scrap{System.Environment.NewLine}{LastShowShopItem.ScrapPrice * long.Parse(QuantityInputField.text)}");
            BuyScrapButton.interactable = true;
            ButtonListener(BuyScrapButton, () => BuyItem(LastShowShopItem, true), true);
        }
        else
        {
            SetText(BuyScrapText, GameSettings.UserLanguage.UNAVAILABLE);
            BuyScrapButton.interactable = false;
        }

        if (LastShowShopItem.MetalPrice > 0)
        {
            SetText(BuyMetalText, $"{GameSettings.UserLanguage.BUY_FOR} Metal{System.Environment.NewLine}{LastShowShopItem.MetalPrice * long.Parse(QuantityInputField.text)}");
            BuyMetalButton.interactable = true;
            ButtonListener(BuyMetalButton, () => BuyItem(LastShowShopItem, false), true);
        }
        else
        {
            SetText(BuyMetalText, GameSettings.UserLanguage.UNAVAILABLE);
            BuyMetalButton.interactable = false;
        }
    }

    public override void ChangeLanguage()
    {
        base.ChangeLanguage();

        SetText(ShipsText, GameSettings.UserLanguage.SHIPS);
        SetText(LasersText, GameSettings.UserLanguage.LASERS);
        SetText(GeneratorsText, GameSettings.UserLanguage.GENERATORS);
        SetText(ExtrasText, GameSettings.UserLanguage.EXTRAS);
        SetText(ResourcesText, GameSettings.UserLanguage.RESOURCES);
        SetText(QuantityText, GameSettings.UserLanguage.QUANTITY);
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

        if(itemType == ItemTypes.Ammunition)
        {
            foreach (IShopItem item in Client.Pilot.ServerResources.Values)
            {
                CreateItem(item);
            }
            return;
        }

        foreach (IShopItem item in ServerItems.Items.Where(o => o.ItemType == itemType).OrderBy(o => o.RequiredLevel).ThenBy(o => o.Prefab.PrefabName))
        {
            CreateItem(item);
        }
    }

    private void CreateItem(IShopItem item)
    {
        GameObject go = Instantiate(ItemPrefab, ItemsTransform);

        if (ResourcesUI.Instance.ShipSprites.ContainsKey(item.Prefab.PrefabName))
            go.GetComponent<Image>().sprite = ResourcesUI.Instance.ShipSprites[item.Prefab.PrefabName];
        else
            Debug.Log($"Brak prefabu: {item.Prefab.PrefabTypeName} {item.Prefab.PrefabName}");

        go.GetComponent<ShopItem>().Item = item;
        go.GetComponent<ShopItem>().OnShowInformation = ShowInformation;
    }


    IShopItem LastShowShopItem = null;

    private void ShowInformation(IShopItem item)
    {
        if (LastShowShopItem != null && LastShowShopItem == item)
            return;
        LastShowShopItem = item;

        Player.DestroyChilds(ContentTransform);
        if (item == null)
        {
            SelectedItemTransform.gameObject.SetActive(false);
            ResourcesUI.Instance.RotateItem(null);
            return;
        }
        SelectedItemTransform.gameObject.SetActive(true);

        SetText(ItemNameText, item.Name);

        if (ResourcesUI.Instance.ShipSprites.ContainsKey(item.Prefab.PrefabName))
        {
            ItemRawImage.texture = ItemRenderTexture;
            ResourcesUI.Instance.RotateItem(item.Prefab.PrefabName);
        }
        else
        {
            ItemRawImage.texture = null;
            ResourcesUI.Instance.RotateItem(null);
            Debug.Log($"Brak prefabu: {item.Prefab.PrefabTypeName} {item.Prefab.PrefabName}");
        }

        QuantityInputField.text = item.ItemType == ItemTypes.Ammunition ? "1000" : "1";
        QuantityInputField.interactable = item.ItemType != ItemTypes.Ship;

        Refresh();

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
        bool status = false;

        if(item.ItemType == ItemTypes.Ship && Client.Pilot.Ship.Id == item.Id)
        {
            GuiScript.CreateLogMessage(new List<string> { GameSettings.UserLanguage.YOU_HAVE_SHIP });
            return;
        }

        if (scrap)
        {
            if (item.ScrapPrice > 0 && Client.Pilot.Scrap >= item.ScrapPrice)
            {
                status = true;
            }
            else
                GuiScript.CreateLogMessage(new List<string> { string.Format(GameSettings.UserLanguage.NOT_HAVE_ENOUGH, "Scrap") });
        }
        else
        {
            if (item.MetalPrice > 0 && Client.Pilot.Metal >= item.MetalPrice)
            {
                status = true;
            }
            else
                GuiScript.CreateLogMessage(new List<string> { string.Format(GameSettings.UserLanguage.NOT_HAVE_ENOUGH, "Metal") });
        }

        if (!status)
            return;

        if (!int.TryParse(QuantityInputField.text, out int quantity))
            return;

        Client.SendToSocket(new CommandData()
        {
            Command = Commands.BuyShopItem,
            SenderId = Client.Pilot.Id,
            Data = new BuyShopItem()
            {
                ItemType = item.ItemType,
                ItemId = item.Id,
                Scrap = scrap,
                Count = quantity
            }
        });
    }

    private void QuantityInputField_Changed(string s)
    {
        if (string.IsNullOrWhiteSpace(s) || !int.TryParse(s, out int number) || number < 1)
        {
            QuantityInputField.text = "1";
            return;
        }

        Refresh();
    }

    private void OnDisable()
    {
        ResourcesUI.Instance.RotateItem(null);
        ResourcesUI.Instance.Dispose();
    }
}