using System;

namespace CosmicSpaceCommunication.Game.Player.ClientToServer
{
    [Serializable]
    public class ChangeAmmunition
    {
        public long SelectedAmmunitionId { get; set; }
        public long SelectedRocketId { get; set; }



        public long ResourceId { get; set; }
        public long Count { get; set; }
    }
}