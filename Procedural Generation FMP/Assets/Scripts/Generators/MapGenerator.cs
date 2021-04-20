using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GenerationHelpers;

public class MapGenerator : Generator
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

    public NoiseData terrainData;
    public NoiseData temperatureData;
    public NoiseData moistureData;

    //Representation of different map regions
    public List<TerrainType> regions;
    public List<Biomes> temperature;

    public override void Initialise(WorldManager worldManager)
    {
        seed = worldSeed;

        Generate(worldManager);
    }

    protected override void Generate(WorldManager worldManager)
    {
        var size = (int)worldManager.worldSize;

        WorldData wd = new WorldData
        {
            tilePositions = new Vector3Int[size * size],
            tiles = new TileBase[size * size],
            tileTypeMap = new TileType[size, size]
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
                if (useFalloff)
                {
                    //Subtracts falloff from height map
                    terrainMap[x, y] = Mathf.Clamp(terrainMap[x, y] - falloffMap[x, y], 0, 1);
                }

                //populates tile position array
                wd.tilePositions[y * size + x] = new Vector3Int(x, y, 0);

                //Get values of a tile
                float currentHeight = terrainMap[x, y];
                float currentTemp = temperatureMap[x, y];
                float currentMoisture = moistureMap[x, y];

                //Assign color and tile based on values
                for (int i = 0; i < regions.Count; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        if (regions[i].allowBiomes)
                        {
                            var biome = temperature[(int)Mathf.Round(currentTemp * (temperature.Count - 1))].moisture[(int)Mathf.Round(currentMoisture * (temperature.Count - 1))];

                            colourMap[y * size + x] = biome.colour;
                            wd.tiles[y * size + x] = biome.tile;
                            wd.tileTypeMap[x, y] = biome;
                        }
                        else
                        {
                            colourMap[y * size + x] = regions[i].colour;
                            wd.tiles[y * size + x] = regions[i].tile;
                            wd.tileTypeMap[x, y] = regions[i];
                        }

                        break;
                    }
                }
            }
        }

        //Handles generating and displaying a type of texture
        switch (drawMode)
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

        worldManager.worldData = wd; //returns generated world data

        ObjectStore.instance.mapDisplay.DrawWorldMap(wd);

        FinishGenerating(worldManager);
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

    //Subscribing to the OnValuesUpdated event
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
    }

    //Editing the generation in the editor
    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            Initialise(ObjectStore.instance.worldManager);
        }
    }
}

[System.Serializable]
public class Biomes
{
    public string type;
    public BiomeType[] moisture;
}

