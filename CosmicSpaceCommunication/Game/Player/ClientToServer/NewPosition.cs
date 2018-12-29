using System;

namespace CosmicSpaceCommunication.Game.Player.ClientToServer
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