using UnityEngine;
using UnityEngine.UI;

public class SignInPage : GameWindow
{
    [Header("Inputs")]
    public InputField UsernameInputField;
    public InputField PasswordInputField;
    public Toggle RememberToggle;
    public Button LogInButton;
    public Button CreateAccountButton;

    [Header("Label for language")]
    public Text WindowName;
    public Text UsernamePlaceholder;
    public Text PasswordPlaceholder;
    public Text RememberLabel;
    public Text LogInButtonLabel;
    public Text CreateAccountButtonLabel;

    public override void Start()
    {
        base.Start();

        ButtonListener(LogInButton, LogInButton_Clicked, true);
        ButtonListener(CreateAccountButton, CreateAccountButton_Clicked, true);
    }

    void LogInButton_Clicked()
    {
        //if (UsernameInputField == null || PasswordInputField == null || RememberToggle == null)
        //    return;

        string username = UsernameInputField?.text;
        string password = PasswordInputField?.text;

        if(!Validation.Text(username, 3, 20, true))
        {
            // Niepoprawna nazwa uzytkownika
        }

        if(!Validation.Text(password))
        {
            // Niepoprawne haslo
        }

        if(RememberToggle.isOn)
        {
            // Zapisz dane logowania na komputerze
        }

        // Logowanie do serwera
    }

    void CreateAccountButton_Clicked()
    {
        (GuiScript.Windows[WindowTypes.MainMenu].Script as MainMenuWindow).ShowPage(MainMenuWindow.Pages.CreateAccountPage);
    }

    public override void ChangeLanguage()
    {
        SetText(WindowName, GameSettings.UserLanguage.SIGN_IN);
        SetText(UsernamePlaceholder, GameSettings.UserLanguage.USERNAME);
        SetText(PasswordPlaceholder, GameSettings.UserLanguage.PASSWORD);
        SetText(RememberLabel, GameSettings.UserLanguage.REMEMBER);
        SetText(LogInButtonLabel, GameSettings.UserLanguage.LOG_IN);
        SetText(CreateAccountButtonLabel, GameSettings.UserLanguage.CREATE_ACCOUNT);
    }

    private void OnDisable()
    {
        if (UsernameInputField == null || PasswordInputField == null || RememberToggle == null)
            return;

        UsernameInputField.text = string.Empty;
        PasswordInputField.text = string.Empty;
        RememberToggle.isOn = false;
    }
}