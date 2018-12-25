using System.Collections;
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
    public static Dictionary<ulong, Pilot> Pilots;



    void Start()
    {
        //if (Application.version != GameData.GameVersion)
        //    Debug.Log($"Wersja: {Application.version} DLL: {GameData.GameVersion}");

        Application.targetFrameRate = 60;

        GameResourcesFromDatabase();

        CreateWebSocket();
    }

    private void GameResourcesFromDatabase()
    {
        Ships = Database.GetShips();
        Debug.Log($"Ships: {Ships.Count}");

        Maps = Database.GetMaps();
        Debug.Log($"Maps: {Maps.Count}");
    }

    void CreateWebSocket()
    {
        Pilots = new Dictionary<ulong, Pilot>();

        WebSocket = new WebSocketServer(GameData.ServerIP);
        WebSocket.AddWebSocketService<Game>("/Game");
        WebSocket.Start();

        Debug.Log($"Server: {(WebSocket.IsListening ? "ONLINE" : "OFFLINE")}");
    }

    private void OnApplicationQuit()
    {
        foreach (KeyValuePair<ulong, Pilot> pilot in Pilots)
        {
            pilot.Value.Send(new CommandData()
            {
                Command = Commands.ServerClosed,
                Data = "{OnApplicationQuit}"
            });
        }

        WebSocket.Stop();
    }
}