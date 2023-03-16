using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static Collectible;

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

    private AchievementManager _achievementManager;

    private void OnEnable()
    {
        _achievementManager = FindFirstObjectByType<AchievementManager>();
        if (_achievementManager == null)
        {
            Debug.LogError("AchievementManager not found in the scene.");
            return;
        }

        GetUIElements();
        StartCoroutine(EnumerateAndInitialize());
    }
    private IEnumerator EnumerateAndInitialize()
    {
        yield return StartCoroutine(EnumerateAllAchievements());
        UpdateUIElements();
        AttachEventHandlers();
        InitializeAchievementList();
    }

    private void GetUIElements()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _achievementList = _root.Q<ListView>("achievementList");

        _subtitleScreenLabel = _root.Q<Label>("subtitleScreenLabel");

        _achievementProgressBar = _root.Q<ProgressBar>("achievementProgressBar");

        _backButton = _root.Q<Button>("backButton");
    }

    private void UpdateUIElements()
    {
        _subtitleScreenLabel.text = _subtitleScreenLabel.text
                .Replace("%1", UnlockedAchievementList.Count().ToString())
                .Replace("%2", AchievementList.Count().ToString());

        _achievementProgressBar.value = CalculateProgressPercentage();
    }

    private void AttachEventHandlers()
    {
        _backButton.clicked += onBackButtonClicked;
    }

    private float CalculateProgressPercentage()
    {
        int collectedCount = AchievementList.Count();
        int totalCount = UnlockedAchievementList.Count();

        if (totalCount == 0)
            return 0f;

        return ((float)collectedCount / totalCount) * 100f;
    }

    private void onBackButtonClicked()
    {
        UIController.BackToMainUI();
    }

    IEnumerator EnumerateAllAchievements()
    {
        yield return new WaitUntil(() => _achievementManager.IsLoaded);

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
    }
}
