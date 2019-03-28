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

    IEnumerator Start()
    {
        if (Application.systemLanguage == SystemLanguage.Polish)
            UserLanguage = Language.Load(Languages.Polish);
        else
            UserLanguage = Language.Load(Languages.English);

        yield return new WaitUntil(() => GuiScript.Ready);
        GuiScript.OpenWindow(WindowTypes.MainMenu);
    }
}