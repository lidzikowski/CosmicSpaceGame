using CosmicSpaceCommunication.Game.Player.ClientToServer;
using System;

namespace CosmicSpaceCommunication.Game.Player.ServerToClient
{
    [Serializable]
    public class NewPosition : Communication
    {
        public bool IsPlayer { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float TargetPositionX { get; set; }
        public float TargetPositionY { get; set; }
        public int Speed { get; set; }
    }
}