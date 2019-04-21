using WebSocketSharp;
using UnityEngine;
using System;
using System.Linq;
using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Account;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Player;

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
            MainThread.Instance().Enqueue(() =>
            {
                CommandData commandData = GameData.Deserialize(e.RawData);

                if(commandData.Command == Commands.LogIn)
                {
                    if (commandData.Data is LogInUser data)
                    {
                        LoginUser(data);
                    }
                }



                else if (commandData.Command == Commands.Register)
                {
                    if (commandData.Data is RegisterUser data)
                    {
                        RegisterUser(data);
                    }
                }



                else if (commandData.Command == Commands.PlayerLeave)
                {
                    PilotDisconnect();
                }



                else if (commandData.Command == Commands.NewPosition)
                {
                    if (commandData.Data is NewPosition data)
                    {
                        if (!CheckPacket(data.PlayerId))
                            return;

                        if (data.IsPlayer)
                            PilotChangePosition(data);
                    }
                }



                else if (commandData.Command == Commands.SelectTarget)
                {
                    if (commandData.Data is NewTarget data)
                    {
                        if (!CheckPacket(data.PlayerId))
                            return;

                        PilotSelectTarget(data);
                    }
                }



                else if (commandData.Command == Commands.AttackTarget)
                {
                    if (commandData.Data is AttackTarget data)
                    {
                        if (!CheckPacket(data.PlayerId))
                            return;

                        PilotAttackTarget(data);
                    }
                }



                else if (commandData.Command == Commands.RepairShip)
                {
                    if (commandData.Data is ulong data)
                    {
                        if (!CheckPacket(data))
                            return;

                        Server.Pilots[data].IsDead = false;
                    }
                }



                else if (commandData.Command == Commands.ChangeMap)
                {
                    if (commandData.Data is PlayerChangeMap data)
                    {
                        if (!CheckPacket(data.PlayerId))
                            return;

                        Server.MapsServer[data.Portal.Map.Id].ChangeMapByPortal(Server.Pilots[data.PlayerId], data.Portal);
                    }
                }



                else if (commandData.Command == Commands.GetEquipment)
                {
                    if (commandData.Data is ulong data)
                    {
                        if (!CheckPacket(data))
                            return;

                        Server.Pilots[data].Send(new CommandData()
                        {
                            Command = Commands.GetEquipment,
                            Data = new Pilot()
                            {
                                Items = Server.Pilots[data].Pilot.Items,
                                Ship = Server.Pilots[data].Pilot.Ship
                            }
                        });
                    }
                }




            });
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

            MapServer attackerMap = Server.MapsServer[attacker.Pilot.Map.Id];

            Opponent opponent = null;
            if (attackTarget.TargetIsPlayer == true) // Pilot
            {
                opponent = attackerMap.PilotsOnMap.FirstOrDefault(o => o.Id == attackTarget.TargetId);
            }
            else if (attackTarget.TargetIsPlayer == false) // Enemy
            {
                opponent = attackerMap.EnemiesOnMap.FirstOrDefault(o => o.Id == attackTarget.TargetId);
            }

            if (attackTarget.Attack && opponent.IsCover)
                return;

            attacker.Ammunition = attackTarget.SelectedAmmunition;
            attacker.Rocket = attackTarget.SelectedRocket;
            attacker.Attack = attackTarget.Attack;
        }
    }





}