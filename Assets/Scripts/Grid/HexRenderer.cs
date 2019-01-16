using Boo.Lang;
using UnityEngine;
using static Logger;

public class HexRenderer : MonoBehaviour {
    private Mesh hexMesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    private void Awake() {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "Grid Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
    }

    private void Start() { }

    public void BuildGrid(HexGrid grid) {
        hexMesh.Clear();
        vertices.Clear();
        triangles.Clear();
        for (int x = 0; x < grid.Width; x++) {
            for (int y = 0; y < grid.Height; y++) {
                if(grid[x, y] != null)
                    AddHex(grid[x, y]);
            }
        }
        hexMesh.vertices = vertices.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.RecalculateNormals();
    }

    private void AddHex(HexCell hexCell) {
        Vector3 center = new Vector3(hexCell.x + hexCell.radius * HexUtils.INNER_CONSTANT, hexCell.y + hexCell.radius, 0f);
        for (int i = 0; i < 6; i++) {
            AddTriangle(center, center + HexUtils.HexVertices[i % 6] * hexCell.radius, center + HexUtils.HexVertices[(i + 1) % 6] * hexCell.radius);
        }
    }

    private void AddTriangle(Vector3 a, Vector3 b, Vector3 c) {
        int index = triangles.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        triangles.Add(index);
        triangles.Add(index + 1);
        triangles.Add(index + 2);
    }
}