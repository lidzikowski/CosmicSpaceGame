using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignInWindow : MonoBehaviour
{
    public InputField UsernameInputField;
    public InputField PasswordInputField;
    public Toggle RememberToggle;

    public Button LogInButton;
    public Button CreateAccountButton;
    public GameObject CreateAccountGameObject;

    void Start()
    {
        SetLanguage();

        LogInButton?.onClick.AddListener(LogInButton_Clicked);
        CreateAccountButton?.onClick.AddListener(CreateAccountButton_Clicked);
    }

    void LogInButton_Clicked()
    {
        if (UsernameInputField == null || PasswordInputField == null || RememberToggle == null)
            return;

        string username = UsernameInputField.text;
        string password = PasswordInputField.text;

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
        if (CreateAccountGameObject == null)
            return;

        CreateAccountGameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    void SetLanguage()
    {

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