using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Enemy;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapServer : MonoBehaviour
{
    protected static readonly float SYNC_DISTANCE = 150;



    public List<PilotServer> PilotsOnMap = new List<PilotServer>();

    ulong enemyId = 100;
    public List<EnemyServer> EnemiesOnMap = new List<EnemyServer>();



    public Map CurrentMap;
    private List<EnemyMap> enemiesOnCurrentMap;
    public List<EnemyMap> EnemiesOnCurrentMap
    {
        get => enemiesOnCurrentMap;
        set
        {
            enemiesOnCurrentMap = value;

            CheckEnemyOnMap();
        }
    }


    
    private void Update()
    {
        foreach (Opponent pilotOnMap in PilotsOnMap)
        {
            FindOpponents(pilotOnMap);

            pilotOnMap.Update();
        }

        foreach (Opponent enemyOnMap in EnemiesOnMap)
        {
            enemyOnMap.Update();
        }
    }


    #region Spawn enemy on map
    private void CheckEnemyOnMap()
    {
        foreach (EnemyMap enemyMap in EnemiesOnCurrentMap)
        {
            int count = EnemiesOnMap.Where(o => o.ParentEnemy.Id == enemyMap.EnemyId).Count();
            if (count < enemyMap.Count)
            {
                SpawnEnemy(Server.Enemies[enemyMap.EnemyId], enemyMap.Count - count);
            }
        }
    }

    private void SpawnEnemy(Enemy enemy, int count)
    {
        for (int i = 0; i < count; i++)
        {
            EnemiesOnMap.Add(new EnemyServer(enemy, enemyId++, RandomPosition(), this));
        }
    }

    public static Vector2 RandomPosition()
    {
        return new Vector2(Random.Range(0, 1000), -Random.Range(0, 1000));
    }
    public static Vector2 RandomPosition(Vector3 position)
    {
        Vector2 pos = RandomCircle(position, Random.Range(5, 50));
        if (pos.x >= 0 && pos.x <= 1000 && pos.y <= 0 && pos.y >= -1000)
            return pos;
        return RandomPosition();
    }

    public static Vector2 RandomCircle(Vector2 center, float radius)
    {
        float ang = Random.value * 360;
        return new Vector2()
        {
            x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad),
            y = center.y + radius * Mathf.Cos(ang * Mathf.Deg2Rad),
        };
    }
    #endregion

    #region Opponents in area
    private void FindOpponents(Opponent pilot)
    {
        SearchOpponent(pilot, PilotsOnMap.Where(o => o != pilot));
        SearchOpponent(pilot, EnemiesOnMap);
    }

    private void SearchOpponent(Opponent pilot, IEnumerable<Opponent> opponents)
    {
        foreach (Opponent opponent in opponents)
        {
            if (Distance(pilot, opponent) <= SYNC_DISTANCE ||
                pilot.Target == opponent ||
                opponent.Target == pilot)
                pilot.AddOpponentInArea(opponent);
            else
                pilot.RemoveOpponentInArea(opponent);
        }
    }

    public static float Distance(Opponent a, Opponent b)
    {
        return Vector2.Distance(a.Position, b.Position);
    }
    #endregion



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
            foreach (Opponent opponent in pilot.OpponentsInArea)
                opponent.RemoveOpponentInArea(pilot, false);

            PilotsOnMap.Remove(pilot);
        }
    }
    #endregion
}