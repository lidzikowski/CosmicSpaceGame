using System;

namespace CosmicSpaceCommunication.Game.Chat
{
    [Serializable]
    public class ChatData
    {
        public ulong MessageId { get; set; }

        public ChatCommands Command { get; set; }
        public ulong ChannelId { get; set; }

        public ulong SenderId { get; set; }
        public string SenderName { get; set; }

        public ulong? RecipientId { get; set; }
        public string RecipientName { get; set; }

        public object Message { get; set; }
        public DateTime Date { get; set; }
    }
}