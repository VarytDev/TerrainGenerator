using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        //calculating offset for map samples based on seed and manual offset
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++)
        {
            float offsetX = prng.Next(-10000, 10000) + offset.x; //or 100,000
            float offsetY = prng.Next(-10000, 10000) + offset.y; //or 100,000
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        //clamping scale to minimum value of 0.0001
        if(scale <= 0) scale = 0.0001f;

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //calculate center of noise map
        float halfWidth = mapWidth / 2;
        float halfHeight = mapHeight / 2;

        //first two loops go through every point on the map
        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                //generating perlin noise value for each point, and layering octaves
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for(int i = 0; i < octaves; i++)
                {    
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                //setting min and max noiseHeight
                maxNoiseHeight = (noiseHeight > maxNoiseHeight ? noiseHeight : maxNoiseHeight);
                minNoiseHeight = (noiseHeight < minNoiseHeight ? noiseHeight : minNoiseHeight);

                noiseMap[x, y] = noiseHeight;
            }
        }

        //normalizing noiseHeight in every point
        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }
}
