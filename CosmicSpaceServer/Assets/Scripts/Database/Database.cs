using CosmicSpaceCommunication.Account;
using CosmicSpaceCommunication.Game;
using CosmicSpaceCommunication.Game.Enemy;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Quest;
using CosmicSpaceCommunication.Game.Resources;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

public class Database
{
    #region ConnectionString
    private static readonly string connectionString = new MySqlConnectionStringBuilder()
    {
        Server = "127.0.0.1",
        Port = 3306,
        UserID = "root",

        #region PUBLIKACJA
        Password = "jr6DPj9Wp3",
        #endregion

        #region DEBUG

        #endregion

        Database = "cosmicspace",
        SslMode = MySqlSslMode.None
    }.ToString();
    #endregion


    /// <summary>
    /// Procedury skladowane w bazie MySQL
    /// </summary>
    public enum Commands
    {
        getmaps,
        getships,
        getammunitions,
        getrockets,
        getenemies,
        getenemymap,
        getportals,
        getrewarditems,
        getquests,
        getquestmaps,
        gettasks,
        gettaskquests,

        occupiedaccount,
        ocuppiednickname,
        registeruser,
        loginuser,
        loguser,

        saveplayerdata,
        saveplayeritems,
        saveplayeritem,

        checkpilottask,
        updatepilottaskquest,
        addpilottaskquest,

        updatepilottask,
        addpilottask,

        cancelpilottask,

        pilottasks,

        getplayerid,
        getplayerdata,
        getpilotresources,
        getpilottasks,
        getpilotprogresstasks,
        getitems,
        getpilotitems,
        getpilottaskquests,
    }

    #region Baza danych i obsluga danych
    private static async Task<DataTable> ExecuteCommand(Commands command, Dictionary<string, object> parameters)
    {
        try
        {
            using (MySqlConnection Connection = new MySqlConnection(connectionString))
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand(command.ToString(), Connection))
                    {
                        try
                        {
                            cmd.CommandType = CommandType.StoredProcedure;

                            foreach (KeyValuePair<string, object> parameter in parameters)
                            {
                                cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                            }

                            await Connection.OpenAsync();

                            using (MySqlDataReader reader = cmd.ExecuteReader())
                            {
                                try
                                {
                                    DataTable dataTable = new DataTable("Result");
                                    dataTable.Load(reader);
                                    return dataTable;
                                }
                                catch (Exception ex)
                                {
                                    Server.Log("Blad odczytu danych.", ex.Message);
                                }
                                finally
                                {
                                    reader.Dispose();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Server.Log("Blad komendy.", ex.Message);
                        }
                        finally
                        {
                            cmd.Dispose();
                        }
                    }
                }
                catch(Exception ex)
                {
                    Server.Log("Blad polaczenia.", ex.Message);
                }
                finally
                {
                    Connection.Dispose();
                }
            }
        }
        catch (MySqlException ex)
        {
            Server.Log("Blad bazy danych.", ex.Message);
        }

