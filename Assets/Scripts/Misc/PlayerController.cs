﻿using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using CellIndex = System.ValueTuple<int, int, int>;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour {
    [Header("Script References")]
    public GridController GridController;
    public TargetFollow CameraFollow;
    public Timer Timer;
    [Space]
    [Header("Graphic References")]
    public SpriteRenderer[] ArrowRenderers;
    public RectTransform ContinuePlayingMenu;
    public RectTransform GameOverMenu;
    public TextMeshProUGUI[] ScoreText;
    public TextMeshProUGUI[] HighscoreText;
    public TextMeshProUGUI BonusScoreText;
    public Sprite[] PlayerSprites; // 0 - front, 1 - back, 2 - left, 3 - right
    public SpriteRenderer PlayerSprite;
    [Space]
    [Header("Variables")]
    public int Milestone = 2;

    private readonly Vector3 CenterOffset = new Vector3(HexUtils.INNER_CONSTANT, 1f, 0);
    private bool zoomedOut;
    private bool enableControls = true;
    private bool levelFinished;
    private bool gameOver;
    private bool hasBonusPoints = false;
    private int gridX;
    private int gridY;
    private int score;
    private int level = 1;

    private void Start() {
        ResetPlayer();
    }

    private void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            #if UNITY_EDITOR
            EditorApplication.isPlaying = false;
            #endif
            Application.Quit();
        }
        
        if (Timer.TimeOut) {
            GameOver();
        }
        if (enableControls) {
            //The player needs to be zoomed-in in order to be able to move
            if (Input.GetKeyDown(KeyCode.E)) TryMove(0);
            if (Input.GetKeyDown(KeyCode.D)) TryMove(1);
            if (Input.GetKeyDown(KeyCode.C)) TryMove(2);
            if (Input.GetKeyDown(KeyCode.Z)) TryMove(3);
            if (Input.GetKeyDown(KeyCode.A)) TryMove(4);
            if (Input.GetKeyDown(KeyCode.Q)) TryMove(5);
        }
        if (Input.GetKeyDown(KeyCode.Space) && !levelFinished) {
            ChangeZoom(!zoomedOut);
        }
        if (levelFinished) {
            //Show menu to continue or exit
            ContinuePlayingMenu.localScale = Vector3.one;
            if(hasBonusPoints)
                BonusScoreText.rectTransform.localScale = Vector3.one;
            if (Input.GetKeyDown(KeyCode.N)) {
                #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
                #endif
                Application.Quit();
            }
            if (Input.GetKeyDown(KeyCode.Y)) {
                levelFinished = false;
                GridController.Grid.GenerateMaze();
                ResetPlayer();
                GridController.UpdateMeshes();
                Timer.NewLevel();
                hasBonusPoints = false;
                BonusScoreText.rectTransform.localScale = Vector3.zero;
                ContinuePlayingMenu.localScale = Vector3.zero;
            }
        }
        if (gameOver) {
            if (Input.GetKeyDown(KeyCode.N)) {
                #if UNITY_EDITOR
                EditorApplication.isPlaying = false;
                #endif
                Application.Quit();
            }
            if (Input.GetKeyDown(KeyCode.Y)) {
                levelFinished = false;
                GridController.Grid.GenerateMaze();
                ResetPlayer();
                GridController.UpdateMeshes();
                Timer.NewLevel();
                GameOverMenu.localScale = Vector3.zero;
            }
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
        
        //Update player sprites
        if (direction == 2 || direction == 3) // front sprite
            PlayerSprite.sprite = PlayerSprites[0];
        if (direction == 0 || direction == 5) // back sprite
            PlayerSprite.sprite = PlayerSprites[1];
        if (direction == 4) // left sprite
            PlayerSprite.sprite = PlayerSprites[2];
        if (direction == 1) // right sprite
            PlayerSprite.sprite = PlayerSprites[3];

        //Get new neighbours after move
        List<CellIndex> neighbours = GridController.Grid.GetPassableNeighbours(gridX, gridY);

        //Make accessible neighbours visible
        GridController.Grid.MakeNeighboursVisible(gridX, gridY, neighbours);

        //Update arrows to show the directions that the player can move in 
        UpdateArrows(neighbours);

        //Check if the player has reached the finish
        if (gridX == GridController.Grid.finishCellX && gridY == GridController.Grid.finishCellY) {
            levelFinished = true;
            level++;
            score++;
            ValueTuple<int, string> bonusScore = CalculateBonusScore();
            score += bonusScore.Item1;
            if (bonusScore.Item1 != 0) {
                hasBonusPoints = true;
                if (bonusScore.Item1 == 1)
                    BonusScoreText.text = "+1 BONUS POINT\n" + bonusScore.Item2;
                else
                    BonusScoreText.text = "+"+bonusScore.Item1+" BONUS POINTS\n" + bonusScore.Item2;
            } 
            UpdateScore();
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

    private void ResetPlayer() {
        gridX = 0;
        gridY = 0;
        
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

    private void UpdateScore() {
        int highscore = 0;
        if (PlayerPrefs.HasKey("Highscore")) highscore = PlayerPrefs.GetInt("Highscore");
        if (score > highscore) highscore = score;
        PlayerPrefs.SetInt("Highscore", highscore);
        ScoreText[0].text = "Score: " + score;
        ScoreText[1].text = "Score: " + score;
        HighscoreText[0].text = "Highscore: " + highscore;
        HighscoreText[1].text = "Highscore: " + highscore;
        
    }
    
    private void GameOver() {
        gameOver = true;
        ChangeZoom(true);
        Timer.BackgroundImage.color = Color.black;
        GameOverMenu.localScale = Vector3.one;
    }

    private ValueTuple<int, string> CalculateBonusScore() {
        int bonus = 0;
        string bonusText = "";
        bool milestone = false;
        if (level != 0 && level % Milestone == 0) {
            bonus += level / Milestone;
            milestone = true;
        }
        if (Timer.TimeRemaining > Timer.StartingTimeLeft / 2)
            bonus++;
        if (Timer.TimeRemaining > Timer.StartingTimeLeft / 4 * 3)
            bonus++;
        if (milestone) {
            if (bonus == 3) bonusText = "(GREAT TIME & MILESTONE)";
            if (bonus == 2) bonusText = "(GOOD TIME & MILESTONE)";
            if (bonus == 1) bonusText = "(MILESTONE)";
        }
        else {
            if (bonus == 2) bonusText = "(GREAT TIME)";
            if (bonus == 1) bonusText = "(GOOD TIME)";
        }
        return ValueTuple.Create(bonus, bonusText);
    }
}