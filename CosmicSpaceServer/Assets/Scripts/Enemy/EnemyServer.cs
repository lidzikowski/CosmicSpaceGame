using CosmicSpaceCommunication.Game.Enemy;
using CosmicSpaceCommunication.Game.Resources;
using System.Linq;
using UnityEngine;

public class EnemyServer : Opponent
{
    public Enemy ParentEnemy { get; set; }
    
    protected MapServer EnemyOnMap { get; set; }



    public EnemyServer(Enemy enemy, ulong id, Vector2 position, MapServer map)
    {
        ParentEnemy = enemy;

        EnemyOnMap = map;

        Id = id;

        hitpoints = MaxHitpoints;
        shields = MaxShields;

        Position = position;
        NewPostion = position;

        Ammunition = 1;
    }



    public override ulong Id { get; protected set; }

    public override Reward Reward => ParentEnemy.Reward;

    public override RewardReasons RewardReason => RewardReasons.KillEnemy;

    public override long MaxHitpoints => ParentEnemy.Hitpoints;

    public override long MaxShields => ParentEnemy.Shields;

    public override float Speed => ParentEnemy.Speed;

    public override long Damage => RandomDamage(ParentEnemy.Damage);

    protected override int ShotDistance => ParentEnemy.ShotDistance;

    public override string Name => ParentEnemy.Name;
    
    int lostTargetTime = 0;
    Vector2 lastTargetPosition;
    public override void Update()
    {
        if (IsDead)
            return;

        base.Update();

        if (timer >= UPDATE_TIME)
        {
            if(TargetIsNull)
            {
                if (ParentEnemy.IsAggressive && OpponentsInArea.Count > 0)
                {
                    Opponent opponent = OpponentsInArea.Where(o => !o.IsDead && !o.IsCover).FirstOrDefault(o => MapServer.Distance(o, this) <= ShotDistance);
                    if (opponent != null)
                    {
                        Target = opponent;
                        Attack = true;
                        return;
                    }
                }

                if (Position == NewPostion)
                {
                    if (Random.Range(0, 100) > 80)
                        NewPostion = MapServer.RandomPosition(Position);
                }
            }
            else
            {
                if (TargetIsDead)
                {
                    Target = null;
                    return;
                }

                if (Target.Position != lastTargetPosition)
                {
                    lastTargetPosition = Target.Position;
                    NewPostion = MapServer.RandomCircle(lastTargetPosition, 15);
                }

                if (lostTargetTime > 5)
                {
                    Target = null;
                    lostTargetTime = 0;
                }
                else
                {
                    if (MapServer.Distance(this, Target) > ShotDistance)
                        lostTargetTime++;
                    else
                        lostTargetTime = 0;
                }
            }
        }
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

    public override void OnTakeDamage(Opponent opponent, long? receivedDamage, int ammunition, bool type)
    {
        base.OnTakeDamage(opponent, receivedDamage, ammunition, type);

        if (!opponent.IsPlayer)
            return;

        if (opponent.IsDead)
            return;

        if (TargetIsNull)
        {
            Target = opponent;
            Attack = true;
        }
    }
}