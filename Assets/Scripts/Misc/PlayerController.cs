using System.Collections.Generic;
using UnityEngine;
using CellIndex = System.ValueTuple<int, int, int>;

public class PlayerController : MonoBehaviour {
    public SpriteRenderer[] ArrowRenderers;
    public GridController GridController;
    public TargetFollow CameraFollow;
    public TimeRemaining Timer;

    private readonly Vector3 CenterOffset = new Vector3(HexUtils.INNER_CONSTANT, 1f, 0);
    private bool zoomedOut;
    private bool enableControls = true;

    private int gridX;
    private int gridY;

    private void Start() {
        Vector3 arrayToWorld = HexUtils.ArrayToWorldCoordinates(0, 0, GridController.CellRadius);
        Vector3 spawnLocation = arrayToWorld + CenterOffset * GridController.CellRadius;
        transform.position = spawnLocation;

        List<CellIndex> neighbours = GridController.Grid.GetPassableNeighbours(gridX, gridY);
        GridController.Grid[0, 0].Visible = true;
        UpdateArrows(neighbours);
        GridController.Grid.MakeNeighboursVisible(0, 0, neighbours);
        
        int x = Random.Range(GridController.Width / 2, GridController.Width);
        int y = Random.Range(GridController.Height / 2, GridController.Height);
        GridController.Grid.SetFinishCell(x, y);
        GridController.Grid.MakeNeighboursVisible(x, y);
        Timer.StartTimer();
    }

    private void Update() {
        if (enableControls) { //The player needs to be zoomed-in in order to be able to move
            if (Input.GetKeyDown(KeyCode.E)) TryMove(0);
            if (Input.GetKeyDown(KeyCode.D)) TryMove(1);
            if (Input.GetKeyDown(KeyCode.C)) TryMove(2);
            if (Input.GetKeyDown(KeyCode.Z)) TryMove(3);
            if (Input.GetKeyDown(KeyCode.A)) TryMove(4);
            if (Input.GetKeyDown(KeyCode.Q)) TryMove(5);
        }
        if (Input.GetKeyDown(KeyCode.Space)) {
            ChangeZoom(!zoomedOut);
        }
    }

    private void ChangeZoom(bool zoomStatus) {
        zoomedOut = zoomStatus;
        //Show whole maze
        if (zoomedOut) {
            enableControls = false;
            CameraFollow.target = null;
            float orthoSize = GridController.CellRadius + GridController.CellRadius * (GridController.Height - 1) * 0.75f;
            CameraFollow.gameObject.GetComponent<Camera>().orthographicSize = orthoSize;
            CameraFollow.transform.position = new Vector3(GridController.Height * GridController.CellRadius * HexUtils.INNER_CONSTANT, orthoSize, -10f);
            
            //Make timer slider semi-transparent
            Color c = Timer.FillImage.color;
            c.a = 0.25f;
            Timer.FillImage.color = c;
            c = Timer.BackgroundImage.color;
            c.a = 0.25f;
            Timer.BackgroundImage.color = c;

        }
        //Show player
        else {
            enableControls = true;
            //reset camera size and target
            CameraFollow.gameObject.GetComponent<Camera>().orthographicSize = GridController.CellRadius * 1.5f;
            CameraFollow.transform.position = transform.position + Vector3.back * 10;
            CameraFollow.target = transform;
            
            //Make timer slider opaque
            Color c = Timer.FillImage.color;
            c.a = 1f;
            Timer.FillImage.color = c;
            c = Timer.BackgroundImage.color;
            c.a = 1f;
            Timer.BackgroundImage.color = c;
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
        GridController.Grid.MakeNeighboursVisible(gridX, gridY, neighbours);

        //Update arrows to show the directions that the player can move in 
        UpdateArrows(neighbours);
        
        //Check if the player has reached the finish
        if (gridX == GridController.Grid.finishCellX && gridY == GridController.Grid.finishCellY) {
            ChangeZoom(true);
            Timer.StopTimer();
            GridController.Grid.ShowAllCells();
        }
        
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
}