using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnityEngine;


public class PilotServer : Opponent
{
    public Pilot Pilot { get; set; }

    public override ulong Id
    {
        get => Pilot.Id;
        protected set => Pilot.Id = value;
    }
    public override string Name => Pilot.Nickname;
    protected override bool isDead
    {
        get => Pilot.IsDead;
        set
        {
            if (Pilot.IsDead == value)
                return;

            Pilot.IsDead = value;

            if(value)
                Pilot.KillerBy = DeadOpponent.Name;
            else
                Pilot.KillerBy = string.Empty;
        }
    }



    #region Socket gracza / Wyslanie danych przy inicjalizacji
    private Headers headers;
    public Headers Headers
    {
        get => headers;
        set
        {
            if (headers == value)
                return;

            headers = value;

            if (value == null)
                return;

            Send(new CommandData()
            {
                Command = Commands.UserData,
                Data = Pilot
            });
        }
    }
    #endregion

    #region Position
    protected override Vector2 position
    {
        get => new Vector2(Pilot.PositionX, Pilot.PositionY);
        set
        {
            Pilot.PositionX = value.x;
            Pilot.PositionY = value.y;
        }
    }
    #endregion

    #region Hitpoints / MaxHitpoints
    protected override ulong hitpoints
    {
        get => Pilot.Hitpoints;
        set => Pilot.Hitpoints = value;
    }
    public override ulong MaxHitpoints => Pilot.Ship.Hitpoints; // + dodatki
    #endregion

    #region Shields / MaxShields
    protected override ulong shields
    {
        get => Pilot.Shields;
        set => Pilot.Shields = value;
    }
    public override ulong MaxShields => 100; // Z wyposazenia + dodatki
    #endregion

    #region Speed
    public override int Speed => Pilot.Ship.Speed; // + wyposazenie + dodatki
    #endregion

    #region Damage
    public override ulong Damage
    {
        get => 10000;
    }
    #endregion



    public void Send(CommandData commandData)
    {
        if (Headers == null)
            return;

        try
        {
            Server.WebSocket.WebSocketServices["/Game"].Sessions.SendTo(GameData.Serialize(commandData), Headers.SocketId);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }
    public static void Send(CommandData commandData, Headers headers)
    {
        try
        {
            Server.WebSocket.WebSocketServices["/Game"].Sessions.SendTo(GameData.Serialize(commandData), headers.SocketId);
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public static async Task<Pilot> GetPilot(DataRow row)
    {
        Pilot pilot = Pilot.GetPilot(row);

        pilot.Map = Server.Maps[ConvertRow.Row<int>(row["mapid"])];
        pilot.Ship = Server.Ships[ConvertRow.Row<int>(row["shipid"])];
        
        PilotResources resources = await Database.GetPilotResources(pilot.Id);

        pilot.Ammunitions = resources.Ammunitions;
        pilot.Rockets = resources.Rockets;
        
        return pilot;
    }
}