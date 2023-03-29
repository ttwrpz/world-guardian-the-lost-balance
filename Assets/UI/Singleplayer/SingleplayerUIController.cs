using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class SingleplayerUIController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;
    private ListView _worldListEntry;
    private Button _playWorldButton;
    private Button _editWorldButton;
    private Button _deleteWorldButton;
    private Button _createWorldButton;
    private Button _backButton;

    [SerializeField]
    private VisualTreeAsset WorldListTemplate;
    List<World> WorldList;

    [SerializeField]
    private WorldData worldData;

    private void OnEnable()
    {
        worldData.ClearData();

        EnumerateAllWorlds();
        if (WorldList.Count == 0)
        {
            SceneManager.LoadScene("Assets/Scenes/Singleplayer/CreateWorld.unity");
        }

        GetUIElements();
        AttachEventHandlers();

        InitializeWorldList();
    }

    private void GetUIElements()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _worldListEntry = _root.Q<ListView>("WorldListEntry");
        _playWorldButton = _root.Q<Button>("playWorldButton");
        _playWorldButton.SetEnabled(false);

        _editWorldButton = _root.Q<Button>("editWorldButton");
        _editWorldButton.SetEnabled(false);

        _deleteWorldButton = _root.Q<Button>("deleteWorldButton");
        _deleteWorldButton.SetEnabled(false);

        _createWorldButton = _root.Q<Button>("createWorldButton");
        _backButton = _root.Q<Button>("backButton");
    }
    private void AttachEventHandlers()
    {
        _worldListEntry.selectionChanged += onWorldListEntrySelected;
        _playWorldButton.clicked += onPlayWorldButtonClicked;
        _editWorldButton.clicked += onEditWorldButtonClicked;
        _deleteWorldButton.clicked += onDeleteWorldButtonClicked;
        _createWorldButton.clicked += onCreateWorldButtonClicked;
        _backButton.clicked += onBackButtonClicked;
    }

    private void onWorldListEntrySelected(IEnumerable<object> selectedItems)
    {
        _playWorldButton.RemoveFromClassList("btn-disabled");
        _playWorldButton.SetEnabled(true);

        _editWorldButton.RemoveFromClassList("btn-disabled");
        _editWorldButton.SetEnabled(true);

        _deleteWorldButton.RemoveFromClassList("btn-disabled");
        _deleteWorldButton.SetEnabled(true);

        var selectedWorld = _worldListEntry.selectedItem as World;
        worldData.Initialize(selectedWorld);
    }

    private async void onPlayWorldButtonClicked()
    {
        if (!_playWorldButton.enabledSelf)
            return;

        await UIManager.LoadSceneAsync("Gameplay/Gameplay");
    }

    private async void onEditWorldButtonClicked()
    {
        if (!_editWorldButton.enabledSelf)
            return;
        
        await UIManager.LoadSceneAsync("Singleplayer/EditWorld");
    }

    private async void onDeleteWorldButtonClicked()
    {
        if (!_deleteWorldButton.enabledSelf)
            return;

        await UIManager.LoadSceneAsync("Singleplayer/DeleteWorld");
    }

    private async void onCreateWorldButtonClicked()
    {
        await UIManager.LoadSceneAsync("Singleplayer/CreateWorld");
    }

    private void onBackButtonClicked()
    {
        UIManager.BackToMainMenuUI();
    }

    void EnumerateAllWorlds()
    {
        WorldList = new List<World>();
        WorldList.AddRange(SaveManager.LoadWorldDataListEntry());
    }

    private readonly Stack<VisualElement> _worldListEntryPool = new();

    private VisualElement GetWorldListEntry()
    {
        if (_worldListEntryPool.Count > 0)
        {
            return _worldListEntryPool.Pop();
        }
        else
        {
            return WorldListTemplate.Instantiate();
        }
    }

    void InitializeWorldList()
    {
        _worldListEntry.makeItem = () =>
        {
            var newListEntry = GetWorldListEntry();

            if (newListEntry.userData is not WorldListEntryController newListEntryLogic)
            {
                newListEntryLogic = new WorldListEntryController();
                newListEntry.userData = newListEntryLogic;
                newListEntryLogic.SetVisualElement(newListEntry);
            }

            return newListEntry;
        };

        _worldListEntry.bindItem = (item, index) =>
        {
            if (item.userData is not WorldListEntryController newListEntryLogic)
            {
                newListEntryLogic = new WorldListEntryController();
                item.userData = newListEntryLogic;
                newListEntryLogic.SetVisualElement(item);
            }

            newListEntryLogic.SetData(WorldList[index]);
        }; 

        _worldListEntry.itemsSource = WorldList;
    }

}
