using UnityEngine;

public static class HexUtils {
    public const float INNER_CONSTANT = 0.8660254037f;

    public static readonly Vector3[] Vertices = {
        new Vector3(0f, 1f, 0f),
        new Vector3(INNER_CONSTANT, 0.5f, 0f),
        new Vector3(INNER_CONSTANT, -0.5f, 0f),
        new Vector3(0f, -1f, 0f),
        new Vector3(-INNER_CONSTANT, -0.5f, 0f),
        new Vector3(-INNER_CONSTANT, 0.5f, 0f)
    };

    public static Vector3 WorldToArrayCoordinates(float x, float y, float radius) {
        return new Vector3(
            x / INNER_CONSTANT / radius / 2f - y % 2 * 0.5f,
            y / radius / 1.5f,
            0f
        );
    }

    public static Vector3 ArrayToWorldCoordinates(float x, float y, float radius) {
        return new Vector3(
            (x + y % 2 * 0.5f) * INNER_CONSTANT * radius * 2f,
            y * radius * 1.5f,
            0f
        );
    }
}