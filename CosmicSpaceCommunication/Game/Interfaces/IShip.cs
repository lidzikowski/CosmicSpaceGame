using CosmicSpaceCommunication.Game.Resources;

namespace CosmicSpaceCommunication.Game.Interfaces
{
    public interface IShip
    {
        long Id { get; set; }
        string Name { get; set; }
        Reward Reward { get; set; }
        Prefab Prefab { get; set; }
    }
}