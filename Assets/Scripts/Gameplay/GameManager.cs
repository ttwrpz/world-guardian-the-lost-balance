using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera m_Camera;
    [SerializeField]
    private Camera miniMapCamera;

    public WorldData worldData;
    public WorldPlayerData worldSave;
    
    [SerializeField]
    private TerrainGenerator terrainGenerator;
    [SerializeField]
    private GameObject terrainMap;

    private List<City> cities;

    [SerializeField]
    private GameplayUIController gameplayUI;
    [SerializeField]
    private SkillManager skillManager;
    [SerializeField]
    private TimeManager timeManager;
    private int previousMonth;

    public List<Skill> Skills { get; private set; }

    private void Start()
    {
        terrainGenerator.heightMapSettings.noiseSettings.seed = worldData.WorldSeed;
        //worldSave = SaveManager.LoadWorldSave(worldData.ConvertToWorld());

        timeManager.MonthElapsed += OnMonthElapsed;
        timeManager.YearElapsed += OnYearElapsed;
        previousMonth = timeManager.inGameMonth;

        cities = new List<City>(FindObjectsByType<City>(FindObjectsSortMode.None));
        Skills = new List<Skill>(Resources.LoadAll<Skill>("Skills"));
    }

    private void Update()
    {
        if (timeManager.inGameMonth != previousMonth)
        {
            UpdateCities();
            previousMonth = timeManager.inGameMonth;
        }
    }

    private void OnMonthElapsed()
    {
        skillManager.AddSkillPoints(10, worldData.WorldDifficulty);
    }

    private void OnYearElapsed()
    {
        // Perform actions you want to do every in-game year.
    }

    private void UpdateCities()
    {
        foreach (City city in cities)
        {
            city.worldDifficulty = worldData.WorldDifficulty;
            city.ModifyParameters();
            city.UpdateCityState();

            if (Random.Range(0f, 1f) <= 0.1f)
            {
                city.GenerateRandomDisaster();
            }
        }
    }

    public CityParameters CalculateAverageCityParameters()
    {
        CityParameters avgParameters = new ();

        foreach (var city in cities)
        {
            avgParameters.forest += city.parameters.forest;
            avgParameters.human += city.parameters.human;
            avgParameters.animal += city.parameters.animal;
            avgParameters.factory += city.parameters.factory;
            avgParameters.technology += city.parameters.technology;
            avgParameters.temperature += city.parameters.temperature;
            avgParameters.gas += city.parameters.gas;
            avgParameters.crops += city.parameters.crops;
        }

        int cityCount = cities.Count;

        avgParameters.forest /= cityCount;
        avgParameters.human /= cityCount;
        avgParameters.animal /= cityCount;
        avgParameters.factory /= cityCount;
        avgParameters.technology /= cityCount;
        avgParameters.temperature /= cityCount;
        avgParameters.gas /= cityCount;
        avgParameters.crops /= cityCount;

        return avgParameters;
    }
}
