using System;

namespace CosmicSpaceCommunication.Game.Player.ClientToServer
{
    [Serializable]
    public class NewTarget : Communication
    {
        public bool AttackerIsPlayer { get; set; }
        public bool? TargetIsPlayer { get; set; }
        public ulong? TargetId { get; set; }
    }
}