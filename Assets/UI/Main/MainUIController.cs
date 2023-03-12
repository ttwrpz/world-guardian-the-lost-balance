using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainUIController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;
    private Button _singleplayerButton;
    private Button _multiplayerButton;
    private Button _achievementButton;
    private Button _collectibleButton;
    private Button _settingButton;
    private Button _quitButton;
    private Button _howtoButton;
    private Button _creditButton;
    private Button _licenseButton;

    private void OnEnable()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _singleplayerButton = _root.Q<Button>("singleplayerButton");
        _singleplayerButton.clicked += onSingleplayerButtonClicked;

        _multiplayerButton = _root.Q<Button>("multiplayerButton");
        _multiplayerButton.clicked += onMultiplayerButtonClicked;

        _achievementButton = _root.Q<Button>("achievementButton");
        _achievementButton.clicked += onAchievementButtonClicked;

        _collectibleButton = _root.Q<Button>("collectibleButton");
        _collectibleButton.clicked += onCollectibleButtonClicked;

        _settingButton = _root.Q<Button>("settingButton");
        _settingButton.clicked += onSettingButtonClicked;

        _quitButton = _root.Q<Button>("quitButton");
        _quitButton.clicked += onQuitButtonClicked;

        _howtoButton = _root.Q<Button>("howtoButton");
        _howtoButton.clicked += onHowToButtonClicked;

        _creditButton = _root.Q<Button>("creditButton");
        _creditButton.clicked += onCreditButtonClicked;

        _licenseButton = _root.Q<Button>("licenseButton");
        _licenseButton.clicked += onLicenseButtonClicked;
    }

    private void onSingleplayerButtonClicked()
    {
        SceneManager.LoadScene("Assets/Scenes/Singleplayer/Singleplayer.unity");
    }

    private void onMultiplayerButtonClicked()
    {
        SceneManager.LoadScene("Assets/Scenes/Multiplayer/Multiplayer.unity");
    }

    private void onAchievementButtonClicked()
    {
        SceneManager.LoadScene("Assets/Scenes/Acheivement/Acheivement.unity");
    }

    private void onCollectibleButtonClicked()
    {
        SceneManager.LoadScene("Assets/Scenes/Collectible/Collectible.unity");
    }

    private void onSettingButtonClicked()
    {
        SceneManager.LoadScene("Assets/Scenes/Setting/Setting.unity");
    }

    private void onQuitButtonClicked()
    {
        Application.Quit();
    }

    private void onHowToButtonClicked()
    {
        SceneManager.LoadScene("Assets/Scenes/Main/HowTo.unity");
    }

    private void onCreditButtonClicked()
    {
        SceneManager.LoadScene("Assets/Scenes/Main/Credit.unity");
    }

    private void onLicenseButtonClicked()
    {
        SceneManager.LoadScene("Assets/Scenes/Main/License.unity");
    }
}
