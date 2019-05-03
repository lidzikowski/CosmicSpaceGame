using System;
using System.Collections.Generic;
using System.Data;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class Ammunition : Base, IShopItem
    {
        public float? MultiplierPlayer { get; set; }
        public float? MultiplierEnemy { get; set; }
        public double? ScrapPrice { get; set; }
        public double? MetalPrice { get; set; }
        public int? SkillId { get; set; }
        public bool IsAmmunition { get; set; }
        public ulong? BaseDamage { get; set; }

        public Prefab Prefab => new Prefab() { PrefabTypeName = "Bullets", PrefabName = Name };

        public ItemTypes ItemType => ItemTypes.Ammunition;

        public int RequiredLevel { get; set; }

        public Dictionary<ItemProperty, object> ItemDescription => new Dictionary<ItemProperty, object>
        {
            { ItemProperty.MultiplierPlayer, MultiplierPlayer },
            { ItemProperty.MultiplierEnemy, MultiplierEnemy },
            { ItemProperty.IsAmmunition, IsAmmunition },
            { ItemProperty.BaseDamage, BaseDamage }
        };

        public static Ammunition GetAmmunition(DataRow row)
        {
            return new Ammunition()
            {
                Id = ConvertRow.Row<int>(row["ammunitionid"]),
                Name = ConvertRow.Row<string>(row["ammunitionname"]),

                MultiplierPlayer = ConvertRow.Row<float?>(row["multiplierplayer"]),
                MultiplierEnemy = ConvertRow.Row<float?>(row["multiplierenemy"]),
                ScrapPrice = ConvertRow.Row<double?>(row["scrapprice"]),
                MetalPrice = ConvertRow.Row<double?>(row["metalprice"]),
                SkillId = ConvertRow.Row<int?>(row["skillid"]),
                IsAmmunition = ConvertRow.Row<bool>(row["isammunition"]),
                BaseDamage = ConvertRow.Row<ulong?>(row["basedamage"]),
            };
        }
    }
}