        return null;
    }

    /// <summary>
    /// Logi do bazy danych z operacji logowania / rejestracji
    /// </summary>
    public static async void LogUser(Headers headers, CosmicSpaceCommunication.Commands command, bool result, ulong? userid)
    {
        await ExecuteCommand(Commands.loguser, new Dictionary<string, object>()
        {
            { "inaction", command.ToString() },
            { "inresult", result },
            { "inuseragent", headers.UserAgent },
            { "inhost", headers.Host },
            { "inuserid", userid == null ? DBNull.Value : (object)userid }
        });
        PilotServer.Send(new CosmicSpaceCommunication.CommandData()
        {
            Command = command,
            Data = result
        }, headers);
    }
    #endregion



    #region Pobieranie zasobow gry z serwera

    /// <summary>
    /// Pobranie wszystkich map z bazy danych
    /// </summary>
    public static async Task<Dictionary<long, Map>> GetMaps()
    {
        DataTable dt = await ExecuteCommand(Commands.getmaps, new Dictionary<string, object>());

        if (dt != null)
        {
            Dictionary<long, Map> maps = new Dictionary<long, Map>();
            foreach (DataRow row in dt.Rows)
            {
                maps.Add(ConvertRow.Row<int>(row["mapid"]), Map.GetMap(row));
            }
            return maps;
        }
        return null;
    }

    /// <summary>
    /// Pobranie wszystkich statkow z bazy danych
    /// </summary>
    public static async Task<Dictionary<long, Ship>> GetShips()
    {
        DataTable dt = await ExecuteCommand(Commands.getships, new Dictionary<string, object>());

        if (dt != null)
        {
            Dictionary<long, Ship> ships = new Dictionary<long, Ship>();
            foreach (DataRow row in dt.Rows)
            {
                Ship ship = Ship.GetShip(row);
                ship.Reward.Items = await GetRewardItems(ship.Reward.Id);
                ships.Add(ship.Id, ship);
            }
            return ships;
        }
        return null;
    }

    /// <summary>
    /// Pobranie wszystkiej amunicji z bazy danych
    /// </summary>
    public static async Task<Dictionary<long, Ammunition>> GetAmmunitions()
    {
        DataTable dt = await ExecuteCommand(Commands.getammunitions, new Dictionary<string, object>());

        if (dt != null)
        {
            Dictionary<long, Ammunition> ammunitions = new Dictionary<long, Ammunition>();
            foreach (DataRow row in dt.Rows)
            {
                ammunitions.Add(ConvertRow.Row<long>(row["ammunitionid"]), Ammunition.GetAmmunition(row));
            }
            return ammunitions;
        }
        return null;
    }

    /// <summary>
    /// Pobranie wszystkich rakiet z bazy danych
    /// </summary>
    public static async Task<Dictionary<long, Rocket>> GetRockets()
    {
        DataTable dt = await ExecuteCommand(Commands.getrockets, new Dictionary<string, object>());

        if (dt != null)
        {
            Dictionary<long, Rocket> rockets = new Dictionary<long, Rocket>();
            foreach (DataRow row in dt.Rows)
            {
                rockets.Add(ConvertRow.Row<long>(row["rocketid"]), Rocket.GetRocket(row));

            }
            return rockets;
        }
        return null;
    }

    public static async Task<Dictionary<long, PilotResource>> GetPilotResources(ulong userId)
    {
        DataTable dt = await ExecuteCommand(Commands.getpilotresources, new Dictionary<string, object>()
        {
            { "inuserId", userId }
        });

        if (dt != null && dt.Rows.Count != 0)
        {
            List<PilotResource> resources = PilotResource.GetPilotResource(dt.Rows[0]);
            Dictionary<long, PilotResource> resorcesList = new Dictionary<long, PilotResource>();
            foreach (PilotResource item in resources)
            {
                resorcesList.Add(item.AmmunitionId, item);
            }
            return resorcesList;
        }
        return null;
    }

    public static async Task<List<PilotTask>> GetPilotTasks(ulong userId)
    {
        DataTable dt = await ExecuteCommand(Commands.getpilottasks, new Dictionary<string, object>()
        {
            { "inuserid", userId }
        });

        if (dt != null && dt.Rows.Count != 0)
        {
            List<PilotTask> pilotTasks = new List<PilotTask>();
            foreach (DataRow row in dt.Rows)
            {
                PilotTask pilotTask = PilotTask.GetPilotTask(row);
                pilotTask.Task = Server.Tasks[ConvertRow.Row<uint>(row["taskid"])];
                pilotTask.TaskQuest = await GetPilotTaskQuests(pilotTask.Id, pilotTask.Task);
                pilotTasks.Add(pilotTask);
            }
            return pilotTasks;
        }
        return null;
    }

    public static async Task<List<PilotProgressTask>> GetPilotProgressTasks(ulong userId)
    {
        DataTable dt = await ExecuteCommand(Commands.getpilotprogresstasks, new Dictionary<string, object>()
        {
            { "inpilotid", userId }
        });

        if (dt != null && dt.Rows.Count != 0)
        {
            List<PilotProgressTask> pilotProgressTasks = new List<PilotProgressTask>();
            foreach (DataRow row in dt.Rows)
            {
                pilotProgressTasks.Add(PilotProgressTask.GetPilotProgressTask(row));
            }
            return pilotProgressTasks;
        }
        return null;
    }

    public static async Task<PilotProgressTask> AddPilotTask(QuestTask questTask, ulong userId)
    {
        DateTime dateTime = DateTime.Now;

        DataTable dt = await ExecuteCommand(Commands.addpilottask, new Dictionary<string, object>()
        {
            { "inpilotid", userId },
            { "intaskid", questTask.Id },
            { "instartdate", dateTime }
        });

        if (dt != null && dt.Rows.Count != 0)
        {
            foreach (Quest quest in Server.Tasks[questTask.Id].Quests)
            {
                await AddPilotTaskQuest(new PilotTaskQuest()
                {
                    TaskId = ConvertRow.Row<uint>(dt.Rows[0][0]),
                    Quest = quest
                }, userId);
            }

            return new PilotProgressTask()
            {
                Id = ConvertRow.Row<uint>(dt.Rows[0][0]),
                Start = dateTime
            };
        }
        return null;
    }

    public static async Task<ulong?> CheckPilotTask(QuestTask questTask, ulong userId)
    {
        DataTable dt = await ExecuteCommand(Commands.checkpilottask, new Dictionary<string, object>()
        {
            { "inpilotid", userId },
            { "intaskid", questTask.Id }
        });

        if (dt != null && dt.Rows.Count != 0)
        {
            return ConvertRow.Row<ulong>(dt.Rows[0][0]);
        }
        return null;
    }

    public static async Task<bool> CancelPilotTask(QuestTask questTask, ulong userId)
    {
        DataTable dt = await ExecuteCommand(Commands.cancelpilottask, new Dictionary<string, object>()
        {
            { "inpilotid", userId },
            { "intaskid", questTask.Id }
        });

        if (dt != null && dt.Rows.Count != 0)
        {
            return true;
        }
        return false;
    }

    public static async Task<List<PilotTaskQuest>> GetPilotTaskQuests(uint id, QuestTask questTask)
    {
        DataTable dt = await ExecuteCommand(Commands.getpilottaskquests, new Dictionary<string, object>()
        {
            { "inpilottaskid", id }
        });

        if (dt != null)
        {
            List<PilotTaskQuest> pilotTaskQuests = new List<PilotTaskQuest>();
            foreach (DataRow row in dt.Rows)
            {
                PilotTaskQuest pilotTaskQuest = PilotTaskQuest.GetPilotTaskQuest(row);
                pilotTaskQuest.Quest = Server.Quests[ConvertRow.Row<uint>(row["questid"])];
                pilotTaskQuest.TaskId = questTask.Id;
                pilotTaskQuests.Add(pilotTaskQuest);
            }
            return pilotTaskQuests;
        }
        return null;
    }



    public static async Task<Dictionary<long, Enemy>> GetEnemies()
    {
        DataTable dt = await ExecuteCommand(Commands.getenemies, new Dictionary<string, object>());

        if (dt != null)
        {
            Dictionary<long, Enemy> enemies = new Dictionary<long, Enemy>();
            foreach (DataRow row in dt.Rows)
            {
                Enemy enemy = Enemy.GetEnemy(row);
                enemy.Reward.Items = await GetRewardItems(enemy.Reward.Id);
                enemies.Add(enemy.Id, enemy);
            }
            return enemies;
        }
        return null;
    }

    public static async Task<List<Portal>> GetPortals(long mapId)
    {
        DataTable dt = await ExecuteCommand(Commands.getportals, new Dictionary<string, object>()
        {
            { "inmapid", mapId }
        });

        if (dt != null)
        {
            List<Portal> portals = new List<Portal>();
            foreach (DataRow row in dt.Rows)
            {
                Portal portal = Portal.GetPortal(row);

                portal.Map = Server.Maps[portal.MapId];
                portal.TargetMap = Server.Maps[portal.TargetMapId];

                portals.Add(portal);
            }
            return portals;
        }
        return null;
    }

    public static async Task<List<EnemyMap>> GetEnemyMap(long mapId)
    {
        DataTable dt = await ExecuteCommand(Commands.getenemymap, new Dictionary<string, object>()
        {
            { "inmapid", mapId }
        });

        if (dt != null)
        {
            List<EnemyMap> enemyMaps = new List<EnemyMap>();
            foreach (DataRow row in dt.Rows)
            {
                enemyMaps.Add(EnemyMap.GetEnemyMap(row));
            }
            return enemyMaps;
        }
        return null;
    }

    public static async Task<Dictionary<long, Item>> GetItems()
    {
        DataTable dt = await ExecuteCommand(Commands.getitems, new Dictionary<string, object>());

        if (dt != null)
        {
            Dictionary<long, Item> items = new Dictionary<long, Item>();
            foreach (DataRow row in dt.Rows)
            {
                items.Add(ConvertRow.Row<int>(row["itemid"]), Item.GetItem(row));
            }
            return items;
        }
        return null;
    }

    public static async Task<List<ItemPilot>> GetPilotItems(ulong pilotId)
    {
        DataTable dt = await ExecuteCommand(Commands.getpilotitems, new Dictionary<string, object>()
        {
            { "inuserid", pilotId }
        });

        if (dt != null)
        {
            List<ItemPilot> items = new List<ItemPilot>();
            foreach (DataRow row in dt.Rows)
            {
                ItemPilot itemPilot = ItemPilot.GetItemPilot(row);
                itemPilot.Item = Server.Items[itemPilot.ItemId];
                items.Add(itemPilot);
            }

            return items;
        }
        return null;
    }

    public static async Task<List<ItemReward>> GetRewardItems(ulong rewardId)
    {
        DataTable dt = await ExecuteCommand(Commands.getrewarditems, new Dictionary<string, object>()
        {
            { "inrewardid", rewardId }
        });

        if (dt != null)
        {
            List<ItemReward> itemRewards = new List<ItemReward>();
            foreach (DataRow row in dt.Rows)
            {
                ItemReward itemReward = ItemReward.GetItemReward(row);
                itemReward.Item = Server.Items[itemReward.ItemId];
                itemRewards.Add(itemReward);
            }
            return itemRewards;
        }
        return null;
    }

    public static async Task<Dictionary<uint, Quest>> GetQuests()
    {
        DataTable dt = await ExecuteCommand(Commands.getquests, new Dictionary<string, object>());

        if (dt != null)
        {
            Dictionary<uint, Quest> quests = new Dictionary<uint, Quest>();
            foreach (DataRow row in dt.Rows)
            {
                Quest quest = Quest.GetQuest(row);
                quest.Maps = await GetQuestMaps(quest.Id);
                quests.Add(quest.Id, quest);
            }
            return quests;
        }
        return null;
    }

    public static async Task<List<ulong>> GetQuestMaps(uint id)
    {
        DataTable dt = await ExecuteCommand(Commands.getquestmaps, new Dictionary<string, object>()
        {
            { "inquestid", id }
        });

        if (dt != null)
        {
            List<ulong> questMaps = new List<ulong>();
            foreach (DataRow row in dt.Rows)
            {
                questMaps.Add(ConvertRow.Row<ulong>(row["mapid"]));
            }
            return questMaps;
        }
        return null;
    }

    public static async Task<Dictionary<uint, QuestTask>> GetTasks()
    {
        DataTable dt = await ExecuteCommand(Commands.gettasks, new Dictionary<string, object>());

        if (dt != null)
        {
            Dictionary<uint, QuestTask> tasks = new Dictionary<uint, QuestTask>();
            foreach (DataRow row in dt.Rows)
            {
                QuestTask task = QuestTask.GetTask(row);
                task.Reward.Items = await GetRewardItems(task.Reward.Id);
                task.Quests = await GetTaskQuests(task.Id);
                tasks.Add(task.Id, task);
            }
            return tasks;
        }
        return null;
    }

    public static async Task<List<Quest>> GetTaskQuests(uint id)
    {
        DataTable dt = await ExecuteCommand(Commands.gettaskquests, new Dictionary<string, object>()
        {
            { "intaskid", id }
        });

        if (dt != null)
        {
            List<Quest> quests = new List<Quest>();
            foreach (DataRow row in dt.Rows)
            {
                quests.Add(Server.Quests[ConvertRow.Row<uint>(row["questid"])]);
            }
            return quests;
        }
        return null;
    }

    #endregion

    #region Proces rejestracji

    /// <summary>
    /// Prawda = Mozna rejestrowac
    /// </summary>
    public static async Task<bool> OccupiedAccount(RegisterUser registerUser)
    {
        DataTable dt = await ExecuteCommand(Commands.occupiedaccount, new Dictionary<string, object>()
        {
            { "inusername", registerUser.Username },
            { "inemail", registerUser.Email }
        });

        if (dt != null)
            return int.Parse(dt.Rows[0][0].ToString()) == 0;
        return false;
    }

    /// <summary>
    /// Prawda = Mozna uzyc podanego nicku
    /// </summary>
    public static async Task<bool> OcuppiedNickname(string nickname)
    {
        DataTable dt = await ExecuteCommand(Commands.ocuppiednickname, new Dictionary<string, object>()
        {
            { "innickname", nickname }
        });

        if (dt != null)
            return int.Parse(dt.Rows[0][0].ToString()) == 0;
        return false;
    }

    /// <summary>
    /// Prawda = konto zostalo zarejestrowane
    /// </summary>
    public static async Task<bool> RegisterUser(RegisterUser registerUser)
    {
        DataTable dt = await ExecuteCommand(Commands.registeruser, new Dictionary<string, object>()
        {
            { "inusername", registerUser.Username },
            { "inpassword", registerUser.Password },
            { "inemail", registerUser.Email },
            { "innewsletter", registerUser.EmailNewsletter },
            { "inrules", registerUser.Rules },
            { "innickname", registerUser.Nickname }
        });

        if (dt != null)
        {
            await SavePlayerItem(new ItemPilot()
            {
                PilotId = (ulong)await GetPilot(registerUser),
                ItemId = 1,
                UpgradeLevel = 1
            });
            return true;
        }
        return false;
    }

    #endregion

    #region Proces logowania oraz inicjalizacji na serwerze

    /// <summary>
    /// Zwraca null jezeli niepoprawne dane logowania lub id logujacego uzytkownika
    /// </summary>
    /// <returns></returns>
    public static async Task<ulong?> LoginUser(LogInUser logInUser)
    {
        DataTable dt = await ExecuteCommand(Commands.loginuser, new Dictionary<string, object>()
        {
            { "inusername", logInUser.Username },
            { "inpassword", logInUser.Password }
        });

        if (dt != null && dt.Rows.Count > 0)
            return ulong.Parse(dt.Rows[0][0].ToString());
        return null;
    }

    /// <summary>
    /// Pobranie informacji z bazy danych o graczu
    /// </summary>
    public static async Task<Pilot> GetPilot(ulong userId)
    {
        DataTable dt = await ExecuteCommand(Commands.getplayerdata, new Dictionary<string, object>()
        {
            { "inuserid", userId }
        });

        if (dt != null)
        {
            foreach (DataRow row in dt.Rows)
            {
                return await PilotServer.GetPilot(row);
            }
        }
        return null;
    }

    /// <summary>
    /// Pobranie id uzytkownika posiadajacego wskazana nazwe uzytkownika
    /// </summary>
    public static async Task<ulong?> GetPilot(LogInUser logInUser)
    {
        DataTable dt = await ExecuteCommand(Commands.getplayerid, new Dictionary<string, object>()
        {
            { "inusername", logInUser.Username }
        });

        if (dt != null && dt.Rows.Count > 0)
            return ulong.Parse(dt.Rows[0][0].ToString());
        return null;
    }

    #endregion

    #region Zapis stanu gracza
    public static async Task SavePlayerData(Pilot pilot)
    {
        pilot.Achievements.TimeInGame += (ulong)(DateTime.Now - pilot.JoinToServer).Seconds;

        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "inuserid", pilot.Id },
            { "inmapid", pilot.Map.Id },
            { "inpositionx", pilot.PositionX },
            { "inpositiony", pilot.PositionY },
            { "inshipid", pilot.Ship.Id },
            { "inexperience", pilot.Experience },
            { "inlevel", pilot.Level },
            { "inscrap", pilot.Scrap },
            { "inmetal", pilot.Metal },
            { "inhitpoints", pilot.Hitpoints },
            { "inshields", pilot.Shields },
            { "inammunitionid", pilot.AmmunitionId },
            { "inrocketid", pilot.RocketId },
            { "inisdead", pilot.IsDead },
            { "inkillerby", string.IsNullOrEmpty(pilot.KillerBy) ? DBNull.Value : (object)pilot.KillerBy },
            { "inachievement", pilot.Achievements.Serialize() }
        };

        foreach (PilotResource resource in pilot.Resources.Values)
        {
            parameters.Add($"in{resource.ColumnName}", resource.Count);
        }

        await ExecuteCommand(Commands.saveplayerdata, parameters);

        foreach (ItemPilot item in pilot.Items)
        {
            await ExecuteCommand(Commands.saveplayeritems, new Dictionary<string, object>()
            {
                { "inrelationid", item.RelationId },
                { "inupgradelevel", item.UpgradeLevel },
                { "inisequipped", item.IsEquipped },
                { "inissold", item.IsSold },
            });
        }
    }

    public static async Task<ItemPilot> SavePlayerItem(ItemPilot item)
    {
        DataTable dt = await ExecuteCommand(Commands.saveplayeritem, new Dictionary<string, object>()
        {
            { "inuserid", item.PilotId },
            { "initemid", item.ItemId },
            { "inupgradelevel", item.UpgradeLevel },
        });

        if (dt != null && dt.Rows.Count > 0)
        {
            item.RelationId = ulong.Parse(dt.Rows[0][0].ToString());
            return item;
        }
        return null;
    }

    public static async Task UpdatePilotTaskQuest(PilotTaskQuest pilotTaskQuest)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "inpilottaskquestid", pilotTaskQuest.Id },
            { "inprogress", pilotTaskQuest.Progress },
            { "inisdone", pilotTaskQuest.IsDone }
        };

        await ExecuteCommand(Commands.updatepilottaskquest, parameters);
    }

    public static async Task<PilotTaskQuest> AddPilotTaskQuest(PilotTaskQuest pilotTaskQuest, ulong pilotId)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "inpilottaskid", pilotTaskQuest.TaskId },
            { "inquestid", pilotTaskQuest.Quest.Id }
        };

        DataTable dt = await ExecuteCommand(Commands.addpilottaskquest, parameters);

        if (dt != null)
        {
            pilotTaskQuest.Id = ConvertRow.Row<ulong>(dt.Rows[0][0]);
            return pilotTaskQuest;
        }
        return null;
    }

    public static async Task<PilotTask> UpdatePilotTask(PilotTask pilotTask)
    {
        Dictionary<string, object> parameters = new Dictionary<string, object>()
        {
            { "inpilottaskid", pilotTask.Id },
            { "instartdate", pilotTask.Start },
            { "inenddate", pilotTask.End }
        };

        DataTable dt = await ExecuteCommand(Commands.updatepilottask, parameters);

        if (dt != null)
        {
            foreach (DataRow row in dt.Rows)
            {
                PilotTask pt = PilotTask.GetPilotTask(row);
                pt.Task = Server.Tasks[ConvertRow.Row<uint>(row["taskid"])];
                pt.TaskQuest = await GetPilotTaskQuests(pilotTask.Id, pilotTask.Task);
                return pt;
            }
        }
        return null;
    }
    #endregion

}