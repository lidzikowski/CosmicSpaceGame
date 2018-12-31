using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapServer : MonoBehaviour
{
    protected static readonly float SYNC_DISTANCE = 150;



    public List<Opponent> PilotsOnMap = new List<Opponent> ();



    //float timer = 0;
    private void Update()
    {
        //timer += Time.deltaTime;
        //bool time = timer >= 0.5f;

        foreach (Opponent pilotOnMap in PilotsOnMap)
        {
            FindOpponents(pilotOnMap);

            pilotOnMap.Update();
        }
        //if (time)
        //    timer = 0;
    }

    private void FindOpponents(Opponent pilot)
    {
        foreach (Opponent opponent in PilotsOnMap.Where(o => o != pilot))
        {
            if(Distance(pilot, opponent) <= SYNC_DISTANCE)
                pilot.AddOpponentInArea(opponent);
            else
                pilot.RemoveOpponentInArea(opponent);
        }
    }

    public static float Distance(Opponent a, Opponent b)
    {
        return Vector2.Distance(a.Position, b.Position);
    }
    


    #region Join and Leave Pilot from Map
    public void Join(PilotServer pilot)
    {
        if (!PilotsOnMap.Contains(pilot))
        {
            PilotsOnMap.Add(pilot);
        }
    }

    public void Leave(PilotServer pilot)
    {
        if (PilotsOnMap.Contains(pilot))
        {
            foreach (Opponent opponent in pilot.PilotsInArea)
                opponent.RemoveOpponentInArea(pilot);

            PilotsOnMap.Remove(pilot);
        }
    }
    #endregion
}