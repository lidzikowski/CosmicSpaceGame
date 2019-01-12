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
        public bool IsDead { get; set; }
        public string KillerBy { get; set; }

        public List<ulong> Ammunitions { get; set; }
        public List<ulong> Rockets { get; set; }



        public long MaxHitpoints { get; set; }
        public long MaxShields { get; set; }
        public int Speed { get; set; }



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
                IsDead = ConvertRow.Row<bool>(row["isdead"]),
                KillerBy = ConvertRow.Row<string>(row["killerby"]),
            };
        }
    }
}