using System;

namespace CosmicSpaceCommunication.Game.Player.ServerToClient
{
    [Serializable]
    public class Killer
    {
        public ulong Id { get; set; }
        public bool IsPlayer { get; set; }
        public string Name { get; set; }
    }
}