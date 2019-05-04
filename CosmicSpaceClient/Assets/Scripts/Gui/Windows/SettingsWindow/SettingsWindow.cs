using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsWindow : GameWindow
{
    [Header("Language")]
    public Text LanguageText;
    public Dropdown LanguageDropdown;

    public override void Start()
    {
        base.Start();

        SetLanguages();
    }

    public override void Refresh()
    {
        base.Refresh();
    }

    public override void ChangeLanguage()
    {
        base.ChangeLanguage();

        SetText(LanguageText, GameSettings.UserLanguage.LANGUAGE);
    }



    private void SetLanguages()
    {
        LanguageDropdown.ClearOptions();
        List<string> languages = new List<string>();
        foreach (Languages language in (Languages[])Enum.GetValues(typeof(Languages)))
        {
            languages.Add(language.ToString());
        }
        LanguageDropdown.AddOptions(languages);

        LanguageDropdown.value = (int)GameSettings.ChooseLanguage;
        LanguageDropdown.onValueChanged.AddListener(delegate
        {
            GameSettings.ChooseLanguage = (Languages)LanguageDropdown.value;
        });
    }


}