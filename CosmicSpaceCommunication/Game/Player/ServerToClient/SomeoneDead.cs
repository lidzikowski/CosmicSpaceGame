using System;

namespace CosmicSpaceCommunication.Game.Player.ServerToClient
{
    [Serializable]
    public class SomeoneDead
    {
        public ulong WhoId { get; set; }
        public bool WhoIsPlayer { get; set; }

        public ulong ById { get; set; }
        public bool ByIsPlayer { get; set; }
        public string ByName { get; set; }
    }
}