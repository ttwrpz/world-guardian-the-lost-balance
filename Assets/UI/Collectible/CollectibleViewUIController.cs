using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static Collectible;

public class CollectibleViewUIController : UIController
{
    private string _sceneName;
    private CollectibleType _sceneNameEnum;

    private Label _subtitleScreenLabel;
    private ProgressBar _collectibleProgressBar;
    private ListView _collectibleList;
    private Button _backButton;

    [SerializeField]
    private CollectibleManager _collectibleManager;

    [SerializeField]
    private VisualTreeAsset CollectibleListEntryTemplate;
    List<Collectible> CollectibleList;

    protected override void OnEnable()
    {
        _sceneName = SceneManager.GetActiveScene().name;
        _sceneNameEnum = (CollectibleType)Enum.Parse(typeof(CollectibleType), _sceneName);

        _collectibleManager.GetCollectedCountByType(_sceneNameEnum);
        _collectibleManager.GetTotalCountByType(_sceneNameEnum);

        base.OnEnable();

        UpdateUIElements();
        EnumerateAllCollectibles();
        InitializeCollectibleList();
    }

    protected override void GetUIElements()
    {
        _subtitleScreenLabel = _root.Q<Label>("SubtitleScreenLabel");
        _collectibleProgressBar = _root.Q<ProgressBar>("CollectibleProgressBar");

        _collectibleList = _root.Q<ListView>("CollectibleList");
        _backButton = _root.Q<Button>("BackButton");

    }

    private void UpdateUIElements()
    {
        _subtitleScreenLabel.text = _subtitleScreenLabel.text
            .Replace("%s1", _collectibleManager.GetCollectedCountByType(_sceneNameEnum).ToString())
            .Replace("%s2", _collectibleManager.GetTotalCountByType(_sceneNameEnum).ToString())
            .Replace("%s3", _sceneName.ToLower());

        _collectibleProgressBar.value = CalculateProgressPercentage(_sceneNameEnum);
    }

    protected override void AttachEventHandlers()
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
