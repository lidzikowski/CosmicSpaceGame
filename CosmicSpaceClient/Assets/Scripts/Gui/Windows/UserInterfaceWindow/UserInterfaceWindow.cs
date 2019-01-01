using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceWindow : GameWindow
{
    public Text UserText;
    public Text MetalText;
    public Text ScrapText;

    public Text MiniMapPanelText;
    public Text MapPosition;



    public override void Refresh()
    {
        base.Refresh();

        SetText(UserText, $"UID: {Client.Pilot.Id} {Client.Pilot.Nickname}");
        SetText(MetalText, $"Metal {Client.Pilot.Metal}");
        SetText(ScrapText, $"Scrap {Client.Pilot.Scrap}");

        string position = $"{(int)Player.LocalShipController.Position.x} / {-(int)Player.LocalShipController.Position.y}";
        if (Player.LocalShipController.Position != Player.LocalShipController.TargetPosition)
        {
            position += $" > {(int)Player.LocalShipController.TargetPosition.x} / {-(int)Player.LocalShipController.TargetPosition.y}";
        }
        SetText(MapPosition, position);
    }

    public override void ChangeLanguage()
    {
        SetText(MiniMapPanelText, $"{GameSettings.UserLanguage.MINIMAP} -> {Client.Pilot.Map.Name}");
    }
}