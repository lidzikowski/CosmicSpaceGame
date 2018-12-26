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

        public static PlayerJoin Create(Pilot pilot)
        {
            return new PlayerJoin()
            {
                PlayerId = pilot.Id,
                Nickname = pilot.Nickname,
                PositionX = pilot.PositionX,
                PositionY = pilot.PositionY,
                Ship = pilot.Ship
            };
        }
    }
}