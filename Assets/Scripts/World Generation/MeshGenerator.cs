using UnityEngine;

public static class MeshGenerator
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, MeshSettings meshSettings, int levelOfDetail)
    {
        int skipIncrement = (levelOfDetail == 0)
            ? 1
            : levelOfDetail * 2;
        int numberVecticlesPerLine = meshSettings.numberVerticlesPerLine;

        Vector2 topLeft = new Vector2(-1, 1) * meshSettings.meshWorldSize / 2f;

        MeshData meshData = new(numberVecticlesPerLine, skipIncrement, meshSettings.useFlatShading);
        if (meshSettings.blockiness == 0) { meshSettings.blockiness = -.1f; }

        int[,] vertexIndicesMap = new int[numberVecticlesPerLine, numberVecticlesPerLine];
        int meshVertexIndex = 0;
        int outOfMeshVertexIndex = -1;

        for (int y = 0; y < numberVecticlesPerLine; y++)
        {
            for (int x = 0; x < numberVecticlesPerLine; x++)
            {
                bool isOutOfMeshVertex = y == 0 || y == numberVecticlesPerLine - 1 || x == 0 || x == numberVecticlesPerLine - 1;
                bool isSkippedVertex = x > 2 && x < numberVecticlesPerLine - 3 && y > 2 && y < numberVecticlesPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);

                if (isOutOfMeshVertex)
                {
                    vertexIndicesMap[x, y] = outOfMeshVertexIndex;
                    outOfMeshVertexIndex--;
                }
                else if (!isSkippedVertex)
                {
                    {
                        vertexIndicesMap[x, y] = meshVertexIndex;
                        meshVertexIndex++;
                    }
                }

            }
        }

        for (int y = 0; y < numberVecticlesPerLine; y++)
        {
            for (int x = 0; x < numberVecticlesPerLine; x++)
            {
                bool isSkippedVertex = x > 2 && x < numberVecticlesPerLine - 3 && y > 2 && y < numberVecticlesPerLine - 3 && ((x - 2) % skipIncrement != 0 || (y - 2) % skipIncrement != 0);


                if (!isSkippedVertex)
                {
                    bool isOutOfMeshVertex = y == 0 || y == numberVecticlesPerLine - 1 || x == 0 || x == numberVecticlesPerLine - 1;
                    bool isMeshEdgeVertex = (y == 1 || y == numberVecticlesPerLine - 2 || x == 1 || x == numberVecticlesPerLine - 2) && !isOutOfMeshVertex;
                    bool isMainVertex = (x - 2) % skipIncrement == 0 && (y - 2) % skipIncrement == 0 && !isOutOfMeshVertex && !isMeshEdgeVertex;
                    bool isEdgeConnectionVertex = (y == 2 || y == numberVecticlesPerLine - 3 || x == 2 || x == numberVecticlesPerLine - 3) && !isOutOfMeshVertex && !isMeshEdgeVertex && !isMainVertex;

                    int vertexIndex = vertexIndicesMap[x, y];

                    Vector2 percent = new Vector2(x - 1, y - 1) / (numberVecticlesPerLine - 3);
                    Vector2 vertexPosition2D = topLeft + new Vector2(percent.x, -percent.y) * meshSettings.meshWorldSize;

                    //float height = heightMap[x, y];
                    float height = SnapHeight(heightMap[x, y], meshSettings.blockiness);

                    if (isEdgeConnectionVertex)
                    {
                        bool isVertical = x == 2 || x == numberVecticlesPerLine - 3;
                        int dstToMainVertexA = ((isVertical) ? y - 2 : x - 2) % skipIncrement;
                        int dstToMainVertexB = skipIncrement - dstToMainVertexA;
                        float dstPercentFromAToB = dstToMainVertexA / (float)skipIncrement;

                        Coord coordA = new((isVertical) ? x : x - dstToMainVertexA, (isVertical) ? y - dstToMainVertexA : y);
                        Coord coordB = new((isVertical) ? x : x + dstToMainVertexB, (isVertical) ? y + dstToMainVertexB : y);

                        float heightMainVertexA = heightMap[coordA.x, coordB.y];
                        float heightMainVertexB = heightMap[coordA.x, coordB.y];

                        height = heightMainVertexA * (1 - dstPercentFromAToB) + heightMainVertexB * dstPercentFromAToB;

                        EdgeConnectionVertexData edgeConnectionVertexData = new(vertexIndex, vertexIndicesMap[coordA.x, coordA.y], vertexIndicesMap[coordB.x, coordB.y], dstPercentFromAToB);
                        meshData.DeclareEdgeConnectionVertex(edgeConnectionVertexData);
                    }

                    meshData.AddVertex(new Vector3(vertexPosition2D.x, height, vertexPosition2D.y), percent, vertexIndex);

                    bool createTriangle = x < numberVecticlesPerLine - 1 && y < numberVecticlesPerLine - 1 && (!isEdgeConnectionVertex || (x != 2 && y != 2));

                    if (createTriangle)
                    {
                        int currentIncrement = (isMainVertex && x != numberVecticlesPerLine - 3 && y != numberVecticlesPerLine - 3)
                            ? skipIncrement
                            : 1;

                        int a = vertexIndicesMap[x, y];
                        int b = vertexIndicesMap[x + currentIncrement, y];
                        int c = vertexIndicesMap[x, y + currentIncrement];
                        int d = vertexIndicesMap[x + currentIncrement, y + currentIncrement];

                        meshData.AddTriangle(a, d, c);
                        meshData.AddTriangle(d, a, b);
                    }

                }
            }
        }

        meshData.ProcessMesh();

        return meshData;
    }

    private static float SnapHeight(float height, float blockiness)
    {
        return Mathf.Round(height * blockiness) / blockiness;
    }

    public struct Coord
    {
        public readonly int x;
        public readonly int y;

        public Coord(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}

public class EdgeConnectionVertexData
{
    public int vertexIndex;
    public int mainVertexAIndex;
    public int mainVertexBIndex;
    public float dstPercentFromAToB;

    public EdgeConnectionVertexData(int vertexIndex, int mainVertexAIndex, int mainVertexBIndex, float dstPercentFromAToB)
    {
        this.vertexIndex = vertexIndex;
        this.mainVertexAIndex = mainVertexAIndex;
        this.mainVertexBIndex = mainVertexBIndex;
        this.dstPercentFromAToB = dstPercentFromAToB;
    }

}

public class MeshData
{
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;
    Vector3[] bakedNormals;

    Vector3[] outOfMeshVertices;
    int[] outOfMeshTriangles;

    int triangleIndex;
    int outOfMeshTriangleIndex;

    EdgeConnectionVertexData[] edgeConnectionVertices;
    int edgeConnectionVertexIndex;

    bool useFlatShading;

    public MeshData(int numberVerticesPerLine, int skipIncrement, bool useFlatShading)
    {
        this.useFlatShading = useFlatShading;

        int numberMeshEdgeVertices = (numberVerticesPerLine - 2) * 4 - 4;
        int numberEdgeConnectionVertices = (skipIncrement - 1) * (numberVerticesPerLine - 5) / skipIncrement * 4;
        int numberMainVericesPerLine = (numberVerticesPerLine - 5) / skipIncrement + 1;
        int numberMainVertices = numberMainVericesPerLine * numberMainVericesPerLine;

        vertices = new Vector3[numberMeshEdgeVertices + numberEdgeConnectionVertices + numberMainVertices];
        uvs = new Vector2[vertices.Length];
        edgeConnectionVertices = new EdgeConnectionVertexData[numberEdgeConnectionVertices];

        int numberMeshEdgeTriangles = (numberVerticesPerLine - 4) * 8;
        int numberMainTriangles = (numberMainVericesPerLine - 1) * (numberMainVericesPerLine - 1) * 2;
        triangles = new int[(numberMeshEdgeTriangles + numberMainTriangles) * 3];

        outOfMeshVertices = new Vector3[numberVerticesPerLine * 4 - 4];
        outOfMeshTriangles = new int[(numberVerticesPerLine - 2) * 24];
    }

    public void AddVertex(Vector3 vertexPosition, Vector2 uv, int vertexIndex)
    {
        if (vertexIndex < 0)
        {
            outOfMeshVertices[-vertexIndex - 1] = vertexPosition;
        }
        else
        {
            vertices[vertexIndex] = vertexPosition;
            uvs[vertexIndex] = uv;
        }
    }

    public void AddTriangle(int a, int b, int c)
    {
        if (a < 0 || b < 0 || c < 0)
        {
            outOfMeshTriangles[outOfMeshTriangleIndex] = a;
            outOfMeshTriangles[outOfMeshTriangleIndex + 1] = b;
            outOfMeshTriangles[outOfMeshTriangleIndex + 2] = c;
            outOfMeshTriangleIndex += 3;
        }
        else
        {
            triangles[triangleIndex] = a;
            triangles[triangleIndex + 1] = b;
            triangles[triangleIndex + 2] = c;
            triangleIndex += 3;
        }
    }

    public void DeclareEdgeConnectionVertex(EdgeConnectionVertexData edgeConnectionVertexData)
    {
        edgeConnectionVertices[edgeConnectionVertexIndex] = edgeConnectionVertexData;
        edgeConnectionVertexIndex++;
    }

    Vector3[] CalculateNormals()
    {
        Vector3[] vertexNormals = new Vector3[vertices.Length];
        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = triangles[normalTriangleIndex];
            int vertexIndexB = triangles[normalTriangleIndex + 1];
            int vertexIndexC = triangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            vertexNormals[vertexIndexA] += triangleNormal;
            vertexNormals[vertexIndexB] += triangleNormal;
            vertexNormals[vertexIndexC] += triangleNormal;
        }

        int borderTriangleCount = outOfMeshTriangles.Length / 3;
        for (int i = 0; i < borderTriangleCount; i++)
        {
            int normalTriangleIndex = i * 3;
            int vertexIndexA = outOfMeshTriangles[normalTriangleIndex];
            int vertexIndexB = outOfMeshTriangles[normalTriangleIndex + 1];
            int vertexIndexC = outOfMeshTriangles[normalTriangleIndex + 2];

            Vector3 triangleNormal = SurfaceNormalFromIndices(vertexIndexA, vertexIndexB, vertexIndexC);
            if (vertexIndexA >= 0)
            {
                vertexNormals[vertexIndexA] += triangleNormal;
            }

            if (vertexIndexB >= 0)
            {
                vertexNormals[vertexIndexB] += triangleNormal;
            }

            if (vertexIndexC >= 0)
            {
                vertexNormals[vertexIndexC] += triangleNormal;
            }

        }

        for (int i = 0; i < vertexNormals.Length; i++)
        {
            vertexNormals[i].Normalize();
        }

        return vertexNormals;
    }

    void ProcessEdgeConnectionVertices()
    {
        foreach (EdgeConnectionVertexData e in edgeConnectionVertices)
        {
            bakedNormals[e.vertexIndex] = bakedNormals[e.mainVertexAIndex] * (1 - e.dstPercentFromAToB) + bakedNormals[e.mainVertexBIndex] * e.dstPercentFromAToB;
        }
    }

    Vector3 SurfaceNormalFromIndices(int indexA, int indexB, int indexC)
    {
        Vector3 pointA = (indexA < 0) ? outOfMeshVertices[-indexA - 1] : vertices[indexA];
        Vector3 pointB = (indexB < 0) ? outOfMeshVertices[-indexB - 1] : vertices[indexB];
        Vector3 pointC = (indexC < 0) ? outOfMeshVertices[-indexC - 1] : vertices[indexC];

        Vector3 sideAB = pointB - pointA;
        Vector3 sideAC = pointC - pointA;
        return Vector3.Cross(sideAB, sideAC).normalized;
    }

    public void ProcessMesh()
    {
        if (useFlatShading)
        {
            FlatShading();
        }
        else
        {
            BakeNormals();
            ProcessEdgeConnectionVertices();
        }
    }

    void BakeNormals()
    {
        bakedNormals = CalculateNormals();
    }

    void FlatShading()
    {
        Vector3[] flatShadedVertices = new Vector3[triangles.Length];
        Vector2[] flatShadedUvs = new Vector2[triangles.Length];

        for (int i = 0; i < triangles.Length; i++)
        {
            flatShadedVertices[i] = vertices[triangles[i]];
            flatShadedUvs[i] = uvs[triangles[i]];
            triangles[i] = i;
        }

        vertices = flatShadedVertices;
        uvs = flatShadedUvs;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new()
        {
            vertices = vertices,
            triangles = triangles,
            uv = uvs,
            normals = bakedNormals
        };

        if (useFlatShading)
        {
            mesh.RecalculateNormals();
        }
        else
        {
            mesh.normals = bakedNormals;
        }

        return mesh;
    }
}