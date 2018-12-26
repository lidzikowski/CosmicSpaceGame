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



    private void Pilot_OnChangePosition(Pilot pilot, Vector2 position, Vector2 targetPosition)
    {
        foreach (PilotServer pilotServer in PilotsOnMap.Values.Where(o => o.Pilot.Id != pilot.Id))
        {
            pilotServer.Send(new CommandData()
            {
                Command = Commands.PlayerNewPosition,
                Data = new NewPosition()
                {
                    PlayerId = pilot.Id,
                    PositionX = position.x,
                    PositionY = position.y,
                    TargetPositionX = targetPosition.x,
                    TargetPositionY = targetPosition.y
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
        PilotsOnMap.Remove(pilot.Pilot.Id);
    }
}