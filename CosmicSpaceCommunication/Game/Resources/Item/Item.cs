using System;
using System.Collections.Generic;
using System.Data;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public class Item : ReqLevel, IShopItem
    {
        public string PrefabName { get; set; }
        public ItemTypes ItemType { get; set; }

        public long? LaserDamagePvp { get; set; }
        public long? LaserDamagePve { get; set; }
        public int? LaserShotRange { get; set; }
        public float? LaserShotDispersion { get; set; }

        public float? GeneratorSpeed { get; set; }
        public long? GeneratorShield { get; set; }
        public float? GeneratorShieldDivision { get; set; }
        public int? GeneratorShieldRepair { get; set; }

        public double? ScrapPrice { get; set; }
        public double? MetalPrice { get; set; }
        public Prefab Prefab => new Prefab() { PrefabName = PrefabName };

        public Dictionary<ItemProperty, object> ItemDescription => new Dictionary<ItemProperty, object>
        {
            { ItemProperty.LaserDamagePvp, LaserDamagePvp },
            { ItemProperty.LaserDamagePve, LaserDamagePve },
            { ItemProperty.LaserShotRange, LaserShotRange },
            { ItemProperty.LaserShotDispersion, LaserShotDispersion },

            { ItemProperty.GeneratorSpeed, GeneratorSpeed },
            { ItemProperty.GeneratorShield, GeneratorShield },
            { ItemProperty.GeneratorShieldDivision, GeneratorShieldDivision },
            { ItemProperty.GeneratorShieldRepair, GeneratorShieldRepair },

            { ItemProperty.RequiredLevel, RequiredLevel },
        };

        public static Item GetItem(DataRow row)
        {
            return new Item()
            {
                Id = ConvertRow.Row<long>(row["itemid"]),
                Name = ConvertRow.Row<string>(row["name"]),
                RequiredLevel = ConvertRow.Row<int>(row["requiredlevel"]),
                PrefabName = ConvertRow.Row<string>(row["prefabname"]),
                ItemType = (ItemTypes)ConvertRow.Row<int>(row["itemtypeid"]),

                LaserDamagePvp = ConvertRow.Row<long?>(row["laser_damage_pvp"]),
                LaserDamagePve = ConvertRow.Row<long?>(row["laser_damage_pve"]),
                LaserShotRange = ConvertRow.Row<int?>(row["laser_shotrange"]),
                LaserShotDispersion = ConvertRow.Row<float?>(row["laser_shotdispersion"]),

                GeneratorSpeed = ConvertRow.Row<float?>(row["generator_speed"]),
                GeneratorShield = ConvertRow.Row<long?>(row["generator_shield"]),
                GeneratorShieldDivision = ConvertRow.Row<float?>(row["generator_shield_division"]),
                GeneratorShieldRepair = ConvertRow.Row<int?>(row["generator_shield_repair"]),

                ScrapPrice = ConvertRow.Row<double?>(row["scrapprice"]),
                MetalPrice = ConvertRow.Row<double?>(row["metalprice"]),
            };
        }
    }
}