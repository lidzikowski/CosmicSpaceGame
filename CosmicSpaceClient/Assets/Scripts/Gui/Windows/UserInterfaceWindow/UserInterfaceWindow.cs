using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterfaceWindow : GameWindow
{
    [Header("User Panel")]
    public Text UserText;
    public Text MetalText;
    public Text ScrapText;

    [Header("Mini Map")]
    public Text MiniMapPanelText;
    public Text MapPosition;

    [Header("Log")]
    public Transform LogTransform;
    public GameObject LogGameObject;

    [Header("Chat")]
    public Transform ContentTransform;
    public GameObject MessageGameObject;
    public InputField MessageInputField;
    public Button SendMessageButton;



    public override void Start()
    {
        base.Start();

        ButtonListener(SendMessageButton, SendMessageButton_Clicked);
    }

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

    public void CreateLogMessage(string message)
    {
        GameObject go = Instantiate(LogGameObject, LogTransform);
        go.GetComponent<LogScript>().SetText(message);
    }

    void SendMessageButton_Clicked()
    {
        if (string.IsNullOrEmpty(MessageInputField.text))
            return;

        GameObject go = Instantiate(MessageGameObject, ContentTransform);
        go.GetComponent<Text>().text = MessageInputField.text;

        MessageInputField.text = string.Empty;
    }
}