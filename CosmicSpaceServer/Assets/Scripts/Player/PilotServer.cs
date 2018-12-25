using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Player;
using System.Data;
using UnityEngine;

public partial class PilotServer
{
    public Pilot Pilot { get; set; }
    public Vector2 Postion
    {
        get => new Vector2(Pilot.PositionX, Pilot.PositionY);
        set
        {
            Pilot.PositionX = value.x;
            Pilot.PositionY = value.y;
        }
    }

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