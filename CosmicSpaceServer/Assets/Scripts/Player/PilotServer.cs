using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player;
using System.Data;
using UnityEngine;

public delegate void ChangePosition(Pilot pilot, Vector2 position, Vector2 targetPosition, int speed);
public delegate void ChangeHitpoints(Pilot pilot, ulong hitpoints, ulong maxHitpoints);
public delegate void ChangeShields(Pilot pilot, ulong shields, ulong maxShields);

public class PilotServer
{
    public Pilot Pilot { get; set; }

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

    #region Pozycja / Nowa pozycja / Zdarzenie na zmiane pozycji gracza
    public Vector2 Position
    {
        get => new Vector2(Pilot.PositionX, Pilot.PositionY);
        set
        {
            if (value == TargetPostion)
                return;

            Pilot.PositionX = value.x;
            Pilot.PositionY = value.y;

            OnChangePosition?.Invoke(Pilot, value, TargetPostion, Speed);
        }
    }
    public Vector2 TargetPostion;

    public event ChangePosition OnChangePosition;
    #endregion

    #region Hitpoints / Shields / Zdarzenie na zmiane hitpoints oraz shields
    public ulong Hitpoints
    {
        get => Pilot.Hitpoints;
        set
        {
            if (Pilot.Hitpoints == value)
                return;

            Pilot.Hitpoints = value;

            OnChangeHitpoints?.Invoke(Pilot, value, MaxHitpoints);
        }
    }
    public ulong MaxHitpoints => Pilot.Ship.Hitpoints;
    public bool CanRepearHitpoints => Hitpoints != MaxHitpoints;
    public event ChangeHitpoints OnChangeHitpoints;

    public ulong Shields
    {
        get => Pilot.Shields;
        set
        {
            if (Pilot.Shields == value)
                return;

            Pilot.Shields = value;

            OnChangeShields?.Invoke(Pilot, value, MaxShields);
        }
    }
    public ulong MaxShields => 0; // Z wyposazenia
    public bool CanRepearShields => Shields != MaxShields;
    public event ChangeShields OnChangeShields;

    #endregion

    #region Speed
    public int Speed => Pilot.Ship.Speed;
    #endregion



    float timer = 0;
    public void Update()
    {
        Fly();

        timer += Time.deltaTime;
        if(timer > 1)
        {
            timer = 0;

            // Last attack bool

            Repair();
        }
    }



    public void Fly()
    {
        if (TargetPostion == Position)
            return;
        Position = Vector3.MoveTowards(Position, TargetPostion, Time.deltaTime * Speed);
    }

    public void Repair()
    {
        if (CanRepearHitpoints)
        {
            var hitpoint = MaxHitpoints / 30;
            if (Hitpoints + hitpoint <= MaxHitpoints)
                Hitpoints += hitpoint;
            else
                Hitpoints = MaxHitpoints;
        }

        if (CanRepearShields)
        {
            var shield = MaxShields / 20;
            if (Shields + shield <= MaxShields)
                Shields += shield;
            else
                Shields = MaxShields;
        }
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

    public static Pilot GetPilot(DataRow row)
    {
        return new Pilot()
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
        };
    }
}