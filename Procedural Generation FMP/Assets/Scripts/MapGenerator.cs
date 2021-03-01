using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap, 
        ColourMap,
        FalloffMap,
        TemperatureMap,
        MoistureMap,
        TileMap
    }

    public DrawMode drawMode;

    public int mapWidth;
    public int mapHeight;

    public NoiseData terrainData;
    public NoiseData temperatureData;
    public NoiseData moistureData;

    public bool useFalloff;

    public bool autoUpdate;

    float[,] falloffMap;

    public TerrainType[] regions;

    private void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapWidth, mapHeight);
    }

    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            GenerateMap();
        }
    }

    public void GenerateMap()
    {
        var terrainMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, terrainData.seed, terrainData.noiseScale, terrainData.octaves, terrainData.persistance, terrainData.lacunarity, terrainData.offset);
        var temperatureMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, terrainData.seed, temperatureData.noiseScale, temperatureData.octaves, temperatureData.persistance, temperatureData.lacunarity, temperatureData.offset);
        var moistureMap = Noise.GenerateNoiseMap(mapWidth, mapHeight, terrainData.seed, moistureData.noiseScale, moistureData.octaves, moistureData.persistance, moistureData.lacunarity, moistureData.offset);

        Color[] colourMap = new Color[mapWidth * mapHeight];

        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                if(useFalloff)
                {
                    terrainMap[x, y] = Mathf.Clamp(terrainMap[x, y] - falloffMap[x, y], 0, 1);
                }

                float currentHeight = terrainMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapWidth + x] = regions[i].colour;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();

        if (drawMode == DrawMode.NoiseMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(terrainMap));
        else if (drawMode == DrawMode.ColourMap)
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapWidth, mapHeight));
        else if (drawMode == DrawMode.FalloffMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapWidth, mapHeight)));
        else if (drawMode == DrawMode.TemperatureMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(temperatureMap));
        else if (drawMode == DrawMode.MoistureMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(moistureMap));
        //else if (drawMode == DrawMode.FalloffMap)
        //Draw Tilemap

    }

    private void OnValidate()
    {
        if (terrainData != null)
        {
            terrainData.OnValuesUpdated -= OnValuesUpdated;
            terrainData.OnValuesUpdated += OnValuesUpdated;
        }
        if (temperatureData != null)
        {
            temperatureData.OnValuesUpdated -= OnValuesUpdated;
            temperatureData.OnValuesUpdated += OnValuesUpdated;
        }
        if (moistureData != null)
        {
            moistureData.OnValuesUpdated -= OnValuesUpdated;
            moistureData.OnValuesUpdated += OnValuesUpdated;
        }

        if (mapWidth < 1)
            mapWidth = 1;

        if (mapHeight < 1)
            mapHeight = 1;

        falloffMap = FalloffGenerator.GenerateFalloffMap(mapWidth, mapHeight);
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color colour;
}

