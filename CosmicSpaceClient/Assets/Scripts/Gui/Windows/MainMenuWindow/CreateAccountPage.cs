using CosmicSpaceCommunication;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccountPage : GameWindow
{
    [Header("Inputs")]
    public InputField UsernameInputField;
    public InputField PasswordInputField;
    public InputField EmailInputField;
    public Toggle RulesToggle;
    public Button RegisterButton;
    public Button SignInButton;

    [Header("Label for language")]
    public Text WindowName;
    public Text UsernamePlaceholder;
    public Text PasswordPlaceholder;
    public Text EmailPlaceholder;
    public Text RulesLabel;
    public Text SignInButtonLabel;
    public Text RegisterButtonLabel;

    public override void Start()
    {
        base.Start();

        ButtonListener(RegisterButton, RegisterButton_Clicked, true);
        ButtonListener(SignInButton, SignInButton_Clicked, true);
    }

    void RegisterButton_Clicked()
    {
        //if (UsernameInputField == null || PasswordInputField == null || EmailInputField == null || RulesToggle == null)
        //    return;

        string username = UsernameInputField?.text;
        string password = PasswordInputField?.text;
        string email = EmailInputField?.text;

        if (!Validation.Text(username, 3, 20, true))
        {
            // Niepoprawna nazwa uzytkownika
        }

        if (!Validation.Text(password))
        {
            // Niepoprawne haslo
        }

        if (!Validation.Email(email))
        {
            // Niepoprawne haslo
        }

        if (!RulesToggle.isOn)
        {
            // Regulamin nie zostal zaakceptowany
        }
        
        Client.SendToSocket(new CommandData()
        {
            Command = Commands.Register,
            Data = new CosmicSpaceCommunication.Account.RegisterUser()
            {
                Username = GameData.HashString(username),
                Password = GameData.HashString(password),
                Email = email,
                EmailNewsletter = false,
                Rules = RulesToggle.isOn
            }
        });
    }

    void SignInButton_Clicked()
    {
        (GuiScript.Windows[WindowTypes.MainMenu].Script as MainMenuWindow).ShowPage(MainMenuWindow.Pages.SignInPage);
    }

    public override void Refresh()
    {
        base.Refresh();
        
        RegisterButton.interactable = Client.SocketConnected;
        SignInButton.interactable = Client.SocketConnected;
    }

    public override void ChangeLanguage()
    {
        SetText(WindowName, GameSettings.UserLanguage.CREATE_ACCOUNT);
        SetText(UsernamePlaceholder, GameSettings.UserLanguage.USERNAME);
        SetText(PasswordPlaceholder, GameSettings.UserLanguage.PASSWORD);
        SetText(EmailPlaceholder, GameSettings.UserLanguage.EMAIL);
        SetText(RulesLabel, GameSettings.UserLanguage.RULES);
        SetText(SignInButtonLabel, GameSettings.UserLanguage.SIGN_IN);
        SetText(RegisterButtonLabel, GameSettings.UserLanguage.REGISTER);
    }

    private void OnDisable()
    {
        if (UsernameInputField == null || PasswordInputField == null || EmailInputField == null || RulesToggle == null)
            return;

        UsernameInputField.text = string.Empty;
        PasswordInputField.text = string.Empty;
        EmailInputField.text = string.Empty;
        RulesToggle.isOn = false;
    }
}