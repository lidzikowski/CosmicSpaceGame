﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuWindow : GameWindow
{
    public enum Pages
    {
        SignInPage,
        CreateAccountPage
    }

    [Header("Page in window")]
    public GameObject SignInPageGameObject;
    public GameObject CreateAccountPageGameObject;

    [Header("Label for language")]
    public Text GameVersionText;
    public Text ServerStatusText;

    public override void Start()
    {
        ShowPage(Pages.SignInPage);
    }

    public void ShowPage(Pages page)
    {
        if (page == Pages.SignInPage)
            SignInPageGameObject?.SetActive(true);
        else
            SignInPageGameObject?.SetActive(false);

        if (page == Pages.CreateAccountPage)
            CreateAccountPageGameObject?.SetActive(true);
        else
            CreateAccountPageGameObject?.SetActive(false);

        Refresh();
    }

    public override void ChangeLanguage()
    {
        SetText(GameVersionText, string.Format(GameSettings.UserLanguage.GAME_VERSION, Application.version));
        SetText(ServerStatusText, string.Format(GameSettings.UserLanguage.SERVER_STATUS, "???"));

        //SignInPageGameObject?.GetComponent<SignInPage>().ChangeLanguage();
        //CreateAccountPageGameObject?.GetComponent<CreateAccountPage>().ChangeLanguage();
    }
}