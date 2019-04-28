using System.Collections.Generic;

namespace CosmicSpaceCommunication.Game.Resources
{
    public interface IShopItem
    {
        long Id { get; }
        string Name { get; }
        Prefab Prefab { get; }
        ItemTypes ItemType { get; }
        double? ScrapPrice { get; }
        double? MetalPrice { get; }
        int RequiredLevel { get; set; }
        Dictionary<ItemProperty, object> ItemDescription { get; }
    }
}