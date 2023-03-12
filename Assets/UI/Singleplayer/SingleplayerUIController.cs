using System.Collections.Generic;
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
            SceneManager.LoadScene("Assets/Scenes/Singleplayer/CreateWorld.unity");

        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _worldListEntry = _root.Q<ListView>("WorldListEntry");
        _worldListEntry.selectionChanged += onWorldListEntrySelected;

        _playWorldButton = _root.Q<Button>("playWorldButton");
        _playWorldButton.clicked += onPlayWorldButtonClicked;

        _editWorldButton = _root.Q<Button>("editWorldButton");
        _editWorldButton.clicked += onEditWorldButtonClicked;

        _deleteWorldButton = _root.Q<Button>("deleteWorldButton");
        _deleteWorldButton.clicked += onDeleteWorldButtonClicked;

        _createWorldButton = _root.Q<Button>("createWorldButton");
        _createWorldButton.clicked += onCreateWorldButtonClicked;

        _backButton = _root.Q<Button>("backButton");
        _backButton.clicked += onBackButtonClicked;

        FillWorldList();
    }

    private void onWorldListEntrySelected(IEnumerable<object> selectedItems)
    {
        _playWorldButton.RemoveFromClassList("btn-disabled");
        _editWorldButton.RemoveFromClassList("btn-disabled");
        _deleteWorldButton.RemoveFromClassList("btn-disabled");

        var selectedWorld = _worldListEntry.selectedItem as World;
        worldData.Initialize(selectedWorld);
    }

    private void onPlayWorldButtonClicked()
    {
        if (!_playWorldButton.ClassListContains("btn-disabled"))
            SceneManager.LoadScene("Assets/Scenes/Gameplay/Gameplay.unity");
    }

    private void onEditWorldButtonClicked()
    {
        if (!_editWorldButton.ClassListContains("btn-disabled"))
            SceneManager.LoadScene("Assets/Scenes/Singleplayer/EditWorld.unity");
    }

    private void onDeleteWorldButtonClicked()
    {
        if (!_deleteWorldButton.ClassListContains("btn-disabled"))
            SceneManager.LoadScene("Assets/Scenes/Singleplayer/DeleteWorld.unity");
    }

    private void onCreateWorldButtonClicked()
    {
        SceneManager.LoadScene("Assets/Scenes/Singleplayer/CreateWorld.unity");
    }

    private void onBackButtonClicked()
    {
        SceneManager.LoadScene("Assets/Scenes/Main/Main.unity");
    }

    void EnumerateAllWorlds()
    {
        WorldList = new List<World>();
        WorldList.AddRange(SaveManager.LoadWorldDataListEntry());
    }

    void FillWorldList()
    {
        _worldListEntry.makeItem = () =>
        {
            var newWorldListEntry = WorldListTemplate.Instantiate();

            var newWorldListEntryLogic = new WorldListEntryController();

            newWorldListEntry.userData = newWorldListEntryLogic;
            newWorldListEntryLogic.SetVisualElement(newWorldListEntry);

            int clickCount = 0;
            float lastClickTime = 0f;

            // Add click handler to the new item
            newWorldListEntry.RegisterCallback<PointerDownEvent>(evt =>
            {
                if (evt.clickCount == 2)
                {
                    // Handle double-click here
                    Debug.Log("Item double-clicked!");
                    evt.StopPropagation();
                }
                else
                {
                    clickCount++;
                    lastClickTime = Time.realtimeSinceStartup;
                }
            });

            newWorldListEntry.RegisterCallback<PointerUpEvent>(evt =>
            {
                if (clickCount == 1 && Time.realtimeSinceStartup - lastClickTime < 0.2f)
                {
                    // Handle single-click here
                    Debug.Log("Item single-clicked!");
                }
                clickCount = 0;
            });

            return newWorldListEntry;
        };

        _worldListEntry.bindItem = (item, index) =>
        {
            (item.userData as WorldListEntryController).SetWorldData(WorldList[index]);
        };

        _worldListEntry.itemsSource = WorldList;
    }

}
