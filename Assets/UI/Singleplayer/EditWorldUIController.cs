using System.Diagnostics;
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
        _docs = GetComponent<UIDocument>();
        _root = _docs.rootVisualElement;

        _subtitleScreenLabel = _root.Q<Label>("subtitleScreenLabel");
        _subtitleScreenLabel.text = _subtitleScreenLabel.text.Replace("%s", worldData.WorldName);

        _worldNameInput = _root.Q<TextField>("worldNameInput");
        _worldNameInput.SetValueWithoutNotify(worldData.WorldName);

        _worldFolderLabel = _root.Q<Label>("worldFolderLabel");
        _worldFolderLabel.text = _worldFolderLabel.text.Replace("%s", worldData.WorldFolder);

        _openSaveFolderButton = _root.Q<Button>("openSaveFolderButton");
        _openSaveFolderButton.clicked += onOpenSaveFolderButtonClicked;

        _backupWorldButton = _root.Q<Button>("backupWorldButton");
        _backupWorldButton.clicked += onBackupWorldButtonClicked;

        _openBackupFolderButton = _root.Q<Button>("openBackupFolderButton");
        _openBackupFolderButton.clicked += onOpenBackupFolderButtonClicked;

        _saveButton = _root.Q<Button>("saveButton");
        _saveButton.clicked += onSaveButtonClicked;

        _backButton = _root.Q<Button>("backButton");
        _backButton.clicked += onBackButtonClicked;
    }

    private void onOpenSaveFolderButtonClicked()
    {
        Process.Start("explorer.exe", Path.GetFullPath(Path.Join(SaveManager._savePath, worldData.WorldFolder)));
    }

    private void onBackupWorldButtonClicked()
    {
        World world = new()
        {
            worldName = worldData.WorldName,
            worldFolder = worldData.WorldFolder,
        };
        string backupName = SaveManager.BackupWorld(world);
        //Show Toast UI
    }

    private void onOpenBackupFolderButtonClicked()
    {
        if (!Directory.Exists(SaveManager._backupPath))
            Directory.CreateDirectory(SaveManager._backupPath);

        Process.Start("explorer.exe", Path.GetFullPath(SaveManager._backupPath));
    }

    private void onSaveButtonClicked()
    {
        World world = new()
        {
            worldName = _worldNameInput.text,
            worldFolder = worldData.WorldFolder,
        };
        //Save Not Working
        SaveManager.SaveWorldData(world);
        SceneManager.LoadScene("Assets/Scenes/Singleplayer/Singleplayer.unity");
    }

    private void onBackButtonClicked()
    {
        SceneManager.LoadScene("Assets/Scenes/Singleplayer/Singleplayer.unity");
    }
}
