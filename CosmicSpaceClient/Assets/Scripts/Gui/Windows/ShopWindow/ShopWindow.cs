using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Resources;
using System;
using System.Collections;
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

    [Header("Buttons Text")]
    public Text ShipsText;
    public Text LasersText;
    public Text GeneratorsText;
    public Text ExtrasText;

    [Header("Items")]
    public Transform ItemsTransform;
    public GameObject ItemPrefab;

    [Header("Selected Item")]
    public Transform SelectedItemTransform;



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
        foreach (IShopItem item in ServerItems.Ships)
        {
            CreateItem(item);
        }
    }

    private void Buttons_Clicked(ItemTypes itemType)
    {
        foreach (IShopItem item in ServerItems.Items.Where(o => o.ItemType == itemType))
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
        Debug.Log(item.Name);
    }
}