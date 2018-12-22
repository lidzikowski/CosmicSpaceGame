using System;

namespace CosmicSpaceCommunication
{
    [Serializable]
    public class CommandData
    {
        public Commands Command { get; set; }
        public object Data { get; set; }
    }
}