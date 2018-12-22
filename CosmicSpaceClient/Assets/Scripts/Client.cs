using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using CosmicSpaceCommunication;

public class Client : MonoBehaviour
{
    public static WebSocket Socket;

    private static bool socketConnected = false;
    public static bool SocketConnected
    {
        get => socketConnected;
        set
        {
            if (value == socketConnected)
                return;

            if (value)
            {
                socketConnected = true;
                //MainThread.Instance().Enqueue(() => Gui.ShowWarning("Test", "Polaczono"));
            }
            else
            {
                socketConnected = false;
                //MainThread.Instance().Enqueue(() => Gui.ShowWarning("Test", "Rozlaczono"));
            }
            
            MainThread.Instance().Enqueue(() => GuiScript.RefreshAllActiveWindow());
        }
    }

    void Start()
    {
        //if (Application.version != GameData.GameVersion)
        //    Debug.Log($"Wersja: {Application.version} DLL: {GameData.GameVersion}");

        Socket = new WebSocket($"{GameData.ServerIP}/Game");

        Socket.OnOpen += Socket_OnOpen;

        Socket.OnClose += Socket_OnClose;

        Socket.OnError += Socket_OnError;

        Socket.OnMessage += Socket_OnMessage;
    }

    float timer = 5;
    void Update()
    {
        if (SocketConnected)
            return;

        if (timer > 5)
        {
            Socket.Connect();
            timer = 0;
        }
        else
            timer += Time.deltaTime;
    }

    private void OnApplicationQuit()
    {
        Socket.Close();
    }

    private void Socket_OnMessage(object sender, MessageEventArgs e)
    {
        Debug.Log($"OnMessage {System.Environment.NewLine} {e.IsBinary} {System.Environment.NewLine} {e.RawData} {System.Environment.NewLine} {e.Data}");

        //CommandData commandData = null;
        //try
        //{
        //    commandData = JsonConvert.DeserializeObject<CommandData>(e.Data);
        //}
        //catch (Exception exception)
        //{
        //    Debug.Log(e.Data);
        //    Debug.Log(exception.Message);
        //}
        //finally
        //{
        //    switch (commandData.Command)
        //    {
        //        // Error system:
        //        case (Command.Message):
        //            MessageData messageData = JsonConvert.DeserializeObject<MessageData>(commandData.Data);
        //            MainThread.Instance().Enqueue(() => ErrorMethod(messageData));
        //            break;
        //        // Error system:
        //        case (Command.GameDataSync):
        //            GameDataSync gameData = JsonConvert.DeserializeObject<GameDataSync>(commandData.Data);
        //            MainThread.Instance().Enqueue(() => CreateGameWorld(gameData));
        //            break;


        //    }
        //}
    }

    private void Socket_OnError(object sender, ErrorEventArgs e)
    {
        SocketConnected = false;

        Debug.Log($"OnError {System.Environment.NewLine} {e.Exception} {System.Environment.NewLine} {e.Message}");
    }

    private void Socket_OnClose(object sender, CloseEventArgs e)
    {
        SocketConnected = false;

        Debug.Log($"OnClose {System.Environment.NewLine} {e.Code} {System.Environment.NewLine} {e.WasClean} {System.Environment.NewLine} {e.Reason}");
    }

    private void Socket_OnOpen(object sender, System.EventArgs e)
    {
        SocketConnected = true;

        Debug.Log("OnOpen");
    }

    public static void SendToSocket(CommandData commandData)
    {
        if (!SocketConnected)
            return;

        try
        {
            Socket.Send(GameData.Serialize(commandData));
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
}