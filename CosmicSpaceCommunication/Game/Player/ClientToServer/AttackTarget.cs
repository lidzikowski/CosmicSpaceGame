using System;

namespace CosmicSpaceCommunication.Game.Player.ClientToServer
{
    [Serializable]
    public class AttackTarget : NewTarget
    {
        public bool Attack { get; set; }
        
        public int? SelectedAmmunition { get; set; }
        public int? SelectedRocket { get; set; }
        //public int? UseSkill { get; set; }
    }
}