using System;

namespace CosmicSpaceCommunication.Game.Player.ServerToClient
{
    [Serializable]
    public class SafeZone
    {
        public ulong PilotId { get; set; }
        public bool IsPlayer { get; set; }
        public bool Status { get; set; }
    }
}