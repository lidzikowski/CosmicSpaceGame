using CosmicSpaceCommunication;
using System.Data;
using UnityEngine;

public partial class Pilot : CosmicSpaceCommunication.Game.Player.Pilot
{
    public Vector2 Postion
    {
        get => new Vector2(PositionX, PositionY);
        set
        {
            PositionX = value.x;
            PositionY = value.y;
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
            // zdarzenie logowania na mape gry / wylogowanie
            // Odpowiedz do klienta = logowanie na mape
        }
    }
    


    public void Send(CommandData commandData)
    {
        if (headers == null)
            return;

        try
        {
            Server.WebSocket.WebSocketServices["/Game"].Sessions.SendTo(GameData.Serialize(commandData), headers.SocketId);
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
            Experience = Database.Row<ulong>(row["experience"]),
            Level = Database.Row<int>(row["level"]),
            Scrap = Database.Row<double>(row["scrap"]),
            Metal = Database.Row<double>(row["metal"])
        };
    }
}