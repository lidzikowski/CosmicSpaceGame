using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceWindow : GameWindow
{
    public enum Pages
    {

    }

    public Text Test;



    public override void Refresh()
    {
        base.Refresh();

        if (Client.Pilot == null)
            return;

        Test.text = Client.Pilot.Nickname;
    }
}