using WebSocketSharp;
using WebSocketSharp.Server;
using UnityEngine;
using System;
using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Account;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using System.Linq;

public class Game : WebSocketBehavior
{
    protected override void OnOpen()
    {
        //Debug.Log("OnOpen");
    }

    protected override void OnClose(CloseEventArgs e)
    {
        PilotDisconnect();
    }

    protected override void OnError(ErrorEventArgs e)
    {
        PilotDisconnect();
        Debug.LogError($"{e.Exception} {Environment.NewLine} {e.Message}");
    }

    private void PilotDisconnect()
    {
        PilotServer pilotServer = Server.Pilots.Values.FirstOrDefault(o => o.Headers.SocketId == ID);

        if (pilotServer == null)
            return;

        Server.MapsServer[pilotServer.Pilot.Map.Id].Leave(pilotServer);
        Server.Pilots.Remove(pilotServer.Pilot.Id);
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        if (!e.IsBinary)
            return;
        
        try
        {
            CommandData commandData = GameData.Deserialize(e.RawData);
            switch (commandData.Command)
            {
                case Commands.LogIn:
                    LogInUser logInUser = (LogInUser)commandData.Data;
                    LoginUser(logInUser);
                    break;

                case Commands.Register:
                    RegisterUser registerUser = (RegisterUser)commandData.Data;
                    RegisterUser(registerUser);
                    break;

                case Commands.PlayerLeave:
                    PilotDisconnect();
                    break;

                case Commands.PlayerNewPosition:
                    NewPosition newPosition = (NewPosition)commandData.Data;
                    PilotChangePosition(newPosition);
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }



    private Headers GetHeaders()
    {
        return new Headers()
        {
            SocketId = ID,
            UserAgent = Headers["User-Agent"],
            Host = Headers["Host"]
        };
    }



    private void LoginUser(LogInUser logInUser)
    {
        ulong? userId = Database.LoginUser(logInUser);
        if (userId != null)
        {
            Database.LogUser(GetHeaders(), Commands.LogIn, true, userId);

            PilotServer pilotServer = new PilotServer()
            {
                Pilot = Database.GetPilot((ulong)userId)
            };
            pilotServer.Headers = GetHeaders();
            pilotServer.TargetPostion = pilotServer.Position;

            if (Server.Pilots.ContainsKey(pilotServer.Pilot.Id))
            {
                Server.Pilots[pilotServer.Pilot.Id].Headers = pilotServer.Headers;

            }
            else
            {
                Server.Pilots.Add(pilotServer.Pilot.Id, pilotServer);
                Server.MapsServer[pilotServer.Pilot.Map.Id].Join(pilotServer);
            }
        }
        else
        {
            Database.LogUser(GetHeaders(), Commands.LogIn, false, Database.GetPilot(logInUser));
        }
    }

    private void RegisterUser(RegisterUser registerUser)
    {
        if (Database.OccupiedAccount(registerUser))
        {
            if (Database.OcuppiedNickname(registerUser.Nickname))
            {
                if (Database.RegisterUser(registerUser))
                {
                    Database.LogUser(GetHeaders(), Commands.Register, true, Database.GetPilot(registerUser));

                    LoginUser(registerUser);
                }
                else
                {
                    Database.LogUser(GetHeaders(), Commands.Register, false, Database.GetPilot(registerUser));
                }
            }
            else
            {
                PilotServer.Send(new CommandData()
                {
                    Command = Commands.NicknameOccupied
                }, GetHeaders());
            }
        }
        else
        {
            PilotServer.Send(new CommandData()
            {
                Command = Commands.AccountOccupied
            }, GetHeaders());
        }
    }
    
    private void PilotChangePosition(NewPosition newPosition)
    {
        PilotServer pilotServer = Server.Pilots.Values.FirstOrDefault(
            o => o.Headers.SocketId == GetHeaders().SocketId);

        if (pilotServer == null)
            return;

        pilotServer.TargetPostion = new Vector2(newPosition.PositionX, newPosition.PositionY);
    }

}