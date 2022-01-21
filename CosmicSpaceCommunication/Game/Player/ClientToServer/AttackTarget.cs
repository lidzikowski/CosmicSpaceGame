using System;

namespace CosmicSpaceCommunication.Game.Player.ClientToServer
{
    [Serializable]
    public class AttackTarget : NewTarget
    {
        public bool Attack { get; set; }
        
        public long? SelectedAmmunition { get; set; }
        public long? SelectedRocket { get; set; }
        //public int? UseSkill { get; set; }
    }
}