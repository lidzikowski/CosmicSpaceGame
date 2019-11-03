using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemHandler : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public ItemPilot ItemPilot;
    public PilotResource PilotResource;

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

    public GameObject ToolTipPrefab;
    public static ToolTip ToolTipInstance;

    public Image ItemTexture;

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;

        if (GuiScript.Windows[WindowTypes.ItemInformationWindow].Active)
            GuiScript.CloseWindow(WindowTypes.ItemInformationWindow);

        parent = transform.parent;
        transform.SetParent(GuiScript.Windows[WindowTypes.HangarWindow].Window.transform);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;

        OnPointerExit(eventData);
        transform.position = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
            return;

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
                    Client.SendToSocket(new CommandData()
                    {
                        Command = Commands.ChangeEquipment,
                        SenderId = Client.Pilot.Id,
                        Data = new PilotEquipment()
                        {
                            Items = new List<ItemPilot>()
                            {
                                ItemPilot
                            }
                        }
                    });
                    Destroy(gameObject);

                    // ADD
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (ToolTipInstance == null)
        {
            ToolTipInstance = Instantiate(ToolTipPrefab, GuiScript.Windows[WindowTypes.HangarWindow].Window.transform).GetComponent<ToolTip>();
        }
        ToolTipInstance.ItemInfo = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (ToolTipInstance != null)
            ToolTipInstance.ItemInfo = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right)
            return;

        if (!GuiScript.Windows[WindowTypes.ItemInformationWindow].Active)
            GuiScript.OpenWindow(WindowTypes.ItemInformationWindow);

        (GuiScript.Windows[WindowTypes.ItemInformationWindow].Script as ItemInformationWindow).ShowItemInformation(ItemPilot, PilotResource);
    }
}