using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapServer : MonoBehaviour
{
    public Dictionary<ulong, PilotServer> PilotsOnMap = new Dictionary<ulong, PilotServer>();



    private void FixedUpdate()
    {
        foreach(KeyValuePair<ulong, PilotServer> pilot in PilotsOnMap)
        {
            pilot.Value.Update();
        }
    }



    private void Pilot_OnChangePosition(Pilot pilot, Vector2 position, Vector2 targetPosition, int speed)
    {
        foreach (PilotServer pilotServer in PilotsOnMap.Values.Where(o => o.Pilot.Id != pilot.Id))
        {
            pilotServer.Send(new CommandData()
            {
                Command = Commands.NewPosition,
                Data = new NewPosition()
                {
                    PlayerId = pilot.Id,
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
    
    private void Pilot_OnChangeHitpoints(Pilot pilot, ulong hitpoints, ulong maxHitpoints)
    {
        foreach (PilotServer pilotServer in PilotsOnMap.Values)
        {
            pilotServer.Send(new CommandData()
            {
                Command = Commands.ChangeHitpoints,
                Data = new NewHitpointsOrShields()
                {
                    PlayerId = pilot.Id,
                    IsPlayer = true,
                    Value = hitpoints,
                    MaxValue = maxHitpoints
                }
            });
        }
    }

    private void Pilot_OnChangeShields(Pilot pilot, ulong shields, ulong maxShields)
    {
        foreach (PilotServer pilotServer in PilotsOnMap.Values)
        {
            pilotServer.Send(new CommandData()
            {
                Command = Commands.ChangeShields,
                Data = new NewHitpointsOrShields()
                {
                    PlayerId = pilot.Id,
                    IsPlayer = true,
                    Value = shields,
                    MaxValue = maxShields
                }
            });
        }
    }



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

        PilotsOnMap.Add(pilot.Pilot.Id, pilot);
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
        
        PilotsOnMap.Remove(pilot.Pilot.Id);
    }
}