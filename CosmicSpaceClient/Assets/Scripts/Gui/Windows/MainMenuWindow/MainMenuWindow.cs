using System.Collections.Generic;
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
    public GameObject SocialMediaGameObject;
    public GameObject NewsletterGameObject;
    public GameObject RulesGameObject;

    [Header("Create account button")]
    public Button CreateAccountButton;
    public Text CreateAccountLabel;

    [Header("Label for language")]
    public Text GameVersionText;
    public Text ServerStatusText;

    public override void Start()
    {
        base.Start();

        ButtonListener(CreateAccountButton, CreateAccountButton_Clicked);
        ShowPage(Pages.SignInPage);
    }

    public void ShowPage(Pages page)
    {
        bool signIn = page == Pages.SignInPage;
        SignInPageGameObject?.SetActive(signIn);
        SocialMediaGameObject?.SetActive(signIn);
        NewsletterGameObject?.SetActive(signIn);

        bool register = page == Pages.CreateAccountPage;
        CreateAccountPageGameObject?.SetActive(register);
        RulesGameObject?.SetActive(register);
        CreateAccountButton?.gameObject.SetActive(!register);
    }

    public override void Refresh()
    {
        base.Refresh();

        CreateAccountButton.interactable = Client.SocketConnected;
    }

    public override void ChangeLanguage()
    {
        if (GameSettings.UserLanguage == null)
            return;

        SetText(GameVersionText, string.Format(GameSettings.UserLanguage.GAME_VERSION, Application.version));
        SetText(ServerStatusText, string.Format(GameSettings.UserLanguage.SERVER_STATUS, Client.SocketConnected ? "ONLINE" : "OFFLINE"));
        SetText(CreateAccountLabel, GameSettings.UserLanguage.CREATE_ACCOUNT);

        SignInPageGameObject?.GetComponent<GameWindow>().Refresh();
        CreateAccountPageGameObject?.GetComponent<GameWindow>().Refresh();
    }

    void CreateAccountButton_Clicked()
    {
        ShowPage(Pages.CreateAccountPage);
    }
}