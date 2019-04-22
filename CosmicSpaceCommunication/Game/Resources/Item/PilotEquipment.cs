using System;
using System.Collections.Generic;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class PilotEquipment
    {
        public ulong PilotId { get; set; }
        public List<ItemPilot> Items { get; set; }
    }
}