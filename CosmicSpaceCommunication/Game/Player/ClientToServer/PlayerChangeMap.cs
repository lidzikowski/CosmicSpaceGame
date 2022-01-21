using CosmicSpaceCommunication.Game.Resources;
using System;

namespace CosmicSpaceCommunication.Game.Player.ClientToServer
{
    [Serializable]
    public class PlayerChangeMap
    {
        public ulong PlayerId { get; set; }
        public Portal Portal { get; set; }
    }
}