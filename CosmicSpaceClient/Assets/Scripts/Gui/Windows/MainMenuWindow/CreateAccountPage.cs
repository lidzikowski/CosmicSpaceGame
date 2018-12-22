using UnityEngine.UI;

public class CreateAccountPage : GameWindow
{
    public InputField UsernameInputField;
    public InputField PasswordInputField;
    public InputField EmailInputField;
    public Toggle RulesToggle;

    public Button RegisterButton;
    public Button SignInButton;
    
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

        // Rejestracja
    }

    void SignInButton_Clicked()
    {
        (GuiScript.Windows[WindowTypes.MainMenu].Script as MainMenuWindow).ShowPage(MainMenuWindow.Pages.SignInPage);
    }

    void SetLanguage()
    {

    }

    private void OnEnable()
    {
        SetLanguage();

        ButtonListener(RegisterButton, RegisterButton_Clicked, true);
        ButtonListener(SignInButton, SignInButton_Clicked, true);
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