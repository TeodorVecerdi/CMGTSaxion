using UnityEngine;
using static Logger;
 
public class HexCell {
    public float WorldX;
    public float WorldY;
    public float ArrayX;
    public float ArrayY;
    public float Radius;
    public bool[] Walls;
    public bool Visible = true;
    public bool isFinish = false;
    public int variation;

    public HexCell(float x, float y, float radius) {
        variation = Random.Range(0, Constants.VARIATION_COUNT);
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

    public static Vector2 GetTexture(HexCell cell) {
        if (!cell.Visible) return new Vector2(2, cell.variation);
        if (cell.isFinish) return new Vector2(1, cell.variation);
        return new Vector2(0, cell.variation);
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