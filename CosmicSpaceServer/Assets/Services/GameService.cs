using WebSocketSharp;
using UnityEngine;
using System;
using System.Linq;
using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Account;
using CosmicSpaceCommunication.Game.Player.ClientToServer;
using CosmicSpaceCommunication.Game.Player.ServerToClient;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Resources;

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

    private async void PilotDisconnect()
    {
        PilotServer pilotServer = Server.Pilots.Values.FirstOrDefault(o => o.Headers?.SocketId == ID);

        if (pilotServer == null)
            return;

        await Database.SavePlayerData(pilotServer.Pilot);

        MainThread.Instance().Enqueue(() =>
        {
            foreach (ChatChannel chatChannel in Server.ChatChannels.Values)
            {
                chatChannel.Disconnect(pilotServer.Pilot.Id);
            }

            Server.MapsServer[pilotServer.Pilot.Map.Id].Leave(pilotServer);
            Server.Pilots.Remove(pilotServer.Pilot.Id);
        });
    }

    protected override void OnMessage(MessageEventArgs e)
    {
        if (!e.IsBinary)
        {
            Server.Log("Dane wejsciowe nie sa binarne.", e.Data);
            return;
        }

        MainThread.Instance().Enqueue(() =>
        {
            try
            {
                int errorStatus = 0;

                CommandData commandData = GameData.Deserialize(e.RawData);

                if (commandData.Command == Commands.LogIn)
                {
                    if (commandData.Data is LogInUser data)
                    {
                        LoginUser(data);
                    }
                    else
                        errorStatus = 2;
                }



                else if (commandData.Command == Commands.Register)
                {
                    if (commandData.Data is RegisterUser data)
                    {
                        RegisterUser(data);
                    }
                    else
                        errorStatus = 2;
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
                    else
                        errorStatus = 2;
                }



                else if (commandData.Command == Commands.SelectTarget)
                {
                    if (commandData.Data is NewTarget data)
                    {
                        if (!CheckPacket(data.PlayerId))
                            return;

                        PilotSelectTarget(data);
                    }
                    else
                        errorStatus = 2;
                }



                else if (commandData.Command == Commands.AttackTarget)
                {
                    if (commandData.Data is AttackTarget data)
                    {
                        if (!CheckPacket(data.PlayerId))
                            return;

                        PilotAttackTarget(data);
                    }
                    else
                        errorStatus = 2;
                }



                else if (commandData.Command == Commands.RepairShip)
                {
                    if (commandData.Data is ulong data)
                    {
                        if (!CheckPacket(data))
                            return;

                        Server.Pilots[data].IsDead = false;
                    }
                    else
                        errorStatus = 2;
                }



                else if (commandData.Command == Commands.ChangeMap)
                {
                    if (commandData.Data is PlayerChangeMap data)
                    {
                        if (!CheckPacket(data.PlayerId))
                            return;

                        Server.MapsServer[data.Portal.Map.Id].ChangeMapByPortal(Server.Pilots[data.PlayerId], data.Portal);
                    }
                    else
                        errorStatus = 2;
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
                    else
                        errorStatus = 2;
                }



                else if (commandData.Command == Commands.ChangeEquipment)
                {
                    if (commandData.Data is PilotEquipment data)
                    {
                        if (!CheckPacket(data.PilotId))
                            return;

                        if (data.Items.FirstOrDefault(o => o.RelationId == 0) != null)
                        {
                            Server.Log("Ekwipowane przedmioty nie posiadaja id relacji.", data.PilotId, data.Items.Select(o => (object)o.RelationId).ToArray());
                            return;
                        }

                        if (data.Items.Count > 0)
                            Server.Pilots[data.PilotId].ItemsChange(data.Items);
                    }
                    else
                        errorStatus = 2;
                }



                else if (commandData.Command == Commands.GetShopItems)
                {
                    if (commandData.Data is ulong data)
                    {
                        if (!CheckPacket(data))
                            return;

                        Server.Pilots[data].Send(new CommandData()
                        {
                            Command = Commands.GetShopItems,
                            Data = new ShopItems()
                            {
                                Items = Server.Items.Values.ToList(),
                                Ships = Server.Ships.Values.ToList()
                            }
                        });
                    }
                    else
                        errorStatus = 2;
                }




                else
                    errorStatus = 1;

                if (errorStatus > 0)
                    Server.Log("Nieoczekiwane dane wejsciowe.", commandData.Command, commandData.Data?.GetType());
            }
            catch (Exception ex)
            {
                Server.Log("Nieoczekiwane dane wejsciowe.", ex.Message);
            }
        });
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
        {
            Server.Log("Nie znaleziono pilota.");
            return;
        }

        pilotServer.NewPostion = new Vector2(newPosition.PositionX, newPosition.PositionY);
    }
    
    private bool? PilotSelectTarget(NewTarget newTarget)
    {
        if (!Server.Pilots.ContainsKey(newTarget.PlayerId))
        {
            Server.Log("Gracz nie istnieje.", newTarget.PlayerId);
            return false;
        }

        ulong targetId = newTarget.TargetId ?? 0;
        if (targetId == 0)
            return false;

        PilotServer attacker = Server.Pilots[newTarget.PlayerId];

        if (!Server.MapsServer.ContainsKey(attacker.Pilot.Map.Id))
        {
            Server.Log("Mapa na ktorej jest gracz nie istnieje.", attacker.Pilot.Id, attacker.Pilot.Map.Id);
            return false;
        }
        MapServer attackerMap = Server.MapsServer[attacker.Pilot.Map.Id];

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
        {
            Server.Log("Wskazany przeciwnik nie istnieje.", newTarget.TargetIsPlayer, targetId);
            return false;
        }

        attacker.Target = opponent;
        return true; //Zaznaczono
    }

    private void PilotAttackTarget(AttackTarget attackTarget)
    {
        if (PilotSelectTarget(attackTarget) == true)
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
            {
                Server.Log("Atakowany cel jest pod ochrona.", attackTarget.AttackerIsPlayer, attackTarget.PlayerId, attackTarget.TargetIsPlayer, attackTarget.TargetId, attackTarget.Attack);
                return;
            }

            attacker.Ammunition = attackTarget.SelectedAmmunition;
            attacker.Rocket = attackTarget.SelectedRocket;
            attacker.Attack = attackTarget.Attack;
        }
    }





}