using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCenter)
    {
        float[,] values = Noise.GenerateNoiseMap(width ,height, settings.noiseSettings, sampleCenter);

        AnimationCurve heightCurve_threadsafe = new (settings.heightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        float[,] falloffMap = (settings.useFalloff)
            ? FalloffGenerator.GenerateFalloffMap(width)
            : null;

        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < height; j++)
            {
                values[i, j] *= heightCurve_threadsafe.Evaluate(values[i,j]) * settings.heightMultiplier;

                if (falloffMap != null)
                {
                    int falloffMapX = Mathf.Clamp(j, 0, width - 1);
                    int falloffMapY = Mathf.Clamp(i, 0, height - 1);
                    values[i, j] *= falloffMap[falloffMapY, falloffMapX];
                }

                if (values[i, j] > maxValue)
                {
                    maxValue = values[i, j];
                }

                if (values[i, j] < minValue)
                {
                    minValue = values[i, j];
                }

            }
        }

        return new HeightMap(values, minValue, maxValue);
    }
}

public struct HeightMap
{
    public readonly float[,] values;
    public readonly float minValue;
    public readonly float maxValue;

    public HeightMap(float[,] values, float minValue, float maxValue)
    {
        this.values = values;
        this.minValue = minValue;
        this.maxValue = maxValue;
    }
}