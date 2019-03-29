using WebSocketSharp;
using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Chat;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

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
    public Text MiniMapPanelText;
    public Text MapPosition;

    [Header("Log")]
    public Transform LogTransform;
    public GameObject LogGameObject;

    [Header("Chat")]
    public Transform ContentTransform;
    public GameObject MessageGameObject;
    public InputField MessageInputField;
    public Button SendMessageButton;



    public override void Start()
    {
        base.Start();

        ButtonListener(SendMessageButton, SendMessageButton_Clicked);
    }

    public override void Refresh()
    {
        base.Refresh();

        SetText(UserText, $"UID: {Client.Pilot.Id} {Client.Pilot.Nickname}");
        SetText(MetalText, $"Metal {Client.Pilot.Metal}");
        SetText(ScrapText, $"Scrap {Client.Pilot.Scrap}");

        string position = $"{(int)Player.LocalShipController.Position.x} / {-(int)Player.LocalShipController.Position.y}";
        if (Player.LocalShipController.Position != Player.LocalShipController.TargetPosition)
        {
            position += $" > {(int)Player.LocalShipController.TargetPosition.x} / {-(int)Player.LocalShipController.TargetPosition.y}";
        }
        SetText(MapPosition, position);
    }

    public override void ChangeLanguage()
    {
        SetText(MiniMapPanelText, $"{GameSettings.UserLanguage.MINIMAP} -> {Client.Pilot.Map.Name}");
    }

    public void CreateLogMessage(string message, float time)
    {
        GameObject go = Instantiate(LogGameObject, LogTransform);
        go.GetComponent<LogScript>().SetText(message, time);
    }

    void SendMessageButton_Clicked()
    {
        if (!ChatSocketConnected || string.IsNullOrEmpty(MessageInputField.text))
            return;

        string text = MessageInputField.text;
        MessageInputField.text = string.Empty;

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
                text = text.Remove(0, inputText[0].Length); // Usuniecie znalezionej komendy

                switch (command)
                {
                    case ChatCommands.help:
                    case ChatCommands.h:
                        ChatData chatData = new ChatData() { SenderName = "System" };
                        string helpString = $"\n/{ChatCommands.help.ToString()}, /{ChatCommands.h.ToString()} - Help guide.\n" + $"/{ChatCommands.users.ToString()}, /{ChatCommands.u.ToString()}, /{ChatCommands.online.ToString()} - Online users on this channel.\n" + $"/{ChatCommands.message.ToString()}, /{ChatCommands.msg.ToString()}, /{ChatCommands.m.ToString()}, /{ChatCommands.pw.ToString()} - Private message to other online user (/pw username message...).";

                        chatData.Message = helpString;
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
                            recipientName = inputText[1];
                            text = text.Remove(0, inputText[1].Length); // Usuniecie nazwy odbiorcy
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



    float timer = 9;
    void Update()
    {
        if (ChatSocketConnected)
            return;

        if (timer >= 10)
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
        ChatSocket = new WebSocket($"{GameData.ServerIP}/Chat");

        ChatSocket.OnOpen += ChatSocket_OnOpen;
        ChatSocket.OnClose += ChatSocket_OnClose;
        ChatSocket.OnError += ChatSocket_OnError;
        ChatSocket.OnMessage += ChatSocket_OnMessage;

        ShowChatMessage(new ChatData() { SenderName = "Server", Message = $"<color=#E0E0E0>{GameSettings.UserLanguage.CONNECTING_TO_CHAT}</color>" });
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
            string recipient = chatData.RecipientId == Client.Pilot.Id ? GameSettings.UserLanguage.YOU : chatData.RecipientName;
            text = $"  {sender} <color=FFD740>>></color> <color=#FFD740>{recipient}</color>: {message}";
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
}