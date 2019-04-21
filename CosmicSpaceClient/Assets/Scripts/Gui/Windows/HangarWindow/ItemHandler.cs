using CosmicSpaceCommunication.Game.Resources;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
{
    public ItemPilot ItemPilot;

    Transform parent;
    public Transform ItemContainer;
    Transform itemContainerTarget;
    public Transform ItemContainerTarget
    {
        get => itemContainerTarget;
        set
        {
            if (itemContainerTarget == value)
                return;

            if (itemContainerTarget != null)
            {
                Image image = itemContainerTarget.GetComponent<Image>();
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
            }

            itemContainerTarget = value;

            if (value != null)
            {
                Image image = value.GetComponent<Image>();
                image.color = new Color(image.color.r, image.color.g, image.color.b, 0.1f);
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        parent = transform.parent;
        transform.SetParent(GuiScript.Windows[WindowTypes.HangarWindow].Window.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (ItemContainerTarget != null)
        {
            if (ItemContainer == ItemContainerTarget)
            {
                RecoverParent();
                return;
            }

            if (ItemContainerTarget.GetComponent<ItemContainer>().AddItem(ItemPilot))
            {
                if (ItemContainer.GetComponent<ItemContainer>().RemoveItem(ItemPilot))
                {
                    Destroy(gameObject);
                }
            }
        }

        RecoverParent();
    }

    private void RecoverParent()
    {
        transform.SetParent(parent);
        transform.localPosition = Vector3.zero;
        ItemContainerTarget = null;
    }
}