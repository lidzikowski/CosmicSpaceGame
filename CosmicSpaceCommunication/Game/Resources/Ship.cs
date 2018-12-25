namespace CosmicSpaceCommunication.Game.Resources
{
    [System.Serializable]
    public class Ship : Base
    {
        public double ScrapPrice { get; set; }
        public double MetalPrice { get; set; }
        public int Lasers { get; set; }
        public int Generators { get; set; }
        public int Extras { get; set; }
    }
}