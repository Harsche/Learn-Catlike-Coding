using System.Collections;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Grid : MonoBehaviour {
    [SerializeField] private int xSize, ySize;
    private Vector3[] vertices;
    private Mesh mesh;

    private void Awake() {
        GenerateGrid();
    }

    private void GenerateGrid() {
        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        GenerateRows();
        GenerateMesh();
        GenerateUV();
    }

    private void OnDrawGizmos() {
        if (vertices == null)
            return;
        Gizmos.color = Color.black;
        for (int i = 0; i < vertices.Length; i++)
            Gizmos.DrawSphere(vertices[i], 0.1f);
    }

    private void GenerateRows() {
        for (int i = 0, y = 0; y <= ySize; y++) {
            GenerateRow(i, y);
            i += xSize + 1;
        }
    }

    private void GenerateRow(int i, int y) {
        for (int x = 0; x <= xSize; x++, i++) {
            vertices[i] = new Vector3(x, y);
        }
    }

    private void GenerateMesh() {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "ProceduralGrid";
        mesh.vertices = vertices;
        mesh.triangles = GenerateQuadRows();
        mesh.RecalculateNormals();
    }

    private int[] GenerateQuadRows() {
        int[] triangles = new int[3 * 2 * xSize * ySize];
        for (int y = 0, ti = 0, vi = 0; y < ySize; y++, vi++)
            GenerateQuadRow(triangles, ref ti, ref vi);
        return triangles;
    }

    private void GenerateQuadRow(int[] triangles, ref int ti, ref int vi) {
        for (int x = 0; x < xSize; ti += 6, vi++, x++) {
            triangles[ti] = vi;
            triangles[ti + 3] = triangles[ti + 2] = vi + 1;
            triangles[ti + 4] = triangles[ti + 1] = xSize + vi + 1;
            triangles[ti + 5] = xSize + vi + 2;
        }
    }

    private void GenerateUV() {
        Vector2[] uv = new Vector2[vertices.Length];
        for (int v = 0; v < vertices.Length; v++)
            uv[v] = new Vector2(vertices[v].x / xSize, vertices[v].y / ySize);
        mesh.uv = uv;
    }

    private void GenerateTangents() {
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);
        for (int i = 0; i < tangents.Length; i++)
            tangents[i] = tangent;
        mesh.tangents = tangents;
    }
}
