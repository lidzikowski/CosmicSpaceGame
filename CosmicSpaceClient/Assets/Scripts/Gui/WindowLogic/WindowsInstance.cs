using UnityEngine;

public class WindowInstance
{
    private GameObject window;
    public GameObject Window
    {
        get => window;
        set
        {
            if (value != null)
            {
                window = value;
            }
            else
            {
                if (Active)
                    Script.Close();
                window = null;
            }
        }
    }
    public GameWindow Script => window?.GetComponent<GameWindow>();
    public bool Active => window != null;
}