using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;

namespace CosmicSpaceCommunication.Game.Player
{
    [System.Serializable]
    public class Pilot
    {
        public ulong Id { get; set; }
        public string Nickname { get; set; }
        public Map Map { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public Ship Ship { get; set; }
        public ulong Experience { get; set; }
        public int Level { get; set; }
        public double Scrap { get; set; }
        public double Metal { get; set; }
        public ulong Hitpoints { get; set; }
        public ulong Shields { get; set; }
        public bool IsDead { get; set; }

        public List<ulong> Ammunitions { get; set; }
        public List<ulong> Rockets { get; set; }
    }
}