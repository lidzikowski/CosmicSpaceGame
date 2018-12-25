using CosmicSpaceCommunication;
using UnityEngine;
using UnityEngine.UI;

public class SignInPage : GameWindow
{
    [Header("Inputs")]
    public InputField UsernameInputField;
    public InputField PasswordInputField;
    public Toggle RememberToggle;
    public Button LogInButton;

    [Header("Label for language")]
    public Text WindowName;
    public Text UsernamePlaceholder;
    public Text PasswordPlaceholder;
    public Text RememberLabel;
    public Text LogInButtonLabel;

    public override void Start()
    {
        base.Start();

        ButtonListener(LogInButton, LogInButton_Clicked, true);
    }

    void LogInButton_Clicked()
    {
        //if (UsernameInputField == null || PasswordInputField == null || RememberToggle == null)
        //    return;

        string username = UsernameInputField?.text;
        string password = PasswordInputField?.text;

        if (!Validation.Text(username, civility: true))
        {
            Debug.Log("Niepoprawna nazwa uzytkownika " + username);
            return;
        }

        if(!Validation.Text(password))
        {
            Debug.Log("Niepoprawne haslo");
            return;
        }

        if(RememberToggle.isOn)
        {
            PlayerPrefs.SetString("CosmicSpaceUsername", username);
            PlayerPrefs.SetString("CosmicSpacePassword", password);
            PlayerPrefs.SetInt("CosmicSpaceRemember", 1);
        }
        else
        {
            PlayerPrefs.SetString("CosmicSpaceUsername", string.Empty);
            PlayerPrefs.SetString("CosmicSpacePassword", string.Empty);
            PlayerPrefs.SetInt("CosmicSpaceRemember", 0);
        }

        Client.SendToSocket(new CommandData()
        {
            Command = Commands.LogIn,
            Data = new CosmicSpaceCommunication.Account.LogInUser()
            {
                Username = GameData.HashString(username),
                Password = GameData.HashString(password)
            }
        });
    }

    void CreateAccountButton_Clicked()
    {
        (GuiScript.Windows[WindowTypes.MainMenu].Script as MainMenuWindow).ShowPage(MainMenuWindow.Pages.CreateAccountPage);
    }

    public override void Refresh()
    {
        base.Refresh();
        
        LogInButton.interactable = Client.SocketConnected;
    }

    public override void ChangeLanguage()
    {
        SetText(WindowName, GameSettings.UserLanguage.SIGN_IN);
        SetText(UsernamePlaceholder, GameSettings.UserLanguage.USERNAME);
        SetText(PasswordPlaceholder, GameSettings.UserLanguage.PASSWORD);
        SetText(RememberLabel, GameSettings.UserLanguage.REMEMBER);
        SetText(LogInButtonLabel, GameSettings.UserLanguage.LOG_IN);
    }

    private void OnEnable()
    {
        UsernameInputField.text = PlayerPrefs.GetString("CosmicSpaceUsername");
        PasswordInputField.text = PlayerPrefs.GetString("CosmicSpacePassword");
        RememberToggle.isOn = PlayerPrefs.GetInt("CosmicSpaceRemember") == 1;
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