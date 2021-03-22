using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GenerationHelpers;

public class VillageGenerator : Generator
{
    public enum VillageSize //Dimensions of the village
    {
        Small = 40,
        Medium = 60,
        Large = 80
    }

    public enum VillageType //Type of generation
    {
        Grid,
    }

    [Header("Village Roads")]
    //Tiles
    public TileBase majorRoadTile;
    public TileBase minorRoadTile;

    //Falloff
    public bool useFalloff;
    public Texture2D falloffMap;

    public VillageSize villageSize;
    public VillageType villageType;

    public int minCellSize = 10;

    public bool useMajorRoads = true;
    [Range(0, 1)]
    public float majorRoadDensity = 0.5f;
    public int majorRoadFrequency = 2;
    public bool thickRoads;
    [Range(0, 1)]
    public float minorRoadDensity = 0.5f;

    public bool linkRoads = true;

    [Header("Buildings")]
    public TileBase wall;
    public TileBase floor;
    [Range(0,1)]
    public float buildingDensity;

    [Range(0, 1)]
    public float chanceOfJoining;

    [Range(0, 1)]
    public float largeBuildingDensity;
    public bool buildHouses;

    List<Vector2Int> potentialLargeBuildingLocations;

    public TilemapData villageData;
    public bool drawVillage;

    public bool useBuildingPrefabs;
    public IndividualBuildingGenerator controller;
    public TilemapPrefab[] largeBuildings;

    List<IndividualBuildingGenerator> largeBuildingsToGenerate;

    public override void Initialise(int seed)
    {
        this.seed = seed;

        Array values = Enum.GetValues(typeof(VillageSize));
        System.Random random = new System.Random(seed + (int)transform.position.x + (int)transform.position.y);
        villageSize = (VillageSize)values.GetValue(random.Next(values.Length));

        Generate();
    }

    public override void Generate()
    {
        largeBuildingsToGenerate = new List<IndividualBuildingGenerator>();

        //Generate the maps
        var roadMap = GenerateRoads(seed + (int)transform.position.x + (int)transform.position.y);
        var buildingMap = GenerateBuildingPoints(seed + (int)transform.position.x + (int)transform.position.y, roadMap);

        //Initiate the tilemap data
        TilemapData village = new TilemapData();

        List<Vector3Int> positions = new List<Vector3Int>();
        List<TileBase> tiles = new List<TileBase>();

        //Implement the tilemap data
        for (int y = 0; y < (int)villageSize; y++)
        {
            for (int x = 0; x < (int)villageSize; x++)
            {
                if (roadMap[x, y] > 0 || buildingMap[x, y] > 0)
                {
                    positions.Add(new Vector3Int((int)transform.position.x + x - (int)villageSize / 2, (int)transform.position.y + y - (int)villageSize / 2, 0));
                    
                    if (roadMap[x, y] == 1)
                        tiles.Add(majorRoadTile);
                    else if (roadMap[x, y] == 2)
                        tiles.Add(minorRoadTile);

                    else if (buildingMap[x, y] == 1 || buildingMap[x, y] == 4)
                        tiles.Add(wall);
                    else if (buildingMap[x, y] == 2 || buildingMap[x, y] == 5)
                        tiles.Add(floor);
                }
            }
        }

        village.tiles = tiles.ToArray();
        village.tilePositions = positions.ToArray();

        villageData = village;

        if (drawVillage)
        {
            FindObjectOfType<MapDisplay>().DrawVillage(villageData);

            foreach(var building in largeBuildingsToGenerate)
            {
                building.Initialise(seed);
                DestroyImmediate(building.gameObject);
            }
        }
    }
   
    int[,] GenerateRoads(int seed)
    {
        // 1 = major road point, 2 = minor road point

        System.Random rand = new System.Random(seed);

        int villageDimension = (int)villageSize;

        int[,] pointMap = new int[villageDimension, villageDimension];
        int[,] roadMap = new int[villageDimension, villageDimension];

        //Major roads
        if (useMajorRoads)
        {
            for (int y = 0; y < villageDimension; y += minCellSize * majorRoadFrequency)
            {
                for (int x = 0; x < villageDimension; x += minCellSize * majorRoadFrequency)
                {
                    if (rand.Next(0, 100) / 100f <= majorRoadDensity)
                    {
                        pointMap[x, y] = 1;
                    }
                }
            }
        }

        //Minor roads
        for (int y = 0; y < villageDimension; y += minCellSize)
        {
            for (int x = 0; x < villageDimension; x += minCellSize)
            {
                if (rand.Next(0, 100) / 100f <= minorRoadDensity)
                {
                    if(pointMap[x,y] != 1)
                        pointMap[x, y] = 2;
                }
            }
        }

        if(linkRoads)
            LinkRoads();

        return linkRoads ? roadMap : pointMap;

        void LinkRoads()
        {
            potentialLargeBuildingLocations = new List<Vector2Int>();

            for (int y = 0; y < villageDimension; y += minCellSize)
            {
                for (int x = 0; x < villageDimension; x += minCellSize)
                {
                    int roadType = pointMap[x, y];
                    if (roadType < 1)
                    {
                        potentialLargeBuildingLocations.Add(new Vector2Int(x, y));
                        continue;
                    }

                    int multiplier = (roadType == 1) ? majorRoadFrequency : 1;

                    if (GenericHelper.InBounds(x, y - minCellSize * multiplier, pointMap))
                    {
                        var other = pointMap[x, y - minCellSize * multiplier];
                        if (other <= roadType && other > 0) //Vertical
                        {
                            RoadGenerator.GenerateRoad(ref roadMap, x, y, minCellSize, multiplier, roadType, thickRoads, Axis.Vertical);
                        }
                    }

                    if(GenericHelper.InBounds(x - minCellSize * multiplier, y, pointMap))
                    {
                        var other = pointMap[x - minCellSize * multiplier, y];

                        if (other <= roadType && other > 0) //Horizontal
                        {
                            RoadGenerator.GenerateRoad(ref roadMap, x, y, minCellSize, multiplier, roadType, thickRoads, Axis.Horizontal);
                        }
                    }
                }
            }
        }
    }

