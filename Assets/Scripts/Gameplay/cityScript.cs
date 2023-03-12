using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Internal;
using Random = System.Random;

public class cityScript : MonoBehaviour
{ 
    public enum cityState {
        GreenCity,
        AgriculturalCity,
        IndustrialCity,
        TechnoCity,
        DepletedCity,
    }

    [SerializeField]
    private cityState cityId;

    [SerializeField]
    public bool landmarkCity;

    [Range(0f, 100f)]
    public float forestParam;

    [Range(0f, 100f)]
    public float populationParam;

    [Range(0f, 100f)]
    public float animalParam;

    [Range(0f, 100f)]
    public float resourceParam;

    [Range(0f, 100f)]
    public float factoryParam;

    [Range(0f, 100f)]
    public float techParam;

    [Range(0f, 100f)]
    public float tempParam;

    [Range(0f, 100f)]
    public float gasParam;

    [Range(0f, 100f)]
    public float cropParam;

    Random random = new Random();
    private void Start()
    { 
        float runInterval = 1.0f;

        double getRandomNum(double min, double max)
        {
            return random.NextDouble() * (max - min) + min;
        }

        bool getRandomBool(int min, int max, int threshold)
        {
            int randomizer = random.Next(min, max);
            return randomizer >= threshold;
        }

        forestParam = (float)getRandomNum(70f, 100f);
        populationParam = (float)getRandomNum(30f, 50f);
        animalParam = (float)getRandomNum(50f, 60f);
        resourceParam = (float)getRandomNum(70f, 100f);

        gasParam = (float)getRandomNum(20f, 30f);
        tempParam = (float)getRandomNum(gasParam*0.5, gasParam+gasParam*0.5);

        landmarkCity = getRandomBool(0, 100, 80);

        cityId = cityState.GreenCity;
        InvokeRepeating("modifyParam", 0, runInterval);
    }

    private void modifyParam()
    {
        forestParam = Math.Clamp(forestParam, 0, 100);
        populationParam = Math.Clamp(populationParam, 0, 100);
        animalParam = Math.Clamp(animalParam, 0, 100);
        resourceParam = Math.Clamp(resourceParam, 0, 100);
        factoryParam = Math.Clamp(factoryParam, 0, 100);
        techParam = Math.Clamp(techParam, 0, 100);
        tempParam = Math.Clamp(tempParam, 0, 100);
        gasParam = Math.Clamp(gasParam, 0, 100);
        cropParam = Math.Clamp(cropParam, 0, 100);

        Array allowDisaster = new[] { cityState.IndustrialCity, cityState.AgriculturalCity};
        bool generateRandomDisaster = random.Next(0, 100) == 50 && !(new[] { cityState.GreenCity, cityState.DepletedCity }.Contains(cityId) ); 
        
        string returnRandomDisaster()
        {
            List<string> disasterList = new List<string> 
            { 
                "Forest fire",
                "Flood",
                "Earthquake",
                "Famine",
            }; 
            
            int disasterInt = random.Next(disasterList.Count);
            string currentDisaster = disasterList[disasterInt]; 

            return currentDisaster;
        }

        string returnSkill()
        {
            List<string> skillList = new List<string>
            {
                "Bless crops",
                "Generate resource",
            };

            int skillInt = random.Next(skillList.Count);
            string currentSkill = skillList[skillInt];

            return currentSkill;
        }

        if (generateRandomDisaster) {
            string currentDisaster = returnRandomDisaster();
            
            switch(currentDisaster)
            {
                case "Forest fire":
                    populationParam -= populationParam * 0.003f;
                    animalParam -= animalParam * 0.007f;
                    forestParam -= forestParam * 0.05f;

                    Debug.Log("Generated Forest fire!");
                    break;

                case "Flood":
                    cropParam -= cropParam * 0.05f;
                    populationParam -= populationParam * 0.002f;

                    Debug.Log("Generated Flood!");
                    break;

                case "Earthquake":
                    factoryParam -= factoryParam * 0.05f;
                    techParam -= techParam * 0.005f;
                    populationParam -= populationParam * 0.005f;

                    Debug.Log("Generated Earthquake!");
                    break;

                case "Famine":
                    populationParam -= populationParam * 0.05f;
                    Debug.Log("Generated Famine!");

                    break;
            }
        }

        switch (cityId)
        {
            case cityState.GreenCity:
                resourceParam -= resourceParam * 0.01f;
                forestParam -= forestParam * 0.01f;
                animalParam -= populationParam * 0.001f;

                cropParam += (resourceParam * 0.01f + forestParam * 0.01f);
                populationParam += ((resourceParam + animalParam) * 0.01f);

                gasParam += forestParam * 0.002f;
                tempParam += gasParam * 0.001f;
                break;

            case cityState.AgriculturalCity:
                populationParam += populationParam * 0.01f;

                resourceParam -= resourceParam * 0.02f;
                forestParam -= forestParam * 0.01f;
                animalParam -= populationParam * 0.001f;

                factoryParam += resourceParam * 0.01f;
                cropParam -= factoryParam * 0.05f;
                break;

            case cityState.IndustrialCity:
                resourceParam -= resourceParam * 0.01f;
                forestParam -= forestParam * 0.01f;

                animalParam -= animalParam * 0.005f;
                populationParam -= populationParam * 0.001f;
                cropParam -= cropParam * 0.001f;

                if (factoryParam == 0f)
                {
                    factoryParam = 1f;
                }

                factoryParam += factoryParam * 0.001f;
                techParam += factoryParam * 0.03f;
                break;

            case cityState.TechnoCity:
                resourceParam -= resourceParam * 0.01f;
                forestParam -= forestParam * 0.001f;
                animalParam -= animalParam * 0.005f;
                cropParam -= animalParam * 0.005f;

                techParam += techParam * 0.01f;
                break;
        }
    }

