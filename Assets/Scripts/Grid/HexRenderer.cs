using Boo.Lang;
using UnityEngine;
using static Logger;

public class HexRenderer : MonoBehaviour {
    private Mesh mesh;
    private List<Vector3> vertices;
    private List<int> triangles;
    public List<Vector2> uvs;

    private Vector2 UV_TEXTURE_SIZE = new Vector2(1f / 3f, 1f / 1f);

    private void Awake() {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Cell Mesh";
        uvs = new List<Vector2>();
        vertices = new List<Vector3>();
        triangles = new List<int>();
    }

    public void BuildCellMesh(HexGrid grid) {
        mesh.Clear();
        vertices.Clear();
        triangles.Clear();
        uvs.Clear();
        for (int x = 0; x < grid.Width; x++) {
            for (int y = 0; y < grid.Height; y++) {
                if(grid[x, y] != null)
                    AddHex(grid[x, y]);
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateNormals();
    }

    private void AddHex(HexCell hexCell) {
        Vector2 UVCenter = UV_TEXTURE_SIZE*0.5f;
        Vector3 center = new Vector3(hexCell.WorldX + hexCell.Radius * HexUtils.INNER_CONSTANT, hexCell.WorldY + hexCell.Radius, 0f);
        for (int i = 0; i < 6; i++) {
            AddTriangle(center, center + HexUtils.Vertices[i % 6] * hexCell.Radius, center + HexUtils.Vertices[(i + 1) % 6] * hexCell.Radius);
            AddUvs(i, UVCenter, HexCell.GetTexture(hexCell));
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

    private void AddUvs(int index, Vector2 center, Vector2 texture) {
        center += texture*UV_TEXTURE_SIZE;
        uvs.Add(center);
        uvs.Add(center + ToVector2(HexUtils.Vertices[index % 6]) * UV_TEXTURE_SIZE / 2f);
        uvs.Add(center + ToVector2(HexUtils.Vertices[(index+1) % 6]) * UV_TEXTURE_SIZE / 2f);
    }

    private Vector2 ToVector2(Vector3 v) {
        return new Vector2(v.x, v.y);
    }
}