using System;

namespace CosmicSpaceCommunication.Game.Player.ClientToServer
{
    [Serializable]
    public class NewPosition : Communication
    {
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public float TargetPositionX { get; set; }
        public float TargetPositionY { get; set; }
    }
}