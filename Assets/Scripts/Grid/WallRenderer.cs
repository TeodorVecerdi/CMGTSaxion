using System.Collections.Generic;
using UnityEngine;
using static Logger;

public class WallRenderer : MonoBehaviour {
    public float WallThickness = 1f;
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    private void Awake() {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Wall Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
    }

    public void BuildWallMesh(HexGrid grid) {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        for (int x = 0; x < grid.Width; x++) {
            for (int y = 0; y < grid.Height; y++) {
                if(grid[x, y] != null)
                    AddHexWalls(grid[x, y]);
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    private void AddHexWalls(HexCell hexCell) {
        Vector3 center = new Vector3(hexCell.WorldX + hexCell.Radius * HexUtils.INNER_CONSTANT, hexCell.WorldY + hexCell.Radius, 0f);
        for (int i = 0; i < 6; i++) {
            if(hexCell.Walls[i])
                AddWall(center, i, hexCell.Radius);
        }
    }

    private void AddWall(Vector3 center, int index, float radius) {
        Vector3 A = HexUtils.Vertices[index % 6] * radius;
        Vector3 B = HexUtils.Vertices[(index + 1) % 6] * radius;
        Vector3 perpendicular = new Vector3(-A.y + B.y, A.x - B.x, 0f).normalized * WallThickness;
        Vector3 C = perpendicular + A;
        Vector3 D = perpendicular + B;
        
        AddTriangle(center + C, center + A, center + B);
        AddTriangle(center + C, center + B, center + D);
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