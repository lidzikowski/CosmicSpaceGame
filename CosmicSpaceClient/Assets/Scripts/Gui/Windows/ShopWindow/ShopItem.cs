using CosmicSpaceCommunication.Game.Resources;
using UnityEngine;
using UnityEngine.UI;

public delegate void ShowInformation(IShopItem item);

public class ShopItem : MonoBehaviour
{
    public Text NameText;

    private IShopItem item;
    public IShopItem Item
    {
        get => item;
        set
        {
            item = value;

            NameText.text = item.Name;
        }
    }

    private ShowInformation onShowInformation;
    public ShowInformation OnShowInformation
    {
        get => onShowInformation;
        set
        {
            onShowInformation = value;

            GameWindow.ButtonListener(GetComponent<Button>(), ClickEvent, true);
        }
    }

    private void ClickEvent()
    {
        if (Item != null)
            OnShowInformation?.Invoke(Item);
    }
}