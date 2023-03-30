using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameplayUIController : UIController
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private TimeManager timeManager;
    [SerializeField]
    private SkillManager skillManager;

    private GroupBox UIWrapper;

    private GroupBox _statusGroup;
    private Label statusTitleLabel;
    private GroupBox _parameterBox;
    private ProgressBar humanBar;
    private ProgressBar animalBar;
    private ProgressBar cropsBar;
    private ProgressBar forestBar;
    private ProgressBar factoryBar;
    private ProgressBar gasBar;
    private ProgressBar temperatureBar;

    private CityParameters parameters;

    private GroupBox _menuGroup;
    private Button achievementButton;
    private Button collectibleButton;
    private Button settingsButton;

    [SerializeField]
    private VisualTreeAsset pauseMenu;
    private VisualElement pauseMenuElement;
    private Button pauseMenuResume;
    private Button pauseMenuBackToMainMenu;

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

    private GroupBox _gameplayGroup;
    private GroupBox _newsBox;
    private Label newsLabel;
    private string newsLabelDefaultText;
    private GroupBox _skillsBox;
    private ProgressBar skillPointBar;
    private ScrollView skillCardSrollView;

    private GroupBox _skillTooltipBox;
    private Label skillTooltipNameLabel;
    private Label skillTooltipDescriptionLabel;
    private Label skillTooltipPointCostLabel;

    [SerializeField]
    private VisualTreeAsset SkillCardEntryTemplate;
    private List<Skill> SkillCardList;
    private List<SkillCard> skillCards;
    private SkillCard activeSkillCard;

    private void Start()
    {
        ChangeTimeState(TimeState.Resume);
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        timeManager.MonthElapsed += OnMonthElapsed;
        City.OnDisasterGenerated += UpdateNewsLabel;

        EnumerateAllSkillCards();
        GetUIElements();
        UpdateUIElements();
        AttachEventHandlers();
        InitializeSkillCardList();
    }

    private void OnDisable()
    {
        timeManager.MonthElapsed -= OnMonthElapsed;
        City.OnDisasterGenerated -= UpdateNewsLabel;
    }

    private void OnMonthElapsed()
    {
        UpdateNewsLabel(newsLabelDefaultText);
        parameters = gameManager.CalculateAverageCityParameters();
        UpdateUIElements();
    }

    private void UpdateTimeLabel()
    {
        if (dateTimeLabel.IsUnityNull())
            return;

        dateTimeLabel.text = $"<b>Month</b> {timeManager.inGameMonth} <b>Year</b> {timeManager.inGameYear}";
    }

    private void UpdateStatusBox()
    {
        humanBar.value = parameters.human;
        animalBar.value = parameters.animal;
        cropsBar.value = parameters.crops;
        forestBar.value = parameters.forest;
        factoryBar.value = parameters.factory;
        gasBar.value = parameters.gas;
        temperatureBar.value = parameters.temperature;
    }

    private void UpdateUIElements()
    {
        UpdateTimeLabel();
        UpdateStatusBox();
        UpdateSkillPointsLabel();
    }

    protected override void GetUIElements()
    {

        UIWrapper = _root.Q<GroupBox>("UIWrapper");

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
        gasBar = _parameterBox.Q<ProgressBar>("GreenhouseGasBar");
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

        pauseMenuElement = pauseMenu.CloneTree();
        pauseMenuElement.style.display = DisplayStyle.None;
        _root.Add(pauseMenuElement);

        pauseMenuResume = pauseMenuElement.Q<Button>("ResumeButton");
        pauseMenuBackToMainMenu = pauseMenuElement.Q<Button>("BackToMainMenuButton");

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
        map = _mapBox.Q<VisualElement>("MapVisual"); //TO-DO Map Screen

        /******************
        *
        *  Gameplay Area
        *
        ******************/

        _gameplayGroup = _root.Q<GroupBox>("Gameplay");
        _newsBox = _gameplayGroup.Q<GroupBox>("News");
        newsLabel = _newsBox.Q<Label>("NewsLabel");
        newsLabelDefaultText = newsLabel.text;
        _skillsBox = _gameplayGroup.Q<GroupBox>("SkillsBox");
        skillPointBar = _skillsBox.Q<ProgressBar>("SkillPoint");
        skillCardSrollView = _skillsBox.Q<ScrollView>("SkillCards");
        skillCards = new();

        _skillTooltipBox = _root.Q<GroupBox>("SkillTooltip");
        skillTooltipNameLabel = _skillTooltipBox.Q<Label>("TooltipNameLabel");
        skillTooltipDescriptionLabel = _skillTooltipBox.Q<Label>("TooltipDescriptionLabel");
        skillTooltipPointCostLabel = _skillTooltipBox.Q<Label>("TooltipPointCostLabel");
    }

    protected override void AttachEventHandlers()
    {
        settingsButton.RegisterCallback<ClickEvent>(evt => ShowPauseMenu());
        pauseMenuResume.RegisterCallback<ClickEvent>(evt => HidePauseMenu());
        pauseMenuBackToMainMenu.RegisterCallback<ClickEvent>(evt =>
        {
            SaveManager.SaveWorldData(gameManager.worldData.ConvertToWorld());
            SaveManager.SaveWorldPlayerData(gameManager.worldData.ConvertToWorld(), gameManager.worldSave);
            UIManager.BackToMainMenuUI();
        });

        timePauseButton.RegisterCallback<ClickEvent>(evt => ChangeTimeState(TimeState.Pause));
        timeResumeButton.RegisterCallback<ClickEvent>(evt => ChangeTimeState(TimeState.Resume));
        timeSpeedUp2xButton.RegisterCallback<ClickEvent>(evt => ChangeTimeState(TimeState.SpeedUp2x));
        timeSpeedUp3xButton.RegisterCallback<ClickEvent>(evt => ChangeTimeState(TimeState.SpeedUp3x));
    }

    private void HidePauseMenu()
    {
        ChangeTimeState(TimeState.Resume);
        UIWrapper.SetEnabled(true);
        pauseMenuElement.style.display = DisplayStyle.None;
    }

    private void ShowPauseMenu()
    {
        ChangeTimeState(TimeState.Pause);
        UIWrapper.SetEnabled(false);
        pauseMenuElement.style.display = DisplayStyle.Flex;

        SaveManager.SaveWorldData(gameManager.worldData.ConvertToWorld());
        SaveManager.SaveWorldPlayerData(gameManager.worldData.ConvertToWorld(), gameManager.worldSave);
    }

    private void ChangeTimeState(TimeState state)
    {
        timePauseButton.RemoveFromClassList("btn-active");
        timeResumeButton.RemoveFromClassList("btn-active");
        timeSpeedUp2xButton.RemoveFromClassList("btn-active");
        timeSpeedUp3xButton.RemoveFromClassList("btn-active");

        switch (state)
        {
            case TimeState.Pause:
                timeManager.PauseTime();
                timePauseButton.AddToClassList("btn-active");
                break;
            case TimeState.Resume:
                timeManager.ResumeTime();
                timeResumeButton.AddToClassList("btn-active");
                break;
            case TimeState.SpeedUp2x:
                timeManager.SpeedUp2x();
                timeSpeedUp2xButton.AddToClassList("btn-active");
                break;
            case TimeState.SpeedUp3x:
                timeManager.SpeedUp3x();
                timeSpeedUp3xButton.AddToClassList("btn-active");
                break;
        }
    }

    private void UpdateSkillPointsLabel()
    {
        skillPointBar.value = skillManager.playerSkillPoints;
        skillPointBar.title = $"SP {skillManager.playerSkillPoints}/{skillManager.maxSkillPoints}";
    }

    private void UpdateNewsLabel(string message)
    {
        newsLabel.text = message;
    }

    private void OnSkillCardPointerEnter(PointerEnterEvent evt)
    {
        if (evt.propagationPhase != PropagationPhase.AtTarget)
            return;

        var hoveredCard = skillCards.Find(card => card.element == (evt.currentTarget as VisualElement).parent.parent);
        if (hoveredCard == null)
            return;

        skillTooltipNameLabel.text = hoveredCard.skill.skillName;
        skillTooltipDescriptionLabel.text = hoveredCard.skill.description;
        skillTooltipPointCostLabel.text = $"{hoveredCard.skill.skillPointCost} Skill Points";
        _skillTooltipBox.style.display = DisplayStyle.Flex;
    }

    private void OnSkillCardPointerLeave(PointerLeaveEvent evt)
    {
        if (evt.propagationPhase != PropagationPhase.AtTarget)
            return;

        _skillTooltipBox.style.display = DisplayStyle.None;
    }

    private void OnSkillCardPointerClick(ClickEvent evt)
    {
        if (evt.target is not VisualElement clickedElement) return;

        if (activeSkillCard?.element == clickedElement)
        {
            activeSkillCard.element.RemoveFromClassList("skillcard--active");
            activeSkillCard = null;
        }
        else
        {
            foreach (var card in skillCards)
            {
                if (card.element == clickedElement)
                {
                    activeSkillCard?.element.RemoveFromClassList("skillcard--active");
                    activeSkillCard = card;
                    activeSkillCard.element.AddToClassList("skillcard--active");
                    break;
                }
            }
        }
    }

    void EnumerateAllSkillCards()
    {
        SkillCardList = skillManager.LoadSkills();
    }

    private readonly Stack<VisualElement> _SkillCardsListEntryPool = new();

    private VisualElement GetSkillCardListEntry()
    {
        if (_SkillCardsListEntryPool.Count > 0)
        {
            return _SkillCardsListEntryPool.Pop();
        }
        else
        {
            var skillElement = SkillCardEntryTemplate.CloneTree();
            var skillCard = new SkillCardEntryController();
            skillCard.SetVisualElement(skillElement);
            skillElement.userData = skillCard;

            return skillElement;
        }
    }

    void InitializeSkillCardList()
    {
        skillCardSrollView.Clear();

        foreach (Skill skill in SkillCardList)
        {
            var skillElement = GetSkillCardListEntry();
            skillElement.RegisterCallback<ClickEvent>(OnSkillCardPointerClick);

            var skillCard = skillElement.userData as SkillCardEntryController;
            skillCard.GetSkillInfoIcon().RegisterCallback<PointerEnterEvent>(OnSkillCardPointerEnter);
            skillCard.GetSkillInfoIcon().RegisterCallback<PointerLeaveEvent>(OnSkillCardPointerLeave);
            skillCard.SetData(skill);

            skillCardSrollView.Add(skillElement);
            skillCards.Add(new SkillCard(skill, skillElement));
        }
    }

}
