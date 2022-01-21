using System;

namespace CosmicSpaceCommunication.Game.Player.ClientToServer
{
    [Serializable]
    public class Communication
    {
        public ulong PlayerId { get; set; }
    }
}