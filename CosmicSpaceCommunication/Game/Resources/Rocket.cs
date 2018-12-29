using System;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class Rocket : Base
    {
        public double? ScrapPrice { get; set; }
        public double? MetalPrice { get; set; }
        public int? SkillId { get; set; }
        public int Damage { get; set; }
    }
}