using UnityEngine;

[CreateAssetMenu()]
public class HeightMapSettings : UpdatableData
{
    public NoiseSettings noiseSettings;

    public float heightMultiplier;
    public AnimationCurve heightCurve;

    [Header("Map Size")]
    public bool useFixedSizeMap;
    public int mapSizeX = 1;
    public int mapSizeZ = 1;

    [Header("Falloff Map")]
    public bool useFalloff;
    public FalloffType falloffType;
    public float falloffA = 3;
    public float falloffB = 2.2f;
    public float edgeBlendSize = 0.1f;

    public float[,] fixedSizeFalloffMap;

    public float minHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(0);
        }
    }

    public float maxHeight
    {
        get
        {
            return heightMultiplier * heightCurve.Evaluate(1);
        }
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        noiseSettings.ValidateValues();
        base.OnValidate();

        if (mapSizeX < 1)
        {
            mapSizeX = 1;
        }

        if (mapSizeZ < 1)
        {
            mapSizeZ = 1;
        }

    }
#endif
}

public enum FalloffType
{
    Normal,
    FixedSize
}