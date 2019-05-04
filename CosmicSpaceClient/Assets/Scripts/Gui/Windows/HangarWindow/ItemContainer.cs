using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Resources;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public List<Transform> Slots = new List<Transform>();
    public List<ItemPilot> Items = new List<ItemPilot>();
    public List<PilotResource> PilotResources = new List<PilotResource>();

    public GameObject ItemPrefab;
    public HangarWindow.HangarPanels HangarType;


    public void ClearItems()
    {
        Items.Clear();
        PilotResources.Clear();
        foreach (Transform t in Slots)
        {
            if (t.childCount > 0)
            {
                Destroy(t.GetChild(0).gameObject);
            }
        }
    }

    public bool AddResource(PilotResource resource)
    {
        if (PilotResources.Count + 1 > Slots.Count)
            return false;

        if (InsertItemInEmptySlot(resource))
        {
            PilotResources.Add(resource);
            return true;
        }
        return false;
    }

    public bool AddItem(ItemPilot item)
    {
        if (Items.Count + 1 > Slots.Count)
            return false;

        if (InsertItemInEmptySlot(item))
        {
            Items.Add(item);
            return true;
        }
        return false;
    }

    private bool InsertItemInEmptySlot(object itemGameObject)
    {
        foreach (Transform t in Slots)
        {
            if (t.childCount == 0)
            {
                GameObject go = Instantiate(ItemPrefab, t);
                ItemHandler itemHandler = go.GetComponent<ItemHandler>();
                itemHandler.ItemContainer = transform;
                itemHandler.transform.localPosition = Vector3.zero;

                if(itemGameObject is ItemPilot itemPilot)
                {
                    ApplyItemPilot(itemHandler, itemPilot);
                    itemHandler.ItemPilot.IsEquipped = HangarType != HangarWindow.HangarPanels.Warehouse;
                }
                else if (itemGameObject is PilotResource pilotResource)
                {
                    ApplyPilotResource(itemHandler, pilotResource);
                }

                return true;
            }
        }
        return false;
    }

    private void ApplyItemPilot(ItemHandler itemHandler, ItemPilot itemPilot)
    {
        itemHandler.ItemPilot = itemPilot;
        if (ResourcesUI.Instance.ShipSprites.ContainsKey(itemPilot.Item.Prefab.PrefabName))
            itemHandler.ItemTexture.sprite = ResourcesUI.Instance.ShipSprites[itemPilot.Item.Prefab.PrefabName];
    }

    private void ApplyPilotResource(ItemHandler itemHandler, PilotResource pilotResource)
    {
        itemHandler.PilotResource = pilotResource;
        if (ResourcesUI.Instance.ShipSprites.ContainsKey(pilotResource.ColumnName))
            itemHandler.ItemTexture.sprite = ResourcesUI.Instance.ShipSprites[pilotResource.ColumnName];
    }


    public bool RemoveItem(ItemPilot item)
    {
        ItemPilot itemInContainer = Items.FirstOrDefault(o => o.RelationId == item.RelationId);
        if (itemInContainer != null)
        {
            Items.Remove(itemInContainer);
            StartCoroutine(RefreshItems());
            return true;
        }

        return false;
    }

    private IEnumerator RefreshItems()
    {
        foreach (Transform t in Slots)
        {
            if(t.childCount > 0)
            {
                foreach (Transform child in t)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        yield return new WaitForEndOfFrame();

        foreach (ItemPilot itemPilot in Items)
        {
            InsertItemInEmptySlot(itemPilot);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            bool status = false;
            ItemHandler item = collision.gameObject.GetComponent<ItemHandler>();

            if (item.ItemPilot.Item == null)
                return;

            switch (HangarType)
            {
                case HangarWindow.HangarPanels.Lasers:
                    if (item.ItemPilot.Item.ItemType == ItemTypes.Laser)
                        status = true;
                    break;
                case HangarWindow.HangarPanels.Generators:
                    if (item.ItemPilot.Item.ItemType == ItemTypes.Generator)
                        status = true;
                    break;
                case HangarWindow.HangarPanels.Extras:
                    if (item.ItemPilot.Item.ItemType == ItemTypes.Extra)
                        status = true;
                    break;
                case HangarWindow.HangarPanels.Warehouse:
                    status = true;
                    break;
            }

            if (status)
                item.ItemContainerTarget = transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        collision.gameObject.GetComponent<ItemHandler>().ItemContainerTarget = null;
    }
}