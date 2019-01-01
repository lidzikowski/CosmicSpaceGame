using CosmicSpaceCommunication.Game.Enemy;
using UnityEngine;

public class EnemyServer : Opponent
{
    public Enemy ParentEnemy { get; set; }
    
    public override ulong Id { get; protected set; }

    protected MapServer EnemyOnMap { get; set; }

    public EnemyServer(Enemy enemy, ulong id, Vector2 position, MapServer map)
    {
        ParentEnemy = enemy;
        Id = id;
        hitpoints = MaxHitpoints;
        shields = MaxShields;
        Position = position;
        TargetPostion = position;

        EnemyOnMap = map;
    }

    protected override void OnDead()
    {
        base.OnDead();

        if (EnemyOnMap.EnemiesOnMap.Contains(this))
        {
            foreach (Opponent opponent in PilotsInArea)
                opponent.RemoveOpponentInArea(this);

            EnemyOnMap.EnemiesOnMap.Remove(this);
        }
    }

    public override ulong MaxHitpoints => ParentEnemy.Hitpoints;

    public override ulong MaxShields => ParentEnemy.Shields;

    public override int Speed => ParentEnemy.Speed;

    public override ulong Damage => ParentEnemy.Damage;

    public override string Name => ParentEnemy.Name;


}