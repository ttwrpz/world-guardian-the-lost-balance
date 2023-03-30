using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class City : MonoBehaviour
{
    public string cityName;

    [SerializeField] private CityState _cityState;
    [SerializeField] private bool _landmarkCity;

    public CityParameters parameters;
    public World.Difficulty worldDifficulty = World.Difficulty.Easy;

    private System.Random _random;

    public delegate void DisasterGeneratedHandler(string message);
    public static event DisasterGeneratedHandler OnDisasterGenerated;

    private void Start()
    {
        _random = new System.Random();
        _landmarkCity = _random.Next(0, 100) < 40;
        parameters.forest = Random.Range(70f, 100f);
        parameters.human = Random.Range(30f, 50f);
        parameters.animal = Random.Range(50f, 60f);
        parameters.gas = Random.Range(20f, 30f);
        parameters.temperature = Random.Range(parameters.gas * 0.5f, parameters.gas + parameters.gas * 0.5f);
        _cityState = CityState.GreenCity;
    }

    public void ModifyParameters()
    {
        float multiplier = GetDifficultyMultiplier();

        switch (_cityState)
        {
            case CityState.GreenCity:
                parameters.forest -= parameters.forest * 0.01f * multiplier;
                parameters.animal -= parameters.human * 0.001f * multiplier;
                parameters.crops += parameters.forest * 0.01f * multiplier;
                parameters.human += parameters.animal * 0.01f * multiplier;
                parameters.gas += parameters.forest * 0.002f * multiplier;
                parameters.temperature += parameters.gas * 0.001f * multiplier;
                break;

            case CityState.AgriculturalCity:
                parameters.human += parameters.human * 0.01f * multiplier;
                parameters.forest -= parameters.forest * 0.01f * multiplier;
                parameters.animal -= parameters.human * 0.001f * multiplier;
                parameters.crops -= parameters.factory * 0.05f * multiplier;
                break;

            case CityState.IndustrialCity:
                parameters.forest -= parameters.forest * 0.01f * multiplier;
                parameters.animal -= parameters.animal * 0.005f * multiplier;
                parameters.human -= parameters.human * 0.001f * multiplier;
                parameters.crops -= parameters.crops * 0.001f * multiplier;
                if (parameters.factory == 0f)
                {
                    parameters.factory = 1f;
                }
                parameters.factory += parameters.factory * 0.001f * multiplier;
                parameters.technology += parameters.factory * 0.03f * multiplier;
                break;

            case CityState.TechnoCity:
                parameters.forest -= parameters.forest * 0.001f * multiplier;
                parameters.animal -= parameters.animal * 0.005f * multiplier;
                parameters.crops -= parameters.crops * 0.005f * multiplier;
                parameters.technology += parameters.technology * 0.01f * multiplier;
                break;

            case CityState.DepletedCity:
                break;

            default:
                Debug.LogWarning("Invalid City State");
                break;
        }

        ClampParameters();
    }

    public void ClampParameters()
    {
        parameters.forest = Mathf.Clamp(parameters.forest, 0, 100);
        parameters.human = Mathf.Clamp(parameters.human, 0, 100);
        parameters.animal = Mathf.Clamp(parameters.animal, 0, 100);
        parameters.factory = Mathf.Clamp(parameters.factory, 0, 100);
        parameters.technology = Mathf.Clamp(parameters.technology, 0, 100);
        parameters.temperature = Mathf.Clamp(parameters.temperature, 0, 100);
        parameters.gas = Mathf.Clamp(parameters.gas, 0, 100);
        parameters.crops = Mathf.Clamp(parameters.crops, 0, 100);
    }

    public void GenerateRandomDisaster()
    {
        bool shouldGenerate = Random.Range(0, 100) == 50 && _cityState != CityState.GreenCity && _cityState != CityState.DepletedCity;

        if (shouldGenerate)
        {
            List<(DisasterType, float, float, float, float)> disasters = new()
            {
                (DisasterType.ForestFire, -0.05f, -0.007f, -0.05f, 0f),
                (DisasterType.Flood, 0f, -0.002f, 0f, -0.05f),
                (DisasterType.Earthquake, -0.05f, -0.005f, 0f, 0f),
                (DisasterType.Famine, -0.05f, 0f, 0f, 0f)
            };

            int index = Random.Range(0, disasters.Count);
            var (disasterType, humanChange, animalChange, forestChange, cropsChange) = disasters[index];

            parameters.human += parameters.human * humanChange;
            parameters.animal += parameters.animal * animalChange;
            parameters.forest += parameters.forest * forestChange;
            parameters.crops += parameters.crops * cropsChange;

            string message = $"{gameObject.name} has encountered a {disasterType}";
            OnDisasterGenerated?.Invoke(message);
        }
    }

    private float GetDifficultyMultiplier()
    {
        switch (worldDifficulty)
        {
            case World.Difficulty.Easy:
                return 1f;
            case World.Difficulty.Medium:
                return 2f;
            case World.Difficulty.Hard:
                return 3f;
            default:
                Debug.LogWarning("Invalid world difficulty");
                return 1f;
        }
    }

    public void UpdateCityState()
    {
        switch (_cityState)
        {
            case CityState.GreenCity:
                if ((parameters.human >= 70.0f || (parameters.factory >= 50.0f && parameters.technology <= parameters.factory)) && parameters.crops < 30.0f)
                {
                    _cityState = CityState.IndustrialCity;
                }
                else if (parameters.crops >= 30.0f)
                {
                    _cityState = CityState.AgriculturalCity;
                }
                else if (parameters.technology > parameters.factory)
                {
                    _cityState = CityState.TechnoCity;
                }
                break;

            case CityState.AgriculturalCity:
                if (parameters.crops <= 15.0f && !(parameters.crops >= 50f))
                {
                    if (parameters.forest >= 70.0f || parameters.animal >= 70.0f)
                    {
                        _cityState = CityState.GreenCity;
                    }
                    else if ((parameters.factory >= 20.0f && parameters.technology <= 50.0f))
                    {
                        _cityState = CityState.IndustrialCity;
                    }
                    else if (parameters.technology > 60.0f)
                    {
                        _cityState = CityState.TechnoCity;
                    }
                    else if (parameters.forest <= 10.0f || parameters.temperature >= 40.0f || parameters.gas >= 80.0f)
                    {
                        _cityState = CityState.DepletedCity;
                    }
                }
                break;

            case CityState.IndustrialCity:
                if (parameters.technology > parameters.factory)
                {
                    _cityState = CityState.TechnoCity;
                }
                else if (parameters.forest >= 70.0f || parameters.animal >= 70.0f)
                {
                    _cityState = CityState.GreenCity;
                }
                else if (parameters.crops >= 50f)
                {
                    _cityState = CityState.AgriculturalCity;
                }
                else if (parameters.forest <= 10.0f || parameters.temperature >= 40.0f || parameters.gas >= 80.0f)
                {
                    _cityState = CityState.DepletedCity;
                }
                break;

            case CityState.TechnoCity:
                if (parameters.technology <= parameters.factory)
                {
                    _cityState = CityState.IndustrialCity;
                }
                else if (parameters.crops > 40.0f)
                {
                    _cityState = CityState.AgriculturalCity;
                }
                else if (parameters.forest >= 70.0f || parameters.animal >= 70.0f)
                {
                    _cityState = CityState.GreenCity;
                }
                else if (parameters.forest <= 10.0f || parameters.temperature >= 40.0f || parameters.gas >= 80.0f)
                {
                    _cityState = CityState.DepletedCity;
                }
                break;

            case CityState.DepletedCity:
                Debug.LogWarning("This city has failed.");
                break;

            default:
                Debug.LogWarning("Invalid City State");
                break;
        }

    }

}

public enum CityState
{
    GreenCity,
    AgriculturalCity,
    IndustrialCity,
    TechnoCity,
    DepletedCity,
}

[System.Serializable]
public struct CityParameters
{
    [Range(0f, 100f)] public float forest;
    [Range(0f, 100f)] public float human;
    [Range(0f, 100f)] public float animal;
    [Range(0f, 100f)] public float factory;
    [Range(0f, 100f)] public float technology;
    [Range(0f, 100f)] public float temperature;
    [Range(0f, 100f)] public float gas;
    [Range(0f, 100f)] public float crops;
}


public enum DisasterType
{
    ForestFire,
    Flood,
    Earthquake,
    Famine
}
