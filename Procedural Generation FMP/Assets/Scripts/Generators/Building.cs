using UnityEngine;
using GenerationHelpers;
using System;
using System.Collections;

public class Building : Generator
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
    [Header("Prefabs")]
    public bool usePrefabBuilding;
    public TilemapPrefab prefab;
    public Village.Direction direction;
    public bool useRandomPrefabBuilding;
    public bool randomiseDirection = true;
    public GameObject collectablePrefab;

    public override void Initialise(WorldManager worldManager)
    {
        seed = worldSeed + (int)transform.position.x + (int)transform.position.y;
        System.Random random = new System.Random(seed);

        if (randomiseDirection)
            direction = (Village.Direction)random.Next(0, 4);

        if (!usePrefabBuilding)
        {
            Array type = Enum.GetValues(typeof(BuildingType));
            Array size = Enum.GetValues(typeof(BuildingSize));         

            buildingType = (BuildingType)type.GetValue(random.Next(type.Length));
            buildingSize = (BuildingSize)type.GetValue(random.Next(size.Length));

            base.Initialise(worldManager);
        }
        else
        {
            if(useRandomPrefabBuilding)
                prefab = worldManager.buildingPrefabs[random.Next(0, worldManager.buildingPrefabs.Length)];

            Generate(prefab);
        }

        Instantiate(collectablePrefab, transform.position, Quaternion.identity);
    }

    public void Initialise(int seed, TilemapPrefab[] potentialBuildings, Village.Direction direction)
    {
        this.seed = seed + (int)transform.position.x + (int)transform.position.y;

        System.Random rand = new System.Random(this.seed);
        prefab = potentialBuildings[rand.Next(0, potentialBuildings.Length)];
        this.direction = direction;
        Generate(prefab);

        Instantiate(collectablePrefab, transform.position, Quaternion.identity);
    }

    public void Initialise(int seed, TilemapPrefab building, Village.Direction direction)
    {
        this.seed = seed + (int)transform.position.x + (int)transform.position.y;

        System.Random rand = new System.Random(this.seed);
        prefab = building;
        this.direction = direction;
        Generate(prefab);

        Instantiate(collectablePrefab, transform.position, Quaternion.identity);
    }

    protected override IEnumerator Generate(WorldManager worldManager)
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

        yield return null;
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

            for (int i = 0; i < prefab.tilePositions.Length; i++)
            {
                var oldPos = prefab.tilePositions[i];

                Vector3Int newPos;

                //Rotate
                switch (direction)
                {
                    default:
                    case Village.Direction.Up:
                        newPos = oldPos;
                        break;
                    case Village.Direction.Down:
                        newPos = oldPos * -1;
                        break;
                    case Village.Direction.Right:
                        newPos = new Vector3Int(oldPos.y, -oldPos.x, 0);
                        break;
                    case Village.Direction.Left:
                        newPos = new Vector3Int(-oldPos.y, oldPos.x, 0);
                        break;
                }

                newPos += Vector3Int.RoundToInt(transform.position);

                data.tilePositions[i] = newPos;
            }

            ObjectStore.instance.mapDisplay.DrawVillage(data);
        }

        else Debug.LogWarning("prefab is not suitable");
    }

    TilemapData GenerateDungeon()
    {
        TilemapData data = new TilemapData()
        {
            tilePositions = GenericHelper.Vector2IntMap((int)buildingSize, new Vector2Int((int)transform.position.x, (int)transform.position.y)),
            tiles = new UnityEngine.Tilemaps.TileBase[(int)buildingSize * (int)buildingSize]
        };

        int[,] map = new int[(int)buildingSize, (int)buildingSize];

        GenerationHelpers.BuildingGenerator.GenerateOutline(ref map, (int)buildingSize, GenerationHelpers.BuildingGenerator.GenerateDoorPosition((int)buildingSize, Direction.Down), new Vector2Int((int)buildingSize / 2, (int)buildingSize / 2));
        GenerationHelpers.BuildingGenerator.GenerateBuilding(ref map, (int)buildingSize - 4, GenerationHelpers.BuildingGenerator.GenerateDoorPosition((int)buildingSize - 4, Direction.Down), new Vector2Int((int)buildingSize / 2, (int)buildingSize / 2));

        for (int x = 0; x < (int)buildingSize; x++)
        {
            for (int y = 0; y < (int)buildingSize; y++)
            {
                Debug.Log(map[x,y]);
                if (map[x, y] == 1) data.tiles[y * (int)buildingSize + x] = wall;
                else if (map[x, y] == 2) data.tiles[y * (int)buildingSize + x] = floor;
            }
        }

        ObjectStore.instance.mapDisplay.DrawVillage(data);

        return data;
    }
}
