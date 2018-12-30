using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnityEngine;


public class PilotServer : Opponent
{
    public Pilot Pilot { get; set; }

    public override ulong Id => Pilot.Id;
    public override string Name => Pilot.Nickname;
    protected override bool isDead
    {
        get => Pilot.IsDead;
        set => Pilot.IsDead = value;
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
        if (headers == null)
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
        Pilot pilot = new Pilot()
        {
            Id = Database.Row<ulong>(row["userid"]),
            Nickname = Database.Row<string>(row["nickname"]),
            Map = Server.Maps[Database.Row<int>(row["mapid"])],
            PositionX = Database.Row<float>(row["positionx"]),
            PositionY = Database.Row<float>(row["positiony"]),
            Ship = Server.Ships[Database.Row<int>(row["shipid"])],
            Experience = Database.Row<ulong>(row["experience"]),
            Level = Database.Row<int>(row["level"]),
            Scrap = Database.Row<double>(row["scrap"]),
            Metal = Database.Row<double>(row["metal"]),
            Hitpoints = Database.Row<ulong>(row["hitpoints"]),
            Shields = Database.Row<ulong>(row["shields"]),
            IsDead = Database.Row<bool>(row["isdead"]),
        };
        
        PilotResources resources = await Database.GetPilotResources(pilot.Id);

        pilot.Ammunitions = resources.Ammunitions;
        pilot.Rockets = resources.Rockets;
        
        return pilot;
    }
}