using WebSocketSharp;
using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Chat;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.EventSystems;

public class UserInterfaceWindow : GameWindow
{
    public static WebSocket ChatSocket;

    private static bool chatsocketConnected = false;
    public static bool ChatSocketConnected
    {
        get => chatsocketConnected;
        set
        {
            if (chatsocketConnected == value)
                return;

            chatsocketConnected = value;

            MainThread.Instance().Enqueue(() => GuiScript.RefreshAllActiveWindow());
        }
    }

    [Header("User Panel")]
    public Text UserText;
    public Text MetalText;
    public Text ScrapText;

    [Header("Mini Map")]
    public Text MapPositionText;
    public GameObject MapGameObject;
    public Button CloseMapWindowButton;
    public GameObject MapBackgroundGameObject;

    [Header("Log")]
    public Transform LogTransform;
    public GameObject LogGameObject;

    [Header("Chat")]
    public Transform ContentTransform;
    public GameObject MessageGameObject;
    public InputField MessageInputField;
    public Button SendMessageButton;
    public GameObject ChatGameObject;
    public Button CloseChatWindowButton;

    [Header("Menu Buttons")]
    public Button HangerButton;
    public Button ShopButton;
    public Button MissionButton;
    public Button MapButton;
    public Button ChatButton;
    public Button SettingsButton;
    public Button QuitButton;

    [Header("Menu Background")]
    public Sprite ActiveSprite;
    public Sprite DisactiveSprite;

    [Header("Safe Zone")]
    public Text SafeZoneText;



    public override void Start()
    {
        base.Start();

        ButtonListener(SendMessageButton, SendMessageButton_Clicked);

        ButtonListener(HangerButton, HangerButton_Clicked);

        ButtonListener(ShopButton, ShopButton_Clicked);

        ButtonListener(MissionButton, MissionButton_Clicked);

        ButtonListener(CloseMapWindowButton, CloseMapWindowButton_Clicked);
        ButtonListener(MapButton, CloseMapWindowButton_Clicked);

        ButtonListener(CloseChatWindowButton, CloseChatWindowButton_Clicked);
        ButtonListener(ChatButton, CloseChatWindowButton_Clicked);

        ButtonListener(SettingsButton, SettingsButton_Clicked);

        ButtonListener(QuitButton, QuitButton_Clicked);

        EventTrigger.Entry entry = new EventTrigger.Entry()
        {
            eventID = EventTriggerType.PointerDown
        };
        entry.callback.AddListener((data) => { ClickMap((PointerEventData)data); });
        MapBackgroundGameObject.GetComponent<EventTrigger>().triggers.Add(entry);
    }

    public override void Refresh()
    {
        base.Refresh();

        SetText(UserText, $"UID: {Client.Pilot.Id} {Client.Pilot.Nickname}");
        SetText(MetalText, $"Metal {Client.Pilot.Metal}");
        SetText(ScrapText, $"Scrap {Client.Pilot.Scrap}");

        string position = $"{(int)Player.LocalShipController.Position.x};{-(int)Player.LocalShipController.Position.y}";
        if (Player.LocalShipController.Position != Player.LocalShipController.TargetPosition)
        {
            position += $" > {(int)Player.LocalShipController.TargetPosition.x};{-(int)Player.LocalShipController.TargetPosition.y}";
        }
        SetText(MapPositionText, $"{Client.Pilot.Map.Name} [{position}]");

        RefreshButtonStatus();
    }

    public override void ChangeLanguage()
    {

    }

    public void CreateLogMessage(string message, float time)
    {
        GameObject go = Instantiate(LogGameObject, LogTransform);
        go.GetComponent<LogScript>().SetText(message, time);
    }

    private System.Collections.IEnumerator SenderTimer()
    {
        CanSend = false;
        yield return new WaitForSeconds(1);
        CanSend = true;
    }

