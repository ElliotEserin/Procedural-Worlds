using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    public enum WorldSize
    {
        Small = 200,
        Medium = 400,
        Large = 600,
    }

    public WorldSize worldSize;

    //Seed for the world
    public int seed;

    //Data for the different noise maps
    public NoiseData terrainData;
    public NoiseData temperatureData;
    public NoiseData moistureData;

    WorldData worldData;

    private void Start()
    {
        MapGenerator mapGen = FindObjectOfType<MapGenerator>();

        //Generates the world data
        worldData = mapGen.GenerateMap((int)worldSize, seed, terrainData, temperatureData, moistureData); 

        //Draws the tilemap
        FindObjectOfType<TilemapGenerator>().DrawMap(worldData);
    }

    //Editing the generation in the editor
    void OnValuesUpdated()
    {
        if (!Application.isPlaying)
        {
            FindObjectOfType<MapGenerator>().GenerateMap((int)worldSize, seed, terrainData, temperatureData, moistureData);
        }
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
}

//Stores the world data
public struct WorldData
{
    public Texture2D worldMap;
    public float[,] heightMap;
    public Vector3Int[] tilePositions;
    public TileBase[] tiles;
}
