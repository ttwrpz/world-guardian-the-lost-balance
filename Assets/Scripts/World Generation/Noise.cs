using System.Threading.Tasks;
using UnityEngine;

public static class Noise
{
    public enum NormalizeMode { Local, Global };

    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, NoiseSettings settings, Vector2 sampleCenter)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        System.Random prng = new System.Random(settings.seed);
        Vector2[] octaveOffsets = GenerateOctaveOffsets(settings, sampleCenter, prng);

        float maxPossibleHeight = CalculateMaxPossibleHeight(settings);
        float minLocalNoiseHeight = float.MaxValue;
        float maxLocalNoiseHeight = float.MinValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        object syncLock = new object();

        Parallel.For(0, mapHeight, y =>
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float noiseHeight = CalculateNoiseHeight(x, y, halfWidth, halfHeight, settings, octaveOffsets);

                lock (syncLock)
                {
                    minLocalNoiseHeight = Mathf.Min(minLocalNoiseHeight, noiseHeight);
                    maxLocalNoiseHeight = Mathf.Max(maxLocalNoiseHeight, noiseHeight);
                }

                noiseMap[x, y] = noiseHeight;

                if (settings.normalizeMode == NormalizeMode.Global)
                {
                    float normalizedHeight = 1 / (1 + Mathf.Exp(noiseMap[x, y] * 3f));
                    noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                }
            }
        });

        if (settings.normalizeMode == NormalizeMode.Local)
        {
            NormalizeLocalNoiseMap(noiseMap, mapWidth, mapHeight, minLocalNoiseHeight, maxLocalNoiseHeight);
        }

        return noiseMap;
    }

    private static Vector2[] GenerateOctaveOffsets(NoiseSettings settings, Vector2 sampleCenter, System.Random prng)
    {
        Vector2[] octaveOffsets = new Vector2[settings.octaves];

        for (int i = 0; i < settings.octaves; i++)
        {
            float offsetX = prng.Next(-10000, 10000) + settings.offset.x + sampleCenter.x;
            float offsetY = prng.Next(-10000, 10000) - settings.offset.y - sampleCenter.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        return octaveOffsets;
    }

    private static float CalculateMaxPossibleHeight(NoiseSettings settings)
    {
        float maxPossibleHeight = 0;
        float amplitude = 1;

        for (int i = 0; i < settings.octaves; i++)
        {
            maxPossibleHeight += amplitude;
            amplitude *= settings.persistence;
        }

        return maxPossibleHeight;
    }

    private static float CalculateNoiseHeight(int x, int y, float halfWidth, float halfHeight, NoiseSettings settings, Vector2[] octaveOffsets)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < settings.octaves; i++)
        {
            float sampleX = ((x - halfWidth) + octaveOffsets[i].x) / settings.scale * frequency;
            float sampleY = ((y - halfHeight) + octaveOffsets[i].y) / settings.scale * frequency;

            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
            noiseHeight += perlinValue * amplitude;

            amplitude *= settings.persistence;
            frequency *= settings.lacunarity;
        }

        return noiseHeight;
    }

    private static void NormalizeLocalNoiseMap(float[,] noiseMap, int mapWidth, int mapHeight, float minLocalNoiseHeight, float maxLocalNoiseHeight)
    {
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
            }
        }
    }
}

[System.Serializable]
public class NoiseSettings
{
    public Noise.NormalizeMode normalizeMode;

    public float scale = 50;

    public int octaves = 6;
    [Range(0, 1)]
    public float persistence = .6f;
    public float lacunarity = 2;

    public int seed;
    public Vector2 offset;

    public void ValidateValues()
    {
        scale = Mathf.Max(scale, 0.01f);
        octaves = Mathf.Max(octaves, 1);
        lacunarity = Mathf.Max(lacunarity, 1);
        persistence = Mathf.Clamp01(persistence);
    }
}