using UnityEngine;

public static class FalloffGenerator
{
    public static float[,] GenerateFalloffMap(int size, HeightMapSettings settings)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));
                map[i, j] = Evaluate(value, settings);
            }
        }

        return map;
    }

    public static float[,] GenerateFixedSizeFalloffMap(int width, int height, HeightMapSettings settings, Vector2 chunkCoord, MeshSettings meshSettings)
    {
        float[,] map = new float[width, height];
        float mapWidth = settings.mapSizeX * (meshSettings.meshWorldSize);
        float mapHeight = settings.mapSizeZ * (meshSettings.meshWorldSize);

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                float adjustedX = (i + chunkCoord.x * (width - 1)) / (mapWidth - 1);
                float adjustedZ = (j + chunkCoord.y * (height - 1)) / (mapHeight - 1);

                float x = Mathf.Clamp01(adjustedX);
                float y = Mathf.Clamp01(adjustedZ);

                float horizontalValue = Mathf.Min(x, 1 - x);
                float verticalValue = Mathf.Min(y, 1 - y);

                float value = Mathf.Max(horizontalValue, verticalValue) * 2 - 1;
                map[j, i] = Evaluate(value, settings);
            }
        }

        return map;
    }

    private static float Evaluate(float value, HeightMapSettings settings)
    {
        float a = settings.falloffA;
        float b = settings.falloffB;
        float blendSize = settings.edgeBlendSize;

        if (value <= blendSize)
        {
            return 0;
        }
        else if (value >= 1 - blendSize)
        {
            return 1;
        }
        else
        {
            float scaledValue = (value - blendSize) / (1 - 2 * blendSize);
            return Mathf.Pow(scaledValue, a) / (Mathf.Pow(scaledValue, a) + Mathf.Pow(b - b * scaledValue, a));
        }
    }
}
