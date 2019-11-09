using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Resources;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemInformationWindow : GameWindow
{
    private ItemPilot ItemPilot;
    private PilotResource PilotResource;

    [Header("Item")]
    public Text ItemNameText;

    [Header("Sell")]
    public Button ItemSellButton;
    public Text ItemSellText;



    public override void Start()
    {
        base.Start();

        ButtonListener(ItemSellButton, ItemSellButton_Clicked);
    }

    public override void Refresh()
    {
        base.Refresh();
    }

    public override void ChangeLanguage()
    {
        base.ChangeLanguage();

        if (ItemPilot?.Item != null)
        {
            bool scrap = ItemPilot.Item.ScrapPrice > 0;
            SetText(ItemSellText, $"{GameSettings.UserLanguage.SELL_ITEM}{System.Environment.NewLine}{(scrap ? ItemPilot.Item.ScrapPrice / 30 : ItemPilot.Item.MetalPrice / 5)} Scrap");
        }
        else
            SetText(ItemSellText, GameSettings.UserLanguage.SELL_ITEM);
    }

    public void ShowItemInformation(ItemPilot itemPilot, PilotResource pilotResource)
    {
        ItemPilot = itemPilot;
        PilotResource = pilotResource;

        ChangeLanguage();

        if(ItemPilot.Item != null)
        {
            SetText(ItemNameText, ItemPilot.Item.Name);
            ItemSellButton.interactable = true;
        }
        else if (PilotResource != null)
        {
            SetText(ItemNameText, PilotResource.ColumnName);
            ItemSellButton.interactable = false;
        }
    }

    private void ItemSellButton_Clicked()
    {
        ItemPilot itemPilot = Client.Pilot.Items.FirstOrDefault(o => o.RelationId == ItemPilot.RelationId);
        if (itemPilot == null)
            return;

        Client.SendToSocket(new CommandData()
        {
            Command = Commands.SellEquipmentItem,
            Data = itemPilot
        });

        Close();
    }
}