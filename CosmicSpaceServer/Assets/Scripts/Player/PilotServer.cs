using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player;
using System.Data;
using UnityEngine;

public delegate void ChangePosition(Pilot pilot, Vector2 position, Vector2 targetPosition);

public class PilotServer
{
    public Pilot Pilot { get; set; }
    public int Speed => Pilot.Ship.Speed;

    public Vector2 Position
    {
        get => new Vector2(Pilot.PositionX, Pilot.PositionY);
        set
        {
            if (value == TargetPostion)
                return;

            Pilot.PositionX = value.x;
            Pilot.PositionY = value.y;

            OnChangePosition?.Invoke(Pilot, value, TargetPostion);
        }
    }
    public Vector2 TargetPostion;

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

    public event ChangePosition OnChangePosition;



    public void Update()
    {
        Fly();
    }



    public void Fly()
    {
        if (TargetPostion == Position)
            return;
        Position = Vector3.MoveTowards(Position, TargetPostion, Time.deltaTime * Speed / 10);
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
        };
    }
}