using UnityEngine;
using GenerationHelpers;
using System;

public class IndividualBuildingGenerator : Generator
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
        Medium = 16,
        Large = 20
    }

    public BuildingType buildingType;
    public BuildingSize buildingSize;

    public UnityEngine.Tilemaps.TileBase wall;
    public UnityEngine.Tilemaps.TileBase floor;

    public bool usePrefabBuilding;
    public TilemapPrefab prefab;
    public bool useRandomPrefabBuilding;

    public override void Initialise(int seed)
    {
        this.seed = seed + (int)transform.position.x + (int)transform.position.y;

        if (usePrefabBuilding)
        {
            if (useRandomPrefabBuilding)
            {
                var worldGen = FindObjectOfType<WorldGenerator>();
                System.Random rand = new System.Random(this.seed);
                prefab = worldGen.tilemapPrefabs[rand.Next(0, worldGen.tilemapPrefabs.Length)];
            }

            Generate(prefab);
        }
        else
        {
            this.seed = seed;

            Array type = Enum.GetValues(typeof(BuildingType));
            Array size = Enum.GetValues(typeof(BuildingSize));
            System.Random random = new System.Random(this.seed);

            buildingType = (BuildingType)type.GetValue(random.Next(type.Length));
            buildingSize = (BuildingSize)type.GetValue(random.Next(size.Length));

            Generate();
        }
    }

    protected override void Generate()
    {
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

    public void Generate(TilemapPrefab prefab)
    {
        if (prefab.isSet && prefab != null)
        {
            TilemapData data = new TilemapData()
            {
                tilePositions = new Vector3Int[prefab.tilePositions.Length],
                tiles = prefab.tiles
            };

            for (int x = 0; x < prefab.tilePositions.Length; x++)
            {
                //Debug.Log(x + "\n" + prefab.tiles[x]);
                int midPoint = (int)Mathf.Sqrt(data.tilePositions.Length) / 2;
                data.tilePositions[x] = prefab.tilePositions[x] + new Vector3Int((int)transform.position.x - midPoint, (int)transform.position.y - midPoint, 0);
            }

            FindObjectOfType<MapDisplay>().DrawVillage(data);
        }

        else Debug.Log("prefab is not suitable");
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

        BuildingGenerator.GenerateOutline(ref map, (int)buildingSize, BuildingGenerator.GenerateDoorPosition((int)buildingSize, Direction.Down), new Vector2Int((int)buildingSize/2, (int)buildingSize/2));
        BuildingGenerator.GenerateBuilding(ref map, (int)buildingSize - 4, BuildingGenerator.GenerateDoorPosition((int)buildingSize - 4, Direction.Down), new Vector2Int((int)buildingSize / 2, (int)buildingSize / 2));

        for (int x = 0; x < (int)buildingSize; x++)
        {
            for (int y = 0; y < (int)buildingSize; y++)
            {
                Debug.Log(map[x,y]);
                if (map[x, y] == 1) data.tiles[y * (int)buildingSize + x] = wall;
                else if (map[x, y] == 2) data.tiles[y * (int)buildingSize + x] = floor;
            }
        }

        FindObjectOfType<MapDisplay>().DrawVillage(data);

        return data;
    }
}
