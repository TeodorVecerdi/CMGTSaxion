using System;
using System.Collections.Generic;
using UnityEngine;
using static Logger;
using CellIndex = System.ValueTuple<int, int, int>;

public class PlayerController : MonoBehaviour {
    public SpriteRenderer[] ArrowRenderers;
    public GridController GridController;

    private readonly Vector3 CenterOffset = new Vector3(HexUtils.INNER_CONSTANT, 1f, 0);

    private int gridX = 0;
    private int gridY = 0;
    
    private void Start() {
        Vector3 arrayToWorld = HexUtils.ArrayToWorldCoordinates(0, 0, GridController.CellRadius);
        Vector3 spawnLocation = arrayToWorld + CenterOffset * GridController.CellRadius;
        transform.position = spawnLocation;
        UpdateArrows();
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.E)) TryMove(0);
        if (Input.GetKeyDown(KeyCode.D)) TryMove(1);
        if (Input.GetKeyDown(KeyCode.C)) TryMove(2);
        if (Input.GetKeyDown(KeyCode.Z)) TryMove(3);
        if (Input.GetKeyDown(KeyCode.A)) TryMove(4);
        if (Input.GetKeyDown(KeyCode.Q)) TryMove(5);
    }

    private void UpdateArrows() {
        List<CellIndex> neighbours = GridController.Grid.GetPassableNeighbours(gridX, gridY);
        foreach (var arrowRenderer in ArrowRenderers) {
            arrowRenderer.gameObject.SetActive(false);
        }
        foreach (var neighbour in neighbours) {
            ArrowRenderers[neighbour.Item3].gameObject.SetActive(true);
        }
    }

    private void TryMove(int direction) {
        List<CellIndex> neighbours = GridController.Grid.GetPassableNeighbours(gridX, gridY);
        foreach (var neighbour in neighbours) {
            if (neighbour.Item3 == direction) {
                transform.position += HexUtils.DistanceToNeighbours[direction] * GridController.CellRadius;
                gridX += HexUtils.NeighbourDelta[gridY % 2][direction].Item1;
                gridY += HexUtils.NeighbourDelta[gridY % 2][direction].Item2;
                UpdateArrows();
                return;
            }
        }
    }
}