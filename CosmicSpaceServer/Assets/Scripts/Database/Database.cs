using CosmicSpaceCommunication.Account;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Database
{
    private static string connectionString = new MySqlConnectionStringBuilder()
    {
        Server = "127.0.0.1",
        Port = 3306,
        UserID = "root",
        //Password = "pHqD6wxEeuZTuSk5FDhpBcwf4R7Z5LCgaSN5vCa2",
        Database = "cosmicspace",
        SslMode = MySqlSslMode.None
    }.ToString();

    public enum Commands
    {
        occupiedaccount,
        registeruser,
        loginuser
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
    /// Prawda = Mozna rejestrowac
    /// </summary>
    public static bool OccupiedAccount(RegisterUser registerUser)
    {
        DataTable dt = ExecuteCommand(Commands.occupiedaccount, new Dictionary<string, object>()
        {
            { "username", registerUser.Username },
            { "email", registerUser.Email }
        });
        
        if (dt == null)
            return false;
        return int.Parse(dt.Rows[0][0].ToString()) == 0;
    }

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

        Debug.Log(dt.Rows);

        if (dt != null)
            return true;
        return false;
    }


}