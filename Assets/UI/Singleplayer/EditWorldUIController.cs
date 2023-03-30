using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class EditWorldUIController : MonoBehaviour
{
    private UIDocument _docs;
    private VisualElement _root;
    private Label _subtitleScreenLabel;
    private TextField _worldNameInput;
    private Label _worldFolderLabel;
    private Button _openSaveFolderButton;
    private Button _backupWorldButton;
    private Button _openBackupFolderButton;
    private Button _saveButton;
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
        _docs = GetComponent<UIDocument>();
        _root = _docs.rootVisualElement;

        _subtitleScreenLabel = _root.Q<Label>("subtitleScreenLabel");
        _subtitleScreenLabel.text = _subtitleScreenLabel.text.Replace("%s", worldData.WorldName);

        _worldNameInput = _root.Q<TextField>("worldNameInput");
        _worldNameInput.SetValueWithoutNotify(worldData.WorldName);

        _worldFolderLabel = _root.Q<Label>("worldFolderLabel");
        _worldFolderLabel.text = _worldFolderLabel.text.Replace("%s", worldData.WorldFolder);

        _openSaveFolderButton = _root.Q<Button>("openSaveFolderButton");
        _backupWorldButton = _root.Q<Button>("backupWorldButton");
        _openBackupFolderButton = _root.Q<Button>("openBackupFolderButton");
        _saveButton = _root.Q<Button>("saveButton");
        _backButton = _root.Q<Button>("backButton");
    }

    private void AttachEventHandlers()
    {
        _worldNameInput.RegisterCallback<ChangeEvent<string>>(onWorldNameInputChanged);
        _openSaveFolderButton.clicked += onOpenSaveFolderButtonClicked;
        _backupWorldButton.clicked += onBackupWorldButtonClicked;
        _openBackupFolderButton.clicked += onOpenBackupFolderButtonClicked;
        _saveButton.clicked += onSaveButtonClicked;
        _backButton.clicked += onBackButtonClicked;
    }

    private void onWorldNameInputChanged(ChangeEvent<string> changeEvent)
    {
        if (changeEvent.newValue.ToString() == "")
        {
            _saveButton.SetEnabled(false);
            _saveButton.AddToClassList("btn-disabled");
        }
        else
        {
            _saveButton.SetEnabled(true);
            _saveButton.RemoveFromClassList("btn-disabled");
        }
    }

    private void onOpenSaveFolderButtonClicked()
    {
        System.Diagnostics.Process.Start("explorer.exe", Path.GetFullPath(Path.Join(SaveManager.SavePath, worldData.WorldFolder)));
    }

    private void onBackupWorldButtonClicked()
    {
        World world = new()
        {
            WorldName = worldData.WorldName,
            WorldFolder = worldData.WorldFolder,
        };
        string backupName = SaveManager.BackupWorld(world);
        _worldFolderLabel.text = $"Your world was saved under {backupName}";
        //Show Toast UI
    }

    private void onOpenBackupFolderButtonClicked()
    {
        if (!Directory.Exists(SaveManager.BackupPath))
            Directory.CreateDirectory(SaveManager.BackupPath);

        System.Diagnostics.Process.Start("explorer.exe", Path.GetFullPath(SaveManager.BackupPath));
    }

    private async void onSaveButtonClicked()
    {
        if (!_saveButton.enabledSelf)
            return;

        World world = new()
        {
            WorldName = _worldNameInput.text,
            WorldFolder = worldData.WorldFolder,
        };
        
        SaveManager.SaveWorldData(world);
        await UIManager.LoadSceneAsync("Singleplayer/Singleplayer");
    }

    private async void onBackButtonClicked()
    {
        await UIManager.LoadSceneAsync("Singleplayer/Singleplayer");
    }
}
