using System;
using System.Collections.Generic;
using static Logger;
using CellIndex = System.ValueTuple<int, int, int>;

public class HexGrid {
    public int Width;
    public int Height;
    public float CellRadius;
    public HexCell[,] Cells;
    public int Length => Cells.Length;

    public HexGrid(int width, int height, float cellRadius) {
        Width = width;
        Height = height;
        CellRadius = cellRadius;
        GenerateMaze();
    }


    public void GenerateMaze() {
        ResetMaze();
        BuildWalls();
    }

    private void BuildWalls() {
        bool[,] visited = new bool[Width, Height];
        Stack<CellIndex> cellStack = new Stack<CellIndex>();
        int cellsLeft = Width * Height - 1;
        CellIndex current = ValueTuple.Create(0, 0, 0);
        visited[0, 0] = true;
        while (cellsLeft > 0) {
            List<CellIndex> neighbours = GetUnvisitedNeighbours(current, visited);
            //if cell has unvisited neighbours
            if (neighbours.Count != 0) {
                //choose a random unvisited neighbour
                CellIndex chosen = neighbours[UnityEngine.Random.Range(0, neighbours.Count)];

                //push current cell to stack
                cellStack.Push(current);

                //remove wall between current cell and chosen cell
                Cells[current.Item1, current.Item2].RemoveWall(ref Cells[chosen.Item1, chosen.Item2], chosen.Item3);
                current = chosen;
                visited[current.Item1, current.Item2] = true;
                cellsLeft--;
            }
            else if (cellStack.Count != 0) {
                current = cellStack.Pop();
            }
        }
    }

    private void ResetMaze() {
        Cells = new HexCell[Width, Height];
        for (int x = 0; x < Width; x++) {
            for (int y = 0; y < Height; y++) {
                Cells[x, y] = new HexCell(x, y, CellRadius);
            }
        }
    }

    private List<CellIndex> GetCellNeighbours(CellIndex index) {
        int x = index.Item1;
        int y = index.Item2;
        List<CellIndex> neighbours = new List<CellIndex>();
        if (y % 2 == 1) {
            if (x < Width - 1 && y < Height - 1) neighbours.Add(ValueTuple.Create(x + 1, y + 1, 0));
            if (x < Width - 1) neighbours.Add(ValueTuple.Create(x + 1, y, 1));
            if (x < Width - 1 && y > 0) neighbours.Add(ValueTuple.Create(x + 1, y - 1, 2));
            if (y > 0) neighbours.Add(ValueTuple.Create(x, y - 1, 3));
            if (x > 0) neighbours.Add(ValueTuple.Create(x - 1, y, 4));
            if (y < Height - 1) neighbours.Add(ValueTuple.Create(x, y + 1, 5));
        }
        else {
            if (y < Height - 1) neighbours.Add(ValueTuple.Create(x, y + 1, 0));
            if (x < Width - 1) neighbours.Add(ValueTuple.Create(x + 1, y, 1));
            if (y > 0) neighbours.Add(ValueTuple.Create(x, y - 1, 2));
            if (x > 0 && y > 0) neighbours.Add(ValueTuple.Create(x - 1, y - 1, 3));
            if (x > 0) neighbours.Add(ValueTuple.Create(x - 1, y, 4));
            if (x > 0 && y < Height - 1) neighbours.Add(ValueTuple.Create(x - 1, y + 1, 5));
        }

        return neighbours;
    }

    private List<CellIndex> GetUnvisitedNeighbours(CellIndex index, bool[,] visited) {
        List<CellIndex> allNeighbours = GetCellNeighbours(index);
        List<CellIndex> unvisitedNeighbours = new List<CellIndex>();
        allNeighbours.ForEach(neighbour => {
            if(!visited[neighbour.Item1, neighbour.Item2])
                unvisitedNeighbours.Add(neighbour);
        });
        return unvisitedNeighbours;
    }
    
    public List<CellIndex> GetPassableNeighbours(int x, int y) {
        List<CellIndex> neighbours = GetCellNeighbours(ValueTuple.Create(x, y, 0));
        List<CellIndex> passable = new List<CellIndex>();
        for (int i = 0; i < 6; i++) {
            if (!Cells[x, y].Walls[i]) {
                foreach (var neighbour in neighbours) {
                    if (neighbour.Item3 == i) {
                        passable.Add(neighbour);
                        break;
                    }
                }
            }
        }

        return passable;
    }

    public HexCell this[int x, int y] {
        get { return Cells[x, y]; }
        set { Cells[x, y] = value; }
    }
}