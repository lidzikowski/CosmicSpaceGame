using CosmicSpaceCommunication.Account;
using CosmicSpaceCommunication.Game;
using CosmicSpaceCommunication.Game.Enemy;
using CosmicSpaceCommunication.Game.Player;
using CosmicSpaceCommunication.Game.Resources;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using UnityEngine;

public class Database
{
    #region ConnectionString
    private static readonly string connectionString = new MySqlConnectionStringBuilder()
    {
        Server = "127.0.0.1",
        Port = 3306,
        UserID = "root",
        //Password = "pHqD6wxEeuZTuSk5FDhpBcwf4R7Z5LCgaSN5vCa2",
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

        occupiedaccount,
        ocuppiednickname,
        registeruser,
        loginuser,
        loguser,

        saveplayerdata,
        saveplayeritems,
        saveplayeritem,

        getplayerid,
        getplayerdata,
        getpilotresources,
        getitems,
        getpilotitems
    }

    #region Baza danych i obsluga danych
    private static async Task<DataTable> ExecuteCommand(Commands command, Dictionary<string, object> parameters)
    {
        try
        {
            using (MySqlConnection Connection = new MySqlConnection(connectionString))
            {
                using (MySqlCommand cmd = new MySqlCommand(command.ToString(), Connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    foreach (KeyValuePair<string, object> parameter in parameters)
                    {
                        cmd.Parameters.AddWithValue(parameter.Key, parameter.Value);
                    }

                    await Connection.OpenAsync();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        DataTable dataTable = new DataTable("Result");
                        dataTable.Load(reader);
                        return dataTable;
                    }
                }
            }
        }
        catch (MySqlException ex)
        {
            Server.Log("Blad bazy danych.", ex.Message);
            return null;
        }
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
            { "inkillerby", string.IsNullOrEmpty(pilot.KillerBy) ? DBNull.Value : (object)pilot.KillerBy }
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
    #endregion

}