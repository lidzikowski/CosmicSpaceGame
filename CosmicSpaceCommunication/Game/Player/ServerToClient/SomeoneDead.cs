using System;
using System.Collections.Generic;

namespace CosmicSpaceCommunication.Game.Player.ServerToClient
{
    [Serializable]
    public class SomeoneDead
    {
        public ulong WhoId { get; set; }
        public bool WhoIsPlayer { get; set; }

        public List<Killer> Killers { get; set; }

        public string KillersToString
        {
            get
            {
                string by = string.Empty;
                for (int i = 0; i < Killers.Count; i++)
                {
                    by += Killers[i].Name;
                    if (i < Killers.Count - 1)
                        by += ", ";
                }
                return by;
            }
        }
    }
}