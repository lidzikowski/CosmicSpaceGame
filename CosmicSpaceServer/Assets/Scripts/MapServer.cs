using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapServer : MonoBehaviour
{
    public Dictionary<ulong, Opponent> PilotsOnMap = new Dictionary<ulong, Opponent>();



    private void FixedUpdate()
    {
        foreach(KeyValuePair<ulong, Opponent> pilot in PilotsOnMap)
        {
            pilot.Value.Update();
        }
    }



    #region Events
    private void Pilot_OnChangePosition(Opponent opponent, Vector2 position, Vector2 targetPosition, int speed)
    {
        // opponent is player?
        foreach (PilotServer pilotServer in PilotsOnMap.Values.Where(o => o.Id != opponent.Id))
        {
            pilotServer.Send(new CommandData()
            {
                Command = Commands.NewPosition,
                Data = new NewPosition()
                {
                    PlayerId = opponent.Id,
                    IsPlayer = true,
                    PositionX = position.x,
                    PositionY = position.y,
                    TargetPositionX = targetPosition.x,
                    TargetPositionY = targetPosition.y,
                    Speed = speed
                }
            });
        }
    }
    
    private void Pilot_OnChangeHitpoints(Opponent opponent, ulong hitpoints, ulong maxHitpoints)
    {
        // opponent is player?
        foreach (PilotServer pilotServer in PilotsOnMap.Values)
        {
            pilotServer.Send(new CommandData()
            {
                Command = Commands.ChangeHitpoints,
                Data = new NewHitpointsOrShields()
                {
                    PlayerId = opponent.Id,
                    IsPlayer = true,
                    Value = hitpoints,
                    MaxValue = maxHitpoints
                }
            });
        }
    }

    private void Pilot_OnChangeShields(Opponent opponent, ulong shields, ulong maxShields)
    {
        // opponent is player?
        foreach (PilotServer pilotServer in PilotsOnMap.Values)
        {
            pilotServer.Send(new CommandData()
            {
                Command = Commands.ChangeShields,
                Data = new NewHitpointsOrShields()
                {
                    PlayerId = opponent.Id,
                    IsPlayer = true,
                    Value = shields,
                    MaxValue = maxShields
                }
            });
        }
    }
    
    private void Pilot_OnSelectTarget(Opponent opponent, Opponent targetOpponent)
    {
        // opponent is player?
        foreach (PilotServer pilotServer in PilotsOnMap.Values.Where(o => o.Id != opponent.Id))
        {
            pilotServer.Send(new CommandData()
            {
                Command = Commands.SelectTarget,
                Data = new NewTarget()
                {
                    PlayerId = opponent.Id,
                    AttackerIsPlayer = opponent.IsPlayer,
                    TargetId = targetOpponent.Id,
                    TargetIsPlayer = targetOpponent.IsPlayer
                }
            });
        }
    }

    private void Pilot_OnAttackTarget(Opponent opponent, Opponent targetOpponent, bool attack)
    {
        // opponent is player?
        foreach (PilotServer pilotServer in PilotsOnMap.Values)
        {
            pilotServer.Send(new CommandData()
            {
                Command = Commands.AttackTarget,
                Data = new AttackTarget()
                {
                    PlayerId = opponent.Id,
                    AttackerIsPlayer = opponent.IsPlayer,
                    TargetId = targetOpponent.Id,
                    TargetIsPlayer = targetOpponent.IsPlayer,

                    Attack = attack,
                    SelectedAmmunition = opponent.Ammunition,
                    SelectedRocket = opponent.Rocket
                }
            });
        }
    }
    
    private void Pilot_OnGetDamage(Opponent whoGet, Opponent whoSet, ulong? damage, int ammunition, bool type)
    {
        // opponent is player?
        foreach (PilotServer pilotServer in PilotsOnMap.Values)
        {
            pilotServer.Send(new CommandData()
            {
                Command = Commands.GetDamage,
                Data = new TakeDamage()
                {
                    ToId = whoGet.Id,
                    ToIsPlayer = whoGet.IsPlayer,

                    FromId = whoSet.Id,
                    FromIsPlayer = whoSet.IsPlayer,

                    Damage = damage,

                    AmmunitionId = ammunition,
                    IsAmmunition = type
                }
            });
        }
    }

    private void Pilot_OnDead(Opponent whoDead, Opponent whoOpponent)
    {
        // opponent is player?
        foreach (PilotServer pilotServer in PilotsOnMap.Values)
        {
            pilotServer.Send(new CommandData()
            {
                Command = Commands.Dead,
                Data = new SomeoneDead()
                {
                    WhoId = whoDead.Id,
                    WhoIsPlayer = whoDead.IsPlayer,

                    ById = whoOpponent.Id,
                    ByIsPlayer = whoOpponent.IsPlayer
                }
            });
        }
    }
    #endregion



    #region Join and Leave Pilot
    public void Join(PilotServer pilot)
    {
        if (PilotsOnMap.ContainsKey(pilot.Pilot.Id))
            return;
        
        foreach (PilotServer pilotServer in PilotsOnMap.Values)
        {
            pilotServer.Send(new CommandData()
            {
                Command = Commands.PlayerJoin,
                Data = PlayerJoin.Create(pilot.Pilot)
            });

            pilot.Send(new CommandData()
            {
                Command = Commands.PlayerJoin,
                Data = PlayerJoin.Create(pilotServer.Pilot)
            });
        }

        pilot.OnChangePosition += Pilot_OnChangePosition;
        pilot.OnChangeHitpoints += Pilot_OnChangeHitpoints;
        pilot.OnChangeShields += Pilot_OnChangeShields;
        pilot.OnSelectTarget += Pilot_OnSelectTarget;
        pilot.OnAttackTarget += Pilot_OnAttackTarget;
        pilot.OnGetDamage += Pilot_OnGetDamage;
        pilot.OnDead += Pilot_OnDead;

        PilotsOnMap.Add(pilot.Id, pilot);
    }

    public void Leave(PilotServer pilot)
    {
        if (!PilotsOnMap.ContainsKey(pilot.Pilot.Id))
            return;
        
        foreach (PilotServer pilotServer in PilotsOnMap.Values)
        {
            pilotServer.Send(new CommandData()
            {
                Command = Commands.PlayerLeave,
                Data = pilot.Pilot.Id
            });
        }

        pilot.OnChangePosition -= Pilot_OnChangePosition;
        pilot.OnChangeHitpoints -= Pilot_OnChangeHitpoints;
        pilot.OnChangeShields -= Pilot_OnChangeShields;
        pilot.OnSelectTarget -= Pilot_OnSelectTarget;
        pilot.OnAttackTarget -= Pilot_OnAttackTarget;
        pilot.OnGetDamage -= Pilot_OnGetDamage;
        pilot.OnDead -= Pilot_OnDead;

        PilotsOnMap.Remove(pilot.Id);
    }
    #endregion
}