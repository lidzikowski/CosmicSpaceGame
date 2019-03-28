namespace CosmicSpaceCommunication.Game.Interfaces
{
    public interface IShip
    {
        int Id { get; set; }
        string Name { get; set; }
        int PrefabId { get; set; }
        string PrefabName { get; set; }
    }
}