using CosmicSpaceCommunication.Game.Player.ClientToServer;
using System;

namespace CosmicSpaceCommunication.Game.Player.ServerToClient
{
    [Serializable]
    public class NewHitpointsOrShields : Communication
    {
        public bool IsPlayer { get; set; }
        public ulong Value { get; set; }
        public ulong MaxValue { get; set; }
    }
}