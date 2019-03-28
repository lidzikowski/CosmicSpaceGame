using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using System;
using CosmicSpaceCommunication.Game.Enemy;
using CosmicSpaceCommunication.Game.Resources;

public class Client : MonoBehaviour
{
    public static WebSocket Socket;

    private static Pilot pilot;
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



    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    void Start()
    {
        //if (Application.version != GameData.GameVersion)
        //    Debug.Log($"Wersja: {Application.version} DLL: {GameData.GameVersion}");

        PlayerScript = GetComponent<Player>();

        CreateSocket();
    }

    void CreateSocket()
    {
        Socket = new WebSocket($"{GameData.ServerIP}/Game");

        Socket.OnOpen += Socket_OnOpen;
        Socket.OnClose += Socket_OnClose;
        Socket.OnError += Socket_OnError;
        Socket.OnMessage += Socket_OnMessage;
    }

    float timer = 9;
    void Update()
    {
        if (SocketConnected)
            return;

        if (timer >= 10)
        {
            timer = 0;
            try
            {
                if (!Socket.IsAlive)
                {
                    Socket.ConnectAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex);
                CreateSocket();
            }
        }
        else
            timer += Time.deltaTime;
    }



    private void OnApplicationQuit()
    {
        if (!SocketConnected)
            return;

        SendToSocket(new CommandData()
        {
            Command = Commands.PlayerLeave
        });

        try
        {
            Socket.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
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
        catch (Exception ex)
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
        #region LOGOWANIE / REJESTRACJA
        if (commandData.Command == Commands.LogIn)
        {
            bool status = (bool)commandData.Data;
            //Debug.Log($"LOG_IN_STATUS: {status}");
        }
        else if (commandData.Command == Commands.Register)
        {
            bool status = (bool)commandData.Data;
            //Debug.Log($"REGISTER_STATUS: {status}");
        }
        else if (commandData.Command == Commands.AccountOccupied)
        {
            //Debug.Log($"ACCOUNT_OCCUPIED");
        }
        else if (commandData.Command == Commands.NicknameOccupied)
        {
            //Debug.Log($"NICKNAME_OCCUPIED");
        }
        #endregion

        #region INICJALIZACJA ZALOGOWANEGO GRACZA
        else if (commandData.Command == Commands.UserData)
        {
            Pilot = (Pilot)commandData.Data;
        }
        #endregion

        #region DOLACZENIE / ODLACZENIE GRACZA OD SERWERA
        else if (commandData.Command == Commands.PlayerJoin)
        {
            GetComponent<Player>().InitPlayer((PlayerJoin)commandData.Data);
        }
        else if (commandData.Command == Commands.PlayerLeave)
        {
            GetComponent<Player>().LeavePlayer((ulong)commandData.Data);
        }
        #endregion

        #region ZDARZENIE NA ZMIANE POZYCJI
        else if (commandData.Command == Commands.NewPosition)
        {
            NewPosition newPosition = (NewPosition)commandData.Data;

            if (newPosition == null)
                return;

            GetComponent<Player>().ChangePosition(newPosition);
        }
        #endregion

        #region ZDARZENIE NA ZMIANE HITPOINTS
        else if (commandData.Command == Commands.ChangeHitpoints)
        {
            NewHitpointsOrShields newValue = (NewHitpointsOrShields)commandData.Data;

            if (newValue == null)
                return;

            GetComponent<Player>().HitpointsOrShields(newValue, true);
        }
        #endregion

        #region ZDARZENIE NA ZMIANE SHIELDS
        else if (commandData.Command == Commands.ChangeShields)
        {
            NewHitpointsOrShields newValue = (NewHitpointsOrShields)commandData.Data;

            if (newValue == null)
                return;

            GetComponent<Player>().HitpointsOrShields(newValue, false);
        }
        #endregion

        #region ZDARZENIE NA ZMIANE TARGET
        else if (commandData.Command == Commands.SelectTarget)
        {
            NewTarget newTarget = (NewTarget)commandData.Data;

            if (newTarget == null)
                return;

            GetComponent<Player>().SelectTarget(newTarget);
        }
        #endregion

        #region ZDARZENIE NA ATAK TARGETU
        else if (commandData.Command == Commands.AttackTarget)
        {
            AttackTarget attackTarget = (AttackTarget)commandData.Data;

            if (attackTarget == null)
                return;

            GetComponent<Player>().AttackTarget(attackTarget);
        }
        #endregion


        #region ZDARZENIE NA OTRZYMYWANIE OBRAZEN
        else if (commandData.Command == Commands.GetDamage)
        {
            TakeDamage takeDamage = (TakeDamage)commandData.Data;

            if (takeDamage == null)
                return;

            GetComponent<Player>().SomeoneTakeDamage(takeDamage);
        }
        #endregion


        #region ZDARZENIE NA SMIERC
        else if (commandData.Command == Commands.Dead)
        {
            SomeoneDead someoneDead = (SomeoneDead)commandData.Data;

            if (someoneDead == null)
                return;

            GetComponent<Player>().SomeoneDead(someoneDead);
        }
        #endregion


        #region ZDARZENIE NA ODRODZENIE
        else if (commandData.Command == Commands.RepairShip)
        {
            ulong userId;
            if (!ulong.TryParse(commandData.Data.ToString(), out userId))
                return;

            GetComponent<Player>().SomeoneAlive(userId);
        }
        #endregion


        #region DOLACZENIE / ODLACZENIE ENEMY OD SERWERA
        else if (commandData.Command == Commands.EnemyJoin)
        {
            GetComponent<Player>().InitEnemy((EnemyJoin)commandData.Data);
        }
        else if (commandData.Command == Commands.EnemyLeave)
        {
            GetComponent<Player>().LeaveEnemy((ulong)commandData.Data);
        }
        #endregion


        #region OTRZYMANIE NAGRODY
        else if (commandData.Command == Commands.NewReward)
        {
            GetComponent<Player>().TakeReward((ServerReward)commandData.Data);
        }
        #endregion



    }

    private void Socket_OnError(object sender, ErrorEventArgs e)
    {
        MainThread.Instance().Enqueue(() => SocketConnected = false);


        Debug.LogError($"OnError {Environment.NewLine} {e.Exception} {Environment.NewLine} {e.Message}");
    }

    private void Socket_OnClose(object sender, CloseEventArgs e)
    {
        MainThread.Instance().Enqueue(() => SocketConnected = false);

        switch(e.Code)
        {
            case 1001:
                //Debug.LogError($"OnClose - Server close");
                break;
            case 1005:
                //Debug.LogError($"OnClose - Client close");
                break;
            case 1006:
                //Debug.LogError($"OnClose - Client close");
                break;
            default:
                Debug.LogError($"OnClose {Environment.NewLine} {e.Code} {Environment.NewLine} {e.WasClean} {Environment.NewLine} {e.Reason}");
                break;
        }
    }

    private void Socket_OnOpen(object sender, EventArgs e)
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
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }
}