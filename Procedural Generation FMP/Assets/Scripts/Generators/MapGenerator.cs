using System.Collections.Generic;
using System.Collections;
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

    public static int mapRatio = 4;
    [HideInInspector] public static int mapDimension;

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
        UIManager.UpdateLoadScreenText("Generating map...");
        base.Initialise(worldManager);
    }

    protected override IEnumerator Generate(WorldManager worldManager)
    {
        var size = (int)worldManager.worldSize;

        WorldData wd = new WorldData
        {
            tilePositions = new Vector3Int[size * size],
            tiles = new TileBase[size * size],
            tileTypeMap = new TileType[size, size]
        };

        falloffMap = FalloffGenerator.GenerateFalloffMap(size, size);

        yield return null;

        UIManager.UpdateLoadScreenText("Generating terrain.");
        //Perlin noise arrays
        var terrainMap = Noise.GenerateNoiseMap(size, seed, terrainData.noiseScale, terrainData.octaves, terrainData.persistance, terrainData.lacunarity, terrainData.offset);
        yield return null;
        UIManager.UpdateLoadScreenText("Generating deserts and tundras.");
        var temperatureMap = Noise.GenerateNoiseMap(size, seed, temperatureData.noiseScale, temperatureData.octaves, temperatureData.persistance, temperatureData.lacunarity, temperatureData.offset);
        yield return null;
        UIManager.UpdateLoadScreenText("Generating woodlands and savanas.");
        var moistureMap = Noise.GenerateNoiseMap(size, seed, moistureData.noiseScale, moistureData.octaves, moistureData.persistance, moistureData.lacunarity, moistureData.offset);
        yield return null;

        mapDimension = (size - (size % mapRatio)) / mapRatio;

        //color array to convert to texture2D
        Color[] colourMap = new Color[mapDimension * mapDimension];

        UIManager.UpdateLoadScreenText("Coloring in the blank spots.");

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

                            if(y % mapRatio == 0 && x % mapRatio == 0)
                                colourMap[(y/mapRatio) * mapDimension + (x/mapRatio)] = biome.colour;

                            wd.tiles[y * size + x] = biome.tile;
                            wd.tileTypeMap[x, y] = biome;
                        }
                        else
                        {
                            if (y % mapRatio == 0 && x % mapRatio == 0)
                                colourMap[(y / mapRatio) * mapDimension + (x / mapRatio)] = regions[i].colour;

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
                DisplayMap(colourMap, mapDimension);
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
        yield return null;

        UIManager.UpdateLoadScreenText("Spawning drago - oh... thats a lot of dragons...");

        //Sets the world map texture
        wd.worldMap = colourMap;
        yield return null;

        //sets the height map for later use
        wd.heightMap = terrainMap;

        worldManager.worldData = wd; //returns generated world data

        //Draw whole map
        ObjectStore.instance.mapDisplay.DrawWorldMap(wd);

        //const int chunkSize = 1024;

        ////Draw map in chunks
        //for (int c = 0; c < wd.tilePositions.Length; c+=chunkSize)
        //{
        //    UIManager.UpdateLoadScreenText($"Sculpting chunk {c/chunkSize}.");
        //    TilemapData chunk = GetChunk(c);
        //    ObjectStore.instance.mapDisplay.DrawTerrain(chunk);
        //    yield return null;
        //}

        FinishGenerating(worldManager);

        // FUNCIONS
        //TilemapData GetChunk(int startPoint)
        //{
        //    List<Vector3Int> positions = new List<Vector3Int>();
        //    List<TileBase> tiles = new List<TileBase>();

        //    for (int i = 0; i < startPoint + chunkSize; i++)
        //    {
        //        if (i < wd.tilePositions.Length)
        //        {
        //            positions.Add(wd.tilePositions[i]);
        //            tiles.Add(wd.tiles[i]);
        //        }
        //    }

        //    return new TilemapData(positions.ToArray(), tiles.ToArray());
        //}
    }

    //Overloaded methods for different types of textures
    public void DisplayMap(float[,] map)
    {
        ObjectStore.instance.mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(map));
    }
    public void DisplayMap(Color[] colourMap, int size)
    {
         ObjectStore.instance.mapDisplay.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, size, size));
    }
    public void DisplayMap(int size)
    {
        ObjectStore.instance.mapDisplay.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(size, size)));
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

