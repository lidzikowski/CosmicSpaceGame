namespace CosmicSpaceCommunication.Game
{
    public interface HitpointsShields
    {
        long Hitpoints { get; set; }
        long MaxHitpoints { get; set; }
        long Shields { get; set; }
        long MaxShields { get; set; }
    }
}