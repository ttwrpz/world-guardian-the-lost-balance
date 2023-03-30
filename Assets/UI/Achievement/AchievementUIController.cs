using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class AchievementUIController : UIController
{
    [SerializeField]
    private AchievementManager _achievementManager;

    private Label _subtitleScreenLabel;
    private ProgressBar _achievementProgressBar;
    private ListView _achievementList;
    private Button _backButton;

    [SerializeField]
    private VisualTreeAsset AchievementListEntryTemplate;
    List<Achievement> AchievementList;
    List<Achievement> UnlockedAchievementList;

    protected override void OnEnable()
    {
        _achievementManager.LoadSavedAchievements();
        _achievementManager.LoadAchievementsFromResources();

        base.OnEnable();

        EnumerateAllAchievements();
        InitializeAchievementList();
    }

    protected override void GetUIElements()
    {
        _achievementList = _root.Q<ListView>("AchievementList");

        _subtitleScreenLabel = _root.Q<Label>("SubtitleScreenLabel");

        _achievementProgressBar = _root.Q<ProgressBar>("AchievementProgressBar");

        _backButton = _root.Q<Button>("BackButton");
    }

    private void UpdateUIElements()
    {
        _subtitleScreenLabel.text = _subtitleScreenLabel.text
                .Replace("%1", UnlockedAchievementList.Count().ToString())
                .Replace("%2", AchievementList.Count().ToString());

        _achievementProgressBar.value = CalculateProgressPercentage();
    }

    protected override void AttachEventHandlers()
    {
        _backButton.clicked += onBackButtonClicked;
    }

    private float CalculateProgressPercentage()
    {
        return (float)UnlockedAchievementList.Count() / AchievementList.Count() * 100f;
    }

    private void onBackButtonClicked()
    {
        UIManager.BackToMainMenuUI();
    }

    void EnumerateAllAchievements()
    {
        AchievementList = _achievementManager.GetAchievements();
        UnlockedAchievementList = _achievementManager.GetUnlockedAchievements();
    }

    private readonly Stack<VisualElement> _AchievementListEntryPool = new();

    private VisualElement GetAchievementListEntry()
    {
        if (_AchievementListEntryPool.Count > 0)
        {
            return _AchievementListEntryPool.Pop();
        }
        else
        {
            return AchievementListEntryTemplate.Instantiate();
        }
    }

    void InitializeAchievementList()
    {

        _achievementList.makeItem = () =>
        {
            var newListEntry = GetAchievementListEntry();

            if (newListEntry.userData is not AchievementListEntryController newListEntryLogic)
            {
                newListEntryLogic = new AchievementListEntryController();
                newListEntry.userData = newListEntryLogic;
                newListEntryLogic.SetVisualElement(newListEntry);
            }

            return newListEntry;
        };

        _achievementList.bindItem = (item, index) =>
        {
            if (item.userData is not AchievementListEntryController newListEntryLogic)
            {
                newListEntryLogic = new AchievementListEntryController();
                item.userData = newListEntryLogic;
                newListEntryLogic.SetVisualElement(item);
            }

            newListEntryLogic.SetData(AchievementList[index]);
        };

        _achievementList.itemsSource = AchievementList;

        UpdateUIElements();
    }
}
