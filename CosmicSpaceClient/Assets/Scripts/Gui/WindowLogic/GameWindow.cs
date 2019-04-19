﻿using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public abstract class GameWindow : MonoBehaviour
{
    public WindowTypes WindowType { get; set; }

    public Text TitleWindow { get; set; }
    public Button CloseButton { get; set; }

    public virtual void Close()
    {
        Destroy(gameObject);
    }

    public virtual void Start()
    {
        foreach (Transform t in transform)
        {
            if(t.name == "WindowShadow")
            {
                foreach (Transform child in t)
                {
                    if (child.tag == "TitleWindow")
                        TitleWindow = child.GetComponent<Text>();
                    else if (child.tag == "CloseWindow")
                        CloseButton = child.GetComponent<Button>();
                }
            }
        }

        ButtonListener(CloseButton, CloseButton_Clicked);

        Refresh();
    }

    public virtual void Refresh()
    {
        ChangeLanguage();
    }

    public static void ButtonListener(Button button, UnityAction function, bool removeAll = false)
    {
        if (button == null)
            return;

        if (removeAll)
            button.onClick.RemoveAllListeners();

        button.onClick.AddListener(function);
    }

    public static void SetText(Text label, string message)
    {
        if (label == null)
            return;

        label.text = message;
    }

    public virtual void ChangeLanguage()
    {
        switch(WindowType)
        {
            case WindowTypes.HangarWindow:
                TitleWindow.text = GameSettings.UserLanguage.HANGAR;
                break;
            case WindowTypes.MissionWindow:
                TitleWindow.text = GameSettings.UserLanguage.MISSIONS;
                break;
            case WindowTypes.SettingsWindow:
                TitleWindow.text = GameSettings.UserLanguage.SETTINGS;
                break;
        }
    }

    public virtual void CloseButton_Clicked()
    {
        GuiScript.CloseWindow(WindowType);
        GuiScript.RefreshAllActiveWindow();
    }
}