using CosmicSpaceCommunication.Game.Resources;

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
    }
}