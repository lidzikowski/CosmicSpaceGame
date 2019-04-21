using CosmicSpaceCommunication.Game.Resources;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    public List<Transform> Slots = new List<Transform>();
    public List<ItemPilot> Items = new List<ItemPilot>();

    public GameObject ItemPrefab;
    public HangarWindow.HangarPanels HangarType;


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

    private bool InsertItemInEmptySlot(ItemPilot itemPilot)
    {
        foreach (Transform t in Slots)
        {
            if (t.childCount == 0)
            {
                GameObject go = Instantiate(ItemPrefab, t);
                ItemHandler itemHandler = go.GetComponent<ItemHandler>();
                itemHandler.ItemPilot = itemPilot;
                itemHandler.ItemContainer = transform;
                itemHandler.transform.localPosition = Vector3.zero;
                return true;
            }
        }
        return false;
    }


    public bool RemoveItem(ItemPilot item)
    {
        ItemPilot itemInContainer = Items.FirstOrDefault(o => o.Item.ItemId == item.Item.ItemId);
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