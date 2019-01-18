using System.Collections.Generic;
using UnityEngine;
using static Logger;

public class GridController : MonoBehaviour {
    public int Width = 1;
    public int Height = 1;
    public float CellRadius = 10f;

    private HexGrid grid;
    private HexRenderer hexRenderer;
    private WallRenderer wallRenderer;

    private void Awake() {
        hexRenderer = GetComponentInChildren<HexRenderer>();
        wallRenderer = GetComponentInChildren<WallRenderer>();
        grid = new HexGrid(Width, Height, CellRadius);
    }

    private void Start() {
        hexRenderer.BuildCellMesh(grid);
        wallRenderer.BuildWallMesh(grid);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            int x = Random.Range(0, grid.Width);
            int y = Random.Range(0, grid.Height);
            grid[x, y] = null;
            Log($"Removed Cell at {x},{y}");
            hexRenderer.BuildCellMesh(grid);
        }
    }
}