using UnityEngine;
using static Logger;

public class GridController : MonoBehaviour {
    public int Width = 1;
    public int Height = 1;
    public float CellRadius = 10f;

    private HexGrid grid;
    private HexRenderer hexGridRenderer;

    private void Awake() {
        hexGridRenderer = GetComponentInChildren<HexRenderer>();
        grid = new HexGrid(Width, Height, CellRadius);
    }

    private void Start() {
        hexGridRenderer.BuildGrid(grid);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.A)) {
            int index = Random.Range(0, grid.Length);
            grid[index] = null;
            Log($"Removed Cell at {index/Width},{index%Width}");
            hexGridRenderer.BuildGrid(grid);
        }
    }
}