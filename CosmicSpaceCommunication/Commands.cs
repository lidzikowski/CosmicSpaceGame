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
        
        NewPosition = 310,
        ChangeHitpoints = 311,
        ChangeShields = 312,

        SelectTarget = 320,
        AttackTarget = 321,

        GetDamage = 330,
        Dead = 331,
        RepairShip = 332,

        EnemyJoin = 340,
        EnemyLeave = 341,

        NewReward = 350,

        ChangeMap = 360,
        SafeZone = 361,

        GetEquipment = 380,
        ChangeEquipment = 381,


        ChatMessage = 1000,
        ChatConnected = 1001,
        ChatDisconnected = 1002,
        ChatUserNotFound = 1003,



    }
}