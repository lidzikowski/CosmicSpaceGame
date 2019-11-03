using Assets.Scripts.Enemy;
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



    protected virtual int ShotDistance { get; set; }
    protected virtual float ShotDispersion { get; set; }



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
            PlayerJoin playerJoin = PlayerJoin.GetNewJoin((joinOpponent as PilotServer).Pilot);
            playerJoin.TargetPositionX = joinOpponent.NewPostion.x;
            playerJoin.TargetPositionY = joinOpponent.NewPostion.y;
            pilot.Send(new CommandData()
            {
                Command = Commands.PlayerJoin,
                Data = playerJoin
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
        if (TargetId == leaveOpponent?.Id || Id == leaveOpponent?.TargetId)
        {
            Target = null;
            leaveOpponent.Target = null;
        }

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
    public abstract ulong ShipId { get; protected set; }
    public bool IsPlayer { get; private set; }

    public Opponent()
    {
        IsPlayer = this is PilotServer;
    }
    #endregion

    #region IsDead / IsCover
    protected float isCoverTimer { get; set; }
    public virtual float IsCoverTimer
    {
        get => isCoverTimer;
        set
        {
            if (isCoverTimer == value)
                return;

            isCoverTimer = value;
            IsCover = value > 0;
        }
    }

    protected bool isCover { get; set; }
    public virtual bool IsCover
    {
        get => isCover;
        set
        {
            if (isCover == value)
                return;

            isCover = value;

            if (!value)
                IsCoverTimer = 0;

            OnCover();
        }
    }
    protected virtual void OnCover()
    {
        SendToPilotsInArea(new CommandData()
        {
            Command = Commands.SafeZone,
            Data = new SafeZone()
            {
                PilotId = Id,
                IsPlayer = IsPlayer,
                Status = IsCover
            }
        }, true);
    }

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
    protected List<OpponentAttack> AttackOpponents { get; set; } = new List<OpponentAttack>();
    protected virtual void OnDead()
    {
        Target = null;

        SomeoneDead someoneDead = new SomeoneDead()
        {
            WhoId = Id,
            WhoIsPlayer = IsPlayer,
            Killers = new List<Killer>()
        };

        int players = 0, enemies = 0;

        for (int i = 0; i < AttackOpponents.Count; i++)
        {
            if (AttackOpponents[i].Opponent.IsPlayer)
                players++;
            else
                enemies++;

            someoneDead.Killers.Add(new Killer()
            {
                Id = AttackOpponents[i].Opponent.Id,
                IsPlayer = AttackOpponents[i].Opponent.IsPlayer,
                Name = AttackOpponents[i].Opponent.Name,
            });
        }

        if (IsPlayer)
        {
            if (players > 0)
            {
                (this as PilotServer).AddAchievement(o => o.DeadByPlayer);
            }

            if (enemies > 0)
            {
                (this as PilotServer).AddAchievement(o => o.DeadByNPC);
            }
        }

        SendToPilotsInArea(new CommandData()
        {
            Command = Commands.Dead,
            Data = someoneDead
        }, true);

        AttackOpponents.Clear();
    }
    protected void OnAlive()
    {
        Hitpoints = MaxHitpoints / 10;
        LastTakeDamage = 0;

        IsCoverTimer = 10;

        SendToPilotsInArea(new CommandData()
        {
            Command = Commands.RepairShip,
            Data = Id
        }, true);
    }
    #endregion

    #region Ammunition / Rocket
    public virtual long? Ammunition { get; set; }
    public virtual long? Rocket { get; set; }
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
    public long Hitpoints
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
    protected virtual long maxHitpoints { get; set; }
    public long MaxHitpoints
    {
        get => maxHitpoints;
        set
        {
            if (maxHitpoints == value)
                return;

            maxHitpoints = value;

            OnChangeHitpoints();
        }
    }

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
    protected virtual long maxShields { get; set; }
    public long MaxShields
    {
        get => maxShields;
        set
        {
            if (maxShields == value)
                return;

            maxShields = value;

            OnChangeShields();
        }
    }

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
    protected virtual float speed { get; set; }
    public float Speed
    {
        get => speed;
        set
        {
            if (speed == value)
                return;

            speed = value;

            OnChangePosition();
        }
    }
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
    protected bool TargetIsCover => !TargetIsDead ? Target.IsCover : false;
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

            if (value && IsCover)
                IsCover = false;

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
        }, true);
    }
    #endregion

    #region Damage / RandomDamage
    public abstract long DamagePvp { get; }
    public abstract long DamagePve { get; }

    protected long RandomDamage(long damage)
    {
        return (long)Random.Range(damage * (1 - ShotDispersion), damage * (1 + ShotDispersion));
    }
    #endregion

    #region TakeDamage
    public float LastTakeDamage = 0;
    public virtual void OnTakeDamage(Opponent opponent, long? receivedDamage, int ammunition, bool type)
    {
        if (IsDead)
            return;

        if (IsCover)
            return;

        OpponentAttack opponentAttack = AttackOpponents.FirstOrDefault(o => o.Opponent.Id == opponent.Id && o.Opponent.IsPlayer == opponent.IsPlayer);
        if (opponentAttack == null)
        {
            AttackOpponents.Add(new OpponentAttack() { Opponent = opponent, LastAttack = 5 });
        }
        else
            opponentAttack.LastAttack = 5;

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

        if (receivedDamage != null)
        {
            if(IsPlayer)
            {
                if (opponent.IsPlayer)
                    (this as PilotServer).AddAchievement(o => o.DamageReceivePlayer, receivedDamage);
                else
                    (this as PilotServer).AddAchievement(o => o.DamageReceiveNPC, receivedDamage);
            }

            if (opponent.IsPlayer)
            {
                if (IsPlayer)
                    (opponent as PilotServer).AddAchievement(o => o.DamageDealPlayer, receivedDamage);
                else
                    (opponent as PilotServer).AddAchievement(o => o.DamageDealNPC, receivedDamage);
            }

            TakeDamage((long)receivedDamage);
            CheckIfDead();
        }
    }
    protected void TakeDamage(long damage)
    {
        long dmgHp = 0;

        if (Shields > 0)
        {
            long dmgShd = (long)(damage * ShieldDivision);
            if (Shields - dmgShd >= 0)
            {
                if (IsPlayer)
                    (this as PilotServer).AddAchievement(o => o.ShieldDestroy, dmgShd);

                dmgHp = damage - dmgShd;
                Shields -= dmgShd;
            }
            else
            {
                if (IsPlayer)
                    (this as PilotServer).AddAchievement(o => o.ShieldDestroy, Shields);

                dmgHp = damage - Shields;
                Shields = 0;
            }
        }
        else
            dmgHp = damage;
        
        if (Hitpoints - dmgHp >= 0)
        {
            if (IsPlayer)
                (this as PilotServer).AddAchievement(o => o.HitpointDestroy, dmgHp);

            Hitpoints -= dmgHp;
        }
        else
        {
            if (IsPlayer)
                (this as PilotServer).AddAchievement(o => o.HitpointDestroy, Hitpoints);

            Hitpoints = 0;
        }
    }
    protected void CheckIfDead()
    {
        if (Hitpoints == 0)
        {
            Reward reward;
            int devide = AttackOpponents.Where(o => o.Opponent.IsPlayer).Count();
            if (devide > 0)
            {
                reward = new Reward()
                {
                    Id = Reward.Id,
                    Items = Reward.Items,
                    AmmunitionId = Reward.AmmunitionId,
                };

                if (Reward.Experience > 0)
                    reward.Experience = (ulong)Mathf.CeilToInt((long)Reward.Experience / devide);
                if (Reward.Metal > 0)
                    reward.Metal = (ulong)Mathf.CeilToInt((long)Reward.Metal / devide);
                if (Reward.Scrap > 0)
                    reward.Scrap = (ulong)Mathf.CeilToInt((long)Reward.Scrap / devide);
                if (Reward.AmmunitionQuantity > 0)
                    reward.AmmunitionQuantity = Mathf.CeilToInt((long)Reward.AmmunitionQuantity / devide);
            }
            else
                reward = Reward;

            foreach (OpponentAttack opponentAttack in AttackOpponents)
            {
                if (opponentAttack.Opponent.IsPlayer)
                {
                    opponentAttack.Opponent.TakeReward(ServerReward.GetReward(
                        reward,
                        RewardReason,
                        Name));

                    if (RewardReason == RewardReasons.KillEnemy)
                    {
                        (opponentAttack.Opponent as PilotServer).AddAchievement(o => o.KillNPC, ShipId);
                    }
                    else if (RewardReason == RewardReasons.KillPlayer)
                    {
                        (opponentAttack.Opponent as PilotServer).AddAchievement(o => o.KillPlayer, ShipId);
                    }
                }
            }

            IsDead = true;
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
                if (TargetIsCover)
                {
                    Target = null;
                }
                else if(MapServer.Distance(this, Target) <= ShotDistance)
                {
                    if (Ammunition != null)
                    {
                        if (!Server.ServerResources.ContainsKey((long)Ammunition))
                        {
                            Attack = false;
                            Ammunition = 100;
                            return;
                        }
                        Ammunition ammunition = Server.ServerResources[(long)Ammunition];

                        long damage = DamagePve;

                        if (IsPlayer)
                        {
                            if(!(this as PilotServer).Pilot.Resources.ContainsKey(ammunition.Id))
                            {
                                Attack = false;
                                (this as PilotServer).Pilot.AmmunitionId = 100;
                                Debug.Log("Brak zaznaczonego zasobu u gracza.");
                                return;
                            }

                            if (!(this as PilotServer).SubtractAmmunition((this as PilotServer).Pilot.Resources[ammunition.Id]))
                            {
                                Attack = false;
                                return;
                            }

                            damage = (long)(Target.IsPlayer ? (ammunition.MultiplierPlayer * DamagePvp) : (ammunition.MultiplierEnemy * DamagePve));
                        }

                        Target.OnTakeDamage(this, RandomDamage(damage), (int)Ammunition, true);
                    }
                }
            }
            else if (CanRepair)
                Repair();
            else
            {
                LastTakeDamage -= UPDATE_TIME;
                for (int i = 0; i < AttackOpponents.Count; i++)
                {
                    AttackOpponents[i].LastAttack -= 1;
                }

                if (CanRepair)
                    AttackOpponents.Clear();
            }
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

        Vector2 oldPos = default;
        if (IsPlayer)
        {
            oldPos = Position;
        }

        Position = Vector3.MoveTowards(Position, NewPostion, Time.deltaTime * Speed);

        if (IsPlayer)
        {
            decimal dist = new decimal(Vector2.Distance(oldPos, Position));
            if (dist > 0)
                (this as PilotServer).AddAchievement(o => o.TravelDistance, dist);
        }
    }

    public virtual void CheckCover()
    {
        if(IsCover)
        {
            IsCoverTimer -= UPDATE_TIME;
        }
    }
    #endregion

    #region Repair
    protected float ShieldDivision = 0.7f; 
    protected int ShieldRepair = 20;
    public bool CanRepair => LastTakeDamage <= 0;
    public void Repair()
    {
        if (CanRepearHitpoints)
        {
            var hitpoint = MaxHitpoints / 30;
            if (Hitpoints + hitpoint <= MaxHitpoints)
            {
                if (IsPlayer)
                    (this as PilotServer).AddAchievement(o => o.HitpointRepair, hitpoint);
                
                Hitpoints += hitpoint;
            }
            else
            {
                if (IsPlayer)
                    (this as PilotServer).AddAchievement(o => o.HitpointRepair, MaxHitpoints - Hitpoints);

                Hitpoints = MaxHitpoints;
            }
        }

        if (CanRepearShields)
        {
            if(Shields > MaxShields)
            {
                Shields = MaxShields;
            }
            else
            {
                var shield = MaxShields / ShieldRepair;
                if (Shields + shield <= MaxShields)
                {
                    if (IsPlayer)
                        (this as PilotServer).AddAchievement(o => o.ShieldRepair, shield);

                    Shields += shield;
                }
                else
                {
                    if (IsPlayer)
                        (this as PilotServer).AddAchievement(o => o.ShieldRepair, MaxShields - Shields);

                    Shields = MaxShields;
                }
            }
        }
    }
    #endregion
}