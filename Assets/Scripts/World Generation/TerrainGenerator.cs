using System.Collections.Generic;
using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    const float viewerMoveThresholdForChunkUpdate = 25f;
    const float sqrViewerMoveThresholdForChunkUpdate = viewerMoveThresholdForChunkUpdate * viewerMoveThresholdForChunkUpdate;

    public int colliderLODIndex;
    public LODInfo[] detailLevels;

    public MeshSettings meshSettings;
    public HeightMapSettings heightMapSettings;
    public TextureData textureSettings;

    public Transform viewer;
    public Vector2 viewerPosition;
    public Vector2 viewerPositionOld;

    public Material terrainMaterial;
    float meshWorldSize;
    int chunksVisibleInViewDst;

    Dictionary<Vector2, TerrainChunk> terrianChunkDictionary = new();
    List<TerrainChunk> visibleTerrainChunks = new();

    private void Start()
    {
        textureSettings.ApplyToMaterial(terrainMaterial);
        textureSettings.UpdateMeshHeights(terrainMaterial, heightMapSettings.minHeight, heightMapSettings.maxHeight);

        float maxViewDst = detailLevels[detailLevels.Length - 1].visibleDstThreshold;
        meshWorldSize = meshSettings.meshWorldSize;
        chunksVisibleInViewDst = Mathf.RoundToInt(maxViewDst / meshWorldSize);

        UpdateVisibleChunks();
    }

    private void Update()
    {
        viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        if (viewerPosition != viewerPositionOld)
        {
            foreach (TerrainChunk terrainChunk in visibleTerrainChunks)
            {
                terrainChunk.UpdateCollisionMesh();
            }
        }

        if ((viewerPositionOld - viewerPosition).sqrMagnitude > sqrViewerMoveThresholdForChunkUpdate)
        {
            viewerPositionOld = viewerPosition;
            UpdateVisibleChunks();
        }
    }

    void UpdateVisibleChunks()
    {
        HashSet<Vector2> alreadyUpdatedChunkCoords = new();

        for (int i = visibleTerrainChunks.Count - 1; i >= 0; i--)
        {
            alreadyUpdatedChunkCoords.Add(visibleTerrainChunks[i].coord);
            visibleTerrainChunks[i].UpdateTerrainChunk();
        }

        visibleTerrainChunks.Clear();

        if (heightMapSettings.useFixedSizeMap)
        {
            for (int z = 0; z < heightMapSettings.mapSizeZ; z++)
            {
                for (int x = 0; x < heightMapSettings.mapSizeX; x++)
                {
                    Vector2 viewedChunkCoord = new Vector2(x, z);

                    if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                    {
                        if (terrianChunkDictionary.TryGetValue(viewedChunkCoord, out TerrainChunk chunk))
                        {
                            chunk.UpdateTerrainChunk();
                        }
                        else
                        {
                            TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, terrainMaterial);
                            terrianChunkDictionary.Add(viewedChunkCoord, newChunk);
                            newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                            newChunk.Load();
                        }
                    }
                }
            }
        } else
        {
            int currentChunkCoordX = Mathf.RoundToInt(viewerPosition.x / meshWorldSize);
            int currentChunkCoordY = Mathf.RoundToInt(viewerPosition.y / meshWorldSize);

            if (heightMapSettings.useFixedSizeMap)
            {
                for (int z = 0; z < heightMapSettings.mapSizeZ; z++)
                {
                    for (int x = 0; x < heightMapSettings.mapSizeX; x++)
                    {
                        Vector2 viewedChunkCoord = new Vector2(x, z);

                        if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                        {
                            if (terrianChunkDictionary.ContainsKey(viewedChunkCoord))
                            {
                                terrianChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                            }
                            else
                            {
                                TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, terrainMaterial);
                                terrianChunkDictionary.Add(viewedChunkCoord, newChunk);
                                newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                                newChunk.Load();
                            }
                        }
                    }
                }
            }
            else
            {
                for (int yOffset = -chunksVisibleInViewDst; yOffset <= chunksVisibleInViewDst; yOffset++)
                {
                    for (int xOffset = -chunksVisibleInViewDst; xOffset <= chunksVisibleInViewDst; xOffset++)
                    {
                        Vector2 viewedChunkCoord = new(currentChunkCoordX + xOffset, currentChunkCoordY + yOffset);

                        if (!alreadyUpdatedChunkCoords.Contains(viewedChunkCoord))
                        {
                            if (terrianChunkDictionary.ContainsKey(viewedChunkCoord))
                            {
                                terrianChunkDictionary[viewedChunkCoord].UpdateTerrainChunk();
                            }
                            else
                            {
                                TerrainChunk newChunk = new TerrainChunk(viewedChunkCoord, heightMapSettings, meshSettings, detailLevels, colliderLODIndex, transform, viewer, terrainMaterial);
                                terrianChunkDictionary.Add(viewedChunkCoord, newChunk);
                                newChunk.onVisibilityChanged += OnTerrainChunkVisibilityChanged;
                                newChunk.Load();
                            }
                        }

                    }
                }
            }
        }
    }

    void OnTerrainChunkVisibilityChanged(TerrainChunk chunk, bool isVisible)
    {
        if (isVisible)
        {
            visibleTerrainChunks.Add(chunk);
        } else
        {
            visibleTerrainChunks.Remove(chunk);
        }
    }

}

[System.Serializable]
public struct LODInfo
{
    [Range(0, MeshSettings.numberSupportedLODs - 1)]
    public int lod;
    public float visibleDstThreshold;

    public float sqrVisibleDstThreshold
    {
        get
        {
            return visibleDstThreshold * visibleDstThreshold;
        }
    }
}
