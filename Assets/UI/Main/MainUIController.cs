using System;
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
    private Button _creditButton;
    private Button _settingButton;
    private Button _quitButton;
    
    private VisualElement _ContentWrapper;

    [SerializeField]
    private VisualTreeAsset _settingsContentTemplate;
    private VisualElement _settingsContent;

    [SerializeField]
    private VisualTreeAsset _creditsContentTemplate;
    private VisualElement _creditsContent;

    private void OnEnable()
    {
        GetUIElements();
        AttachEventHandlers();
    }

    private void GetUIElements()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _singleplayerButton = _root.Q<Button>("SingleplayerButton");
        _multiplayerButton = _root.Q<Button>("MultiplayerButton");
        _achievementButton = _root.Q<Button>("AchievementButton");
        _collectibleButton = _root.Q<Button>("CollectibleButton");
        _creditButton = _root.Q<Button>("CreditButton");
        _settingButton = _root.Q<Button>("SettingButton");
        _quitButton = _root.Q<Button>("QuitButton");

        _ContentWrapper = _root.Q<VisualElement>("ContentWrapper");

        _settingsContent = _settingsContentTemplate.CloneTree();

        _creditsContent = _creditsContentTemplate.CloneTree();
    }

    private void AttachEventHandlers()
    {
        _singleplayerButton.clicked += onSingleplayerButtonClicked;
        _multiplayerButton.clicked += onMultiplayerButtonClicked;
        _achievementButton.clicked += onAchievementButtonClicked;
        _collectibleButton.clicked += onCollectibleButtonClicked;
        _creditButton.clicked += onCreditButtonButtonClicked;
        _settingButton.clicked += onSettingButtonClicked;
        _quitButton.clicked += onQuitButtonClicked;
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
        SceneManager.LoadScene("Assets/Scenes/Achievement/Achievement.unity");
    }

    private void onCollectibleButtonClicked()
    {
        SceneManager.LoadScene("Assets/Scenes/Collectible/Collectible.unity");
    }

    private void onCreditButtonButtonClicked()
    {
        if (_ContentWrapper.enabledSelf && _ContentWrapper.Q<Label>("TitleScreenLabel")?.text == _creditsContent.Q<Label>("TitleScreenLabel").text)
        {
            SetEnableContentWrapper(false);
        }
        else
        {
            SetEnableContentWrapper(true);
            _ContentWrapper.Add(_creditsContent);
        }
    }

    private void onSettingButtonClicked()
    {
        if (_ContentWrapper.enabledSelf && _ContentWrapper.Q<Label>("TitleScreenLabel")?.text == _settingsContent.Q<Label>("TitleScreenLabel").text)
        {
            SetEnableContentWrapper(false);
        }
        else
        {
            SetEnableContentWrapper(true);
            _ContentWrapper.Add(_settingsContent);
        }
    }

    private void onCloseContentWrapperButtonClicked()
    {
        SetEnableContentWrapper(false);
    }

    private void SetEnableContentWrapper(bool enable)
    {
        _ContentWrapper.SetEnabled(enable);
        _ContentWrapper.style.display = enable
            ? DisplayStyle.Flex
            : DisplayStyle.None;
        _ContentWrapper.style.visibility = enable
            ? Visibility.Visible
            : Visibility.Hidden;
        _ContentWrapper.Clear();
    }

    private void onQuitButtonClicked()
    {
        Application.Quit();
    }
}
