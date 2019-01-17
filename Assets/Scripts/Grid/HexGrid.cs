using System;
using System.Collections.Generic;
using static Logger;
using CIndex = System.ValueTuple<int, int>;

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
        Stack<CIndex> cellStack = new Stack<CIndex>();
        int cellsLeft = Width * Height - 1;
        CIndex current = ValueTuple.Create(0, 0);
        visited[0, 0] = true;
        while (cellsLeft > 0) {
            List<CIndex> neighbours = GetCellNeighbours(current, visited);
            //if cell has unvisited neighbours
            if (neighbours.Count != 0) {
                //choose a random unvisited neighbour
                CIndex chosen = neighbours[UnityEngine.Random.Range(0, neighbours.Count)];

                //push current cell to stack
                cellStack.Push(current);

                //remove wall between current cell and chosen cell
                int x = chosen.Item1 - current.Item1;
                int y = chosen.Item2 - current.Item2;
                int dir = -1;
                if (current.Item2 % 2 == 1) {
                    if (x == 1) {
                        if (y == 1) dir = 0;
                        else if (y == 0) dir = 1;
                        else if (y == -1) dir = 2;
                    }
                    else if (x == 0) {
                        if (y == -1) dir = 3;
                        if (y == 1) dir = 5;
                    }
                    else if (x == -1 && y == 0) dir = 4;
                }
                else {
                    if (x == 0) {
                        if (y == 1) dir = 0;
                        else if (y == -1) dir = 2;
                    } else if (x == 1 && y == 0) dir = 1;
                    else if (x == -1) {
                        if (y == -1) dir = 3;
                        else if (y == 0) dir = 4;
                        else if (y == 1) dir = 5;
                    }
                }
                Cells[current.Item1, current.Item2].RemoveWall(ref Cells[chosen.Item1, chosen.Item2], dir);
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
                string s = "";
                GetCellNeighbours(ValueTuple.Create(x, y), new bool[Width, Height]).ForEach(v => s += v + ", ");
                Cells[x, y] = new HexCell(x, y, CellRadius);
            }
        }
    }

    private List<CIndex> GetCellNeighbours(CIndex index, bool[,] visited) {
        int x = index.Item1;
        int y = index.Item2;
        List<CIndex> neighbours = new List<CIndex>();
        if (y % 2 == 1) {
            if (x < Width - 1 && y < Height - 1 && !visited[x + 1, y + 1]) neighbours.Add(ValueTuple.Create(x + 1, y + 1));
            if (x < Width - 1 && !visited[x + 1, y]) neighbours.Add(ValueTuple.Create(x + 1, y));
            if (x < Width - 1 && y > 0 && !visited[x + 1, y - 1]) neighbours.Add(ValueTuple.Create(x + 1, y - 1));
            if (y > 0 && !visited[x, y - 1]) neighbours.Add(ValueTuple.Create(x, y - 1));
            if (x > 0 && !visited[x - 1, y]) neighbours.Add(ValueTuple.Create(x - 1, y));
            if (y < Height - 1 && !visited[x, y + 1]) neighbours.Add(ValueTuple.Create(x, y + 1));
        }
        else {
            if (y < Height - 1 && !visited[x, y + 1]) neighbours.Add(ValueTuple.Create(x, y + 1));
            if (x < Width - 1 && !visited[x + 1, y]) neighbours.Add(ValueTuple.Create(x + 1, y));
            if (y > 0 && !visited[x, y - 1]) neighbours.Add(ValueTuple.Create(x, y - 1));
            if (x > 0 && y > 0 && !visited[x - 1, y - 1]) neighbours.Add(ValueTuple.Create(x - 1, y - 1));
            if (x > 0 && !visited[x - 1, y]) neighbours.Add(ValueTuple.Create(x - 1, y));
            if (x > 0 && y < Height - 1 && !visited[x - 1, y + 1]) neighbours.Add(ValueTuple.Create(x - 1, y + 1));
        }
        return neighbours;
    }

    public HexCell this[int x, int y] {
        get { return Cells[x, y]; }
        set { Cells[x, y] = value; }
    }
}