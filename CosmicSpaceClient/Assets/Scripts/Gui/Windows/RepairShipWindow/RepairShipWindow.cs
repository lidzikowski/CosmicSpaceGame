using CosmicSpaceCommunication;
using UnityEngine.UI;

public class RepairShipWindow : GameWindow
{
    public Button RepairButton;
    public Text RepairButtonText;
    public Text DestroyedText;
    public Text KillerText;

    public override void Start()
    {
        base.Start();

        ButtonListener(RepairButton, RepairButton_Clicked);
    }

    public override void Refresh()
    {
        base.Refresh();

        RepairButton.interactable = Client.Pilot.IsDead;
        KillerText.text = Player.LocalShipController.KillerBy;
    }

    public override void ChangeLanguage()
    {
        SetText(RepairButtonText, GameSettings.UserLanguage.REPAIR);
        SetText(DestroyedText, GameSettings.UserLanguage.DESTROYED);
    }

    void RepairButton_Clicked()
    {
        Client.SendToSocket(new CommandData()
        {
            Command = Commands.RepairShip
        });
    }
}