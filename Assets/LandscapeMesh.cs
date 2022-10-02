using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class LandscapeMesh : MonoBehaviour
{
    public int xSize = 10;
    public int ySize = 10;
    public float unitSize = 1;
    [Range(0, 1)]
    public float xHarmonic1;
    [Range(0, 1)]
    public float yHarmonic1;
    [Range(0, 1)]
    public float xHarmonic2;
    [Range(0, 1)]
    public float yHarmonic2;
    public float perlinScale1;
    public float perlinScale2;
    private Mesh mesh;
    private Vector3[] vertices;
    public float PerspectiveScaleMod = 1f;
    // more compact towards far end
    public float PerspectivePowScale = 2f;
    public float WarpScale = 1f;
    public float WarpScaleGradient = 2f;
    public bool WarpScaleYMod = true;
    // more compact towards far end
    public float WarpScaleYModGradient = 2f;

    void Awake()
    {
        Generate();
    }

    public void Generate()
    {
        if (mesh != null) mesh.Clear();
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        var seed = Random.value * 100;
        for (int i = 0, y = 0; y <= ySize; y++)
        {
            var yNorm = (float)y / (float)ySize;
            var yScale = PerspectiveScaleMod == 0 ? 1 : Mathf.Pow(yNorm, PerspectivePowScale) * PerspectiveScaleMod;

            for (int x = 0; x <= xSize; x++, i++)
            {
                var xNorm = (float)x / (float)xSize;
                var warpCoeff = Mathf.Pow(Mathf.Abs(0.5f - xNorm), WarpScaleGradient) * WarpScale * (WarpScaleYMod ? Mathf.Pow(yNorm, WarpScaleYModGradient) : 1f);

                vertices[i] = new Vector3(x * unitSize + (xNorm - 0.5f) * warpCoeff, y * unitSize, -1 * (Perlin(seed, x, y, xHarmonic1, yHarmonic1, perlinScale1 * yScale) + Perlin(seed, x, y, xHarmonic2, yHarmonic2, perlinScale2 * yScale)) + warpCoeff);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[xSize * ySize * 6];
        for (int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++)
        {
            for (int x = 0; x < xSize; x++, ti += 6, vi++)
            {
                triangles[ti] = vi;
                triangles[ti + 3] = triangles[ti + 2] = vi + 1;
                triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
                triangles[ti + 5] = vi + xSize + 2;
            }
        }
        mesh.triangles = triangles;
        mesh.RecalculateNormals();


    }

    float Perlin(float seed, int y, int x, float xH, float yH, float scale)
    {
        return Mathf.PerlinNoise((x + seed) * xH, (y + seed) * yH) * scale;
    }

    void Update()
    {

    }
}
