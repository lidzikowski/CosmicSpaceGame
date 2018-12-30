using UnityEngine;

public delegate void ChangePosition(Opponent opponent, Vector2 position, Vector2 targetPosition, int speed);
public delegate void ChangeHitpoints(Opponent opponent, ulong hitpoints, ulong maxHitpoints);
public delegate void ChangeShields(Opponent opponent, ulong shields, ulong maxShields);
public delegate void SelectTarget(Opponent opponent, Opponent targetOpponent);
public delegate void Attack(Opponent opponent, Opponent targetOpponent, bool attack);
public delegate void GetDamage(Opponent whoGet, Opponent whoSet, ulong? damage, int ammunition, bool type);
public delegate void Dead(Opponent whoDead, Opponent whoOpponent);


public abstract class Opponent
{
    protected static readonly float UPDATE_TIME = 1;
    protected static readonly float REPAIR_EVERY_UPDATE = 5;


    #region Id
    public abstract ulong Id { get; }
    public abstract string Name { get; }
    #endregion

    public bool IsPlayer => this is PilotServer;
    public virtual bool IsDead { get; set; } = false;

    public virtual int? Ammunition { get; set; }
    public virtual int? Rocket { get; set; }

    #region Position / TargetPosition
    public abstract Vector2 Position { get; set; }
    public Vector2 TargetPostion { get; set; }

    public virtual event ChangePosition OnChangePosition;
    #endregion

    #region Hitpoints
    public abstract ulong Hitpoints { get; set; }
    public abstract ulong MaxHitpoints { get; }
    public virtual bool CanRepearHitpoints => Hitpoints != MaxHitpoints;
    public virtual event ChangeHitpoints OnChangeHitpoints;
    #endregion

    #region Shields
    public abstract ulong Shields { get; set; }
    public abstract ulong MaxShields { get; }
    public virtual bool CanRepearShields => Shields != MaxShields;
    public virtual event ChangeShields OnChangeShields;
    #endregion

    #region Speed
    public abstract int Speed { get; }
    #endregion

    #region Target
    protected virtual Opponent target { get; set; }
    public virtual Opponent Target
    {
        get => target;
        set
        {
            if (target == value)
                return;

            target = value;

            if (value == null)
            {
                Attack = false;
                return;
            }

            OnSelectTarget?.Invoke(this, value);
        }
    }
    public virtual event SelectTarget OnSelectTarget;
    #endregion

    #region Attack
    protected virtual bool attack { get; set; }
    public virtual bool Attack
    {
        get => attack;
        set
        {
            if (attack == value)
                return;

            attack = value;

            OnAttackTarget?.Invoke(this, Target, attack);
        }
    }
    public virtual event Attack OnAttackTarget;
    #endregion

    #region Damage
    public abstract ulong Damage { get; }
    #endregion

    #region TakeDamage
    protected float LastTakeDamage = 0;
    public virtual void TakeDamage(Opponent opponent, ulong? damage, int ammunition, bool type)
    {
        OnGetDamage?.Invoke(opponent, this, damage, ammunition, type);

        if (damage == null) // Pudlo
            return;

        ulong dmg = (ulong)damage;

        LastTakeDamage = REPAIR_EVERY_UPDATE;

        if (Hitpoints - dmg <= MaxHitpoints)
            Hitpoints -= dmg;
        else
            Hitpoints = 0;

        if (Hitpoints == 0)
        {
            IsDead = true;
            OnDead?.Invoke(this, opponent);

            Debug.Log(Name + " zginal od " + opponent.Name);
        }
    }
    public virtual event GetDamage OnGetDamage;
    public virtual event Dead OnDead;
    #endregion



    #region Update
    protected float timer = 0;
    public virtual void Update()
    {
        Fly();

        timer += Time.deltaTime;
        if (timer >= UPDATE_TIME)
        {
            timer = 0;

            if (Attack && !Target.IsDead)
            {
                // Sprawdzenie dystansu

                if (Ammunition != null)
                {
                    LastTakeDamage = REPAIR_EVERY_UPDATE;
                    Target.TakeDamage(this, Damage, (int)Ammunition, true);
                }

                //if (Rocket != null)
                //    Target.TakeDamage(this, Damage, (int)Rocket, true);
            }
            else if (CanRepair)
                Repair();
            else
                LastTakeDamage -= UPDATE_TIME;
        }
    }
    #endregion

    

    #region Fly
    public void Fly()
    {
        if (TargetPostion == Position)
            return;
        Position = Vector3.MoveTowards(Position, TargetPostion, Time.deltaTime * Speed);
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