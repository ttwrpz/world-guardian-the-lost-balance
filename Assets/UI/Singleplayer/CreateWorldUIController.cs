using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class CreateWorldUIController : MonoBehaviour
{
    private SpriteAtlasManager spriteAtlasManager;

    private UIDocument _doc;
    private VisualElement _root;
    private TextField _worldNameInput;
    private TextField _worldSeedInput;
    private VisualElement _worldGameModeIcon;
    private RadioButtonGroup _worldGameModeRadioInput;
    private VisualElement _worldDifficultyIcon;
    private RadioButtonGroup _worldDifficultyRadioInput;
    private VisualElement _worldSizeIcon;
    private RadioButtonGroup _worldSizeRadioInput;
    private Button _createWorldButton;
    private Button _backButton;

    [SerializeField]
    private WorldData worldData;

    private void OnEnable()
    {
        spriteAtlasManager = new("GUI/singleplayer");

        _doc = GetComponent<UIDocument>();
        _root = _doc.rootVisualElement;

        _worldNameInput = _root.Q<TextField>("worldNameInput");
        _worldNameInput.RegisterCallback<ChangeEvent<string>>(onWorldNameInputChanged);
        _worldNameInput.Focus();

        _worldSeedInput = _root.Q<TextField>("worldSeedInput");

        _worldGameModeIcon = _root.Q<VisualElement>("WorldGameModeIcon");
        _worldGameModeRadioInput = _root.Q<RadioButtonGroup>("worldGameModeRadioInput");
        _worldGameModeRadioInput.RegisterValueChangedCallback<int>(onWorldGameModeRadioInputChanged);

        _worldDifficultyIcon = _root.Q<VisualElement>("WorldDifficultyIcon");
        _worldDifficultyRadioInput = _root.Q<RadioButtonGroup>("worldDifficultyRadioInput");
        _worldDifficultyRadioInput.RegisterValueChangedCallback<int>(onWorldDifficultyRadioInputChanged);

        _worldSizeIcon = _root.Q<VisualElement>("WorldSizeIcon");
        _worldSizeRadioInput = _root.Q<RadioButtonGroup>("worldSizeRadioInput");
        _worldSizeRadioInput.RegisterValueChangedCallback<int>(onWorldSizeRadioInputChanged);

        _createWorldButton = _root.Q<Button>("createWorldButton");
        _createWorldButton.RegisterCallback<ClickEvent>(onCreateWorldButtonClicked);

        _backButton = _root.Q<Button>("backButton");
        _backButton.RegisterCallback<ClickEvent>(onBackButtonClicked);
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
            case (int)World.WorldGameMode.Story:
                _worldGameModeIcon.style.backgroundImage = new StyleBackground(spriteAtlasManager.GetAtlas("world-gamemode-story"));
                break;
            case (int)World.WorldGameMode.Sandbox:
                _worldGameModeIcon.style.backgroundImage = new StyleBackground(spriteAtlasManager.GetAtlas("world-gamemode-sandbox"));
                break;
        }
    }

    private void onWorldDifficultyRadioInputChanged(ChangeEvent<int> changeEvent)
    {
        switch (changeEvent.newValue)
        {
            case (int)World.WorldDifficulty.Easy:
                _worldDifficultyIcon.style.backgroundImage = new StyleBackground(spriteAtlasManager.GetAtlas("world-difficulty-easy"));
                break;
            case (int)World.WorldDifficulty.Medium:
                _worldDifficultyIcon.style.backgroundImage = new StyleBackground(spriteAtlasManager.GetAtlas("world-difficulty-medium"));
                break;
            case (int)World.WorldDifficulty.Hard:
                _worldDifficultyIcon.style.backgroundImage = new StyleBackground(spriteAtlasManager.GetAtlas("world-difficulty-hard"));
                break;
        }
    }

    private void onWorldSizeRadioInputChanged(ChangeEvent<int> changeEvent)
    {
        switch (changeEvent.newValue)
        {
            case (int)World.WorldSize.Small:
                _worldSizeIcon.style.backgroundImage = new StyleBackground(spriteAtlasManager.GetAtlas("world-size-small"));
                break;
            case (int)World.WorldSize.Medium:
                _worldSizeIcon.style.backgroundImage = new StyleBackground(spriteAtlasManager.GetAtlas("world-size-medium"));
                break;
            case (int)World.WorldSize.Large:
                _worldSizeIcon.style.backgroundImage = new StyleBackground(spriteAtlasManager.GetAtlas("world-size-large"));
                break;
        }
    }

    private void onCreateWorldButtonClicked(ClickEvent clickEvent)
    {
        World world = new()
        {
            worldName = _worldNameInput.text,
            worldSeed = World.ConvertFormat(_worldSeedInput.text),
            worldGameMode = (World.WorldGameMode)_worldGameModeRadioInput.value,
            worldDifficulty = (World.WorldDifficulty)_worldDifficultyRadioInput.value,
            worldSize = (World.WorldSize)_worldSizeRadioInput.value,
            worldCreatedAt = DateTime.Now
        };

        SaveManager.CreateWorld(world);
    }

    private void onBackButtonClicked(ClickEvent clickEvent)
    {
        if (SaveManager.LoadWorldListEntry().Count() > 0)
        {
            SceneManager.LoadScene("Assets/Scenes/Singleplayer/Singleplayer.unity");
        }
        else
        {
            SceneManager.LoadScene("Assets/Scenes/Main/Main.unity");
        }
    }
}
