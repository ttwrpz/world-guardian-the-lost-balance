using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainUIController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;
    private Label _versionLabel;
    private Button _singleplayerButton;
    private Button _achievementButton;
    private Button _collectibleButton;
    private Button _creditButton;
    private Button _settingButton;
    private Button _quitButton;

    private void OnEnable()
    {
        GetUIElements();
        AttachEventHandlers();
    }

    private void GetUIElements()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _versionLabel = _root.Q<Label>("Version");
        _versionLabel.text = _versionLabel.text.Replace("%s", Application.version);

        _singleplayerButton = _root.Q<Button>("SingleplayerButton");
        _achievementButton = _root.Q<Button>("AchievementButton");
        _collectibleButton = _root.Q<Button>("CollectibleButton");
        _creditButton = _root.Q<Button>("CreditButton");
        _settingButton = _root.Q<Button>("SettingButton");
        _quitButton = _root.Q<Button>("QuitButton");
    }

    private void AttachEventHandlers()
    {
        _singleplayerButton.clicked += onSingleplayerButtonClicked;
        _achievementButton.clicked += onAchievementButtonClicked;
        _collectibleButton.clicked += onCollectibleButtonClicked;
        _creditButton.clicked += onCreditButtonButtonClicked;
        _settingButton.clicked += onSettingButtonButtonClicked;
        _quitButton.clicked += onQuitButtonClicked;
    }

    private async void onSingleplayerButtonClicked()
    {
        await UIManager.LoadSceneAsync("Singleplayer/Singleplayer");
    }

    private async void onAchievementButtonClicked()
    {
        await UIManager.LoadSceneAsync("Achievement/Achievement");
    }

    private async void onCollectibleButtonClicked()
    {
        await UIManager.LoadSceneAsync("Collectible/Collectible");
    }

    private async void onCreditButtonButtonClicked()
    {
        await UIManager.LoadSceneAsync("Main/Credit");
    }

    private async void onSettingButtonButtonClicked()
    {
        await UIManager.LoadSceneAsync("Setting/Setting");
    }

    private void onQuitButtonClicked()
    {
        Application.Quit();
    }
}
