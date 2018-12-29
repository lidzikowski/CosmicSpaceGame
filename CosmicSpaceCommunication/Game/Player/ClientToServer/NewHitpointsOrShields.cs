using System;

namespace CosmicSpaceCommunication.Game.Player.ClientToServer
{
    [Serializable]
    public class NewHitpointsOrShields : Communication
    {
        public bool IsPlayer { get; set; }
        public ulong Value { get; set; }
        public ulong MaxValue { get; set; }
    }
}