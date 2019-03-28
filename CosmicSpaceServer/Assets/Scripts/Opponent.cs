using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Enemy;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Opponent
{
    protected static readonly float UPDATE_TIME = 1;
    protected static readonly float REPAIR_EVERY_UPDATE = 5;



    protected virtual int ShotDistance => 50;



    #region OpponentsInArea / PilotsInArea / Join / Leave
    public virtual List<Opponent> OpponentsInArea { get; set; } = new List<Opponent>();
    public List<Opponent> PilotsInArea => OpponentsInArea.Where(o => o.IsPlayer).ToList();

    public virtual void AddOpponentInArea(Opponent opponent, bool relation = true)
    {
        if (!OpponentsInArea.Contains(opponent))
        {
            OpponentsInArea.Add(opponent);

            Join(opponent);

            if (relation)
                opponent.AddOpponentInArea(this, false);
        }
    }
    protected void Join(Opponent joinOpponent)
    {
        if (!IsPlayer) // Jakby obiekt byl npc - brak synchronizacji do niego
            return;

        PilotServer pilot = this as PilotServer;

        if (joinOpponent.IsPlayer) // PILOT
        {
            pilot.Send(new CommandData()
            {
                Command = Commands.PlayerJoin,
                Data = PlayerJoin.GetNewJoin((joinOpponent as PilotServer).Pilot)
            });
        }
        else // ENEMY
        {
            pilot.Send(new CommandData()
            {
                Command = Commands.EnemyJoin,
                Data = new EnemyJoin(
                    joinOpponent.Id,
                    (joinOpponent as EnemyServer).ParentEnemy,
                    joinOpponent.Position.x, 
                    joinOpponent.Position.y,
                    joinOpponent.NewPostion.x,
                    joinOpponent.NewPostion.y,
                    joinOpponent.Hitpoints,
                    joinOpponent.Shields
                    )
            });
        }
    }

    public virtual void RemoveOpponentInArea(Opponent opponent, bool relation = true)
    {
        if (OpponentsInArea.Contains(opponent))
        {
            OpponentsInArea.Remove(opponent);

            Leave(opponent);

            if (relation)
                opponent.RemoveOpponentInArea(this, false);
        }
    }
    protected void Leave(Opponent leaveOpponent)
    {
        if (Target == leaveOpponent)
            Target = null;

        if (!IsPlayer)
            return;

        PilotServer pilot = this as PilotServer;

        if (leaveOpponent.IsPlayer) // PILOT
        {
            pilot.Send(new CommandData()
            {
                Command = Commands.PlayerLeave,
                Data = leaveOpponent.Id
            });
        }
        else // ENEMY
        {
            pilot.Send(new CommandData()
            {
                Command = Commands.EnemyLeave,
                Data = leaveOpponent.Id
            });
        }
    }


    protected void SendToPilotsInArea(CommandData commandData)
    {
        foreach (Opponent pilot in PilotsInArea)
        {
            (pilot as PilotServer).Send(commandData);
        }
    }
    protected void SendToPilotsInArea(CommandData commandData, bool andLocalPilot)
    {
        SendToPilotsInArea(commandData);

        if (IsPlayer && andLocalPilot)
            (this as PilotServer).Send(commandData);
    }
    #endregion



    #region Id / Name / IsPlayer / Opponent
    public abstract ulong Id { get; protected set; }
    public abstract string Name { get; }
    public bool IsPlayer { get; private set; }

    public Opponent()
    {
        IsPlayer = this is PilotServer;
    }
    #endregion

    #region IsDead / IsCover
    public virtual float IsCoverTimer { get; set; }
    public virtual bool IsCover { get; set; }
    protected virtual bool isDead { get; set; }
    public bool IsDead
    {
        get => isDead;
        set
        {
            if (isDead == value)
                return;

            isDead = value;

            if(value)
            {
                OnDead();
            }
            else
            {
                OnAlive();
            }
        }
    }
    protected Opponent DeadOpponent { get; set; }
    protected virtual void OnDead()
    {
        Target = null;

        SendToPilotsInArea(new CommandData()
        {
            Command = Commands.Dead,
            Data = new SomeoneDead()
            {
                WhoId = Id,
                WhoIsPlayer = IsPlayer,

                ById = DeadOpponent.Id,
                ByIsPlayer = DeadOpponent.IsPlayer,
                ByName = DeadOpponent.Name
            }
        }, true);

        DeadOpponent = null;
    }
    protected void OnAlive()
    {
        Hitpoints = 1000;
        LastTakeDamage = 0;

        IsCoverTimer = 10;
        IsCover = true;

        SendToPilotsInArea(new CommandData()
        {
            Command = Commands.RepairShip,
            Data = Id
        }, true);
    }
    #endregion

    #region Ammunition / Rocket
    public virtual int? Ammunition { get; set; }
    public virtual int? Rocket { get; set; }
    #endregion

    #region Position / NewPosition
    protected virtual Vector2 position { get; set; }
    public Vector2 Position
    {
        get => position;
        set
        {
            if (position == value)
                return;

            position = value;
        }
    }
    protected Vector2 newPostion { get; set; }
    public Vector2 NewPostion
    {
        get => newPostion;
        set
        {
            if (newPostion == value)
                return;

            newPostion = value;

            OnChangePosition();
        }
    }

    protected void OnChangePosition()
    {
        SendToPilotsInArea(new CommandData()
        {
            Command = Commands.NewPosition,
            Data = new NewPosition()
            {
                PlayerId = Id,
                IsPlayer = IsPlayer,
                PositionX = Position.x,
                PositionY = Position.y,
                TargetPositionX = NewPostion.x,
                TargetPositionY = NewPostion.y,
                Speed = Speed
            }
        }, IsPlayer);
    }
    #endregion

    #region Hitpoints
    protected virtual long hitpoints { get; set; }
    public virtual long Hitpoints
    {
        get => hitpoints;
        set
        {
            if (hitpoints == value)
                return;

            hitpoints = value;

            OnChangeHitpoints();
        }
    }
    public abstract long MaxHitpoints { get; }

    public virtual bool CanRepearHitpoints => Hitpoints != MaxHitpoints;
    protected void OnChangeHitpoints()
    {
        SendToPilotsInArea(new CommandData()
        {
            Command = Commands.ChangeHitpoints,
            Data = new NewHitpointsOrShields()
            {
                PlayerId = Id,
                IsPlayer = IsPlayer,
                Value = Hitpoints,
                MaxValue = MaxHitpoints
            }
        }, true);
    }
    #endregion

    #region Shields
    protected virtual long shields { get; set; }
    public long Shields
    {
        get => shields;
        set
        {
            if (shields == value)
                return;

            shields = value;

            OnChangeShields();
        }
    }
    public abstract long MaxShields { get; }

    public virtual bool CanRepearShields => Shields != MaxShields;
    protected void OnChangeShields()
    {
        SendToPilotsInArea(new CommandData()
        {
            Command = Commands.ChangeShields,
            Data = new NewHitpointsOrShields()
            {
                PlayerId = Id,
                IsPlayer = IsPlayer,
                Value = Shields,
                MaxValue = MaxShields
            }
        }, true);
    }
    #endregion

    #region Speed
    public abstract int Speed { get; }
    #endregion

    #region Target
    protected virtual Opponent target { get; set; }
    public Opponent Target
    {
        get => target;
        set
        {
            if (target == value)
                return;

            target = value;

            if (value == null)
                Attack = false;

            OnSelectTarget();
        }
    }
    protected bool TargetIsNull => Target == null;
    protected bool TargetIsDead => !TargetIsNull ? Target.IsDead : false;
    protected ulong? TargetId => !TargetIsNull ? Target.Id : (ulong?)null; 
    protected bool? TargetIsPlayer => !TargetIsNull ? Target.IsPlayer : (bool?)null; 
    protected void OnSelectTarget()
    {
        SendToPilotsInArea(new CommandData()
        {
            Command = Commands.SelectTarget,
            Data = new NewTarget()
            {
                PlayerId = Id,
                AttackerIsPlayer = IsPlayer,
                TargetId = TargetId,
                TargetIsPlayer = TargetIsPlayer
            }
        });
    }
    #endregion

    #region Attack
    protected virtual bool attack { get; set; }
    public virtual bool Attack
    {
        get
        {
            if(attack)
            {
                if (TargetIsNull || TargetIsDead)
                {
                    Attack = false;
                    return attack;
                }
                return attack;
            }
            return attack;
        }
        set
        {
            if (attack == value)
                return;

            attack = value;

            OnAttackTarget();
        }
    }
    protected void OnAttackTarget()
    {
        SendToPilotsInArea(new CommandData()
        {
            Command = Commands.AttackTarget,
            Data = new AttackTarget()
            {
                PlayerId = Id,
                AttackerIsPlayer = IsPlayer,
                TargetId = TargetId,
                TargetIsPlayer = TargetIsPlayer,

                Attack = Attack,
                SelectedAmmunition = Ammunition,
                SelectedRocket = Rocket
            }
        });
    }
    #endregion

    #region Damage
    public abstract long Damage { get; }
    #endregion

    #region TakeDamage
    protected float LastTakeDamage = 0;
    public virtual void OnTakeDamage(Opponent opponent, long? receivedDamage, int ammunition, bool type)
    {
        if (IsDead)
            return;

        LastTakeDamage = REPAIR_EVERY_UPDATE;

        SendToPilotsInArea(new CommandData()
        {
            Command = Commands.GetDamage,
            Data = new TakeDamage()
            {
                ToId = Id,
                ToIsPlayer = IsPlayer,

                FromId = opponent.Id,
                FromIsPlayer = opponent.IsPlayer,

                Damage = IsCover ? null : receivedDamage,

                AmmunitionId = ammunition,
                IsAmmunition = type
            }
        }, true);

        if (IsCover)
            return;

        if (receivedDamage != null)
        {
            TakeDamage((long)receivedDamage);
            CheckIfDead(opponent);
        }
    }
    protected void TakeDamage(long damage)
    {
        long dmgHp = 0;

        if (Shields > 0)
        {
            long dmgShd = (long)(damage * 0.7);
            if (Shields - dmgShd >= 0)
            {
                dmgHp = damage - dmgShd;
                Shields -= dmgShd;
            }
            else
            {
                dmgHp = damage - Shields;
                Shields = 0;
            }
        }
        else
            dmgHp = damage;
        
        if (Hitpoints - dmgHp >= 0)
            Hitpoints -= dmgHp;
        else
            Hitpoints = 0;
    }
    protected void CheckIfDead(Opponent opponent)
    {
        if (Hitpoints.Equals(0))
        {
            DeadOpponent = opponent;
            IsDead = true;

            opponent.TakeReward(ServerReward.GetReward(
                Reward,
                RewardReason,
                Name));
        }
    }
    #endregion

    #region Reward / TakeReward
    public abstract Reward Reward { get; }
    public abstract RewardReasons RewardReason { get; }
    public virtual void TakeReward(ServerReward reward) { }
    #endregion



    #region Update
    protected float timer = 0;

    public virtual void Update()
    {
        if (IsDead)
            return;

        Fly();

        if (timer >= UPDATE_TIME)
        {
            timer = 0;

            CheckCover();

            if (Attack)
            {
                if (TargetIsDead)
                {
                    Target = null;
                }
                else if(MapServer.Distance(this, Target) <= ShotDistance)
                {
                    if (Ammunition != null)
                    {
                        LastTakeDamage = REPAIR_EVERY_UPDATE;
                        Target.OnTakeDamage(this, Damage, (int)Ammunition, true);
                    }
                }
            }
            else if (CanRepair)
                Repair();
            else
                LastTakeDamage -= UPDATE_TIME;
        }
        else
            timer += Time.deltaTime;
    }
    #endregion

    

    #region Fly / Cover
    public virtual void Fly()
    {
        if (NewPostion == Position)
            return;
        Position = Vector3.MoveTowards(Position, NewPostion, Time.deltaTime * Speed);
    }

    public virtual void CheckCover()
    {
        if(IsCover)
        {
            IsCoverTimer -= UPDATE_TIME;
            if (IsCoverTimer <= 0)
                IsCover = false;
        }
    }
    #endregion

    #region Repair
    protected bool CanRepair => LastTakeDamage <= 0;
    public void Repair()
    {
        if (CanRepearHitpoints)
        {
            var hitpoint = MaxHitpoints / 30;
            if (Hitpoints + hitpoint <= MaxHitpoints)
                Hitpoints += hitpoint;
            else
                Hitpoints = MaxHitpoints;
        }

        if (CanRepearShields)
        {
            var shield = MaxShields / 20;
            if (Shields + shield <= MaxShields)
                Shields += shield;
            else
                Shields = MaxShields;
        }
    }
    #endregion
}