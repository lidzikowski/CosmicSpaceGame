using CosmicSpaceCommunication.Game.Resources;
using System;

namespace CosmicSpaceCommunication.Game.Player.ServerToClient
{
    [Serializable]
    public class ShoppingStatus
    {
        public ShopStatus Status { get; set; }
        public IShopItem ShopItem { get; set; }
    }
}