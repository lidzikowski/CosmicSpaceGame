﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public abstract class GameWindow : MonoBehaviour
{
    public virtual void Close()
    {
        Destroy(gameObject);
    }

    public virtual void Refresh() { }

    public static void ButtonListener(Button button, UnityAction function, bool removeAll = false)
    {
        if (button == null)
            return;

        if (removeAll)
            button.onClick.RemoveAllListeners();

        button.onClick.AddListener(function);
    }
}