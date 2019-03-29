using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChatChannel
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public List<ulong> Users { get; set; }

    private ulong MESSAGE_ID = 1000;
    public Dictionary<ulong, ChatData> Messages { get; set; }



    public ChatChannel(ulong id, string name)
    {
        Id = id;
        Name = name;
        Users = new List<ulong>();
        Messages = new Dictionary<ulong, ChatData>();
    }



    public void Connect(ulong userId)
    {
        if (Users.Contains(userId))
            return;

        Users.Add(userId);
        ConnectStatus(userId, true);
    }

    public void Disconnect(ulong userId)
    {
        if (!Users.Contains(userId))
            return;

        Users.Remove(userId);
        ConnectStatus(userId, false);
    }

    private void ConnectStatus(ulong userId, bool status)
    {
        PilotServer pilot = GetPilotById(userId);
        if (pilot == null)
            return;

        Broadcast(new ChatData()
        {
            SenderId = ulong.MaxValue,
            SenderName = "Server",
            Message = $"{pilot.Name} is {(status ? "" : "dis")}connected.."
        });
    }



    public void SendMessage(ChatData chatData)
    {
        if (!Users.Contains(chatData.SenderId) || chatData.SenderId == ulong.MaxValue)
            return;

        chatData.MessageId = MESSAGE_ID++;
        chatData.Date = DateTime.Now;
        chatData.SenderName = chatData.SenderId == ulong.MaxValue ? "Server" : GetPilotById(chatData.SenderId)?.Name;

        // Cenzura wiadomosci

        Messages.Add(chatData.MessageId, chatData);

        switch (chatData.Command)
        {
            case ChatCommands.system:
            case ChatCommands.channel:
                Broadcast(chatData);
                break;


            case ChatCommands.message:
            case ChatCommands.msg:
            case ChatCommands.m:
            case ChatCommands.pw:
                if(!string.IsNullOrEmpty(chatData.RecipientName))
                {
                    PilotServer pilot = GetPilotByName(chatData.RecipientName);
                    if (pilot == null)
                        return;

                    chatData.RecipientId = pilot.Id;
                }
                else if (chatData.RecipientId is ulong recipientId && recipientId > ulong.MinValue)
                {
                    chatData.RecipientName = GetPilotById(recipientId)?.Name;
                }

                if (chatData.RecipientId < 1)
                    return;

                foreach (ChatChannel chatChannel in Server.ChatChannels.Values)
                {
                    chatChannel.SendToPilot(chatData.SenderId, chatData);
                    chatChannel.SendToPilot((ulong)chatData.RecipientId, chatData);
                }
                break;


            case ChatCommands.users:
            case ChatCommands.u:
            case ChatCommands.online:
                chatData.Message = GetUsers();
                chatData.SenderName = "Server";
                SendToPilot(chatData.SenderId, chatData);
                break;
        }
    }



    public void Broadcast(ChatData chatData)
    {
        foreach (ulong pilotId in Users)
            SendToPilot(pilotId, chatData);
    }

    public void SendToPilot(ulong id, ChatData chatData)
    {
        PilotServer pilot = GetPilotById(id);
        if (pilot == null)
            return;

        pilot.SendChat(new CommandData()
        {
            Command = Commands.ChatMessage,
            Data = chatData
        });
    }

    private PilotServer GetPilotById(ulong id)
    {
        return Server.Pilots.ContainsKey(id) ? Server.Pilots[id] : null;
    }
    private PilotServer GetPilotByName(string name)
    {
        return Server.Pilots.Values.FirstOrDefault(o => o.Name == name);
    }

    public string GetUsers()
    {
        string users = string.Empty;
        foreach (ulong userId in Users)
        {
            if (Server.Pilots.ContainsKey(userId))
            {
                users += $" {Server.Pilots[userId].Name},";
            }
        }
        return users.Remove(users.Length - 1, 1);
    }
}