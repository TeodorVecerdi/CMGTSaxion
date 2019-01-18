using static Logger;
 
public class HexCell {
    public float WorldX;
    public float WorldY;
    public float ArrayX;
    public float ArrayY;
    public float Radius;
    public bool[] Walls;

    public HexCell(float x, float y, float radius) {
        Radius = radius;
        WorldX = (x + y % 2 * 0.5f) * HexUtils.INNER_CONSTANT * radius * 2f;
        WorldY = y * radius * 1.5f;
        ArrayX = x;
        ArrayY = y;
        Walls = new [] {true, true, true, true, true, true};
    }

    public void RemoveWall(ref HexCell other, int wallIndex) {
        Walls[wallIndex] = false;
        other.Walls[(wallIndex + 3) % 6] = false;
    }
}

public struct Wall {
    public bool exists;
    public bool breakable;
    public int durability;

    public Wall(bool breakable = false, int durability = 0) {
        this.breakable = breakable;
        this.durability = durability;
        exists = true;
    }
}