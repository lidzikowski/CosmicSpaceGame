﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp.Server;
using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Resources;

public class Server : MonoBehaviour
{
    public static WebSocketServer WebSocket;



    public static Dictionary<int, Ship> Ships;
    public static Dictionary<int, Map> Maps;

    private int mapId = 1000; // Instancje
    public static Dictionary<int, MapServer> MapsServer;

    public static Dictionary<ulong, PilotServer> Pilots;



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
        Ships = await Database.GetShips();

        Maps = await Database.GetMaps();
        GameObject maps = new GameObject() { name = $"Maps [{Maps?.Count}]" };
        Instantiate(maps, transform);

        MapsServer = new Dictionary<int, MapServer>();
        foreach (Map map in Maps.Values)
        {
            GameObject go = new GameObject() { name = $"{map.Id} -> {map.Name}" };
            go.transform.parent = maps.transform;
            go.AddComponent<MapServer>();

            MapsServer.Add(map.Id, go.GetComponent<MapServer>());
        }
    }

    void CreateWebSocket()
    {
        Pilots = new Dictionary<ulong, PilotServer>();

        WebSocket = new WebSocketServer(GameData.ServerIP);
        WebSocket.AddWebSocketService<Game>("/Game");
        WebSocket.Start();

        Debug.Log($"Server: {(WebSocket.IsListening ? "ONLINE" : "OFFLINE")}");
    }

    private void OnApplicationQuit()
    {
        WebSocket.Stop();

        foreach (KeyValuePair<ulong, PilotServer> pilot in Pilots)
        {
            // Zapis do bazy danych
        }
    }
}