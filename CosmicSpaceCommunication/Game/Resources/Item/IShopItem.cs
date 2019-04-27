using System.Collections.Generic;

namespace CosmicSpaceCommunication.Game.Resources
{
    public interface IShopItem
    {
        long ItemId { get; }
        string Name { get; }
        Prefab Prefab { get; }
        ItemTypes ItemType { get; }
        double? ScrapPrice { get; }
        double? MetalPrice { get; }
        Dictionary<ItemProperty, object> ItemDescription { get; }
    }
}