using UnityEngine;
using UnityEngine.UI;

public class QuitWindow : GameWindow
{
    [Header("Question")]
    public Text ExitQuestionText;

    [Header("Yes")]
    public Button YesButton;
    public Text YesButtonText;

    [Header("No")]
    public Button NoButton;
    public Text NoButtonText;



    public override void Start()
    {
        base.Start();

        ButtonListener(YesButton, YesButton_Clicked);
        ButtonListener(NoButton, NoButton_Clicked);
    }

    public override void Refresh()
    {
        base.Refresh();
    }

    public override void ChangeLanguage()
    {
        base.ChangeLanguage();

        SetText(ExitQuestionText, GameSettings.UserLanguage.EXIT_QUESTION);
        SetText(YesButtonText, GameSettings.UserLanguage.YES);
        SetText(NoButtonText, GameSettings.UserLanguage.NO);
    }


    private void YesButton_Clicked()
    {
        Application.Quit();
    }

    private void NoButton_Clicked()
    {
        CloseButton_Clicked();
    }
}