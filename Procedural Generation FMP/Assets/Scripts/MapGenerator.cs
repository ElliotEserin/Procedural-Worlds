using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapGenerator : MonoBehaviour
{
    //Type of texture to generate
    public enum DrawMode
    {
        None,
        NoiseMap,
        ColourMap,
        FalloffMap,
        TemperatureMap,
        MoistureMap,
        TileMap
    }

    public DrawMode drawMode;

    //adds ocean around the island
    public bool useFalloff;
    float[,] falloffMap;

    public bool autoUpdate;

    //Representation of different map regions
    public List<TerrainType> regions;
    public List<Biomes> temperature;

    //Generates the terrain - returns world data
    public WorldData GenerateMap(int size, int seed, NoiseData terrainData, NoiseData temperatureData, NoiseData moistureData)
    {
        WorldData wd = new WorldData
        {
            tilePositions = new Vector3Int[size * size],
            tiles = new TileBase[size * size]
        };

        falloffMap = FalloffGenerator.GenerateFalloffMap(size, size);

        //Perlin noise arrays
        var terrainMap = Noise.GenerateNoiseMap(size, seed, terrainData.noiseScale, terrainData.octaves, terrainData.persistance, terrainData.lacunarity, terrainData.offset);
        var temperatureMap = Noise.GenerateNoiseMap(size, seed, temperatureData.noiseScale, temperatureData.octaves, temperatureData.persistance, temperatureData.lacunarity, temperatureData.offset);
        var moistureMap = Noise.GenerateNoiseMap(size, seed, moistureData.noiseScale, moistureData.octaves, moistureData.persistance, moistureData.lacunarity, moistureData.offset);

        //color array to convert to texture2D
        Color[] colourMap = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if(useFalloff)
                {
                    //Subtracts falloff from height map
                    terrainMap[x, y] = Mathf.Clamp(terrainMap[x, y] - falloffMap[x, y], 0, 1);
                }

                //populates tile position array
                wd.tilePositions[y * size + x] = new Vector3Int(size - x, size - y, 0);

                //Get values of a tile
                float currentHeight = terrainMap[x, y];
                float currentTemp = temperatureMap[x, y];
                float currentMoisture = moistureMap[x, y];

                //Assign color and tile based on values
                for (int i = 0; i < regions.Count; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        if(regions[i].allowBiomes)
                        {
                            colourMap[y * size + x] = temperature[(int)Mathf.Round(currentTemp * (temperature.Count-1))].moisture[(int)Mathf.Round(currentMoisture * (temperature.Count - 1))].colour;
                            wd.tiles[y * size + x] = regions[i].tile;
                        }
                        else
                        {
                            colourMap[y * size + x] = regions[i].colour;
                            wd.tiles[y * size + x] = regions[i].tile;
                        }

                        break;
                    }
                }
            }
        }

        //Handles generating and displaying a type of texture
        switch(drawMode)
        {
            default:
            case DrawMode.ColourMap:
                DisplayMap(colourMap, size);
                break;
            case DrawMode.FalloffMap:
                DisplayMap(size);
                break;
            case DrawMode.None:
                break;
            case DrawMode.NoiseMap:
                DisplayMap(terrainMap);
                break;
            case DrawMode.TemperatureMap:
                DisplayMap(temperatureMap);
                break;
            case DrawMode.MoistureMap:
                DisplayMap(moistureMap);
                break;
        }
        
        //Sets the world map texture
        wd.worldMap = TextureGenerator.TextureFromColourMap(colourMap, size, size);

        //sets the height map for later use
        wd.heightMap = terrainMap;

        return wd; //returns generated world data
    }

    //Overloaded methods for different types of textures
    public void DisplayMap(float[,] map)
    {
        MapDisplay display = FindObjectOfType<MapDisplay>();
        
        display.DrawTexture(TextureGenerator.TextureFromHeightMap(map));
    }
    public void DisplayMap(Color[] colourMap, int size)
    {
        MapDisplay display = FindObjectOfType<MapDisplay>();

        display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, size, size));
    }
    public void DisplayMap(int size)
    {
        MapDisplay display = FindObjectOfType<MapDisplay>();

        display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(size, size)));
    }
}

[System.Serializable]
public class Biomes
{
    public string type;
    public BiomeType[] moisture;
}

