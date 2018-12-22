using System.Collections;
using System.Collections.Generic;
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

    IEnumerator Start()
    {
        if (Application.systemLanguage == SystemLanguage.Polish)
            UserLanguage = new Polish();
        else
            UserLanguage = new English();

        yield return new WaitUntil(() => GuiScript.Ready);
        GuiScript.OpenWindow(WindowTypes.MainMenu);

    }
}