using CosmicSpaceCommunication.Game.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class HangarWindow : GameWindow
{
    public enum HangarPanels
    {
        Lasers,
        Generators,
        Extras,
        Warehouse
    }

    [Header("Equipment")]
    public Transform EquipmentContent;

    [Header("Warehouse")]
    public Transform WarehouseContent;

    [Header("Prefabs")]
    public GameObject TitlePanelPrefab;
    public GameObject SlotPanelPrefab;
    public GameObject SlotPrefab;

    Dictionary<HangarPanels, Transform> Panels;



    public override void Start()
    {
        base.Start();
    }

    public override void Refresh()
    {
        base.Refresh();
    }

    public override void ChangeLanguage()
    {
        base.ChangeLanguage();
    }

    private void CreateHanger()
    {
        Panels = new Dictionary<HangarPanels, Transform>();

        Player.DestroyChilds(EquipmentContent);
        Player.DestroyChilds(WarehouseContent);

        foreach (HangarPanels hangarPanel in (HangarPanels[])Enum.GetValues(typeof(HangarPanels)))
        {
            bool isWarehouse = hangarPanel == HangarPanels.Warehouse;

            Transform slotPanel = CreateSlotPanel(isWarehouse ? WarehouseContent : EquipmentContent, hangarPanel);
            ItemContainer itemContainer = slotPanel.GetComponent<ItemContainer>();
            itemContainer.HangarType = hangarPanel;

            int slotCount = 0;
            switch (hangarPanel)
            {
                case HangarPanels.Warehouse:
                    slotCount = Client.Pilot.Items.Count + 10;
                    break;
                case HangarPanels.Lasers:
                    slotCount = Client.Pilot.Ship.Lasers;
                    break;
                case HangarPanels.Generators:
                    slotCount = Client.Pilot.Ship.Generators;
                    break;
                case HangarPanels.Extras:
                    slotCount = Client.Pilot.Ship.Extras;
                    break;
            }

            for (int i = 0; i < slotCount; i++)
            {
                CreateSlot(itemContainer);
            }

            StartCoroutine(SetPanelCollider(slotPanel));

            switch (hangarPanel)
            {
                case HangarPanels.Warehouse:
                    foreach (ItemPilot item in Client.Pilot.Items.Where(o => !o.IsEquipped))
                    {
                        itemContainer.AddItem(item);
                    }
                    break;
                case HangarPanels.Lasers:
                    foreach (ItemPilot item in Client.Pilot.Items.Where(o => o.IsEquipped && o.Item.ItemType == ItemTypes.Laser))
                    {
                        itemContainer.AddItem(item);
                    }
                    break;
                case HangarPanels.Generators:
                    foreach (ItemPilot item in Client.Pilot.Items.Where(o => o.IsEquipped && o.Item.ItemType == ItemTypes.Generator))
                    {
                        itemContainer.AddItem(item);
                    }
                    break;
                case HangarPanels.Extras:
                    foreach (ItemPilot item in Client.Pilot.Items.Where(o => o.IsEquipped && o.Item.ItemType == ItemTypes.Extra))
                    {
                        itemContainer.AddItem(item);
                    }
                    break;
            }
        }
    }

    IEnumerator SetPanelCollider(Transform slotPanel)
    {
        yield return new WaitForEndOfFrame();
        RectTransform containerRect = slotPanel.GetComponent<RectTransform>();
        slotPanel.GetComponent<BoxCollider2D>().size = new Vector2(containerRect.rect.width + 5, containerRect.rect.height + 15);
    }

    private Transform CreateSlotPanel(Transform content, HangarPanels hangarPanel)
    {
        string title = string.Empty;
        switch(hangarPanel)
        {
            case HangarPanels.Lasers:
                title = GameSettings.UserLanguage.LASERS;
                break;
            case HangarPanels.Generators:
                title = GameSettings.UserLanguage.GENERATORS;
                break;
            case HangarPanels.Extras:
                title = GameSettings.UserLanguage.EXTRAS;
                break;
            case HangarPanels.Warehouse:
                title = GameSettings.UserLanguage.WAREHOUSE;
                break;
        }

        GameObject go = Instantiate(TitlePanelPrefab, content);
        go.GetComponent<Text>().text = $"  {title}";

        Transform panel = Instantiate(SlotPanelPrefab, content).transform;
        panel.name = hangarPanel.ToString();
        Panels.Add(hangarPanel, panel);

        return panel;
    }

    private Transform CreateSlot(ItemContainer itemContainer)
    {
        Transform slot = Instantiate(SlotPrefab, itemContainer.transform).transform;
        itemContainer.Slots.Add(slot);
        return slot;
    }

    private void OnEnable()
    {
        CreateHanger();
    }
}