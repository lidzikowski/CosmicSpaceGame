using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreateAccountWindow : MonoBehaviour
{
    public InputField UsernameInputField;
    public InputField PasswordInputField;
    public InputField EmailInputField;
    public Toggle RulesToggle;

    public Button RegisterButton;
    public Button SignInButton;
    public GameObject SignInGameObject;

    void Start()
    {
        SetLanguage();

        RegisterButton?.onClick.AddListener(RegisterButton_Clicked);
        SignInButton?.onClick.AddListener(SignInButton_Clicked);
    }

    void RegisterButton_Clicked()
    {
        if (UsernameInputField == null || PasswordInputField == null || EmailInputField == null || RulesToggle == null)
            return;

        string username = UsernameInputField.text;
        string password = PasswordInputField.text;
        string email = EmailInputField.text;

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
        if (SignInGameObject == null)
            return;

        SignInGameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    void SetLanguage()
    {

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