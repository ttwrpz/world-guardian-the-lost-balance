using UnityEngine;

public class TerrainChunk
{
    public event System.Action<TerrainChunk, bool> onVisibilityChanged;

    public Vector2 coord;
    const float colliderGenerationDistanceThreshold = 8;

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

    private int collisionLODIndex;

    public TerrainChunk(Vector2 coord, HeightMapSettings heightMapSettings, MeshSettings meshSettings, LODInfo[] detailLevels, int collisionLODIndex, Transform parent, Transform viewer, Material material)
    {
        this.coord = coord;
        this.detailLevels = detailLevels;
        this.collisionLODIndex = collisionLODIndex;
        this.heightMapSettings = heightMapSettings;
        this.meshSettings = meshSettings;
        this.viewer = viewer;

        sampleCenter = coord * meshSettings.meshWorldSize / meshSettings.meshScale;
        Vector2 position = coord * meshSettings.meshWorldSize;
        bounds = new Bounds(sampleCenter, Vector2.one * meshSettings.meshWorldSize);

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
            lodMeshes[i].updateCallback += UpdateTerrainChunk;

            if (i == collisionLODIndex)
            {
                lodMeshes[i].updateCallback += UpdateCollisionMesh;
            }
        }

        maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
    }

    public void Load()
    {
        ThreadedDataRequester.RequestData(() => HeightMapGenerator.GenerateHeightMap(meshSettings.numberVerticlesPerLine, meshSettings.numberVerticlesPerLine, heightMapSettings, sampleCenter), OnHeightMapReceived);
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

    public void UpdateTerrainChunk()
    {
        if (!heightMapReceived)
            return;

        //float viewDstFromNearestEdge = bounds.SqrDistance(viewerPosition);
        float distanceFromViewer = Vector2.Distance(viewerPosition, sampleCenter);
        bool visible = distanceFromViewer <= maxViewDst;

        if (visible)
        {
            int lodIndex = 0;

            for (int i = 1; i < detailLevels.Length; i++)
            {
                if (distanceFromViewer > detailLevels[i].visibleDstThreshold)
                {
                    break;
                }

                lodIndex = i;
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

        if (IsVisible() != visible)
        {
            SetVisible(visible);
            onVisibilityChanged?.Invoke(this, visible);
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
        if (sqrDstFromViewerToEdge > detailLevels[colliderLODIndex].sqrVisibleDstThreshold)
            return;

        if (lodMeshes[colliderLODIndex].hasRequestedMesh)
        {
            lodMeshes[colliderLODIndex].RequestMesh(heightMap, meshSettings);
        }

        if (lodMeshes[colliderLODIndex].hasMesh)
        {
            meshCollider.sharedMesh = lodMeshes[colliderLODIndex].mesh;
            hasSetCollider = true;
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

        public event System.Action updateCallback;

        public LODMesh(int lod)
        {
            this.lod = lod;
        }

        void OnMeshDataReceived(object meshDataObject)
        {
            mesh = ((MeshData)meshDataObject).CreateMesh();
            hasMesh = true;

            updateCallback();
        }

        public void RequestMesh(HeightMap heightMap, MeshSettings meshSettings)
        {
            hasRequestedMesh = true;
            ThreadedDataRequester.RequestData(() => MeshGenerator.GenerateTerrainMesh(heightMap.values, meshSettings, lod), OnMeshDataReceived);
        }
    }
}
