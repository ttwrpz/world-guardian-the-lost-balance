using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CreateWorldUIController : UIController
{
    private SpriteAtlasManager spriteAtlasManager;

    private TextField _worldNameInput;
    private TextField _worldSeedInput;
    private VisualElement _worldGameModeIcon;
    private RadioButtonGroup _worldGameModeRadioInput;
    private VisualElement _worldDifficultyIcon;
    private RadioButtonGroup _worldDifficultyRadioInput;
    private Button _createWorldButton;
    private Button _backButton;

    [SerializeField]
    private WorldData worldData;

    protected override void OnEnable()
    {
        spriteAtlasManager = new("GUI/singleplayer");

        GetUIElements();
        AttachEventHandlers();
    }

    protected override void GetUIElements()
    {
        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _worldNameInput = _root.Q<TextField>("worldNameInput");
        _worldSeedInput = _root.Q<TextField>("worldSeedInput");

        _worldGameModeIcon = _root.Q<VisualElement>("WorldGameModeIcon");
        _worldGameModeRadioInput = _root.Q<RadioButtonGroup>("worldGameModeRadioInput");

        _worldDifficultyIcon = _root.Q<VisualElement>("WorldDifficultyIcon");
        _worldDifficultyRadioInput = _root.Q<RadioButtonGroup>("worldDifficultyRadioInput");

        _createWorldButton = _root.Q<Button>("createWorldButton");
        _backButton = _root.Q<Button>("backButton");
    }

    protected override void AttachEventHandlers()
    {
        _worldNameInput.RegisterCallback<ChangeEvent<string>>(onWorldNameInputChanged);
        _worldNameInput.Focus();

        _worldGameModeRadioInput.RegisterValueChangedCallback<int>(onWorldGameModeRadioInputChanged);
        _worldDifficultyRadioInput.RegisterValueChangedCallback<int>(onWorldDifficultyRadioInputChanged);
        _createWorldButton.clicked += onCreateWorldButtonClicked;
        _backButton.clicked += onBackButtonClicked;
    }

    private void onWorldNameInputChanged(ChangeEvent<string> changeEvent)
    {
        if (changeEvent.newValue.ToString() == "")
        {
            _createWorldButton.SetEnabled(false);
            _createWorldButton.AddToClassList("btn-disabled");
        }
        else
        {
            _createWorldButton.SetEnabled(true);
            _createWorldButton.RemoveFromClassList("btn-disabled");
        }
    }

    private void onWorldGameModeRadioInputChanged(ChangeEvent<int> changeEvent)
    {
        switch (changeEvent.newValue)
        {
            case (int)World.GameMode.Story:
                _worldGameModeIcon.style.backgroundImage = new StyleBackground(spriteAtlasManager.GetAtlas("world-gamemode-story"));
                break;
            case (int)World.GameMode.Sandbox:
                _worldGameModeIcon.style.backgroundImage = new StyleBackground(spriteAtlasManager.GetAtlas("world-gamemode-sandbox"));
                break;
        }
    }

    private void onWorldDifficultyRadioInputChanged(ChangeEvent<int> changeEvent)
    {
        switch (changeEvent.newValue)
        {
            case (int)World.Difficulty.Easy:
                _worldDifficultyIcon.style.backgroundImage = new StyleBackground(spriteAtlasManager.GetAtlas("world-difficulty-easy"));
                break;
            case (int)World.Difficulty.Medium:
                _worldDifficultyIcon.style.backgroundImage = new StyleBackground(spriteAtlasManager.GetAtlas("world-difficulty-medium"));
                break;
            case (int)World.Difficulty.Hard:
                _worldDifficultyIcon.style.backgroundImage = new StyleBackground(spriteAtlasManager.GetAtlas("world-difficulty-hard"));
                break;
        }
    }

    private async void onCreateWorldButtonClicked()
    {
        if (!_createWorldButton.enabledSelf)
            return;

        World world = new()
        {
            WorldName = _worldNameInput.text,
            WorldSeed = (_worldSeedInput.text != null)
            ? World.ConvertSeedFormat(_worldSeedInput.text)
            : World.GenerateRandomSeed(),
            WorldGameMode = (World.GameMode)_worldGameModeRadioInput.value,
            WorldDifficulty = (World.Difficulty)_worldDifficultyRadioInput.value,
            WorldCreatedAt = DateTime.Now
        };

        WorldPlayerData playerData = new()
        {
            ElapsedTime = 0f,
            SkillPoints = 0
        };

        string worldFolder = SaveManager.CreateWorld(world);
        world.WorldFolder = worldFolder;
        SaveManager.SaveWorldPlayerData(world, playerData);

        await UIManager.LoadSceneAsync("Singleplayer/Singleplayer");
    }

    private async void onBackButtonClicked()
    {
        if (SaveManager.LoadWorldListEntry().Count() > 0)
        {
            await UIManager.LoadSceneAsync("Singleplayer/Singleplayer");
            return;
        }
        UIManager.BackToMainMenuUI();
    }
}
