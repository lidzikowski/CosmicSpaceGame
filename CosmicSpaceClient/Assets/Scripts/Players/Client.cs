using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player;

public class Client : MonoBehaviour
{
    public static WebSocket Socket;

    public static Pilot pilot;
    public static Pilot Pilot
    {
        get => pilot;
        set
        {
            MainThread.Instance().Enqueue(() =>
            {
                if (pilot == value)
                    return;

                pilot = value;

                GuiScript.CloseAllWindow();
                if (value == null)
                {
                    GuiScript.OpenWindow(WindowTypes.MainMenu);
                    PlayerScript?.ClearGameArea();
                }
                else
                {
                    GuiScript.OpenWindow(WindowTypes.UserInterface);
                    PlayerScript?.InitLocalPlayer();
                }
            });
        }
    }

    private static bool socketConnected = false;
    public static bool SocketConnected
    {
        get => socketConnected;
        set
        {
            if (socketConnected == value)
                return;

            if (value)
            {
                socketConnected = true;
            }
            else
            {
                socketConnected = false;
                Pilot = null;
            }
            
            MainThread.Instance().Enqueue(() => GuiScript.RefreshAllActiveWindow());
        }
    }
    public static Player PlayerScript;



    void Start()
    {
        //if (Application.version != GameData.GameVersion)
        //    Debug.Log($"Wersja: {Application.version} DLL: {GameData.GameVersion}");

        PlayerScript = GetComponent<Player>();

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
            timer = 0;
            MainThread.Instance().Enqueue(() => Socket.Connect());
        }
        else
            timer += Time.deltaTime;
    }



    private void OnApplicationQuit()
    {
        // WIADOMOSC NA SERWER = LOGAM SIE.

        MainThread.Instance().Enqueue(() => Socket.Close());
    }

    private void Socket_OnMessage(object sender, MessageEventArgs e)
    {
        if (!e.IsBinary)
            return;

        CommandData commandData = null;

        try
        {
            commandData = GameData.Deserialize(e.RawData);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
        finally
        {
            if (commandData != null)
                MainThread.Instance().Enqueue(() => SocketMessage(commandData));
        }
    }

    private void SocketMessage(CommandData commandData)
    {
        if (commandData.Command == Commands.LogIn)
        {
            bool status = (bool)commandData.Data;
            Debug.Log($"LOG_IN_STATUS: {status}");
        }
        else if (commandData.Command == Commands.Register)
        {
            bool status = (bool)commandData.Data;
            Debug.Log($"REGISTER_STATUS: {status}");
        }
        else if (commandData.Command == Commands.AccountOccupied)
        {
            Debug.Log($"ACCOUNT_OCCUPIED");
        }
        else if (commandData.Command == Commands.NicknameOccupied)
        {
            Debug.Log($"NICKNAME_OCCUPIED");
        }
        else if (commandData.Command == Commands.UserData)
        {
            Pilot = (Pilot)commandData.Data;
        }
    }

    private void Socket_OnError(object sender, ErrorEventArgs e)
    {
        MainThread.Instance().Enqueue(() => SocketConnected = false);


        Debug.Log($"OnError {System.Environment.NewLine} {e.Exception} {System.Environment.NewLine} {e.Message}");
    }

    private void Socket_OnClose(object sender, CloseEventArgs e)
    {
        MainThread.Instance().Enqueue(() => SocketConnected = false);

        Debug.Log($"OnClose {System.Environment.NewLine} {e.Code} {System.Environment.NewLine} {e.WasClean} {System.Environment.NewLine} {e.Reason}");
    }

    private void Socket_OnOpen(object sender, System.EventArgs e)
    {
        MainThread.Instance().Enqueue(() => SocketConnected = true);
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