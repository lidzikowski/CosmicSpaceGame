using CosmicSpaceCommunication.Game.Quest;
using CosmicSpaceCommunication.Game.Resources;
using System;
using System.Collections.Generic;
using System.Data;

namespace CosmicSpaceCommunication.Game.Player
{
    [Serializable]
    public class Pilot
    {
        public ulong Id { get; set; }
        public string Nickname { get; set; }
        public Map Map { get; set; }
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public Ship Ship { get; set; }
        public ulong Experience { get; set; }
        public int Level { get; set; }
        public double Scrap { get; set; }
        public double Metal { get; set; }
        public long Hitpoints { get; set; }
        public long Shields { get; set; }
        public long AmmunitionId { get; set; }
        public long RocketId { get; set; }
        public bool IsDead { get; set; }
        public string KillerBy { get; set; }



        public Dictionary<long, PilotResource> Resources { get; set; }
        public List<ItemPilot> Items { get; set; }
        public Dictionary<long, Ammunition> ServerResources { get; set; }
        public List<PilotTask> Tasks { get; set; }



        public long MaxHitpoints { get; set; }
        public long MaxShields { get; set; }
        public float Speed { get; set; }



        public static Pilot GetPilot(DataRow row)
        {
            return new Pilot()
            {
                Id = ConvertRow.Row<ulong>(row["userid"]),
                Nickname = ConvertRow.Row<string>(row["nickname"]),
                PositionX = ConvertRow.Row<float>(row["positionx"]),
                PositionY = ConvertRow.Row<float>(row["positiony"]),
                Experience = ConvertRow.Row<ulong>(row["experience"]),
                Level = ConvertRow.Row<int>(row["level"]),
                Scrap = ConvertRow.Row<double>(row["scrap"]),
                Metal = ConvertRow.Row<double>(row["metal"]),
                Hitpoints = ConvertRow.Row<long>(row["hitpoints"]),
                Shields = ConvertRow.Row<long>(row["shields"]),
                AmmunitionId = ConvertRow.Row<int>(row["ammunitionid"]),
                RocketId = ConvertRow.Row<int>(row["rocketid"]),
                IsDead = ConvertRow.Row<bool>(row["isdead"]),
                KillerBy = ConvertRow.Row<string>(row["killerby"]),
            };
        }
    }
}