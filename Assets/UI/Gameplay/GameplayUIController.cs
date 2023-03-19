using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayUIController : MonoBehaviour
{
    private UIDocument _doc;
    private VisualElement _root;

    private GroupBox _statusGroup;
    private Label statusTitleLabel;
    private GroupBox _parameterBox;
    private ProgressBar humanBar;
    private ProgressBar animalBar;
    private ProgressBar cropsBar;
    private ProgressBar forestBar;
    private ProgressBar factoryBar;
    private ProgressBar ghgBar;
    private ProgressBar temperatureBar;

    private GroupBox _menuGroup;
    private Button achievementButton;
    private Button collectibleButton;
    private Button settingsButton;

    private GroupBox _controlGroup;
    private GroupBox _timeControlBox;
    private Label dateTimeLabel;
    private GroupBox _timeActionButtons;
    private Button timePauseButton;
    private Button timeResumeButton;
    private Button timeSpeedUp2xButton;
    private Button timeSpeedUp3xButton;
    private GroupBox _mapBox;
    private VisualElement map;

    [SerializeField]
    private VisualTreeAsset _skillInfoTemplate;
    private GroupBox _gameplayGroup;
    private GroupBox _newsBox;
    private Label newsLabel;
    private GroupBox _skillsBox;
    private ListView skillsList;

    private TimeManager timeManager;

    void Start()
    {
        timeManager = FindFirstObjectByType<TimeManager>();
    }

    void Update()
    {
        if (timeManager != null)
        {
            Debug.Log("Time Manager found");
            if (dateTimeLabel != null)
            {
                dateTimeLabel.text = $"<b>Month</b> {timeManager.inGameMonth} <b>Year</b> {timeManager.inGameYear}";
            }
        }
    }

    private void OnEnable()
    {
        GetUIElements();
        AttachEventHandlers();
    }

    private void GetUIElements()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        /****************
        *
        *  Status Area
        *
        ****************/

        _statusGroup = _root.Q<GroupBox>("Status");
        statusTitleLabel = _statusGroup.Q<Label>("TitleLabel");
        _parameterBox = _statusGroup.Q<GroupBox>("ParameterBox");
        humanBar = _parameterBox.Q<ProgressBar>("HumanBar");
        animalBar = _parameterBox.Q<ProgressBar>("AnimalBar");
        cropsBar = _parameterBox.Q<ProgressBar>("CropsBar");
        forestBar = _parameterBox.Q<ProgressBar>("ForestBar");
        factoryBar = _parameterBox.Q<ProgressBar>("FactoryBar");
        ghgBar = _parameterBox.Q<ProgressBar>("GreenhouseGasBar");
        temperatureBar = _parameterBox.Q<ProgressBar>("TemperatureBar");

        /*****************
        *
        *  Menu Buttons
        *
        *****************/

        _menuGroup = _root.Q<GroupBox>("MenuButtons");
        achievementButton = _menuGroup.Q<Button>("AchievementButton");
        collectibleButton = _menuGroup.Q<Button>("CollectibleButton");
        settingsButton = _menuGroup.Q<Button>("SettingsButton");

        /*********************
        *
        *  Gameplay Control
        *
        *********************/

        _controlGroup = _root.Q<GroupBox>("Control");
        _timeControlBox = _controlGroup.Q<GroupBox>("TimeControl");
        dateTimeLabel = _timeControlBox.Q<Label>("DateTimeLabel");
        _timeActionButtons = _timeControlBox.Q<GroupBox>("ActionsButtons");
        timePauseButton = _timeActionButtons.Q<Button>("PauseButton");
        timeResumeButton = _timeActionButtons.Q<Button>("ResumeButton");
        timeSpeedUp2xButton = _timeActionButtons.Q<Button>("SpeedUp2xButton");
        timeSpeedUp3xButton = _timeActionButtons.Q<Button>("SpeedUp3xButton");
        _mapBox = _controlGroup.Q<GroupBox>("MapBox");
        map = _mapBox.Q<VisualElement>("MapVisual");

        /******************
        *
        *  Gameplay Area
        *
        ******************/

        _gameplayGroup = _root.Q<GroupBox>("Gameplay");
        _newsBox = _gameplayGroup.Q<GroupBox>("News");
        newsLabel = _newsBox.Q<Label>("NewsLabel"); ;
        _skillsBox = _gameplayGroup.Q<GroupBox>("SkillsBox"); ;
        skillsList = _skillsBox.Q<ListView>("SkillsList"); ;
    }

    private void AttachEventHandlers()
    {
        timePauseButton.RegisterCallback<ClickEvent>(evt => timeManager.PauseTime());
        timeResumeButton.RegisterCallback<ClickEvent>(evt => timeManager.ResumeTime());
        timeSpeedUp2xButton.RegisterCallback<ClickEvent>(evt => timeManager.SpeedUp2x());
        timeSpeedUp3xButton.RegisterCallback<ClickEvent>(evt => timeManager.SpeedUp3x());
    }
}
