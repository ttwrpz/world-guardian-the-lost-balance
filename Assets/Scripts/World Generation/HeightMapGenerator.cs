using UnityEngine;

public class HeightMapGenerator
{
    public static HeightMap GenerateHeightMap(int width, int height, HeightMapSettings settings, Vector2 sampleCenter, Vector2 chunkCoord, MeshSettings meshSettings)
    {
        float[,] values = Noise.GenerateNoiseMap(width, height, settings.noiseSettings, sampleCenter);

        AnimationCurve heightCurve_threadsafe = new (settings.heightCurve.keys);

        float minValue = float.MaxValue;
        float maxValue = float.MinValue;

        float[,] falloffMap = null;

        if (settings.useFalloff)
        {
            if (settings.falloffType == FalloffType.FixedSize)
            {
                falloffMap = FalloffGenerator.GenerateFixedSizeFalloffMap(width, height, settings, chunkCoord, meshSettings);
            }
            else
            {
                falloffMap = FalloffGenerator.GenerateFalloffMap(width, settings);
            }
        }

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                values[i, j] *= heightCurve_threadsafe.Evaluate(values[i, j]) * settings.heightMultiplier;

                if (falloffMap != null)
                {
                    if (settings.falloffType == FalloffType.FixedSize)
                    {
                        int globalI = i + (int)chunkCoord.x * (width - 1);
                        int globalJ = j + (int)chunkCoord.y * (height - 1);

                        int clampedGlobalI = Mathf.Clamp(globalI, 0, falloffMap.GetLength(0) - 1);
                        int clampedGlobalJ = Mathf.Clamp(globalJ, 0, falloffMap.GetLength(1) - 1);

                        values[i, j] *= 1 - falloffMap[clampedGlobalJ, clampedGlobalI];
                    }
                    else
                    {
                        int falloffMapX = Mathf.Clamp(j, 0, width - 1);
                        int falloffMapY = Mathf.Clamp(i, 0, height - 1);

                        values[i, j] *= 1 - falloffMap[falloffMapY, falloffMapX];
                    }
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