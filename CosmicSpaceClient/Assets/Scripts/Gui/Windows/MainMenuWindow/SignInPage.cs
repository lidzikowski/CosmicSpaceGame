using UnityEngine.UI;

public class SignInPage : GameWindow
{
    public InputField UsernameInputField;
    public InputField PasswordInputField;
    public Toggle RememberToggle;

    public Button LogInButton;
    public Button CreateAccountButton;
    
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

    void SetLanguage()
    {

    }

    private void OnEnable()
    {
        SetLanguage();

        ButtonListener(LogInButton, LogInButton_Clicked, true);
        ButtonListener(CreateAccountButton, CreateAccountButton_Clicked, true);
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