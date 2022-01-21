using System;

namespace Assets.Scripts.Enemy
{
    [Serializable]
    public class OpponentAttack
    {
        public Opponent Opponent { get; set; }
        public int LastAttack { get; set; }
    }
}