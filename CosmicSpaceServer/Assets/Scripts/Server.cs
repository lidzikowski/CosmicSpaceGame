using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp.Server;
using CosmicSpaceCommunication.Game.Resources;
using CosmicSpaceCommunication.Game.Enemy;
using System;
using CosmicSpaceCommunication.Game.Quest;

public class Server : MonoBehaviour
{
    public static WebSocketServer WebSocket;



    public static Dictionary<long, Ship> Ships;
    public static Dictionary<long, Map> Maps;
    public static Dictionary<long, Ammunition> ServerResources;
    public static Dictionary<long, Rocket> Rockets;
    public static Dictionary<long, Enemy> Enemies;
    public static Dictionary<long, Item> Items;
    public static Dictionary<uint, Quest> Quests;
    public static Dictionary<uint, QuestTask> Tasks;

    //private int mapId = 1000; // Instancje map
    public static Dictionary<long, MapServer> MapsServer;
    public static Dictionary<ulong, PilotServer> Pilots;

    //private ulong chatChannelId = 1000; // Instancje kanalow
    public static Dictionary<ulong, ChatChannel> ChatChannels;



    void Start()
    {
        Application.targetFrameRate = 60;

        GameResourcesFromDatabase();

        CreateWebSocket();
    }



    private async void GameResourcesFromDatabase()
    {
        Items = await Database.GetItems();

        Ships = await Database.GetShips();

        Maps = await Database.GetMaps();

        ServerResources = await Database.GetAmmunitions();

        Rockets = await Database.GetRockets();

        Enemies = await Database.GetEnemies();

        GameObject maps = new GameObject() { name = $"Maps [{Maps?.Count}]" };
        Instantiate(maps, transform);

        MapsServer = new Dictionary<long, MapServer>();
        foreach (Map map in Maps.Values)
        {
            GameObject go = new GameObject() { name = $"{map.Id} -> {map.Name}" };
            go.transform.parent = maps.transform;
            MapServer mapServer = go.AddComponent<MapServer>();

            mapServer.CurrentMap = map;

            mapServer.EnemiesOnCurrentMap = await Database.GetEnemyMap(map.Id);
            map.Portals = await Database.GetPortals(map.Id);

            MapsServer.Add(map.Id, mapServer);
        }

        Quests = await Database.GetQuests();

        Tasks = await Database.GetTasks();



        ChatChannels = new Dictionary<ulong, ChatChannel>();
        // Global channel:
        ChatChannels.Add(100, new ChatChannel(100, "Global"));
    }



    void CreateWebSocket()
    {
        Pilots = new Dictionary<ulong, PilotServer>();

        try
        {
            #region PUBLIKACJA
            WebSocket = new WebSocketServer("ws://77.55.215.236:24231");
            #endregion

            #region DEBUG
            //WebSocket = new WebSocketServer("ws://127.0.0.1:24231");
            #endregion

            WebSocket.AddWebSocketService<GameService>("/Game");
            WebSocket.AddWebSocketService<ChatService>("/Chat");
            WebSocket.Start();
        }
        catch (Exception ex)
        {
            Log("Blad tworzenia socketu.", ex.Message);
        }

        Debug.Log($"Server: {(WebSocket.IsListening ? "ONLINE" : "OFFLINE")}");
    }

    private void OnApplicationQuit()
    {
        try
        {
            WebSocket.Stop();
        }
        catch (Exception ex)
        {
            Log("Blad zatrzymania socketu.", ex.Message);
        }

        SavePlayers();
    }

    private async void SavePlayers()
    {
        foreach (PilotServer pilot in Pilots.Values)
        {
            await Database.SavePlayerData(pilot.Pilot);
        }
    }

    public static void Log(string message, params object[] data)
    {
        var method = new System.Diagnostics.StackTrace().GetFrame(1);
        string dataMessage = string.Empty;
        if(data != null)
        {
            foreach (object o in data)
            {
                if (o.GetType().IsArray && o is object[] array)
                {
                    dataMessage += $"{Environment.NewLine}Tablica [{array.Length}]:";
                    for (int i = 0; i < array.Length; i++)
                        dataMessage += $"{Environment.NewLine}[{i}]: {array[i]}";
                }
                else
                    dataMessage += $"{Environment.NewLine}{o}";
            }
        }
        Debug.LogError($"[Error] '{method.GetMethod().ReflectedType.Name}.{method.GetMethod().Name}' {message}: {dataMessage}");
    }
}