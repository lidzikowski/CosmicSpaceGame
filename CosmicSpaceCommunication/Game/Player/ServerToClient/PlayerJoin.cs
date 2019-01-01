using System;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Resources;

namespace CosmicSpaceCommunication.Game.Player.ServerToClient
{
    [Serializable]
    public class PlayerJoin : NewPosition
    {
        public string Nickname { get; set; }
        public Ship Ship { get; set; }
        public bool IsDead { get; set; }
        public string KillerBy { get; set; }

        public static PlayerJoin GetNewJoin(Pilot pilot)
        {
            return new PlayerJoin()
            {
                PlayerId = pilot.Id,

                IsPlayer = true,
                PositionX = pilot.PositionX,
                PositionY = pilot.PositionY,
                TargetPositionX = pilot.PositionX,
                TargetPositionY = pilot.PositionY,
                Speed = pilot.Ship.Speed,

                Nickname = pilot.Nickname,
                Ship = pilot.Ship,
                IsDead = pilot.IsDead,
                KillerBy = pilot.KillerBy,
            };
        }
    }
}