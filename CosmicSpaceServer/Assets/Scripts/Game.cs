using WebSocketSharp;
using WebSocketSharp.Server;
using UnityEngine;
using System;
using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Account;
using CosmicSpaceCommunication.Game.Player;

public class Game : WebSocketBehavior
{
    protected override void OnOpen()
    {
        Debug.Log("OnOpen");
    }

    protected override void OnClose(CloseEventArgs e)
    {
        Debug.Log($"OnClose {Environment.NewLine} {e.Code} {Environment.NewLine} {e.WasClean} {Environment.NewLine} {e.Reason}");
    }

    protected override void OnError(ErrorEventArgs e)
    {
        Debug.Log($"OnError {Environment.NewLine} {e.Exception} {Environment.NewLine} {e.Message}");
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
                    LoginUser(logInUser, GetHeaders());
                    break;

                case Commands.Register:
                    RegisterUser registerUser = (RegisterUser)commandData.Data;
                    RegisterUser(registerUser, GetHeaders());
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



    private static void LoginUser(LogInUser logInUser, Headers headers)
    {
        ulong? userId = Database.LoginUser(logInUser);
        if (userId != null)
        {
            Database.LogUser(headers, Commands.LogIn, true, userId);

            PilotServer pilotServer = new PilotServer()
            {
                Pilot = Database.GetPilot((ulong)userId)
            };
            pilotServer.Headers = headers;

            if (Server.Pilots.ContainsKey(pilotServer.Pilot.Id))
                Server.Pilots[pilotServer.Pilot.Id].Headers = pilotServer.Headers;
            else
                Server.Pilots.Add(pilotServer.Pilot.Id, pilotServer);
        }
        else
        {
            Database.LogUser(headers, Commands.LogIn, false, Database.GetPilot(logInUser));
        }
    }

    private static void RegisterUser(RegisterUser registerUser, Headers headers)
    {
        if (Database.OccupiedAccount(registerUser))
        {
            if (Database.OcuppiedNickname(registerUser.Nickname))
            {
                if (Database.RegisterUser(registerUser))
                {
                    Database.LogUser(headers, Commands.Register, true, Database.GetPilot(registerUser));

                    LoginUser(registerUser, headers);
                }
                else
                {
                    Database.LogUser(headers, Commands.Register, false, Database.GetPilot(registerUser));
                }
            }
            else
            {
                PilotServer.Send(new CommandData()
                {
                    Command = Commands.NicknameOccupied
                }, headers);
            }
        }
        else
        {
            PilotServer.Send(new CommandData()
            {
                Command = Commands.AccountOccupied
            }, headers);
        }
    }


}