    private bool CanSend = true;
    void SendMessageButton_Clicked()
    {
        if (!CanSend)
        {
            MessageInputField.text = MessageInputField.text;
            return;
        }
        StartCoroutine(SenderTimer());

        if (!ChatSocketConnected || string.IsNullOrEmpty(MessageInputField.text) || string.IsNullOrWhiteSpace(MessageInputField.text))
        {
            MessageInputField.text = string.Empty;
            return;
        }


        string text = MessageInputField.text.Trim('\r', '\n', '\t');
        MessageInputField.text = string.Empty;

        // Usuwanie spacji, przerw oraz zbednych odstepow
        string msg = string.Empty;
        foreach (string m in text.ToString().Split().Where(o => o != string.Empty))
        {
            msg += $"{m} ";
        }
        msg.Remove(msg.Length - 1, 1);
        text = msg;

        if (text[0] == '/')
        {
            text = text.Remove(0, 1); // Usuniecie '/'
            string[] inputText = text.Split(' ');
            if(inputText.Length > 0)
            {
                ChatCommands command;
                if (!Enum.TryParse<ChatCommands>(inputText[0], out command))
                {
                    GuiScript.CreateLogMessage(new List<string>()
                    {
                        string.Format(GameSettings.UserLanguage.COMMAND_NOT_FOUND,inputText[0])
                    }, 5);
                    return;
                }
                text = text.Remove(0, inputText[0].Length + 1); // Usuniecie znalezionej komendy

                switch (command)
                {
                    case ChatCommands.help:
                    case ChatCommands.h:
                        ChatData chatData = new ChatData() { SenderName = "Server" };
                        chatData.Message = $"\n<color=#00E676>/{ChatCommands.help.ToString()}, /{ChatCommands.h.ToString()}</color> - {GameSettings.UserLanguage.CHAT_HELP_LIST}\n" + $"<color=#00E676>/{ChatCommands.users.ToString()}, /{ChatCommands.u.ToString()}, /{ChatCommands.online.ToString()}</color> - {GameSettings.UserLanguage.CHAT_HELP_ONLINE}\n" + $"<color=#00E676>/{ChatCommands.message.ToString()}, /{ChatCommands.msg.ToString()}, /{ChatCommands.m.ToString()}, /{ChatCommands.pw.ToString()}</color> - {GameSettings.UserLanguage.CHAT_HELP_PRIVATE}";

                        chatData.SenderName = "Server";
                        chatData.RecipientId = Client.Pilot.Id;
                        chatData.RecipientName = Client.Pilot.Nickname;
                        ShowChatMessage(chatData);
                        return;


                    case ChatCommands.users:
                    case ChatCommands.u:
                    case ChatCommands.online:
                        SendToChatSocket(new CommandData()
                        {
                            Command = Commands.ChatMessage,
                            Data = new ChatData()
                            {
                                Command = ChatCommands.users,
                                ChannelId = 100,
                                SenderId = Client.Pilot.Id
                            }
                        });
                        break;


                    case ChatCommands.message:
                    case ChatCommands.msg:
                    case ChatCommands.m:
                    case ChatCommands.pw:
                        string recipientName = string.Empty;
                        if (inputText.Length > 1 && !string.IsNullOrEmpty(inputText[1]) && !string.IsNullOrWhiteSpace(inputText[1]))
                        {
                            recipientName = inputText[1].Trim(' ');
                            text = text.Remove(0, inputText[1].Length + 1); // Usuniecie nazwy odbiorcy
                        }
                        else
                        {
                            GuiScript.CreateLogMessage(new List<string>()
                            {
                                string.Format(GameSettings.UserLanguage.COMMAND_NOT_FOUND,inputText[0])
                            }, 5);
                            return;
                        }

                        if (!string.IsNullOrEmpty(text) && !string.IsNullOrWhiteSpace(text))
                        {
                            SendToChatSocket(new CommandData()
                            {
                                Command = Commands.ChatMessage,
                                Data = new ChatData()
                                {
                                    Command = ChatCommands.message,
                                    ChannelId = 100,
                                    SenderId = Client.Pilot.Id,
                                    RecipientName = recipientName,
                                    Message = text
                                }
                            });
                        }
                        else
                        {
                            GuiScript.CreateLogMessage(new List<string>()
                            {
                                GameSettings.UserLanguage.EMPTY_MESSAGE
                            }, 5);
                        }
                        break;
                }
            }
        }
        else
        {
            SendToChatSocket(new CommandData()
            {
                Command = Commands.ChatMessage,
                Data = new ChatData()
                {
                    Command = ChatCommands.channel,
                    ChannelId = 100,
                    SenderId = Client.Pilot.Id,
                    Message = text
                }
            });
        }
    }



