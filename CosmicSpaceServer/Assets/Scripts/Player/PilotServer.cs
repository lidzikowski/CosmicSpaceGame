using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnityEngine;


public class PilotServer : Opponent
{
    public Pilot Pilot { get; set; }

    public override ulong Id => Pilot.Id;
    public override string Name => Pilot.Nickname;
    public override bool IsDead
    {
        get => Pilot.IsDead;
        set => Pilot.IsDead = true;
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
    public override Vector2 Position
    {
        get => new Vector2(Pilot.PositionX, Pilot.PositionY);
        set
        {
            if (value == TargetPostion)
                return;

            Pilot.PositionX = value.x;
            Pilot.PositionY = value.y;

            OnChangePosition?.Invoke(this, value, TargetPostion, Speed);
        }
    }
    public override event ChangePosition OnChangePosition;
    #endregion

    #region Hitpoints / MaxHitpoints
    public override ulong Hitpoints
    {
        get => Pilot.Hitpoints;
        set
        {
            if (Pilot.Hitpoints == value)
                return;

            Pilot.Hitpoints = value;

            OnChangeHitpoints?.Invoke(this, value, MaxHitpoints);
        }
    }
    public override ulong MaxHitpoints => Pilot.Ship.Hitpoints; // + dodatki
    public override event ChangeHitpoints OnChangeHitpoints;
    #endregion

    #region Shields / MaxShields
    public override ulong Shields
    {
        get => Pilot.Shields;
        set
        {
            if (Pilot.Shields == value)
                return;

            Pilot.Shields = value;

            OnChangeShields?.Invoke(this, value, MaxShields);
        }
    }
    public override ulong MaxShields => 0; // Z wyposazenia + dodatki
    public override event ChangeShields OnChangeShields;
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



    public override void Update()
    {
        if (IsDead)
            return;

        base.Update();
    }



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