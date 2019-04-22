using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp.Server;
using CosmicSpaceCommunication.Game.Resources;
using CosmicSpaceCommunication.Game.Enemy;
using System;

public class Server : MonoBehaviour
{
    public static WebSocketServer WebSocket;



    public static Dictionary<int, Ship> Ships;
    public static Dictionary<int, Map> Maps;
    public static Dictionary<int, Ammunition> Ammunitions;
    public static Dictionary<int, Rocket> Rockets;
    public static Dictionary<int, Enemy> Enemies;
    public static Dictionary<long, Item> Items;

    //private int mapId = 1000; // Instancje map
    public static Dictionary<int, MapServer> MapsServer;
    public static Dictionary<ulong, PilotServer> Pilots;

    //private ulong chatChannelId = 1000; // Instancje kanalow
    public static Dictionary<ulong, ChatChannel> ChatChannels;



    void Start()
    {
        //if (Application.version != GameData.GameVersion)
        //    Debug.Log($"Wersja: {Application.version} DLL: {GameData.GameVersion}");

        Application.targetFrameRate = 60;

        GameResourcesFromDatabase();

        CreateWebSocket();
    }



    private async void GameResourcesFromDatabase()
    {
        Items = await Database.GetItems();

        Ships = await Database.GetShips();

        Maps = await Database.GetMaps();

        Ammunitions = await Database.GetAmmunitions();

        Rockets = await Database.GetRockets();

        Enemies = await Database.GetEnemies();

        GameObject maps = new GameObject() { name = $"Maps [{Maps?.Count}]" };
        Instantiate(maps, transform);

        MapsServer = new Dictionary<int, MapServer>();
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

        ChatChannels = new Dictionary<ulong, ChatChannel>();
        // Global channel:
        ChatChannels.Add(100, new ChatChannel(100, "Global"));
    }



    void CreateWebSocket()
    {
        Pilots = new Dictionary<ulong, PilotServer>();

        try
        {
            //WebSocket = new WebSocketServer("ws://77.55.212.240:24231");
            WebSocket = new WebSocketServer("ws://127.0.0.1:24231");
            WebSocket.AddWebSocketService<GameService>("/Game");
            WebSocket.AddWebSocketService<ChatService>("/Chat");
            WebSocket.Start();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
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
            Debug.Log(ex);
        }

        //foreach (KeyValuePair<ulong, PilotServer> pilot in Pilots)
        //{
        //    // Zapis do bazy danych
        //}
    }
}