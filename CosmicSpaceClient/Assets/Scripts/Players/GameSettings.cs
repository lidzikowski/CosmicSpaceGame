using System;
using System.Collections;
using UnityEngine;

public delegate void ChangeLanguage();

public class GameSettings : MonoBehaviour
{
    private static Language userLanguage;
    public static Language UserLanguage
    {
        get => userLanguage;
        set
        {
            userLanguage = value;
            GuiScript.RefreshAllActiveWindow();
        }
    }

    private static Languages chooseLanguage;
    public static Languages ChooseLanguage
    {
        get => chooseLanguage;
        set
        {
            chooseLanguage = value;
            UserLanguage = Language.Load(value);

            PlayerPrefs.SetInt("CosmicSpaceLanguage", (int)value);
        }
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
    }

    IEnumerator Start()
    {
        SetLanguage();

        yield return new WaitUntil(() => GuiScript.Ready);
        GuiScript.OpenWindow(WindowTypes.MainMenu);
    }

    private bool SetLanguage()
    {
        if(PlayerPrefs.HasKey("CosmicSpaceLanguage"))
        {
            int languageId = PlayerPrefs.GetInt("CosmicSpaceLanguage");
            if (Enum.IsDefined(typeof(Languages), languageId))
            {
                ChooseLanguage = (Languages)languageId;
                return true;
            }
        }

        if (Application.systemLanguage == SystemLanguage.Polish)
            ChooseLanguage = Languages.Polish;
        else
            ChooseLanguage = Languages.English;

        return true;
    }
}