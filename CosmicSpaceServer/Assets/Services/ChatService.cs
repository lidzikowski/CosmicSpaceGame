using System;
using System.Linq;
using UnityEngine;
using WebSocketSharp;
using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Chat;

public class ChatService : WebSocket
{
    protected override void OnClose(CloseEventArgs e)
    {
        PilotDisconnect();
    }

    protected override void OnError(ErrorEventArgs e)
    {
        PilotDisconnect();
        Server.Log("Blad ChatService.", e.Message);
        base.OnError(e);
    }

    private void PilotDisconnect()
    {
        PilotServer pilotServer = Server.Pilots.Values.FirstOrDefault(o => o.ChatHeaders.SocketId == ID);

        if (pilotServer == null)
        {
            Server.Log("Nie znaleziono gracza na serwerze.");
            return;
        }

        Server.ChatChannels[100].Disconnect(pilotServer.Pilot.Id);
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        if (!e.IsBinary)
            return;

        try
        {
            CommandData commandData = GameData.Deserialize(e.RawData);
            switch (commandData.Command)
            {
                case Commands.LogIn:
                    ulong userId;
                    if (ulong.TryParse(commandData.Data.ToString(), out userId))
                    {
                        if(Server.Pilots.ContainsKey(userId))
                            Server.Pilots[userId].ChatHeaders = GetHeaders();
                        Server.ChatChannels[100].Connect(userId);
                    }
                    break;


                case Commands.ChatMessage:
                    ChatData chatData = (ChatData)commandData.Data;

                    if (chatData == null)
                        return;

                    if (!CheckPacket(chatData.SenderId, false))
                        return;

                    ChatMessage(chatData);
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }



    private void ChatMessage(ChatData chatData)
    {
        if (Server.ChatChannels.ContainsKey(chatData.ChannelId))
            Server.ChatChannels[chatData.ChannelId].SendMessage(chatData);
    }

}