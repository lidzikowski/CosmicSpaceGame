using CosmicSpaceCommunication.Game.Resources;
using System;
using System.Collections.Generic;

namespace CosmicSpaceCommunication.Game.Player.ServerToClient
{
    [Serializable]
    public class ShopItems
    {
        public List<Item> Items { get; set; }
        public List<Ship> Ships { get; set; }
    }
}