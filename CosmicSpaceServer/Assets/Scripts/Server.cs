using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp.Server;
using CosmicSpaceCommunication;

public class Server : MonoBehaviour
{
    public static WebSocketServer WebSocket;

    void Start()
    {
        //if (Application.version != GameData.GameVersion)
        //    Debug.Log($"Wersja: {Application.version} DLL: {GameData.GameVersion}");

        Application.targetFrameRate = 60;

        CreateSocketServer();

        //Database.Test();
        //return;

        Debug.Log(Database.OccupiedAccount(new CosmicSpaceCommunication.Account.RegisterUser()
        {
            Username = "test",
            Email = "test"
        }));
    }

    void CreateSocketServer()
    {
        WebSocket = new WebSocketServer(GameData.ServerIP);
        WebSocket.AddWebSocketService<Game>("/Game");
        WebSocket.Start();

        Debug.Log($"Status serwera: {WebSocket.IsListening}");
    }

    private void OnApplicationQuit()
    {
        WebSocket.Stop();
    }
}