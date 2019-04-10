﻿using WebSocketSharp;
using UnityEngine;
using System;
using System.Linq;
using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Account;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;

public class GameService : WebSocket
{
    protected override void OnClose(CloseEventArgs e)
    {
        PilotDisconnect();
    }

    protected override void OnError(ErrorEventArgs e)
    {
        PilotDisconnect();
        base.OnError(e);
    }

    private void PilotDisconnect()
    {
        PilotServer pilotServer = Server.Pilots.Values.FirstOrDefault(o => o.Headers.SocketId == ID);

        if (pilotServer == null)
            return;

        Database.SavePlayerData(pilotServer.Pilot);

        Server.MapsServer[pilotServer.Pilot.Map.Id].Leave(pilotServer);
        Server.Pilots.Remove(pilotServer.Pilot.Id);

        foreach (ChatChannel chatChannel in Server.ChatChannels.Values)
        {
            chatChannel.Disconnect(pilotServer.Pilot.Id);
        }
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

                    if (!CheckPacket(newPosition.PlayerId))
                        return;

                    if (newPosition.IsPlayer)
                        PilotChangePosition(newPosition);
                    break;


                case Commands.SelectTarget:
                    NewTarget newTarget = (NewTarget)commandData.Data;

                    if (newTarget == null)
                        return;

                    if (!CheckPacket(newTarget.PlayerId))
                        return;

                    PilotSelectTarget(newTarget);
                    break;


                case Commands.AttackTarget:
                    AttackTarget attackTarget = (AttackTarget)commandData.Data;

                    if (attackTarget == null)
                        return;

                    if (!CheckPacket(attackTarget.PlayerId))
                        return;

                    PilotAttackTarget(attackTarget);
                    break;


                case Commands.RepairShip:
                    ulong userId;
                    if (!ulong.TryParse(commandData.Data.ToString(), out userId))
                        return;

                    if (!CheckPacket(userId))
                        return;

                    Server.Pilots[userId].IsDead = false;
                    break;




            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    private async void LoginUser(LogInUser logInUser)
    {
        ulong? userId = await Database.LoginUser(logInUser);
        if (userId != null)
        {
            Database.LogUser(GetHeaders(), Commands.LogIn, true, userId);

            PilotServer pilotServer = new PilotServer(await Database.GetPilot((ulong)userId));

            if (Server.Pilots.ContainsKey(pilotServer.Pilot.Id))
            {
                Server.Pilots[pilotServer.Pilot.Id].Headers = pilotServer.Headers;
            }
            else
            {
                Server.Pilots.Add(pilotServer.Pilot.Id, pilotServer);
                Server.Pilots[pilotServer.Pilot.Id].Headers = GetHeaders();
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

        pilotServer.NewPostion = new Vector2(newPosition.PositionX, newPosition.PositionY);
    }
    
    private bool? PilotSelectTarget(NewTarget newTarget)
    {
        if (!Server.Pilots.ContainsKey(newTarget.PlayerId))
            return false;
        PilotServer attacker = Server.Pilots[newTarget.PlayerId];

        if (!Server.MapsServer.ContainsKey(attacker.Pilot.Map.Id))
            return false;
        MapServer attackerMap = Server.MapsServer[attacker.Pilot.Map.Id];

        ulong targetId = newTarget.TargetId ?? 0;
        if (targetId == 0)
            return false;

        Opponent opponent = null;
        if (newTarget.TargetIsPlayer == true) // Pilot
        {
            opponent = attackerMap.PilotsOnMap.FirstOrDefault(o => o.Id == targetId);
        }
        else if (newTarget.TargetIsPlayer == false) // Enemy
        {
            opponent = attackerMap.EnemiesOnMap.FirstOrDefault(o => o.Id == targetId);
        }

        if (opponent == null)
            return false;

        attacker.Target = opponent;
        return true; //Zaznaczono
    }

    private void PilotAttackTarget(AttackTarget attackTarget)
    {
        if(PilotSelectTarget(attackTarget) == true)
        {
            PilotServer attacker = Server.Pilots[attackTarget.PlayerId];

            attacker.Ammunition = attackTarget.SelectedAmmunition;
            attacker.Rocket = attackTarget.SelectedRocket;

            if (attacker.Target == null)
                return;

            attacker.Attack = attackTarget.Attack;
        }
    }





}