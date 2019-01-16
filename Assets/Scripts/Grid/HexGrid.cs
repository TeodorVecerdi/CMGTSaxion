using static Logger;

public class HexGrid {
    public int Width;
    public int Height;
    public float CellRadius;
    public HexCell[] Cells;

    public HexGrid(int width, int height, float cellRadius) {
        Width = width;
        Height = height;
        CellRadius = cellRadius;
        Cells = new HexCell[width * height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                // reverse y coord to adapt to Unity's bottom-left origin
                Cells[x*width+height-y-1] = new HexCell(x, y, cellRadius);
            }
        }
    }

    public int Length => Cells.Length;

    public HexCell this[int x, int y] {
        get { return this[x * Width + y]; }
        set { this[x * Width + y] = value;  }
    }
    
    public HexCell this[int index] {
        get { return Cells[index]; }
        set { Cells[index] = value; }
    }
}