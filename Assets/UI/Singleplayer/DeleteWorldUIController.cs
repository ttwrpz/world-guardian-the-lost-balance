using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DeleteWorldUIController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;
    private Label _subtitleScreenLabel;
    private Label _deleteLabel;
    private Button _deleteButton;
    private Button _backButton;

    [SerializeField]
    private WorldData worldData;

    private void OnEnable()
    {
        GetUIElements();
        AttachEventHandlers();
    }

    private void GetUIElements()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _subtitleScreenLabel = _root.Q<Label>("subtitleScreenLabel");
        _subtitleScreenLabel.text = _subtitleScreenLabel.text.Replace("%s", worldData.WorldName);

        _deleteLabel = _root.Q<Label>("deleteLabel");
        _deleteLabel.text = _deleteLabel.text.Replace("%s", worldData.WorldName);

        _deleteButton = _root.Q<Button>("deleteButton");
        _backButton = _root.Q<Button>("backButton");
    }

    private void AttachEventHandlers()
    {
        _deleteButton.clicked += onDeleteWorldButtonClicked;
        _backButton.clicked += onBackButtonClicked;
    }

    private async void onDeleteWorldButtonClicked()
    {
        World world = new()
        {
            WorldName = worldData.WorldName,
            WorldFolder = worldData.WorldFolder
        };
        SaveManager.DeleteWorld(world);
        await UIManager.LoadSceneAsync("Singleplayer/Singleplayer");
    }

    private async void onBackButtonClicked()
    {
        await UIManager.LoadSceneAsync("Singleplayer/Singleplayer");
    }
}
