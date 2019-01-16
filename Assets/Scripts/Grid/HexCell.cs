using static Logger;

public class HexCell {
    public float x;
    public float y;
    public float radius;

    public HexCell(float x, float y, float radius) {
        this.radius = radius;
        this.x = (x + y % 2 * 0.5f) * HexUtils.INNER_CONSTANT * radius * 2f;
        this.y = y * radius * 1.5f;
    }
}