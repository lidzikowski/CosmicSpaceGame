using WebSocketSharp;
using WebSocketSharp.Server;
using UnityEngine;
using System;
using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Account;
using System.Linq;

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
                    LoginUser(logInUser);
                    break;

                case Commands.Register:
                    RegisterUser registerUser = (RegisterUser)commandData.Data;
                    RegisterUser(registerUser);
                    break;
            }
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    private static void LoginUser(LogInUser logInUser)
    {
        //Server.Database.Users.FirstOrDefault(
        //    o => o.Username == logInUser.Username &&
        //    o.Password == logInUser.Password
        //    );
    }

    private static void RegisterUser(RegisterUser registerUser)
    {
        if (Database.OccupiedAccount(registerUser))
        {
            if (Database.RegisterUser(registerUser))
            {
                // Konto utworzone = Proces logowania
                Debug.Log("zarejestrowano " + registerUser.Username);
            }
            else
            {
                // Odpowiedz = problem w zakladaniu konta
            }
        }
        else
        {
            // Odpowiedz = uzytkownik / email zajety
        }

        //if (user != null)
        //{
        //    // Uzytkownik jest w bazie = zajety
        //    Debug.Log("zajety");
        //}
        //else
        //{
        //    db.Users.Add(new Users()
        //    {
        //        Username = registerUser.Username,
        //        Password = registerUser.Password,
        //        Email = registerUser.Email,
        //        Rules = registerUser.Rules,
        //        EmailNewsletter = true,
        //        RegisterDate = DateTime.Now
        //    });
        //    db.SaveChanges();
        //    Debug.Log("dodano");
        //    // Dodano do bazy
        //}
    }
}