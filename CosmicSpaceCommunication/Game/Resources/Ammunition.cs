using System;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class Ammunition : Base
    {
        public float? MultiplierPlayer { get; set; }
        public float? MultiplierEnemy { get; set; }
        public double? ScrapPrice { get; set; }
        public double? MetalPrice { get; set; }
        public int? SkillId { get; set; }
    }
}