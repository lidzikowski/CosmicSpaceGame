using CosmicSpaceCommunication;
using CosmicSpaceCommunication.Game.Quest;
using CosmicSpaceCommunication.Game.Resources;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MissionWindow : GameWindow
{
    public static Dictionary<uint, QuestTask> Tasks;
    private List<PilotProgressTask> progressTasks;
    public List<PilotProgressTask> ProgressTasks
    {
        get => progressTasks;
        set
        {
            progressTasks = value;

            if(value == null)
            {
                Debug.LogError(nameof(System.NullReferenceException));
                return;
            }

            StopCoroutine(nameof(DisableButtons));

            CreateQuestList();
        }
    }

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

        Player.DestroyChilds(TasksTransform);

        Client.SendToSocket(new CommandData()
        {
            Command = Commands.GetProgressTasks
        });

        ButtonListener(CancelButton, CancelButton_Cliecked);
        ButtonListener(AcceptButton, AcceptButton_Cliecked);

        ShowInformation(null, null);
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

            QuestItem questItem = go.GetComponent<QuestItem>();
            questItem.PilotProgressTask = ProgressTasks.Single(o => o.Id == task.Id);
            questItem.Quest = task;
            questItem.OnShowInformation = ShowInformation;
        }
    }

    QuestTask SelectedTask = null;
    private void ShowInformation(QuestTask task, PilotProgressTask pilotProgressTask)
    {
        SelectedTask = task;

        if (SelectedTask == null || pilotProgressTask == null)
        {
            SelectedTaskTransform.gameObject.SetActive(false);
            return;
        }
        SelectedTaskTransform.gameObject.SetActive(true);

        string taskName = $"{SelectedTask.Name} {(pilotProgressTask.End.HasValue ? "Completed" : string.Empty)}";
        SetText(TaskNameText, taskName);

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

        CancelButton.interactable = pilotProgressTask.Start.HasValue && !pilotProgressTask.End.HasValue;
        AcceptButton.interactable = !pilotProgressTask.Start.HasValue && !pilotProgressTask.End.HasValue;
    }

    private void CancelButton_Cliecked()
    {
        if (SelectedTask == null)
            Debug.LogError(nameof(System.NullReferenceException));

        StartCoroutine(nameof(DisableButtons));

        Client.SendToSocket(new CommandData()
        {
            Command = Commands.QuestAccept,
            Data = SelectedTask
        });
    }

    private void AcceptButton_Cliecked()
    {
        if (SelectedTask == null)
            Debug.LogError(nameof(System.NullReferenceException));

        StartCoroutine(nameof(DisableButtons));

        Client.SendToSocket(new CommandData()
        {
            Command = Commands.QuestCancel,
            Data = SelectedTask
        });
    }

    System.Collections.IEnumerator DisableButtons()
    {
        bool cancel = CancelButton.interactable;
        bool accept = AcceptButton.interactable;

        CancelButton.interactable = false;
        AcceptButton.interactable = false;

        yield return new WaitForSeconds(5);

        CancelButton.interactable = cancel;
        AcceptButton.interactable = accept;
    }
}