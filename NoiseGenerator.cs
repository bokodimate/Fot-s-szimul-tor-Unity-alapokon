using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator : MonoBehaviour
{

    public static Texture2D GenerateTexture(int width, int height, float scale) {
        Texture2D texture = new Texture2D(width, height);
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Color color = CalculateColor(x, y, width, height, scale);
                texture.SetPixel(x, y, color);
            }
        }
        texture.Apply();
        return texture;
    }

    static Color CalculateColor(int x, int y, int width, int height, float scale) {
        float xCoord = (float)x / width * scale;
        float yCoord = (float)y / height * scale;

        float sample = Mathf.PerlinNoise(xCoord, yCoord);
        return new Color(sample, sample, sample);
    }
}
