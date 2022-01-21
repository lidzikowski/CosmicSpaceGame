using CosmicSpaceCommunication.Game.Player.ServerToClient;
using System;

namespace CosmicSpaceCommunication.Game.Enemy
{
    [Serializable]
    public class EnemyJoin : NewPosition, HitpointsShields
    {
        public ulong Id { get; set; }
        public Enemy ParentEnemy { get; set; }

        public long Hitpoints { get; set; }
        public long MaxHitpoints { get => ParentEnemy.Hitpoints; set => ParentEnemy.Hitpoints = value; }
        public long Shields { get; set; }
        public long MaxShields { get => ParentEnemy.Shields; set => ParentEnemy.Shields = value; }

        public EnemyJoin(ulong id, Enemy enemy, 
            float posX, float posY, 
            float newPosX, float newPosY, 
            long hitpoints, long shields)
        {
            PlayerId = id;
            IsPlayer = false;
            Id = id;
            ParentEnemy = enemy;
            PositionX = posX;
            PositionY = posY;
            TargetPositionX = newPosX;
            TargetPositionY = newPosY;
            Speed = enemy.Speed;
            Hitpoints = hitpoints;
            Shields = shields;
        }
    }
}