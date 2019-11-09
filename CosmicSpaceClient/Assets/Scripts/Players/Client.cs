using System;
using UnityEngine;
using WebSocketSharp;
using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Enemy;
using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;
using System.Linq;
using CosmicSpaceCommunication.Game.Quest;

public class Client : MonoBehaviour
{
    public static WebSocket Socket;
    //public const string SERVER_IP = "ws://77.55.212.240:24231";
    public const string SERVER_IP = "ws://127.0.0.1:24231";

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
                    ServerResources = null;

                    ShopWindow.ServerItems = null;
                    GalacticWindow.ServerMaps = null;
                    MissionWindow.Tasks = null;

                    GuiScript.OpenWindow(WindowTypes.MainMenu);
                    PlayerScript?.ClearGameArea();
                }
                else
                {
                    GuiScript.OpenWindow(WindowTypes.UserInterface);
                    PlayerScript?.InitLocalPlayer();

                    if (GuiScript.Windows[WindowTypes.UserInterface].Script is UserInterfaceWindow userInterface)
                    {
                        userInterface.CreateChatSocket();
                    }
                }
            });
        }
    }
    public static AuthUserData ServerResources;

    private static bool socketConnected = false;
    public static bool SocketConnected
    {
        get => socketConnected;
        set
        {
            if (socketConnected == value)
                return;

            socketConnected = value;
            if (!value)
            {
                if(UserInterfaceWindow.ChatSocketConnected)
                {
                    try
                    {
                        UserInterfaceWindow.ChatSocket?.Close();
                    }
                    catch(Exception ex)
                    {
                        Debug.Log(ex.Message);
                    }
                }
                Pilot = null;
            }
            
            MainThread.Instance().Enqueue(() => GuiScript.RefreshAllActiveWindow());
        }
    }
    public static Player PlayerScript;



    private void Awake()
    {
        Application.targetFrameRate = 144;
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
        Socket = new WebSocket($"{SERVER_IP}/Game");

        Socket.WaitTime = new TimeSpan(0, 20, 0);

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
                Debug.Log(ex.Message);
                CreateSocket();
            }
        }
        else
            timer += Time.deltaTime;
    }



    private void OnApplicationQuit()
    {
        if (UserInterfaceWindow.ChatSocket?.IsAlive ?? false)
        {
            try
            {
                UserInterfaceWindow.ChatSocket.Close();
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        if (SocketConnected)
        {
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
                Debug.LogError(ex.Message);
            }
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
            if (commandData.Data is bool data)
            {
                bool status = data;
                //Debug.Log($"LOG_IN_STATUS: {status}");
            }
            else
                WrongDataLog(typeof(bool), commandData.Data?.GetType());
        }
        else if (commandData.Command == Commands.Register)
        {
            if (commandData.Data is bool data)
            {
                bool status = data;
                //Debug.Log($"REGISTER_STATUS: {status}");
            }
            else
                WrongDataLog(typeof(bool), commandData.Data?.GetType());
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
            if (commandData.Data is AuthUserData data)
            {
                Pilot = data.Pilot;
                ServerResources = data;
            }
            else
                WrongDataLog(typeof(AuthUserData), commandData.Data?.GetType());
        }
        #endregion

        #region DOLACZENIE / ODLACZENIE GRACZA OD SERWERA
        else if (commandData.Command == Commands.PlayerJoin)
        {
            if (commandData.Data is PlayerJoin data)
            {
                GetComponent<Player>().InitPlayer(data);
            }
            else
                WrongDataLog(typeof(PlayerJoin), commandData.Data?.GetType());
        }
        else if (commandData.Command == Commands.PlayerLeave)
        {
            if (commandData.Data is ulong data)
            {
                GetComponent<Player>().LeavePlayer(data);
            }
            else
                WrongDataLog(typeof(ulong), commandData.Data?.GetType());
        }
        #endregion

        #region ZDARZENIE NA ZMIANE POZYCJI
        else if (commandData.Command == Commands.NewPosition)
        {
            if (commandData.Data is NewPosition data)
            {
                GetComponent<Player>().ChangePosition(data);
            }
            else
                WrongDataLog(typeof(NewPosition), commandData.Data?.GetType());
        }
        #endregion

        #region ZDARZENIE NA ZMIANE HITPOINTS / SHIELDS
        else if (commandData.Command == Commands.ChangeHitpoints || commandData.Command == Commands.ChangeShields)
        {
            if (commandData.Data is NewHitpointsOrShields data)
            {
                GetComponent<Player>().HitpointsOrShields(data, commandData.Command == Commands.ChangeHitpoints);
            }
            else
                WrongDataLog(typeof(NewHitpointsOrShields), commandData.Data?.GetType());
        }
        #endregion

        #region ZDARZENIE NA ZMIANE TARGET
        else if (commandData.Command == Commands.SelectTarget)
        {
            if (commandData.Data is NewTarget data)
            {
                GetComponent<Player>().SelectTarget(data);
            }
            else
                WrongDataLog(typeof(NewTarget), commandData.Data?.GetType());
        }
        #endregion

        #region ZDARZENIE NA ATAK TARGETU
        else if (commandData.Command == Commands.AttackTarget)
        {
            if (commandData.Data is AttackTarget data)
            {
                GetComponent<Player>().AttackTarget(data);
            }
            else
                WrongDataLog(typeof(AttackTarget), commandData.Data?.GetType());
        }
        #endregion


        #region ZDARZENIE NA OTRZYMYWANIE OBRAZEN
        else if (commandData.Command == Commands.GetDamage)
        {
            if (commandData.Data is TakeDamage data)
            {
                GetComponent<Player>().SomeoneTakeDamage(data);
            }
            else
                WrongDataLog(typeof(TakeDamage), commandData.Data?.GetType());
        }
        #endregion


        #region ZDARZENIE NA SMIERC
        else if (commandData.Command == Commands.Dead)
        {
            if (commandData.Data is SomeoneDead data)
            {
                GetComponent<Player>().SomeoneDead(data);
            }
            else
                WrongDataLog(typeof(SomeoneDead), commandData.Data?.GetType());
        }
        #endregion


        #region ZDARZENIE NA ODRODZENIE
        else if (commandData.Command == Commands.RepairShip)
        {
            if (commandData.Data is ulong data)
            {
                GetComponent<Player>().SomeoneAlive(data);
            }
            else
                WrongDataLog(typeof(ulong), commandData.Data?.GetType());
        }
        #endregion


        #region DOLACZENIE / ODLACZENIE ENEMY OD SERWERA
        else if (commandData.Command == Commands.EnemyJoin)
        {
            if (commandData.Data is EnemyJoin data)
            {
                GetComponent<Player>().InitEnemy(data);
            }
            else
                WrongDataLog(typeof(EnemyJoin), commandData.Data?.GetType());
        }
        else if (commandData.Command == Commands.EnemyLeave)
        {
            if (commandData.Data is ulong data)
            {
                GetComponent<Player>().LeaveEnemy(data);
            }
            else
                WrongDataLog(typeof(ulong), commandData.Data?.GetType());
        }
        #endregion


        #region OTRZYMANIE NAGRODY
        else if (commandData.Command == Commands.NewReward)
        {
            if (commandData.Data is ServerReward data)
            {
                GetComponent<Player>().TakeReward(data);
            }
            else
                WrongDataLog(typeof(ServerReward), commandData.Data?.GetType());
        }
        #endregion


        #region ZMIANA MAPY
        else if (commandData.Command == Commands.ChangeMap)
        {
            if (commandData.Data is Pilot pilot)
            {
                GetComponent<Player>().ChangeMap(pilot);
            }
            else
            {
                Player.LocalShipController.TargetGameObject = null;
                GetComponent<Player>().ClearGameArea(false);

                WrongDataLog(typeof(Pilot), commandData.Data?.GetType());
            }
        }
        #endregion


        #region STREFA OCHRONNA
        else if (commandData.Command == Commands.SafeZone)
        {
            if (commandData.Data is SafeZone data)
            {
                GetComponent<Player>().SafeZone(data);
            }
            else
                WrongDataLog(typeof(SafeZone), commandData.Data?.GetType());
        }
        #endregion


        #region EKWIPUNEK
        else if (commandData.Command == Commands.GetEquipment)
        {
            if (commandData.Data is Pilot data)
            {
                Pilot.Ship = data.Ship;
                Pilot.Items = data.Items;
                (GuiScript.Windows[WindowTypes.UserInterface].Script as UserInterfaceWindow).SetActiveWindow(WindowTypes.HangarWindow);
            }
            else
                WrongDataLog(typeof(Pilot), commandData.Data?.GetType());
        }
        #endregion


        #region SHOP ITEMS
        else if (commandData.Command == Commands.GetShopItems)
        {
            if (commandData.Data is ShopItems data)
            {
                ShopWindow.ServerItems = data;
                (GuiScript.Windows[WindowTypes.UserInterface].Script as UserInterfaceWindow).SetActiveWindow(WindowTypes.ShopWindow);
            }
            else
                WrongDataLog(typeof(ShopItems), commandData.Data?.GetType());
        }
        #endregion


        #region ZAKUP PRZEDMIOTU
        else if (commandData.Command == Commands.BuyShopItem)
        {
            if (commandData.Data is ShoppingStatus data)
            {
                switch (data.Status)
                {
                    case ShopStatus.Error:
                        GuiScript.CreateLogMessage(new List<string> {
                            GameSettings.UserLanguage.ITEM_ERROR
                        });
                        break;
                    case ShopStatus.NoScrap:
                    case ShopStatus.NoMetal:
                        GuiScript.CreateLogMessage(new List<string> {
                            string.Format(GameSettings.UserLanguage.NOT_HAVE_ENOUGH, data.Status == ShopStatus.NoMetal ? "Metal" : "Scrap"),
                            string.Format(GameSettings.UserLanguage.NEED, data.Status == ShopStatus.NoMetal ? data.ShopItem.MetalPrice : data.ShopItem.ScrapPrice)
                        });
                        break;
                    case ShopStatus.WrongRequiredLevel:
                        GuiScript.CreateLogMessage(new List<string> {
                            GameSettings.UserLanguage.NO_REQUIRED_LEVEL,
                            string.Format(GameSettings.UserLanguage.NEED, data.ShopItem.RequiredLevel),
                        });
                        break;
                }
            }
            else
                WrongDataLog(typeof(ShoppingStatus), commandData.Data?.GetType());
        }
        #endregion


        #region ZMIANA STATKU
        else if (commandData.Command == Commands.ChangeShip)
        {
            if (commandData.Data is Ship data)
            {
                GetComponent<Player>().SomeoneChangeShip(commandData.SenderId, data);
            }
            else
                WrongDataLog(typeof(Ship), commandData.Data?.GetType());
        }
        #endregion


        #region SPRZEDAZ PRZEDMIOTU Z EKWIPUNKU
        else if (commandData.Command == Commands.SellEquipmentItem)
        {
            if (commandData.Data is ulong data)
            {
                ItemPilot itemPilot = Pilot.Items.FirstOrDefault(o => o.RelationId == data);
                if (itemPilot != null)
                {
                    Pilot.Items.Remove(itemPilot);
                    if (GuiScript.Windows[WindowTypes.HangarWindow].Active)
                    {
                        (GuiScript.Windows[WindowTypes.HangarWindow].Script as HangarWindow).RefreshAllPanels();
                    }
                }
                else
                    GuiScript.CreateLogMessage(new List<string> {
                        GameSettings.UserLanguage.SELL_ITEM_ERROR
                    });
            }
            else
                WrongDataLog(typeof(ulong), commandData.Data?.GetType());
        }
        #endregion


        #region ZMIANA AMUNICJI
        else if (commandData.Command == Commands.ChangeAmmunition)
        {
            if (commandData.Data is ChangeAmmunition data)
            {
                Pilot.AmmunitionId = data.SelectedAmmunitionId;
                Pilot.RocketId = data.SelectedRocketId;

                if (Pilot.Resources.ContainsKey(data.ResourceId))
                    Pilot.Resources[data.ResourceId].Count = data.Count;
            }
            else
                WrongDataLog(typeof(ChangeAmmunition), commandData.Data?.GetType());
        }
        #endregion


        #region POBRANIE GALAKTYKI
        else if (commandData.Command == Commands.GetAllMaps)
        {
            if (commandData.Data is Dictionary<long, Map> data)
            {
                (GuiScript.Windows[WindowTypes.UserInterface].Script as UserInterfaceWindow).SetActiveWindow(WindowTypes.GalacticWindow);
                GalacticWindow.ServerMaps = data;
            }
            else
                WrongDataLog(typeof(Dictionary<long, Map>), commandData.Data?.GetType());
        }
        #endregion


        #region ZADANIA - POSTEP
        else if (commandData.Command == Commands.QuestProgress)
        {
            if (commandData.Data is PilotTask data)
            {
                PilotTask localTask = Pilot.Tasks.FirstOrDefault(o => o.Id == data.Id);
                if(localTask == null)
                {
                    Debug.LogError($"{nameof(PilotTask)} => {nameof(NullReferenceException)}");
                    return;
                }

                localTask.TaskQuest = data.TaskQuest;
            }
            else
                WrongDataLog(typeof(PilotTask), commandData.Data?.GetType());
        }
        #endregion


        #region ZADANIA - ZAKONCZENIE
        else if (commandData.Command == Commands.QuestEndTask)
        {
            if (commandData.Data is PilotTask data)
            {
                PilotTask localTask = Pilot.Tasks.FirstOrDefault(o => o.Id == data.Id);
                if (localTask == null)
                {
                    Debug.LogError($"{nameof(PilotTask)} => {nameof(NullReferenceException)}");
                    return;
                }

                localTask.End = data.End;
                Pilot.Tasks.Remove(localTask);
            }
            else
                WrongDataLog(typeof(PilotTask), commandData.Data?.GetType());
        }
        #endregion


        #region POBRANIE ZADAN
        else if (commandData.Command == Commands.QuestList)
        {
            if (commandData.Data is Dictionary<uint, QuestTask> data)
            {
                (GuiScript.Windows[WindowTypes.UserInterface].Script as UserInterfaceWindow).SetActiveWindow(WindowTypes.MissionWindow);
                MissionWindow.Tasks = data;
            }
            else
                WrongDataLog(typeof(Dictionary<uint, QuestTask>), commandData.Data?.GetType());
        }
        #endregion


        #region POBRANIE POSTEPU ZADAN
        else if (commandData.Command == Commands.GetProgressTasks)
        {
            if (commandData.Data is List<PilotProgressTask> data)
            {
                (GuiScript.Windows[WindowTypes.MissionWindow].Script as MissionWindow).ProgressTasks = data;
            }
            else
                WrongDataLog(typeof(List<PilotProgressTask>), commandData.Data?.GetType());
        }
        #endregion





        else
            Debug.LogError(commandData.Command);
    }

    private void Socket_OnError(object sender, ErrorEventArgs e)
    {
        MainThread.Instance().Enqueue(() => SocketConnected = false);
        Debug.LogError($"OnError {Environment.NewLine} {e.Exception} {Environment.NewLine} {e.Message}");
    }

    private void Socket_OnClose(object sender, CloseEventArgs e)
    {
        MainThread.Instance().Enqueue(() => SocketConnected = false);
        SocketErrorSwitch(e);
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
            if (Pilot != null)
                commandData.SenderId = Pilot.Id;

            Socket.Send(GameData.Serialize(commandData));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    public static void SocketErrorSwitch(CloseEventArgs e)
    {
        switch (e.Code)
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

    private void WrongDataLog(Type expectedType, Type receivedType)
    {
        Debug.LogError($"[{nameof(Client.SocketMessage)}] Nieoczekiwany typ danych => Oczekiwany: '{expectedType?.Name}', Otrzymany: '{receivedType?.Name}'");
    }
}