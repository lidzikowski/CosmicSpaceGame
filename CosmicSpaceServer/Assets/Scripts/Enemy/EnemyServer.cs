using CosmicSpaceCommunication.Game.Enemy;
using UnityEngine;

public class EnemyServer : Opponent
{
    public Enemy ParentEnemy { get; set; }
    
    public override ulong Id { get; protected set; }

    public EnemyServer(Enemy enemy, ulong id, Vector2 position)
    {
        ParentEnemy = enemy;
        Id = id;
        hitpoints = MaxHitpoints;
        shields = MaxShields;
        Position = position;
        TargetPostion = position;
    }

    public override ulong MaxHitpoints => ParentEnemy.Hitpoints;

    public override ulong MaxShields => ParentEnemy.Shields;

    public override int Speed => ParentEnemy.Speed;

    public override ulong Damage => ParentEnemy.Damage;

    public override string Name => $"{ParentEnemy.Name} :{Id}";
}