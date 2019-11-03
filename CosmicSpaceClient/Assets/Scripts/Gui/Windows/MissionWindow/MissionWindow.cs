using CosmicSpaceCommunication.Game.Quest;
using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MissionWindow : GameWindow
{
    public static Dictionary<uint, QuestTask> Tasks;

    [Header("Buttons")]
    public Button CancelButton;
    public Button AcceptButton;

    [Header("Buttons Text")]
    public Text CancelText;
    public Text AcceptText;

    [Header("Tasks List")]
    public Transform TasksTransform;
    public GameObject TaskPrefab;

    [Header("Selected task Panel")]
    public Transform SelectedTaskTransform;
    public Text TaskNameText;
    public Text TaskRewardText;
    public Transform ContentQuestsTransform;
    public Transform ContentRewardsTransform;
    public GameObject PropertyPrefab;



    public override void Start()
    {
        base.Start();

        ButtonListener(CancelButton, CancelButton_Cliecked);
        ButtonListener(AcceptButton, AcceptButton_Cliecked);

        ShowInformation(null);
        CreateQuestList();
    }

    public override void Refresh()
    {
        base.Refresh();

        SetText(TaskNameText, GameSettings.UserLanguage.CANCEL);
    }

    public override void ChangeLanguage()
    {
        base.ChangeLanguage();

        SetText(CancelText, GameSettings.UserLanguage.CANCEL);
        SetText(AcceptText, GameSettings.UserLanguage.ACCEPT);
        SetText(TaskRewardText, GameSettings.UserLanguage.REWARDS);
    }

    private void CreateQuestList()
    {
        Player.DestroyChilds(TasksTransform);

        foreach (QuestTask task in Tasks.Values)
        {
            GameObject go = Instantiate(TaskPrefab, TasksTransform);

            go.GetComponent<QuestItem>().Quest = task;
            go.GetComponent<QuestItem>().OnShowInformation = ShowInformation;
        }
    }

    QuestTask SelectedTask = null;
    private void ShowInformation(QuestTask task)
    {
        SelectedTask = task;

        if (SelectedTask == null)
        {
            SelectedTaskTransform.gameObject.SetActive(false);
            return;
        }
        SelectedTaskTransform.gameObject.SetActive(true);

        SetText(TaskNameText, SelectedTask.Name);

        Player.DestroyChilds(ContentQuestsTransform);
        foreach (Quest quest in SelectedTask.Quests)
        {
            string questName = UserInterfaceWindow.FindQuestType(quest.QuestType, quest.TargetId);

            if (quest.Maps?.Count > 0)
            {
                foreach (var item in quest.Maps)
                {
                    questName += $"{System.Environment.NewLine}      <color=#C0C0C0>{GameSettings.UserLanguage.Q_ON_MAP}: {Client.ServerResources.Maps[long.Parse(item.ToString())].Name}</color>";
                }
            }

            Instantiate(PropertyPrefab, ContentQuestsTransform).GetComponent<ToolTipProperty>().SetProperty(questName, quest.Quantity.ToString());
        }

        Player.DestroyChilds(ContentRewardsTransform);

        if (SelectedTask.Reward.Experience.HasValue)
        {
            Instantiate(PropertyPrefab, ContentRewardsTransform).GetComponent<ToolTipProperty>().SetProperty(GameSettings.UserLanguage.EXPERIENCE, SelectedTask.Reward.Experience.ToString());
        }
        if (SelectedTask.Reward.Metal.HasValue)
        {
            Instantiate(PropertyPrefab, ContentRewardsTransform).GetComponent<ToolTipProperty>().SetProperty("Metal", SelectedTask.Reward.Metal.ToString());
        }
        if (SelectedTask.Reward.Scrap.HasValue)
        {
            Instantiate(PropertyPrefab, ContentRewardsTransform).GetComponent<ToolTipProperty>().SetProperty("Scrap", SelectedTask.Reward.Scrap.ToString());
        }
        if(SelectedTask.Reward.AmmunitionId.HasValue)
        {
            Instantiate(PropertyPrefab, ContentRewardsTransform).GetComponent<ToolTipProperty>().SetProperty($"{GameSettings.UserLanguage.RESOURCE} {SelectedTask.Reward.AmmunitionId}", SelectedTask.Reward.AmmunitionQuantity.ToString());
        }

        if(SelectedTask.Reward.Items != null && SelectedTask.Reward.Items.Count > 0)
        {
            foreach (ItemReward item in SelectedTask.Reward.Items)
            {
                Instantiate(PropertyPrefab, ContentRewardsTransform).GetComponent<ToolTipProperty>().SetProperty($"{item.Item.Name} [{item.UpgradeLevel} lvl]", $"{GameSettings.UserLanguage.CHANCE} {item.Chance/10}%");
            }
        }
    }

    private void CancelButton_Cliecked()
    {
        Debug.LogError(nameof(CancelButton_Cliecked));
    }

    private void AcceptButton_Cliecked()
    {
        Debug.LogError(nameof(AcceptButton_Cliecked));
    }
}