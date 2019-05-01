using System;

namespace CosmicSpaceCommunication.Game.Resources
{
    [Serializable]
    public enum ShopStatus
    {
        Error,
        NoScrap,
        NoMetal,
        WrongRequiredLevel,
        Buy,
    }
}