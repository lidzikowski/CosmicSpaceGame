namespace CosmicSpaceCommunication.Game.Resources
{
    [System.Serializable]
    public class Ship : ReqLevel
    {
        public double? ScrapPrice { get; set; }
        public double? MetalPrice { get; set; }
        public int Lasers { get; set; }
        public int Generators { get; set; }
        public int Extras { get; set; }
        public int Speed { get; set; }
        public int Cargo { get; set; }
        public ulong Hitpoints { get; set; }
    }
}