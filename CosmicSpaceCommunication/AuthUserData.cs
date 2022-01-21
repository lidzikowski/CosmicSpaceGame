using CosmicSpaceCommunication.Game.Enemy;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Resources;
using System;
using System.Collections.Generic;

namespace CosmicSpaceCommunication
{
    [Serializable]
    public class AuthUserData
    {
        public Pilot Pilot { get; set; }
        public Dictionary<long, Enemy> Enemies { get; set; }
        public Dictionary<long, Ship> Ships { get; set; }
        public Dictionary<long, Ammunition> Resources { get; set; }
        public Dictionary<long, Map> Maps { get; set; }
    }
}