    int[,] GenerateBuildingPoints(int seed, int[,] roadMap)
    {
        //1 = wall, 2 = floor, 3 = block (No building), 4 = building, 5 = large building

        System.Random rand = new System.Random(seed);

        int villageDimension = (int)villageSize;

        int[,] originPoints = new int[villageDimension, villageDimension];
        int[,] buildingMap = new int[villageDimension, villageDimension];

        //Placing large building locations
        foreach(Vector2Int position in potentialLargeBuildingLocations)
        {
            if (position.x == 0 || position.x == villageDimension - 5 ||
                position.y == 0 || position.y == villageDimension - 5 ||
                roadMap[position.x, position.y] > 0)
                continue;

            if (rand.Next(0, 100) / 100f <= largeBuildingDensity)
            {
                originPoints[position.x, position.y] = 5;

                var blockOffset = minCellSize / 2;

                if(GenericHelper.InBounds(position.x + blockOffset, position.y + blockOffset, originPoints)) originPoints[position.x + blockOffset, position.y + blockOffset] = 3;
                if(GenericHelper.InBounds(position.x - blockOffset, position.y + blockOffset, originPoints)) originPoints[position.x - blockOffset, position.y + blockOffset] = 3;
                if(GenericHelper.InBounds(position.x + blockOffset, position.y - blockOffset, originPoints)) originPoints[position.x + blockOffset, position.y - blockOffset] = 3;
                if(GenericHelper.InBounds(position.x - blockOffset, position.y - blockOffset, originPoints)) originPoints[position.x - blockOffset, position.y - blockOffset] = 3;
            }
        }

        //Place buildings
        for (int y = minCellSize/2; y < villageDimension; y += minCellSize) 
        {
            for (int x = minCellSize/2; x < villageDimension; x += minCellSize)
            {
                if (CheckForRoads(x,y))
                {
                    if (rand.Next(0, 100) / 100f <= buildingDensity)
                    {
                        if (originPoints[x, y] == 3)
                            continue;
                        originPoints[x, y] = 4;
                    }
                }
            }
        }

        //Generate the buildings
        if (buildHouses)
            ConstructBuildings();

        return (buildHouses) ? buildingMap : originPoints;

        //Checks for adjacent roads
        bool CheckForRoads(int x, int y)
        {
            bool roadPresent = false;

            if (GenericHelper.InBounds(x + minCellSize / 2, y, roadMap)) if (roadMap[x + minCellSize / 2, y] > 0) roadPresent = true;
                else if (GenericHelper.InBounds(x - minCellSize / 2, y, roadMap)) if (roadMap[x - minCellSize / 2, y] > 0) roadPresent = true;
                    else if (GenericHelper.InBounds(x, y + minCellSize / 2, roadMap)) if (roadMap[x, y + minCellSize / 2] > 0) roadPresent = true;
                        else if (GenericHelper.InBounds(x, y - minCellSize / 2, roadMap)) if (roadMap[x, y - minCellSize / 2] > 0) roadPresent = true;

            return roadPresent;
        }

        //Generating building shapes
        void ConstructBuildings()
        {
            for (int y = minCellSize / 2; y < villageDimension; y += minCellSize / 2)
            {
                for (int x = minCellSize / 2; x < villageDimension; x += minCellSize / 2)
                {
                    int maxBuildSize;
                    Vector2Int doorPosition;

                    if (originPoints[x, y] == 4) //Standard buildings
                    {
                        maxBuildSize = minCellSize - 3;

                        doorPosition = BuildingGenerator.GenerateDoorPosition(maxBuildSize, rand);
                        BuildingGenerator.GenerateBuilding(ref buildingMap, maxBuildSize, doorPosition, new Vector2Int(x, y));
                    }

                    else if (originPoints[x, y] == 5) //Large buildings
                    {
                        if (useBuildingPrefabs)
                        {
                            var build = Instantiate(controller, transform.position + new Vector3(x - villageDimension/2, y-villageDimension/2), Quaternion.identity);
                            build.prefab = largeBuildings[rand.Next(0, largeBuildings.Length)];
                            largeBuildingsToGenerate.Add(build);
                        }
                        else
                        {
                            int sizeVar = 5 + (rand.Next(0, 3) * 2);
                            maxBuildSize = minCellSize * 2 - sizeVar;

                            doorPosition = BuildingGenerator.GenerateDoorPosition(maxBuildSize, rand);
                            BuildingGenerator.GenerateBuilding(ref buildingMap, maxBuildSize, doorPosition, new Vector2Int(x, y));

                        }
                    }
                }
            }
        }
    }
}