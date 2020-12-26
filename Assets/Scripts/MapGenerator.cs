using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static TextureGenerator;

//Made with tutorial: https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap,
        ColorMap,
        Mesh
    }
    public DrawMode drawMode;

    public int mapWidth, mapHeight;
    public float noiseScale;
    public int octaves;
    [Range(0, 1)] public float persistance;
    public float lacunarity;
    public int seed;
    public Vector2 offset;

    public float heightMultiplier;
    public AnimationCurve meshHeightCurve;
    public bool autoUpdate;

    public TerrainType[] regions;

    void Start()
    {
        GenerateMap();
    }
        
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colorMap = new Color[mapWidth * mapHeight];
        for(int y = 0; y < mapHeight; y++)
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float currentHeight = noiseMap[x, y];

                for(int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapWidth + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay mapDisplay = FindObjectOfType<MapDisplay>();
        if(drawMode == DrawMode.NoiseMap) mapDisplay.DrawTexture(TextureFromHeightMap(noiseMap));
        else if(drawMode == DrawMode.ColorMap) mapDisplay.DrawTexture(TextureFromColorMap(colorMap, mapWidth, mapHeight));
        else if(drawMode == DrawMode.Mesh) mapDisplay.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, heightMultiplier, meshHeightCurve), TextureFromColorMap(colorMap, mapWidth, mapHeight));
    }

    void OnValidate()
    {
        //clamping values in inspector
        mapWidth = (mapWidth < 1 ? 1 : mapWidth);
        mapHeight = (mapHeight < 1 ? 1 : mapHeight);

        lacunarity = (lacunarity < 1 ? 1 : lacunarity);
        octaves = (octaves < 0 ? 0 : octaves);
    }

    
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}