using CosmicSpaceCommunication.Game.Quest;
using UnityEngine;
using UnityEngine.UI;

public delegate void ShowQuestInformation(QuestTask item, PilotProgressTask pilotProgressTask);

public class QuestItem : MonoBehaviour
{
    private QuestTask quest;
    public QuestTask Quest
    {
        get => quest;
        set
        {
            quest = value;
            GetComponent<Text>().text = $"{value?.Name} {PilotProgressTask?.End}";
        }
    }

    public PilotProgressTask PilotProgressTask { get; set; }

    private ShowQuestInformation onShowInformation;
    public ShowQuestInformation OnShowInformation
    {
        get => onShowInformation;
        set
        {
            onShowInformation = value;

            GameWindow.ButtonListener(GetComponent<Button>(), ClickEvent, true);
        }
    }

    private void ClickEvent()
    {
        if (Quest != null)
            OnShowInformation?.Invoke(Quest, PilotProgressTask);
    }
}