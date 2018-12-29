﻿using WebSocketSharp;
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

        Database.SavePlayerData(pilotServer.Pilot);

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

                case Commands.NewPosition:
                    NewPosition newPosition = (NewPosition)commandData.Data;

                    if (newPosition == null)
                        return;

                    if (newPosition.IsPlayer)
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



    private async void LoginUser(LogInUser logInUser)
    {
        ulong? userId = await Database.LoginUser(logInUser);
        if (userId != null)
        {
            Database.LogUser(GetHeaders(), Commands.LogIn, true, userId);

            PilotServer pilotServer = new PilotServer()
            {
                Pilot = await Database.GetPilot((ulong)userId)
            };
            pilotServer.TargetPostion = pilotServer.Position;
            pilotServer.Headers = GetHeaders();

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
            Database.LogUser(GetHeaders(), Commands.LogIn, false, await Database.GetPilot(logInUser));
        }
    }

    private async void RegisterUser(RegisterUser registerUser)
    {
        if (await Database.OccupiedAccount(registerUser))
        {
            if (await Database.OcuppiedNickname(registerUser.Nickname))
            {
                if (await Database.RegisterUser(registerUser))
                {
                    Database.LogUser(GetHeaders(), Commands.Register, true, await Database.GetPilot(registerUser));

                    LoginUser(registerUser);
                }
                else
                {
                    Database.LogUser(GetHeaders(), Commands.Register, false, await Database.GetPilot(registerUser));
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