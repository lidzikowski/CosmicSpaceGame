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

    public GameObject SignInPageGameObject;
    public GameObject CreateAccountPageGameObject;

    public Text GameVersionText;
    public Text ServerStatusText;

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
    }

    private void OnEnable()
    {
        SetLanguage();

        ShowPage(Pages.SignInPage);
    }

    void SetLanguage()
    {

    }
}