    private void Update()
    {
        switch (cityId)
        {
            case cityState.GreenCity:
                if ( ( populationParam >= 70.0f 
                    || resourceParam <= 40.0f || 
                    (factoryParam >= 50.0f && techParam <= factoryParam)
                    ) 
                    && cropParam < 30.0f )
                {
                    cityId = cityState.IndustrialCity;
                }
                else if (cropParam >= 30.0f) 
                {
                    cityId = cityState.AgriculturalCity;
                }

                break;
            case cityState.AgriculturalCity:
                if ( cropParam <= 5.0f )
                {
                    if (forestParam >= 70.0f || animalParam >= 70.0f)
                    {
                        cityId = cityState.GreenCity;
                    }
                    else if ((factoryParam >= 20.0f && techParam <= 50.0f))
                    {
                        cityId = cityState.IndustrialCity;
                    }
                    else if (techParam > 60.0f)
                    {
                        cityId = cityState.TechnoCity;
                    }
                    else if (forestParam <= 10.0f || resourceParam <= 10.0f || tempParam >= 40.0f || gasParam >= 80.0f)
                    {
                        cityId = cityState.DepletedCity;
                    }
                }
                break;

            case cityState.IndustrialCity:
                if ( techParam > factoryParam )
                {
                    cityId = cityState.TechnoCity;
                }
                else if (forestParam <= 10.0f || resourceParam <= 10.0f || tempParam >= 40.0f || gasParam >= 80.0f)
                { 
                    cityId = cityState.DepletedCity;
                }
                break;

            case cityState.TechnoCity:

                if (techParam <= factoryParam)
                {
                    cityId = cityState.IndustrialCity;
                }
                else if (cropParam > 40.0f)
                {
                    cityId = cityState.AgriculturalCity;
                }
                else if (forestParam <= 10.0f || resourceParam <= 10.0f || tempParam >= 40.0f || gasParam >= 80.0f)
                {
                    cityId = cityState.DepletedCity;
                }
                break;

            case cityState.DepletedCity:
                Debug.LogError("This city has failed.");
                break;
            default:
                Debug.LogError("Invalid City State");
                break;
        } 
    }

}
