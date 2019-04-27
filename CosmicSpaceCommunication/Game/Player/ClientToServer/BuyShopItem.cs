using CosmicSpaceCommunication.Game.Resources;
using System;

namespace CosmicSpaceCommunication.Game.Player.ClientToServer
{
    [Serializable]
    public class BuyShopItem
    {
        public ulong PilotId { get; set; }
        public ItemTypes ItemType { get; set; }
        public long ItemId { get; set; }
        public int Count { get; set; } = 1;
    }
}