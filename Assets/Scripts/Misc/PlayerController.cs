using System.Collections.Generic;
using UnityEngine;
using CellIndex = System.ValueTuple<int, int, int>;

public class PlayerController : MonoBehaviour {
    public SpriteRenderer[] ArrowRenderers;
    public GridController GridController;
    public TargetFollow CameraFollow;

    private readonly Vector3 CenterOffset = new Vector3(HexUtils.INNER_CONSTANT, 1f, 0);
    private bool zoomedOut;

    private int gridX;
    private int gridY;

    private void Start() {
        Vector3 arrayToWorld = HexUtils.ArrayToWorldCoordinates(0, 0, GridController.CellRadius);
        Vector3 spawnLocation = arrayToWorld + CenterOffset * GridController.CellRadius;
        transform.position = spawnLocation;

        List<CellIndex> neighbours = GridController.Grid.GetPassableNeighbours(gridX, gridY);
        GridController.Grid[0, 0].Visible = true;
        UpdateArrows(neighbours);
        MakeNeighboursVisible(neighbours);
    }

    private void Update() {
        if (!zoomedOut) { //The player needs to be zoomed-in in order to be able to move
            if (Input.GetKeyDown(KeyCode.E)) TryMove(0);
            if (Input.GetKeyDown(KeyCode.D)) TryMove(1);
            if (Input.GetKeyDown(KeyCode.C)) TryMove(2);
            if (Input.GetKeyDown(KeyCode.Z)) TryMove(3);
            if (Input.GetKeyDown(KeyCode.A)) TryMove(4);
            if (Input.GetKeyDown(KeyCode.Q)) TryMove(5);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            //Zoom out/in
            zoomedOut = !zoomedOut;
            //Show whole maze
            if (zoomedOut) {
                CameraFollow.target = null;
                float orthoSize = GridController.CellRadius + GridController.CellRadius * (GridController.Height - 1) * 0.75f;
                CameraFollow.gameObject.GetComponent<Camera>().orthographicSize = orthoSize;
                CameraFollow.transform.position = new Vector3(GridController.Height * GridController.CellRadius * HexUtils.INNER_CONSTANT, orthoSize, -10f);
            }
            //Show player
            else {
                //reset camera size and target
                CameraFollow.gameObject.GetComponent<Camera>().orthographicSize = GridController.CellRadius * 1.5f;
                CameraFollow.transform.position = transform.position + Vector3.back * 10;
                CameraFollow.target = transform;
            }
        }
    }

    private void TryMove(int direction) {
        List<CellIndex> neighbours = GridController.Grid.GetPassableNeighbours(gridX, gridY);
        foreach (var neighbour in neighbours) {
            if (neighbour.Item3 == direction) {
                Move(direction);
                return;
            }
        }
    }

    private void Move(int direction) {
        //Move the player to the new cell and update internal array position
        transform.position += HexUtils.DistanceToNeighbours[direction] * GridController.CellRadius;
        gridX += HexUtils.NeighbourDelta[gridY % 2][direction].Item1;
        gridY += HexUtils.NeighbourDelta[gridY % 2][direction].Item2;

        //Get new neighbours after move
        List<CellIndex> neighbours = GridController.Grid.GetPassableNeighbours(gridX, gridY);

        //Make accessible neighbours visible
        MakeNeighboursVisible(neighbours);

        //Update arrows to show the directions that the player can move in 
        UpdateArrows(neighbours);

        //Redraw meshes
        GridController.UpdateMeshes();
    }

    private void UpdateArrows(List<CellIndex> neighbours = null) {
        //Get neighbours if not received through parameter
        if (neighbours == null)
            neighbours = GridController.Grid.GetPassableNeighbours(gridX, gridY);

        //Show only the arrows that point to cells accessible by the player.
        foreach (var arrowRenderer in ArrowRenderers) {
            arrowRenderer.gameObject.SetActive(false);
        }
        foreach (var neighbour in neighbours) {
            ArrowRenderers[neighbour.Item3].gameObject.SetActive(true);
        }
    }

    private void MakeNeighboursVisible(List<CellIndex> neighbours = null) {
        //Get neighbours if not received through parameter
        if (neighbours == null)
            neighbours = GridController.Grid.GetPassableNeighbours(gridX, gridY);

        foreach (var neighbour in neighbours)
            GridController.Grid.Cells[neighbour.Item1, neighbour.Item2].Visible = true;
    }
}