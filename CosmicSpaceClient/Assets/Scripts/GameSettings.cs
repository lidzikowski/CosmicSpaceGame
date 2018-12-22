using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : MonoBehaviour
{
    public static Language UserLanguage;

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