using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CollectibleViewUIController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;
    private ProgressBar _collectibleProgressBar;
    private ListView _collectibleList;
    private Button _backButton;
    private Button _endingButton;
    private Button _itemButton;
    private Button _landmarkButton;
    private Button _loreButton;
    private Label _endingLabel;
    private Label _itemLabel;
    private Label _landmarkLabel;
    private Label _loreLabel;

    [SerializeField]
    private VisualTreeAsset CollectibleListEntryTemplate;
    List<CollectibleSO> CollectibleList;

    private void OnEnable()
    {
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
            .Replace("%s1", SceneManager.GetActiveScene().name + "s")
            .Replace("%s2", "");

        _collectibleList = _root.Q<ListView>("CollectibleList");

        _endingButton = _root.Q<Button>("EndingButton");
        _endingLabel = _root.Q<Label>("EndingLabel");
        _endingLabel.text = _endingLabel.text
            .Replace("%s1", "")
            .Replace("%s2", "");

        _itemButton = _root.Q<Button>("ItemButton");
        _itemLabel = _root.Q<Label>("ItemLabel");
        _itemLabel.text = _itemLabel.text
            .Replace("%s1", "")
            .Replace("%s2", "");

        _landmarkButton = _root.Q<Button>("LandmarkButton");
        _landmarkLabel = _root.Q<Label>("LandmarkLabel");
        _landmarkLabel.text = _landmarkLabel.text
            .Replace("%s1", "")
            .Replace("%s2", "");

        _loreButton = _root.Q<Button>("LoreButton");
        _loreLabel = _root.Q<Label>("LoreLabel");
        _loreLabel.text = _loreLabel.text
            .Replace("%s1", "")
            .Replace("%s2", "");

        _backButton = _root.Q<Button>("BackButton");

    }

    private void AttachEventHandlers()
    {
        _endingButton.clicked += CollectibleUIEventHandlers.onEndingButtonClicked;
        _itemButton.clicked += CollectibleUIEventHandlers.onItemButtonClicked;
        _landmarkButton.clicked += CollectibleUIEventHandlers.onLandmarkButtonClicked;
        _loreButton.clicked += CollectibleUIEventHandlers.onLoreButtonClicked;
        _backButton.clicked += CollectibleUIEventHandlers.onBackToCollectibleButtonClicked;
    }

    void EnumerateAllCollectibles()
    {
        CollectibleList = new();
        //CollectibleList.AddRange(CollectibleManager);
    }

    private readonly Queue<VisualElement> _CollectibleListEntryPool = new();

    private VisualElement GetCollectibleListEntry()
    {
        if (_CollectibleListEntryPool.Count > 0)
        {
            return _CollectibleListEntryPool.Dequeue();
        }
        else
        {
            return CollectibleListEntryTemplate.Instantiate();
        }
    }

    private void ReleaseCollectibleListEntry(VisualElement worldListEntry)
    {
        _CollectibleListEntryPool.Enqueue(worldListEntry);
    }

    void InitializeCollectibleList()
    {
        _collectibleList.makeItem = () =>
        {
            var newListEntry = GetCollectibleListEntry();

            var newListEntryLogic = new CollectibleListEntryController();

            newListEntry.userData = newListEntryLogic;
            newListEntryLogic.SetVisualElement(newListEntry);

            return newListEntry;
        };

        _collectibleList.bindItem = (item, index) =>
        {
            (item.userData as CollectibleListEntryController).SetData(CollectibleList[index]);
        };

        _collectibleList.itemsSource = CollectibleList;
    }
}
