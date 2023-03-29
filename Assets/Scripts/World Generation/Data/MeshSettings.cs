using UnityEngine;

[CreateAssetMenu()]
public class MeshSettings : UpdatableData
{
    public const int numberSupportedLODs = 5;
    public const int numberSupportedChunkSizes = 9;
    public const int numberSupportedFlatShadedChunkSizes = 3;

    public static readonly int[] supportedChunkSizes = { 48, 72, 96, 120, 144, 168, 192, 216, 240 };

    public float meshScale = 1f;
    public bool useFlatShading;
    [Range(0.01f, 5f)]
    public float blockiness = -.1f;

    [Range(0, numberSupportedChunkSizes - 1)]
    public int chunkSizeIndex;
    [Range(0, numberSupportedFlatShadedChunkSizes - 1)]
    public int flatShadedChunkSizeIndex;


    /// <summary>
    /// Number verticles per line of mesh rendered at LOD = 0.
    /// Includes the 2 extra verticles that are excluded from final mesh, but used for calculating normals
    /// </summary>
    public int numberVerticlesPerLine
    {
        get
        {
            return supportedChunkSizes[(useFlatShading) ? flatShadedChunkSizeIndex : chunkSizeIndex] + 5;
        }
    }

    /// <summary>
    /// Calculate by NumberVerticlesPerLine units between verticles and 2 extra verticles
    /// </summary>
    public float meshWorldSize
    {
        get
        {
            return (numberVerticlesPerLine - 3) * meshScale;
        }
    }
}
