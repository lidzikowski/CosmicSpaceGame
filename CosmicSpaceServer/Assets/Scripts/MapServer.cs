using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Enemy;
using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MapServer : MonoBehaviour
{
    protected static readonly float SYNC_DISTANCE = 150;
    protected static readonly int MAP_SIZE = 1000;



    public List<PilotServer> PilotsOnMap = new List<PilotServer>();

    private static ulong enemyId = 1000;
    private static ulong enemyMaxId = ulong.MaxValue - 1000;
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
            FindSafeZone(pilotOnMap);
            FindOpponents(pilotOnMap);
            pilotOnMap.Update();
        }

        foreach (Opponent enemyOnMap in EnemiesOnMap)
        {
            enemyOnMap.Update();
        }
    }

    private void FindSafeZone(Opponent pilotOnMap)
    {
        if (!pilotOnMap.CanRepair)
            return;

        if (CurrentMap.Portals.FirstOrDefault(o => Distance(new Vector2(o.PositionX, o.PositionY), pilotOnMap.Position) < 50) != null)
        {
            if (pilotOnMap.Attack)
                pilotOnMap.IsCoverTimer = 0;
            else
                pilotOnMap.IsCoverTimer = 2;
        }
    }


    #region Spawn enemy on map
    public void CheckEnemyOnMap()
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
        if (enemyId + ulong.Parse(count.ToString()) > enemyMaxId)
        {
            enemyId = 1000;
        }

        for (int i = 0; i < count; i++)
        {
            EnemyServer enemyServer = new EnemyServer(enemy, enemyId++, RandomPosition(), this);
            EnemiesOnMap.Add(enemyServer);
            //Debug.Log($"{nameof(SpawnEnemy)}: [{enemyServer.Id}] {enemyServer.ParentEnemy.Name}");
        }
    }

    public static Vector2 RandomPosition()
    {
        return new Vector2(Random.Range(0, MAP_SIZE), -Random.Range(0, MAP_SIZE));
    }
    public static Vector2 RandomPosition(Vector3 position)
    {
        Vector2 pos = RandomCircle(position, Random.Range(5, 50));
        if (pos.x >= 0 && pos.x <= MAP_SIZE && pos.y <= 0 && pos.y >= -MAP_SIZE)
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
        if (pilot == null)
        {
            Server.Log("Gracz nie istnieje - null.");
            return;
        }

        SearchOpponent(pilot, PilotsOnMap.Where(o => o.Id != pilot.Id));
        SearchOpponent(pilot, EnemiesOnMap);
    }

    private void SearchOpponent(Opponent pilot, IEnumerable<Opponent> opponents)
    {
        foreach (Opponent opponent in opponents)
        {
            if (Distance(pilot, opponent) <= SYNC_DISTANCE ||
                pilot.Target?.Id == opponent?.Id ||
                opponent.Target?.Id == pilot?.Id)
                pilot.AddOpponentInArea(opponent);
            else
                pilot.RemoveOpponentInArea(opponent);
        }
    }

    public static float Distance(Opponent a, Opponent b)
    {
        if (a == null || b == null)
            return float.MaxValue;
        return Distance(a.Position, b.Position);
    }
    public static float Distance(Vector2 a, Vector2 b)
    {
        if (a == null || b == null)
            return float.MaxValue;
        return Vector2.Distance(a, b);
    }
    #endregion



    #region Join and Leave Pilot from Map
    public bool Join(PilotServer pilot)
    {
        if (!PilotsOnMap.Contains(pilot))
        {
            PilotsOnMap.Add(pilot);

            pilot.Pilot.Map = CurrentMap;

            return true;
        }
        else
            Server.Log("Gracz juz jest na tej mapie", CurrentMap.Id, pilot?.Name, pilot?.Pilot?.Map.Id);
        return false;
    }

    public bool Leave(PilotServer pilot)
    {
        if (PilotsOnMap.Contains(pilot))
        {
            foreach (Opponent opponent in pilot.OpponentsInArea)
                opponent.RemoveOpponentInArea(pilot, false);

            PilotsOnMap.Remove(pilot);
            return true;
        }
        return false;
    }
    #endregion

    #region Change map
    public bool ChangeMapByPortal(PilotServer pilot, Portal portal)
    {
        // Zgodnosc mapy pilota z mapa portalu
        if (pilot.Pilot.Map.Id != CurrentMap.Id)
        {
            Server.Log("Gracz juz jest na tej mapie", CurrentMap.Id, pilot.Pilot.Map.Id);
            return false;
        }

        // Czy portal istnieje na obecnej mapie
        if (portal.Map.Id != CurrentMap.Id)
        {
            Server.Log("Portalu nie ma na tej mapie.", CurrentMap.Id, portal.Map.Id);
            return false;
        }

        // Sprawdzenie czy pilot jest przy portalu
        if (Distance(pilot.Position, new Vector2(portal.PositionX, portal.PositionY)) >= 15)
        {
            Server.Log("Gracz nie jest przy portalu.", pilot.Position, new Vector2(portal.PositionX, portal.PositionY), Distance(pilot.Position, new Vector2(portal.PositionX, portal.PositionY)));
            return false;
        }

        return ChangeMap(pilot, portal);
    }

    public bool ChangeMap(PilotServer pilotServer, Portal portal)
    {
        if (Leave(pilotServer))
        {
            pilotServer.Send(new CommandData()
            {
                Command = Commands.ChangeMap,
                Data = true
            });

            pilotServer.NewPostion = pilotServer.Position = new Vector2(portal.TargetPositionX, portal.TargetPositionY);

            pilotServer.IsCoverTimer = 5;
            pilotServer.OpponentsInArea.Clear();

            if (Server.MapsServer[portal.TargetMap.Id].Join(pilotServer))
            {
                pilotServer.Send(new CommandData()
                {
                    Command = Commands.ChangeMap,
                    Data = pilotServer.Pilot
                });

                return true;
            }
            else
                Server.Log("Gracz nie dolaczyc do nowej mapy.", pilotServer.Id, portal.TargetMap.Id);
        }
        else
            Server.Log("Gracz nie mogl opuscic mapy.", pilotServer.Id, CurrentMap.Id);

        return false;
    }
    #endregion
}