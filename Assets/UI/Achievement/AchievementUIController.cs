using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class AchievementUIController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;
    private Label _subtitleScreenLabel;
    private ProgressBar _achievementProgressBar;
    private ListView _achievementList;
    private Button _backButton;

    [SerializeField]
    private VisualTreeAsset AchievementListEntryTemplate;
    List<Achievement> AchievementList;
    List<Achievement> UnlockedAchievementList;

    private void OnEnable()
    {
        EnumerateAllAchievements();
        GetUIElements();
        AttachEventHandlers();
        InitializeAchievementList();
    }

    private void GetUIElements()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _subtitleScreenLabel = _root.Q<Label>("subtitleScreenLabel");
        _subtitleScreenLabel.text = _subtitleScreenLabel.text.Replace("%1", UnlockedAchievementList.Count().ToString()).Replace("%2", AchievementList.Count().ToString());

        _achievementProgressBar = _root.Q<ProgressBar>("achievementProgressBar");
        _achievementProgressBar.value = AchievementList.Count() != 0 ? (UnlockedAchievementList.Count() / AchievementList.Count()) * 100f : 0f;

        _achievementList = _root.Q<ListView>("achievementList");

        _backButton = _root.Q<Button>("backButton");
    }

    private void AttachEventHandlers()
    {
        _backButton.clicked += onBackButtonClicked;
    }

    private void onBackButtonClicked()
    {
        UIController.BackToMainUI();
    }

    void EnumerateAllAchievements()
    {
        AchievementList = new();
        AchievementList.AddRange(AchievementManager.GetAchievements());
        UnlockedAchievementList = AchievementList.Count > 0 ? AchievementList.Where(a => a.IsUnlocked).ToList() : new();
    }

    private readonly Queue<VisualElement> _AchievementListEntryPool = new();

    private VisualElement GetAchievementListEntry()
    {
        if (_AchievementListEntryPool.Count > 0)
        {
            return _AchievementListEntryPool.Dequeue();
        }
        else
        {
            return AchievementListEntryTemplate.Instantiate();
        }
    }

    private void ReleaseAchievementListEntry(VisualElement worldListEntry)
    {
        _AchievementListEntryPool.Enqueue(worldListEntry);
    }

    void InitializeAchievementList()
    {
        _achievementList.makeItem = () =>
        {
            var newListEntry = GetAchievementListEntry();

            var newListEntryLogic = new AchievementListEntryController();

            newListEntry.userData = newListEntryLogic;
            newListEntryLogic.SetVisualElement(newListEntry);

            return newListEntry;
        };

        _achievementList.bindItem = (item, index) =>
        {
            (item.userData as AchievementListEntryController).SetData(AchievementList[index]);
        };

        _achievementList.itemsSource = AchievementList;
    }
}
