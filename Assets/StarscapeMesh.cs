using System;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class StarscapeMesh : MonoBehaviour
{
    public int xSize = 10;
    public int ySize = 10;
    public int zSize = 10;
    public float unitSize = 1;
    public float distance = 1;
    public float randomDistanceShift = 0;
    public float randomSizeShift = 0;
    private Mesh mesh;
    private Vector3[] vertices;

    public bool Tetra = true;


    void Awake()
    {
        Generate();
    }

    public void Generate()
    {
        if (mesh != null) mesh.Clear();
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[xSize * ySize * zSize * 4];
        int[] triangles = new int[xSize * ySize * zSize * (Tetra ? 12 : 6)];

        for (int z = 0; z < zSize; z++)
        {
            for (int y = 0; y < ySize; y++)
            {
                for (int x = 0; x < xSize; x++)
                {
                    if (Tetra) MakeTetra(x, y, z, distance, vertices, triangles);
                    else MakeQuad(x, y, z, distance, vertices, triangles);
                    //uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                    //tangents[i] = tangent;
                }
            }
        }
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();


    }

    private void MakeTetra(int x, int y, int z, float distance, Vector3[] vertices, int[] triangles)
    {
        var i = (xSize * ySize * z + xSize * y + x) * 4;
        var j = (xSize * ySize * z + xSize * y + x) * 12;
        var start = new Vector3(x * (unitSize + distance), y * (unitSize + distance), z * (unitSize + distance))
            - new Vector3((unitSize + distance) * xSize * 0.5f, (unitSize + distance) * ySize * 0.5f, (unitSize + distance) * zSize * 0.5f)
            + Random.onUnitSphere * Random.Range(-randomDistanceShift, randomDistanceShift);

        var mySize = randomSizeShift != 0 ? unitSize * Random.Range(-randomSizeShift, randomSizeShift) : unitSize;
        vertices[i] = new Vector3(start.x, start.y, start.z);
        vertices[i + 1] = new Vector3(start.x + mySize * 0.5f, start.y + mySize, start.z + mySize * 0.35f);
        vertices[i + 2] = new Vector3(start.x + mySize, start.y, start.z);
        vertices[i + 3] = new Vector3(start.x + mySize * 0.5f, start.y, start.z + mySize * 0.7f);

        triangles[j] = i;
        triangles[j + 1] = i + 1;
        triangles[j + 2] = i + 2;

        triangles[j + 3] = i;
        triangles[j + 4] = i + 3;
        triangles[j + 5] = i + 1;

        triangles[j + 6] = i;
        triangles[j + 7] = i + 2;
        triangles[j + 8] = i + 3;

        triangles[j + 9] = i + 1;
        triangles[j + 10] = i + 3;
        triangles[j + 11] = i + 2;
    }

    private void MakeQuad(int x, int y, int z, float distance, Vector3[] vertices, int[] triangles)
    {
        var i = (xSize * ySize * z + xSize * y + x) * 4;
        var j = (xSize * ySize * z + xSize * y + x) * 6;
        var start = new Vector3(x * (unitSize + distance), y * (unitSize + distance), z * (unitSize + distance))
            - new Vector3((unitSize + distance) * xSize * 0.5f, (unitSize + distance) * ySize * 0.5f, (unitSize + distance) * zSize * 0.5f)
            + Random.onUnitSphere * Random.Range(-randomDistanceShift, randomDistanceShift);
        var mySize = randomSizeShift != 0 ? unitSize * Random.Range(-randomSizeShift, randomSizeShift): unitSize;
        vertices[i] = new Vector3(start.x, start.y, start.z);
        vertices[i + 1] = new Vector3(start.x, start.y + mySize, start.z);
        vertices[i + 2] = new Vector3(start.x + mySize, start.y + mySize, start.z);
        vertices[i + 3] = new Vector3(start.x + mySize, start.y, start.z);
        triangles[j] = i;
        triangles[j + 1] = i + 1;
        triangles[j + 2] = i + 3;
        triangles[j + 3] = i + 3;
        triangles[j + 4] = i + 1;
        triangles[j + 5] = i + 2;
    }

    void Update()
    {

    }
}
