using System;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public enum RewardReasons
    {
        KillPlayer = 2,
        KillEnemy = 4,

        BuyItem,
        SellItem,
    }
}