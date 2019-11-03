using CosmicSpaceCommunication.Game.Player;
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
        Warehouse,
        Resources
    }

    [Header("Equipment")]
    public Transform EquipmentContent;

    [Header("Warehouse")]
    public Transform WarehouseContent;

    [Header("Prefabs")]
    public GameObject TitlePanelPrefab;
    public GameObject SlotPanelPrefab;
    public GameObject SlotPrefab;

    Dictionary<HangarPanels, ItemContainer> Panels;



    public override void Start()
    {
        ResourcesUI.Instance.LoadImages();

        base.Start();
    }

    public override void Refresh()
    {
        base.Refresh();

        CreateHanger();
    }

    public override void ChangeLanguage()
    {
        base.ChangeLanguage();
    }

    private void CreateHanger()
    {
        Panels = new Dictionary<HangarPanels, ItemContainer>();

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
                case HangarPanels.Resources:
                    slotCount = Client.Pilot.Resources.Count;
                    break;
            }

            for (int i = 0; i < slotCount; i++)
            {
                CreateSlot(itemContainer);
            }

            StartCoroutine(SetPanelCollider(slotPanel));

            StartCoroutine(RefreshPanel(hangarPanel));
        }
    }

    public void RefreshAllPanels()
    {
        foreach (HangarPanels hangarPanel in Panels.Keys)
        {
            StartCoroutine(RefreshPanel(hangarPanel));
        }
    }

    public IEnumerator RefreshPanel(HangarPanels hangarPanel)
    {
        ItemContainer itemContainer = Panels[hangarPanel];
        itemContainer.ClearItems();
        yield return new WaitForEndOfFrame();
        switch (hangarPanel)
        {
            case HangarPanels.Warehouse:
                foreach (ItemPilot item in Client.Pilot.Items.Where(o => !o.IsEquipped).OrderBy(o => o.RelationId))
                {
                    itemContainer.AddItem(item);
                }
                break;
            case HangarPanels.Lasers:
                foreach (ItemPilot item in Client.Pilot.Items.Where(o => o.IsEquipped && o.Item.ItemType == ItemTypes.Laser).OrderBy(o => o.RelationId))
                {
                    itemContainer.AddItem(item);
                }
                break;
            case HangarPanels.Generators:
                foreach (ItemPilot item in Client.Pilot.Items.Where(o => o.IsEquipped && o.Item.ItemType == ItemTypes.Generator).OrderBy(o => o.RelationId))
                {
                    itemContainer.AddItem(item);
                }
                break;
            case HangarPanels.Extras:
                foreach (ItemPilot item in Client.Pilot.Items.Where(o => o.IsEquipped && o.Item.ItemType == ItemTypes.Extra).OrderBy(o => o.RelationId))
                {
                    itemContainer.AddItem(item);
                }
                break;
            case HangarPanels.Resources:
                foreach (PilotResource resource in Client.Pilot.Resources.Values)
                {
                    itemContainer.AddResource(resource);
                }
                break;
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
        int itemCount = 0;
        int itemSlot = 0;

        switch(hangarPanel)
        {
            case HangarPanels.Lasers:
                title = GameSettings.UserLanguage.LASERS;
                itemCount = Client.Pilot.Items.Where(o => o.IsEquipped && o.Item.ItemType == ItemTypes.Laser).Count();
                itemSlot = Client.Pilot.Ship.Lasers;
                break;
            case HangarPanels.Generators:
                title = GameSettings.UserLanguage.GENERATORS;
                itemCount = Client.Pilot.Items.Where(o => o.IsEquipped && o.Item.ItemType == ItemTypes.Generator).Count();
                itemSlot = Client.Pilot.Ship.Generators;
                break;
            case HangarPanels.Extras:
                title = GameSettings.UserLanguage.EXTRAS;
                itemCount = Client.Pilot.Items.Where(o => o.IsEquipped && o.Item.ItemType == ItemTypes.Extra).Count();
                itemSlot = Client.Pilot.Ship.Extras;
                break;
            case HangarPanels.Warehouse:
                title = GameSettings.UserLanguage.WAREHOUSE;
                itemCount = Client.Pilot.Items.Where(o => !o.IsEquipped).Count();
                itemSlot = Client.Pilot.Items.Count + 10;
                break;
            case HangarPanels.Resources:
                title = GameSettings.UserLanguage.RESOURCES;
                break;
        }

        GameObject go = Instantiate(TitlePanelPrefab, content);
        go.GetComponent<Text>().text = $"  {title}";

        Transform panel = Instantiate(SlotPanelPrefab, content).transform;
        panel.name = hangarPanel.ToString();
        Panels.Add(hangarPanel, panel.GetComponent<ItemContainer>());

        panel.GetComponent<ItemContainer>().ContainerTitle = title;
        panel.GetComponent<ItemContainer>().ContainerTitleText = go.GetComponent<Text>();

        return panel;
    }

    private Transform CreateSlot(ItemContainer itemContainer)
    {
        Transform slot = Instantiate(SlotPrefab, itemContainer.transform).transform;
        itemContainer.Slots.Add(slot);
        return slot;
    }

    private void OnDisable()
    {
        ResourcesUI.Instance.Dispose();
    }
}