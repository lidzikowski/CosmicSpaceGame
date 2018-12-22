using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GuiScript : MonoBehaviour
{
    public static Dictionary<WindowTypes, WindowInstance> Windows = new Dictionary<WindowTypes, WindowInstance>();
    protected static Transform canvas;
    public static bool Ready = false;

    void Start()
    {
        for (int i = 0; i < WindowTypes.GetNames(typeof(WindowTypes)).Length; i++)
        {
            Windows.Add((WindowTypes)i, new WindowInstance());
        }

        canvas = transform;

        foreach (Transform t in transform)
            Destroy(t.gameObject);

        Ready = true;
    }

    public static void OpenWindow(WindowTypes windowType)
    {
        InstantiateWindow(windowType, true);
    }

    public static void CloseWindow(WindowTypes windowType)
    {
        InstantiateWindow(windowType, false);
    }

    public static void CloseAllWindow()
    {
        foreach (WindowInstance window in Windows.Values.Where(o => o.Active))
        {
            window.Window = null;
        }
    }

    public static void RefreshAllActiveWindow()
    {
        foreach (WindowInstance window in Windows.Values.Where(o => o.Active))
        {
            window.Script.Refresh();
        }
    }

    private static void InstantiateWindow(WindowTypes windowType, bool showWindow, bool closeAllWindow = false)
    {
        WindowInstance window = Windows[windowType];

        if (window.Active == showWindow)
            return;

        if (closeAllWindow)
            CloseAllWindow();

        if (showWindow)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>("GameWindows/" + windowType), canvas);
            window.Window = go;
        }
        else
            window.Window = null;
    }
}