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
using CosmicSpaceCommunication.Game.Quest;
using System.Collections.Generic;

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

                if (commandData.Command != Commands.LogIn && commandData.Command != Commands.Register && commandData.Command != Commands.PlayerLeave)
                {
                    if (!CheckPacket(commandData.SenderId))
                        return;
                }



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
                        if (data.PlayerId != commandData.SenderId)
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
                        if (data.PlayerId != commandData.SenderId)
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
                        if (data.PlayerId != commandData.SenderId)
                            return;

                        PilotAttackTarget(data);
                    }
                    else
                        errorStatus = 2;
                }



                else if (commandData.Command == Commands.RepairShip)
                {
                    Server.Pilots[commandData.SenderId].IsDead = false;
                }



                else if (commandData.Command == Commands.ChangeMap)
                {
                    if (commandData.Data is PlayerChangeMap data)
                    {
                        Server.MapsServer[data.Portal.Map.Id].ChangeMapByPortal(Server.Pilots[commandData.SenderId], data.Portal);
                    }
                    else
                        errorStatus = 2;
                }



                else if (commandData.Command == Commands.GetEquipment)
                {
                    Server.Pilots[commandData.SenderId].Send(new CommandData()
                    {
                        Command = Commands.GetEquipment,
                        Data = new Pilot()
                        {
                            Items = Server.Pilots[commandData.SenderId].Pilot.Items.Where(o => !o.IsSold).ToList(),
                            Ship = Server.Pilots[commandData.SenderId].Pilot.Ship
                        }
                    });
                }



                else if (commandData.Command == Commands.ChangeEquipment)
                {
                    if (commandData.Data is PilotEquipment data)
                    {
                        if (data.Items.FirstOrDefault(o => o.RelationId == 0) != null)
                        {
                            Server.Log("Ekwipowane przedmioty nie posiadaja id relacji.", commandData.SenderId, data.Items.Select(o => (object)o.RelationId).ToArray());
                            return;
                        }

                        if (data.Items.Count > 0)
                            Server.Pilots[commandData.SenderId].ItemsChange(data.Items);
                    }
                    else
                        errorStatus = 2;
                }



                else if (commandData.Command == Commands.GetShopItems)
                {
                    Server.Pilots[commandData.SenderId].Send(new CommandData()
                    {
                        Command = Commands.GetShopItems,
                        Data = new ShopItems()
                        {
                            Items = Server.Items.Values.ToList(),
                            Ships = Server.Ships.Values.ToList(),
                        }
                    });
                }



                else if (commandData.Command == Commands.BuyShopItem)
                {
                    if (commandData.Data is BuyShopItem data)
                    {
                        ShoppingStatus status = Server.Pilots[commandData.SenderId].BuyItem(data);

                        Server.Pilots[commandData.SenderId].Send(new CommandData()
                        {
                            Command = Commands.BuyShopItem,
                            Data = status
                        });
                    }
                    else
                        errorStatus = 2;
                }



                else if (commandData.Command == Commands.SellEquipmentItem)
                {
                    if (commandData.Data is ItemPilot data)
                    {
                        bool status = Server.Pilots[commandData.SenderId].SellItem(data);

                        Server.Pilots[commandData.SenderId].Send(new CommandData()
                        {
                            Command = Commands.SellEquipmentItem,
                            Data = status ? data.RelationId : (ulong?)null
                        });
                    }
                    else
                        errorStatus = 2;
                }



                else if (commandData.Command == Commands.ChangeAmmunition)
                {
                    if (commandData.Data is ChangeAmmunition data)
                    {
                        Server.Pilots[commandData.SenderId].Ammunition = data.SelectedAmmunitionId;
                        Server.Pilots[commandData.SenderId].Rocket = data.SelectedRocketId;
                    }
                    else
                        errorStatus = 2;
                }



                else if (commandData.Command == Commands.GetAllMaps)
                {
                    Server.Pilots[commandData.SenderId].Send(new CommandData()
                    {
                        Command = Commands.GetAllMaps,
                        Data = Server.Maps
                    });
                }



                else if (commandData.Command == Commands.QuestList)
                {
                    Server.Pilots[commandData.SenderId].Send(new CommandData()
                    {
                        Command = Commands.QuestList,
                        Data = Server.Tasks
                    });
                }



                else if (commandData.Command == Commands.GetProgressTasks)
                {
                    Server.Pilots[commandData.SenderId].Send(new CommandData()
                    {
                        Command = Commands.GetProgressTasks,
                        Data = Database.GetPilotProgressTasks(commandData.SenderId).Result
                    });
                }



                else if (commandData.Command == Commands.QuestAccept)
                {
                    if (commandData.Data is QuestTask data)
                    {
                        PilotTask pilotTask = Server.Pilots[commandData.SenderId].Pilot.Tasks.FirstOrDefault(o => o.Task.Id == data.Id);

                        List<PilotProgressTask> pilotProgressTasks = Database.GetPilotProgressTasks(commandData.SenderId).Result;

                        if (pilotTask == null)
                        {
                            if (pilotProgressTasks.Any(o => o.Id == data.Id && !o.End.HasValue))
                            {
                                PilotProgressTask pilotProgressTask;

                                ulong? pilotTaskId = Database.CheckPilotTask(data, commandData.SenderId).Result;
                                if (pilotTaskId != null)
                                {
                                    PilotTask pt = Database.UpdatePilotTask(new PilotTask()
                                    {
                                        Id = (uint)pilotTaskId,
                                        Start = DateTime.Now,
                                        Task = new QuestTask()
                                        {
                                            Id = data.Id
                                        }
                                    }).Result;

                                    pilotProgressTask = new PilotProgressTask()
                                    {
                                        Id = pt.Task.Id,
                                        Start = pt.Start
                                    };
                                }
                                else
                                {
                                    pilotProgressTask = Database.AddPilotTask(data, commandData.SenderId).Result;
                                }

                                Server.Pilots[commandData.SenderId].Pilot.Tasks = Database.GetPilotTasks(commandData.SenderId).Result ?? new List<PilotTask>();

                                Server.Pilots[commandData.SenderId].Send(new CommandData()
                                {
                                    Command = Commands.QuestList,
                                    Data = Server.Pilots[commandData.SenderId].Pilot.Tasks
                                });

                                if (pilotProgressTask != null)
                                {
                                    pilotProgressTasks.Add(pilotProgressTask);
                                }
                                else
                                {
                                    Server.Log("Blad podczas akceptacji zadania.", commandData.SenderId, data.Id);
                                }
                            }
                            else
                            {
                                Server.Log("Zadanie zostalo juz wykonane.", commandData.SenderId, data.Id);
                            }
                        }
                        else
                        {
                            Server.Log("Zadanie jest w trakcie wykonywania.", commandData.SenderId, pilotTask.Id);
                        }

                        Server.Pilots[commandData.SenderId].Send(new CommandData()
                        {
                            Command = Commands.GetProgressTasks,
                            Data = pilotProgressTasks
                        });
                    }
                    else
                        errorStatus = 2;
                }



                else if (commandData.Command == Commands.QuestCancel)
                {
                    if (commandData.Data is QuestTask data)
                    {
                        PilotTask pilotTask = Server.Pilots[commandData.SenderId].Pilot.Tasks.FirstOrDefault(o => o.Task.Id == data.Id);

                        List<PilotProgressTask> pilotProgressTasks = Database.GetPilotProgressTasks(commandData.SenderId).Result;

                        if (pilotTask != null)
                        {
                            PilotProgressTask pilotProgressTask = pilotProgressTasks.FirstOrDefault(o => o.Id == data.Id && !o.End.HasValue);

                            if (pilotProgressTask != null)
                            {
                                if (Database.CancelPilotTask(data, commandData.SenderId).Result)
                                {
                                    foreach (PilotTaskQuest pilotTaskQuest in pilotTask.TaskQuest)
                                    {
                                        pilotTaskQuest.Progress = default;
                                        pilotTaskQuest.IsDone = false;
                                        Database.UpdatePilotTaskQuest(pilotTaskQuest).ConfigureAwait(false);
                                    }

                                    pilotProgressTask.Start = default;

                                    Server.Pilots[commandData.SenderId].Pilot.Tasks = Database.GetPilotTasks(commandData.SenderId).Result ?? new List<PilotTask>();

                                    Server.Pilots[commandData.SenderId].Send(new CommandData()
                                    {
                                        Command = Commands.QuestList,
                                        Data = Server.Pilots[commandData.SenderId].Pilot.Tasks
                                    });
                                }
                                else
                                {
                                    Server.Log("Blad podczas anulowania zadania.", commandData.SenderId, data.Id);
                                }
                            }
                            else
                            {
                                Server.Log("Zadanie zostalo juz wykonane.", commandData.SenderId, data.Id);
                            }
                        }
                        else
                        {
                            Server.Log("Zadanie nie jest wykonywane.", commandData.SenderId, data.Id);
                        }

                        Server.Pilots[commandData.SenderId].Send(new CommandData()
                        {
                            Command = Commands.GetProgressTasks,
                            Data = pilotProgressTasks
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
                Server.Log("Nieoczekiwane dane wejsciowe.", ex.Message, ex.InnerException);
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