    float timer = 14;
    void Update()
    {
        if (MessageInputField.isFocused && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
            SendMessageButton_Clicked();

        if (ChatSocketConnected)
            return;

        if (timer >= 15)
        {
            timer = 0;
            try
            {
                if (!ChatSocket.IsAlive)
                {
                    ChatSocket.ConnectAsync();
                }
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                CreateChatSocket();
            }
        }
        else
            timer += Time.deltaTime;
    }

    private void OnApplicationQuit()
    {
        if (!ChatSocketConnected)
            return;

        try
        {
            ChatSocket.Close();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void CreateChatSocket()
    {
        ShowChatMessage(new ChatData() { SenderName = "Server", Message = GameSettings.UserLanguage.CONNECTING_TO_CHAT });

        ChatSocket = new WebSocket($"{Client.SERVER_IP}/Chat");

        ChatSocket.OnOpen += ChatSocket_OnOpen;
        ChatSocket.OnClose += ChatSocket_OnClose;
        ChatSocket.OnError += ChatSocket_OnError;
        ChatSocket.OnMessage += ChatSocket_OnMessage;
    }

    private void ChatSocket_OnMessage(object sender, MessageEventArgs e)
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
                MainThread.Instance().Enqueue(() => ChatSocketMessage(commandData));
        }
    }

    private void ChatSocketMessage(CommandData commandData)
    {
        #region CHAT - OTRZYMANIE WIADOMOSCI
        if (commandData.Command == Commands.ChatMessage)
        {
            ChatData chatData = (ChatData)commandData.Data;
            if (chatData == null)
                return;
            ShowChatMessage(chatData);
        }
        else if (commandData.Command == Commands.ChatConnected)
        {
            ChatData chatData = (ChatData)commandData.Data;
            if (chatData == null)
                return;
            chatData.Message = string.Format(GameSettings.UserLanguage.CHAT_CONNECTED, chatData.Message);
            ShowChatMessage(chatData);
        }
        else if (commandData.Command == Commands.ChatDisconnected)
        {
            ChatData chatData = (ChatData)commandData.Data;
            if (chatData == null)
                return;
            chatData.Message = string.Format(GameSettings.UserLanguage.CHAT_DISCONNECTED, chatData.Message);
            ShowChatMessage(chatData);
        }
        else if (commandData.Command == Commands.ChatUserNotFound)
        {
            ChatData chatData = (ChatData)commandData.Data;
            if (chatData == null)
                return;
            chatData.Message = string.Format(GameSettings.UserLanguage.CHAT_USER_NOT_FOUND, $"<color=#FFE082>{chatData.Message}</color>");
            ShowChatMessage(chatData);
        }
        #endregion
    }

    private void ShowChatMessage(ChatData chatData)
    {
        string text;

        string sender, message;
        if (chatData.SenderName == "Server")
        {
            sender = $"<b><color=#FF5722>{chatData.SenderName}</color></b>";
            message = $"<color=#FF9100>{chatData.Message}</color>";
        }
        else
        {
            sender = $"<b><color=#40C4FF>{chatData.SenderName}</color></b>";
            message = $"<color=#E0E0E0>{chatData.Message}</color>";
        }


        if (chatData.RecipientId != null)
        {
            if (chatData.SenderId == Client.Pilot.Id)
                sender = $"<b><color=#FFD740>{GameSettings.UserLanguage.YOU}</color></b>";

            string recipient = chatData.RecipientId == Client.Pilot.Id ? $"<b><color=#FFD740>{GameSettings.UserLanguage.YOU}</color></b>" : $"<b><color=#40C4FF>{chatData.RecipientName}</color></b>";
            text = $"{sender} <color=616161>></color> {recipient}: {message}";
        }
        else
        {
            text = $"{sender}<color=9E9E9E>:</color> {message}";
        }

        GameObject go = Instantiate(MessageGameObject, ContentTransform);
        go.GetComponent<Text>().text = text;
    }

    private void ChatSocket_OnError(object sender, ErrorEventArgs e)
    {
        MainThread.Instance().Enqueue(() => ChatSocketConnected = false);
        Debug.LogError($"OnError {Environment.NewLine} {e.Exception} {Environment.NewLine} {e.Message}");
    }

    private void ChatSocket_OnClose(object sender, CloseEventArgs e)
    {
        MainThread.Instance().Enqueue(() => ChatSocketConnected = false);
        Client.SocketErrorSwitch(e);
    }

    private void ChatSocket_OnOpen(object sender, EventArgs e)
    {
        MainThread.Instance().Enqueue(() => {
            foreach (Transform child in ContentTransform)
            {
                Destroy(child.gameObject);
            }

            ChatSocketConnected = true;

            SendToChatSocket(new CommandData()
            {
                Command = Commands.LogIn,
                Data = Client.Pilot.Id
            });
        });
    }

    public static void SendToChatSocket(CommandData commandData)
    {
        if (!ChatSocketConnected)
            return;

        try
        {
            ChatSocket.Send(GameData.Serialize(commandData));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
        }
    }

    private void CloseMapWindowButton_Clicked()
    {
        SetActiveWindow(MapGameObject, MapButton);
    }

    private void CloseChatWindowButton_Clicked()
    {
        SetActiveWindow(ChatGameObject, ChatButton);
    }

    private void SetActiveWindow(GameObject windowGameObject, Button windowButton)
    {
        bool status = !windowGameObject.activeSelf;
        windowGameObject?.SetActive(status);

        if (windowButton != null)
            SetActiveButton(windowButton, status);
    }

    private void SetActiveButton(Button windowButton, bool status)
    {
        if (windowButton == null)
            return;

        windowButton.gameObject.GetComponent<Image>().sprite = status ? ActiveSprite : DisactiveSprite;
    }



    private void HangerButton_Clicked()
    {
        Client.SendToSocket(new CommandData()
        {
            Command = Commands.GetEquipment,
            Data = Client.Pilot.Id
        });
    }

    private void ShopButton_Clicked()
    {
        SetActiveWindow(WindowTypes.ShopWindow);
    }

    private void MissionButton_Clicked()
    {
        SetActiveWindow(WindowTypes.MissionWindow);
    }

    private void SettingsButton_Clicked()
    {
        SetActiveWindow(WindowTypes.SettingsWindow);
    }

    private void QuitButton_Clicked()
    {
        SetActiveWindow(WindowTypes.QuitWindow);
    }



    public void SetActiveWindow(WindowTypes windowType)
    {
        foreach (KeyValuePair<WindowTypes, WindowInstance> window in GuiScript.Windows.Where(o => o.Key != windowType && o.Key != WindowTypes.UserInterface))
        {
            if (window.Value.Active)
                GuiScript.CloseWindow(window.Key);
        }

        if (GuiScript.Windows[windowType].Active)
            GuiScript.CloseWindow(windowType);
        else
            GuiScript.OpenWindow(windowType);

        RefreshButtonStatus();
    }

    private void RefreshButtonStatus()
    {
        SetActiveButton(SettingsButton, GuiScript.Windows[WindowTypes.SettingsWindow].Active);
        SetActiveButton(MissionButton, GuiScript.Windows[WindowTypes.MissionWindow].Active);
        SetActiveButton(HangerButton, GuiScript.Windows[WindowTypes.HangarWindow].Active);
        SetActiveButton(ShopButton, GuiScript.Windows[WindowTypes.ShopWindow].Active);
        SetActiveButton(QuitButton, GuiScript.Windows[WindowTypes.QuitWindow].Active);
    }


    public void ClickMap(PointerEventData data)
    {
        RectTransform Map = MapBackgroundGameObject.transform as RectTransform;
        Vector3 localPosition = MapBackgroundGameObject.transform.InverseTransformPoint(data.pressPosition);
        Player.TargetPosition = new Vector2(Map.rect.width / 2 + localPosition.x, -(Map.rect.height / 2 - localPosition.y)) * 4;
    }
}