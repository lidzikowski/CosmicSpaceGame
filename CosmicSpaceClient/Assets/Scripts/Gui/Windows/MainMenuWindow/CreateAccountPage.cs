using CosmicSpaceCommunication;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccountPage : GameWindow
{
    [Header("Inputs")]
    public InputField UsernameInputField;
    public InputField PasswordInputField;
    public InputField EmailInputField;
    public InputField NicknameInputField;
    public Toggle NewsletterToggle;
    public Toggle RulesToggle;
    public Button RegisterButton;
    public Button SignInButton;

    [Header("Label for language")]
    public Text WindowName;
    public Text UsernamePlaceholder;
    public Text PasswordPlaceholder;
    public Text EmailPlaceholder;
    public Text NicknamePlaceholder;
    public Text NewsletterLabel;
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
        string username = UsernameInputField?.text;
        string password = PasswordInputField?.text;
        string email = EmailInputField?.text;
        string nickname = NicknameInputField?.text;

        if (!Validation.Text(username, civility: true))
        {
            Debug.Log("Nieprawidlowa nazwa uzytkownika");
            return;
        }

        if (!Validation.Text(password))
        {
            Debug.Log("Nieprawidlowe haslo");
            return;
        }

        if (!Validation.Email(email))
        {
            Debug.Log("Nieprawidlowy email");
            return;
        }

        if (!Validation.Text(nickname, civility: true))
        {
            Debug.Log("Niepoprawna nazwa uzytkownika");
            return;
        }

        if (!RulesToggle.isOn)
        {
            Debug.Log("Regulamin nie zostal zaakceptowany");
            return;
        }

        Client.SendToSocket(new CommandData()
        {
            Command = Commands.Register,
            Data = new CosmicSpaceCommunication.Account.RegisterUser()
            {
                Username = GameData.HashString(username),
                Password = GameData.HashString(password),
                Email = email,
                Nickname = nickname,
                EmailNewsletter = NewsletterToggle.isOn,
                Rules = RulesToggle.isOn
            }
        });
    }

    void SignInButton_Clicked()
    {
        if (GuiScript.Windows[WindowTypes.MainMenu].Script is MainMenuWindow mainMenuWindow)
        {
            mainMenuWindow.ShowPage(MainMenuWindow.Pages.SignInPage);
        }
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
        SetText(NicknamePlaceholder, GameSettings.UserLanguage.NICKNAME);
        SetText(NewsletterLabel, GameSettings.UserLanguage.NEWSLETTER);
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