using System.Collections.Generic;
using UnityEngine;
using static Logger;

public class GridController : MonoBehaviour {
    public int Width = 1;
    public int Height = 1;
    public float CellRadius = 10f;
    public HexGrid Grid;

    private HexRenderer hexRenderer;
    private WallRenderer wallRenderer;

    private void Awake() {
        hexRenderer = GetComponentInChildren<HexRenderer>();
        wallRenderer = GetComponentInChildren<WallRenderer>();
        Grid = new HexGrid(Width, Height, CellRadius);
    }

    private void Start() {
        UpdateMeshes();
    }

    public void UpdateMeshes() {
        hexRenderer.BuildCellMesh(Grid);
        wallRenderer.BuildWallMesh(Grid);
    }
}