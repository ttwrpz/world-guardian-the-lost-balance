using UnityEngine;

public class MapPreview : MonoBehaviour
{
    public Renderer textureRender;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public enum DrawMode { NoiseMap, Mesh, FalloffMap };
    public DrawMode drawMode;

    [Header("Data Management")]
    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureData;

    public Material terrainMaterial;

    [Header("Level of Detail Management")]
    [Range(0, MeshSettings.numberSupportedLODs - 1)]
    public int editorPreviewLOD;

    [Header("Editor Properties")]
    public bool autoUpdate;

    public void DrawMapInEditor()
    {
        textureData.ApplyToMaterial(terrainMaterial);
        textureData.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        HeightMap heightMap = HeightMapGenerator.GenerateHeightMap(meshSettings.numberVerticlesPerLine, meshSettings.numberVerticlesPerLine, heightMapSettings, Vector2.zero, Vector2.zero, meshSettings);

        MapPreview display = FindAnyObjectByType<MapPreview>();
        switch (drawMode)
        {
            case DrawMode.NoiseMap:
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(heightMap));
                break;
            case DrawMode.Mesh:
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, editorPreviewLOD));
                break;
            case DrawMode.FalloffMap:

                float[,] falloffMap;
                if (heightMapSettings.falloffType == FalloffType.FixedSize)
                {
                    falloffMap = FalloffGenerator.GenerateFixedSizeFalloffMap(meshSettings.numberVerticlesPerLine, meshSettings.numberVerticlesPerLine, heightMapSettings, Vector2.zero, meshSettings);
                }
                else
                {
                    falloffMap = FalloffGenerator.GenerateFalloffMap(meshSettings.numberVerticlesPerLine, heightMapSettings);
                }

                display.DrawTexture(TextureGenerator.TextureFromHeightMap(new HeightMap(falloffMap, 0, 1)));
                break;
        }
    }

    public void DrawTexture(Texture2D texture)
    {
        textureRender.sharedMaterial.mainTexture = texture;
        textureRender.transform.localScale = new Vector3(texture.width, 1, texture.height) / 10f / meshSettings.meshScale;

        textureRender.gameObject.SetActive(true);
        meshFilter.gameObject.SetActive(false);
    }

    public void DrawMesh(MeshData meshData)
    {
        meshFilter.sharedMesh = meshData.CreateMesh();

        textureRender.gameObject.SetActive(false);
        meshFilter.gameObject.SetActive(true);
    }


    public void GenerateRandomMap()
    {
        heightMapSettings.noiseSettings.seed = UnityEngine.Random.Range(-10000, 10000);
        DrawMapInEditor();
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            DrawMapInEditor();
        }
    }

    void OnTextureValuesUpdated()
    {
        textureData.ApplyToMaterial(terrainMaterial);
    }

    private void OnValidate()
    {
        if (meshSettings != null)
        {
            meshSettings.OnValuesUpdated -= OnValuesUpdated;
            meshSettings.OnValuesUpdated += OnValuesUpdated;
        }

        if (heightMapSettings != null)
        {
            heightMapSettings.OnValuesUpdated -= OnValuesUpdated;
            heightMapSettings.OnValuesUpdated += OnValuesUpdated;
        }

        if (textureData != null)
        {
            textureData.OnValuesUpdated -= OnTextureValuesUpdated;
            textureData.OnValuesUpdated += OnTextureValuesUpdated;
        }
    }
}
