using System;

namespace CosmicSpaceCommunication.Game.Player.ServerToClient
{
    [Serializable]
    public class TakeDamage
    {
        public ulong FromId { get; set; }
        public bool FromIsPlayer { get; set; }

        public ulong ToId { get; set; }
        public bool ToIsPlayer { get; set; }

        public ulong? Damage { get; set; }

        public int AmmunitionId { get; set; }
        /// <summary>
        /// Ammunition = true / Rocket = false
        /// </summary>
        public bool IsAmmunition { get; set; }
    }
}