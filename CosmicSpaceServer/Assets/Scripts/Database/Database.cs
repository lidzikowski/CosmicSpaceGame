using CosmicSpaceCommunication.Account;
using CosmicSpaceCommunication.Game.Resources;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Database
{
    #region ConnectionString
    private static string connectionString = new MySqlConnectionStringBuilder()
    {
        Server = "127.0.0.1",
        Port = 3306,
        UserID = "root",
        //Password = "pHqD6wxEeuZTuSk5FDhpBcwf4R7Z5LCgaSN5vCa2",
        Database = "cosmicspace",
        SslMode = MySqlSslMode.None
    }.ToString();
    #endregion



    #region Baza danych i obsluga danych

    public enum Commands
    {
        occupiedaccount,
        registeruser,
        loginuser,
        getplayerdata,
        getmaps,
        getships,
        loguser,
        getplayerid
    }

    private static DataTable ExecuteCommand(Commands command, Dictionary<string, object> parameters)
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

                    Connection.Open();
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
            Debug.Log(ex.Message);
            return null;
        }
    }

    /// <summary>
    /// Konwersacja obiektu z bazy na szczegolny typ
    /// </summary>
    public static T Row<T>(object obj)
    {
        if (System.Convert.IsDBNull(obj))
            return default(T);
        return (T)System.Convert.ChangeType(obj, typeof(T));
    }

    /// <summary>
    /// Logi do bazy danych z operacji
    /// </summary>
    public static void LogUser(Headers headers, CosmicSpaceCommunication.Commands command, bool result, ulong? userid)
    {
        ExecuteCommand(Commands.loguser, new Dictionary<string, object>()
        {
            { "inaction", command.ToString() },
            { "inresult", result },
            { "inuseragent", headers.UserAgent },
            { "inhost", headers.Host },
            { "inuserid", userid == null ? System.DBNull.Value : (object)userid }
        });
        Pilot.Send(new CosmicSpaceCommunication.CommandData()
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
    public static Dictionary<int, Map> GetMaps()
    {
        DataTable dt = ExecuteCommand(Commands.getmaps, new Dictionary<string, object>());

        if (dt != null)
        {
            Dictionary<int, Map> maps = new Dictionary<int, Map>();
            foreach (DataRow row in dt.Rows)
            {
                maps.Add(Row<int>(row["mapid"]), new Map()
                {
                    Id = Row<int>(row["mapid"]),
                    Name = Row<string>(row["mapname"]),
                    Description = Row<string>(row["description"]),
                    RequiredLevel = Row<int>(row["requiredlevel"]),

                    IsPvp = Row<bool>(row["ispvp"])
                });
            }
            return maps;
        }
        return null;
    }

    /// <summary>
    /// Pobranie wszystkich statkow z bazy danych
    /// </summary>
    public static Dictionary<int, Ship> GetShips()
    {
        DataTable dt = ExecuteCommand(Commands.getships, new Dictionary<string, object>());

        if (dt != null)
        {
            Dictionary<int, Ship> ships = new Dictionary<int, Ship>();
            foreach (DataRow row in dt.Rows)
            {
                ships.Add(Row<int>(row["shipid"]), new Ship()
                {
                    Id = Row<int>(row["shipid"]),
                    Name = Row<string>(row["shipname"]),
                    Description = Row<string>(row["description"]),
                    RequiredLevel = Row<int>(row["requiredlevel"]),

                    ScrapPrice = Row<double>(row["scrapprice"]),
                    MetalPrice = Row<double>(row["metalprice"]),
                    Lasers = Row<int>(row["lasers"]),
                    Generators = Row<int>(row["generators"]),
                    Extras = Row<int>(row["extras"]),
                });

            }
            return ships;
        }
        return null;
    }

    #endregion

    #region Proces rejestracji

    /// <summary>
    /// Prawda = Mozna rejestrowac
    /// </summary>
    public static bool OccupiedAccount(RegisterUser registerUser)
    {
        DataTable dt = ExecuteCommand(Commands.occupiedaccount, new Dictionary<string, object>()
        {
            { "inusername", registerUser.Username },
            { "inemail", registerUser.Email }
        });
        
        if (dt != null)
            return int.Parse(dt.Rows[0][0].ToString()) == 0;
        return false;
    }

    /// <summary>
    /// Prawda = konto zostalo zarejestrowane
    /// </summary>
    public static bool RegisterUser(RegisterUser registerUser)
    {
        DataTable dt = ExecuteCommand(Commands.registeruser, new Dictionary<string, object>()
        {
            { "inusername", registerUser.Username },
            { "inpassword", registerUser.Password },
            { "inemail", registerUser.Email },
            { "innewsletter", registerUser.EmailNewsletter },
            { "inrules", registerUser.Rules }
        });

        if (dt != null)
            return true;
        return false;
    }

    #endregion

    #region Proces logowania oraz inicjalizacji na serwerze

    /// <summary>
    /// Zwraca null jezeli niepoprawne dane logowania lub id logujacego uzytkownika
    /// </summary>
    /// <returns></returns>
    public static ulong? LoginUser(LogInUser logInUser)
    {
        DataTable dt = ExecuteCommand(Commands.loginuser, new Dictionary<string, object>()
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
    public static Pilot GetPilot(ulong userId)
    {
        DataTable dt = ExecuteCommand(Commands.getplayerdata, new Dictionary<string, object>()
        {
            { "inuserid", userId }
        });

        if (dt != null)
        {
            foreach(DataRow row in dt.Rows)
            {
                return Pilot.GetPilot(row);
            }
        }
        return null;
    }

    /// <summary>
    /// Pobranie id uzytkownika posiadajacego wskazana nazwe uzytkownika
    /// </summary>
    public static ulong? GetPilot(LogInUser logInUser)
    {
        DataTable dt = ExecuteCommand(Commands.getplayerid, new Dictionary<string, object>()
        {
            { "inusername", logInUser.Username }
        });

        if (dt != null && dt.Rows.Count > 0)
            return ulong.Parse(dt.Rows[0][0].ToString());
        return null;
    }

    #endregion



}