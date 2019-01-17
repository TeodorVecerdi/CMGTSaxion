﻿using Boo.Lang;
using UnityEngine;
using static Logger;

public class HexRenderer : MonoBehaviour {
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;

    private void Awake() {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Cell Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
    }

    public void BuildCellMesh(HexGrid grid) {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        for (int x = 0; x < grid.Width; x++) {
            for (int y = 0; y < grid.Height; y++) {
                if(grid[x, y] != null)
                    AddHex(grid[x, y]);
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();
    }

    private void AddHex(HexCell hexCell) {
        Vector3 center = new Vector3(hexCell.WorldX + hexCell.Radius * HexUtils.INNER_CONSTANT, hexCell.WorldY + hexCell.Radius, 0f);
        for (int i = 0; i < 6; i++) {
            AddTriangle(center, center + HexUtils.Vertices[i % 6] * hexCell.Radius, center + HexUtils.Vertices[(i + 1) % 6] * hexCell.Radius);
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