using System;

namespace CosmicSpaceCommunication
{
    [Serializable]
    public enum Commands
    {
        LogIn = 100,

        Register = 110,
        AccountOccupied = 111,
        NicknameOccupied = 112,

        UserData = 200,

        PlayerJoin = 300,
        PlayerLeave = 301,
        
        PlayerNewPosition = 310,
        PlayerChangeHitpoints = 311,
        PlayerChangeShields = 312,


    }
}