using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static Collectible;

public class CollectibleViewUIController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;
    private ProgressBar _collectibleProgressBar;
    private ListView _collectibleList;
    private Button _backButton;

    private string _sceneName;
    private CollectibleType _sceneNameEnum;

    private CollectibleManager _collectibleManager;

    [SerializeField]
    private VisualTreeAsset CollectibleListEntryTemplate;
    List<Collectible> CollectibleList;

    private void OnEnable()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        _sceneNameEnum = (CollectibleType)Enum.Parse(typeof(CollectibleType), _sceneName);

        _collectibleManager = FindFirstObjectByType<CollectibleManager>();
        if (_collectibleManager == null)
        {
            Debug.LogError("AchievementManager not found in the scene.");
            return;
        }

        EnumerateAllCollectibles();
        GetUIElements();
        AttachEventHandlers();
        InitializeCollectibleList();
    }

    private void GetUIElements()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _collectibleProgressBar = _root.Q<ProgressBar>("CollectibleProgressBar");
        _collectibleProgressBar.title = _collectibleProgressBar.title
            .Replace("%s1", _sceneName)
            .Replace("%s2", CalculateProgressPercentage(_sceneNameEnum).ToString());
        _collectibleProgressBar.value = CalculateProgressPercentage(_sceneNameEnum);

        _collectibleList = _root.Q<ListView>("CollectibleList");
        _backButton = _root.Q<Button>("BackButton");

    }

    private void AttachEventHandlers()
    {
        _backButton.clicked += CollectibleUIEventHandlers.onBackToCollectibleButtonClicked;
    }

    private float CalculateProgressPercentage(CollectibleType type)
    {
        int collectedCount = _collectibleManager.GetCollectedCountByType(type);
        int totalCount = _collectibleManager.GetTotalCountByType(type);

        if (totalCount == 0)
            return 0f;

        return ((float)collectedCount / totalCount) * 100f;
    }

    void EnumerateAllCollectibles()
    {
        CollectibleList = new();
        CollectibleList.AddRange(_collectibleManager.LoadCollectiblesByType(_sceneNameEnum));
    }

    private readonly Stack<VisualElement> _CollectibleListEntryPool = new();

    private VisualElement GetCollectibleListEntry()
    {
        if (_CollectibleListEntryPool.Count > 0)
        {
            return _CollectibleListEntryPool.Pop();
        }
        else
        {
            return CollectibleListEntryTemplate.Instantiate();
        }
    }

    void InitializeCollectibleList()
    {
        _collectibleList.makeItem = () =>
        {
            var newListEntry = GetCollectibleListEntry();

            if (newListEntry.userData is not CollectibleListEntryController newListEntryLogic)
            {
                newListEntryLogic = new CollectibleListEntryController();
                newListEntry.userData = newListEntryLogic;
                newListEntryLogic.SetVisualElement(newListEntry);
            }

            return newListEntry;
        };

        _collectibleList.bindItem = (item, index) =>
        {
            if (item.userData is not CollectibleListEntryController newListEntryLogic)
            {
                newListEntryLogic = new CollectibleListEntryController();
                item.userData = newListEntryLogic;
                newListEntryLogic.SetVisualElement(item);
            }

            newListEntryLogic.SetData(CollectibleList[index]);
        };

        _collectibleList.itemsSource = CollectibleList;
    }
}
