using System;
using System.Collections.Generic;

namespace CosmicSpaceCommunication.Game.Player
{
    [Serializable]
    public class PilotResources
    {
        public List<ulong> Ammunitions { get; set; } = new List<ulong>();
        public List<ulong> Rockets { get; set; } = new List<ulong>();
    }
}