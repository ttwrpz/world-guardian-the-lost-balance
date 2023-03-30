using UnityEngine;

public class TerrainChunk
{
    public event System.Action<TerrainChunk, bool> onVisibilityChanged;

    const float colliderGenerationDistanceThreshold = 8;
    public Vector2 coord;

    GameObject meshObject;
    Vector2 sampleCenter;
    Bounds bounds;

    MeshRenderer meshRenderer;
    MeshFilter meshFilter;
    MeshCollider meshCollider;

    LODInfo[] detailLevels;
    LODMesh[] lodMeshes;
    int colliderLODIndex;

    HeightMap heightMap;
    bool heightMapReceived;
    int previousLODIndex = -1;
    bool hasSetCollider;
    float maxViewDst;

    HeightMapSettings heightMapSettings;
    MeshSettings meshSettings;
    Transform viewer;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int collisiderLODIndex, Transform parent, Transform viewer, Material material)
    {
        this.coord = coord;
        this.detailLevels = detailLevels;
        this.colliderLODIndex = collisiderLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;

        sampleCenter = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(position, Vector2.one * meshSettings.meshWorldSize);

        meshObject = new GameObject("Terrain Chunk");
        meshRenderer = meshObject.AddComponent<MeshRenderer>();
        meshFilter = meshObject.AddComponent<MeshFilter>();
        meshCollider = meshObject.AddComponent<MeshCollider>();
        meshRenderer.material = material;

        meshObject.transform.position = new Vector3(position.x, 0, position.y);
        meshObject.transform.parent = parent;
        SetVisible(false);

        lodMeshes = new LODMesh[detailLevels.Length];
        for (int i = 0; i < detailLevels.Length; i++)
        {
            lodMeshes[i] = new LODMesh(detailLevels[i].lod);
            lodMeshes[i].UpdateCallback += UpdateTerrainChunk;

            if (i == collisiderLODIndex)
            {
                lodMeshes[i].UpdateCallback += UpdateCollisionMesh;
            }
        }

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numberVerticlesPerLine, meshSettings.numberVerticlesPerLine, heightMapSettings, sampleCenter, coord, meshSettings), OnHeightMapReceived);
    }

    void OnHeightMapReceived(object heightMapObject)
    {
        this.heightMap = (HeightMap)heightMapObject;
        heightMapReceived = true;

        UpdateTerrainChunk();
    }

    Vector2 viewerPosition
    {
        get
        {
            return new Vector2(viewer.position.x, viewer.position.z);
        }
    }

    public float GetHeightAtNormalizedPosition(Vector2 normalizedPosition)
    {
        if (!heightMapReceived)
            return 0;

        int x = Mathf.Clamp(Mathf.FloorToInt(normalizedPosition.x * meshSettings.numberVerticlesPerLine), 0, meshSettings.numberVerticlesPerLine - 1);
        int z = Mathf.Clamp(Mathf.FloorToInt(normalizedPosition.y * meshSettings.numberVerticlesPerLine), 0, meshSettings.numberVerticlesPerLine - 1);

        return heightMap.values[x, z] * heightMapSettings.heightMultiplier;
    }

    public MeshSettings GetMeshSettings()
    {
        return meshSettings;
    }

    public void SetMeshSettings(MeshSettings newMeshSettings)
    {
        meshSettings = newMeshSettings;
    }

    public GameObject GetMeshObject()
    {
        return meshObject;
    }

    public void SetMeshObject(GameObject newMeshObject)
    {
        meshObject = newMeshObject;
    }

    public void UpdateTerrainChunk()
    {
        if (!heightMapReceived)
            return;
        
        bool wasVisible = IsVisible();
        bool visible = true;

        if (!heightMapSettings.useFixedSizeMap)
        {
            float viewerDstFromNearestEdge = Mathf.Sqrt(bounds.SqrDistance(viewerPosition));
            visible = viewerDstFromNearestEdge <= maxViewDst;

            if (visible)
            {
                int lodIndex = 0;

                for (int i = 1; i < detailLevels.Length; i++)
                {
                    if (viewerDstFromNearestEdge > detailLevels[i].visibleDstThreshold)
                    {
                        lodIndex = i + 1;
                    }
                    else
                    {
                        break;
                    }
                }

                if (lodIndex != previousLODIndex)
                {
                    LODMesh lodMesh = lodMeshes[lodIndex];

                    if (lodMesh.hasMesh)
                    {
                        previousLODIndex = lodIndex;
                        meshFilter.mesh = lodMesh.mesh;
                    }
                    else if (!lodMesh.hasRequestedMesh)
                    {
                        lodMesh.RequestMesh(heightMap, meshSettings);
                    }
                }
            }
        }
        else
        {
            if (!lodMeshes[0].hasMesh)
            {
                lodMeshes[0].RequestMesh(heightMap, meshSettings);
            }

            if (lodMeshes[0].hasMesh)
            {
                meshFilter.mesh = lodMeshes[0].mesh;
            }
        }

        if (IsVisible() != visible)
        {
            SetVisible(visible);

            if (onVisibilityChanged != null)
            {
                onVisibilityChanged?.Invoke(this, visible);
            }
        }
    }

    private int collisionMeshGenerationDelay = 3;
    private int framesSinceLastCollisionMeshGeneration = 0;

    public void UpdateCollisionMesh()
    {
        framesSinceLastCollisionMeshGeneration++;

        if (framesSinceLastCollisionMeshGeneration < collisionMeshGenerationDelay)
            return;

        framesSinceLastCollisionMeshGeneration = 0;

        if (hasSetCollider)
            return;

        float sqrDstFromViewerToEdge = bounds.SqrDistance(viewerPosition);
        if (sqrDstFromViewerToEdge < detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
        {
            if (!lodMeshes[colliderLODIndex].hasRequestedMesh)
            {
                lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings);
            }
        }

        if (sqrDstFromViewerToEdge < colliderGenerationDistanceThreshold * colliderGenerationDistanceThreshold)
        {
            if (lodMeshes[colliderLODIndex].hasMesh)
            {
                meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
                hasSetCollider = true;
            }
        }
    }

    public void SetVisible(bool visible)
    {
        meshObject.SetActive(visible);
    }

    public bool IsVisible()
    {
        return meshObject.activeSelf;
    }

    class LODMesh
    {
        public Mesh mesh;
        public bool hasRequestedMesh;
        public bool hasMesh;
        int lod;

        public event System.Action UpdateCallback;

        public LODMesh(int lod)
        {
            this.lod = lod;
        }

        void OnMeshDataReceived(object meshDataObject)
        {
            mesh = ((MeshData)meshDataObject).CreateMesh();
            hasMesh = true;

            UpdateCallback();
        }

        public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
        {
            hasRequestedMesh = true;
            ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
        }
    }
}
