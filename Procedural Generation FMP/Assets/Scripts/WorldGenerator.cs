using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldGenerator : MonoBehaviour
{
    public enum WorldSize
    {
        Island = 100,
        Small = 500,
        Medium = 750,
        Large = 1000,
    }

    public WorldSize worldSize;

    public bool useCustomSize;
    public int customSize;

    //Seed for the world
    public int seed;

    //Data for the different noise maps
    public NoiseData terrainData;
    public NoiseData temperatureData;
    public NoiseData moistureData;

    WorldData worldData;

    public UnityEngine.UI.InputField iField;
    public UnityEngine.UI.Dropdown dropdown;

    private void Start()
    {
        MapGenerator mapGen = FindObjectOfType<MapGenerator>();

        //Generates the world data
        worldData = mapGen.GenerateMap(useCustomSize ? customSize:(int)worldSize, seed, terrainData, temperatureData, moistureData); 

        //Draws the tilemap
        //FindObjectOfType<MapDisplay>().DrawMap(worldData);
    }

    public void GenerateWorld()
    {
        MapGenerator mapGen = FindObjectOfType<MapGenerator>();

        //Generates the world data
        worldData = mapGen.GenerateMap(useCustomSize ? customSize : (int)worldSize, seed, terrainData, temperatureData, moistureData);

        //Draws the tilemap
        //FindObjectOfType<MapDisplay>().DrawMap(worldData);
    }

    public void ChangeWorldSize()
    {
        switch (dropdown.value)
        {
            case 0:
                worldSize = WorldSize.Island;
                break;
            case 1:
                worldSize = WorldSize.Small;
                break;
            case 2:
                worldSize = WorldSize.Medium;
                break;
            case 3:
                worldSize = WorldSize.Large;
                break;
        }
    }

    public void ChangeSeed()
    {
        seed = iField.text.GetHashCode();
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
