using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenerationHelpers;
using System;

public class IndividualBuildingGenerator : MonoBehaviour
{
    public enum BuildingType
    {
        Ruin,
        Dungeon,
        House,
    }

    public enum BuildingSize
    {
        Small = 10,
        Medium = 15,
        Large = 20
    }

    public BuildingType buildingType;
    public BuildingSize buildingSize;
    public int seed;

    UnityEngine.Tilemaps.TileBase wall;
    UnityEngine.Tilemaps.TileBase floor;

    private void Start()
    {
        Initialise(seed);
    }

    public void Initialise(int seed)
    {
        this.seed = seed;

        Array type = Enum.GetValues(typeof(BuildingType));
        Array size = Enum.GetValues(typeof(BuildingSize));
        System.Random random = new System.Random(seed + (int)transform.position.x + (int)transform.position.y);

        //buildingType = (BuildingType)type.GetValue(random.Next(type.Length));
        //buildingSize = (BuildingSize)type.GetValue(random.Next(size.Length));

        GenerateStructure(seed);
    }

    public void GenerateStructure(int seed)
    {
        System.Random rand = new System.Random(seed);

        switch (buildingType)
        {
            case BuildingType.Dungeon:
                GenerateDungeon();
                break;
            case BuildingType.House:
                break;
            case BuildingType.Ruin:
                break;
        }
    }

    TilemapData GenerateDungeon()
    {
        Debug.Log("Generating");
        TilemapData data = new TilemapData()
        {
            tilePositions = GenericHelper.Vector2IntMap((int)buildingSize, new Vector2Int((int)transform.position.x, (int)transform.position.y)),
            tiles = new UnityEngine.Tilemaps.TileBase[(int)buildingSize * (int)buildingSize]
        };

        int[,] map = new int[(int)buildingSize, (int)buildingSize];

        BuildingGenerator.GenerateOutline(ref map, (int)buildingSize, BuildingGenerator.GenerateDoorPosition((int)buildingSize, Direction.Down), new Vector2Int((int)transform.position.x, (int)transform.position.y));
        BuildingGenerator.GenerateBuilding(ref map, (int)buildingSize - 4, BuildingGenerator.GenerateDoorPosition((int)buildingSize, Direction.Down), new Vector2Int((int)transform.position.x, (int)transform.position.y));

        for (int x = 0; x < (int)buildingSize; x++)
        {
            for (int y = 0; y < (int)buildingSize; y++)
            {
                if (map[x, y] == 1) data.tiles[y * (int)buildingSize + x] = wall;
                else if (map[x, y] == 2) data.tiles[y * (int)buildingSize + x] = floor;
            }
        }
        Debug.Log("Done");
        return data;
    }
}
