using CosmicSpaceCommunication.Game.Player.ServerToClient;
using System;
using System.Data;

namespace CosmicSpaceCommunication.Game.Enemy
{
    [Serializable]
    public class EnemyJoin : NewPosition
    {
        public ulong Id { get; set; }
        public Enemy ParentEnemy { get; set; }

        public EnemyJoin(ulong id, Enemy enemy, float posX, float posY)
        {
            PlayerId = id;
            IsPlayer = false;
            Id = id;
            ParentEnemy = enemy;
            PositionX = posX;
            PositionY = posY;
            TargetPositionX = posX;
            TargetPositionY = posY;
            Speed = enemy.Speed;
        }
    }
}