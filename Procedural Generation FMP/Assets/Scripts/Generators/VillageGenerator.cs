using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GenerationHelpers;

public class VillageGenerator : Generator
{
    public enum VillageSize //Dimensions of the village
    {
        Small = 50,
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

    //TODO Falloff
    public bool useFalloff;
    public Texture2D falloffMap;

    public VillageSize villageSize;
    public VillageType villageType;

    public int minCellSize = 10;
    int border = 10;
    int VillageDimension 
    {
        get { return (int)villageSize; }
    }
    int BorderDimension
    {
        get { return VillageDimension + border * 2; }
    }

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

    System.Random rand;

    public override void Initialise(int seed)
    {
        this.seed = seed;

        Array values = Enum.GetValues(typeof(VillageSize));
        rand = new System.Random(seed + (int)transform.position.x + (int)transform.position.y);
        villageSize = (VillageSize)values.GetValue(rand.Next(values.Length));

        Generate();
    }

    protected override void Generate()
    {
        largeBuildingsToGenerate = new List<IndividualBuildingGenerator>();

        if(rand == null)
            rand = new System.Random(seed + (int)transform.position.x + (int)transform.position.y);

        //Generate the maps
        var roadMap = GenerateRoads();
        var buildingMap = GenerateBuildings(roadMap);

        //Initiate the tilemap data
        TilemapData village = new TilemapData();

        List<Vector3Int> positions = new List<Vector3Int>();
        List<TileBase> tiles = new List<TileBase>();

        //Implement the tilemap data
        for (int y = 0; y < BorderDimension; y++)
        {
            for (int x = 0; x < BorderDimension; x++)
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
   
    int[,] GenerateRoads()
    {
        // 1 = major road point, 2 = minor road point

        int[,] pointMap = new int[BorderDimension, BorderDimension];
        int[,] roadMap = new int[BorderDimension, BorderDimension];

        //Major roads
        if (useMajorRoads)
        {
            for (int y = 0; y < VillageDimension; y += minCellSize * majorRoadFrequency)
            {
                for (int x = 0; x < VillageDimension; x += minCellSize * majorRoadFrequency)
                {
                    var chance = rand.Next(0, 100) / 100f;

                    if (chance <= majorRoadDensity)
                    {
                        pointMap[Position(x), Position(y)] = 1;
                    }
                }
            }
        }

        //Minor roads
        for (int y = 0; y < VillageDimension; y += minCellSize)
        {
            for (int x = 0; x < VillageDimension; x += minCellSize)
            {
                var chance = rand.Next(0, 100) / 100f;

                if (chance <= minorRoadDensity)
                {
                    if(pointMap[Position(x), Position(y)] != 1)
                        pointMap[Position(x), Position(y)] = 2;
                }
            }
        }

        if(linkRoads)
            LinkRoads();

        return linkRoads ? roadMap : pointMap;

        void LinkRoads()
        {
            potentialLargeBuildingLocations = new List<Vector2Int>();

            for (int y = 0; y < VillageDimension; y += minCellSize)
            {
                for (int x = 0; x < VillageDimension; x += minCellSize)
                {
                    var posX = Position(x);
                    var posY = Position(y);

                    //Placing large buildings in valid spots
                    int roadType = pointMap[posX, posY];
                    if (roadType < 1)
                    {
                        potentialLargeBuildingLocations.Add(new Vector2Int(posX, posY));
                        continue;
                    }

                    //Placing roads
                    int multiplier = (roadType == 1) ? majorRoadFrequency : 1;

                    if (GenericHelper.InBounds(posX, posY - minCellSize * multiplier, pointMap))
                    {
                        var other = pointMap[posX, posY - minCellSize * multiplier];
                        if (other <= roadType && other > 0) //Vertical
                        {
                            RoadGenerator.GenerateRoad(ref roadMap, posX, posY, minCellSize, multiplier, roadType, thickRoads, Axis.Vertical);
                        }
                    }

                    if(GenericHelper.InBounds(posX - minCellSize * multiplier, posY, pointMap))
                    {
                        var other = pointMap[posX - minCellSize * multiplier, posY];

                        if (other <= roadType && other > 0) //Horizontal
                        {
                            RoadGenerator.GenerateRoad(ref roadMap, posX, posY, minCellSize, multiplier, roadType, thickRoads, Axis.Horizontal);
                        }
                    }
                }
            }
        }
    }

    int[,] GenerateBuildings(int[,] roadMap)
    {
        //1 = wall, 2 = floor, 3 = block (No building), 4 = building, 5 = large building

        int[,] originPoints = new int[BorderDimension, BorderDimension];
        int[,] buildingMap = new int[BorderDimension, BorderDimension];

        //Placing large building locations
        foreach(Vector2Int position in potentialLargeBuildingLocations)
        {
            int posX = position.x;
            int posY = position.y;

            //position.x == 0 || position.x == VillageDimension - 5 ||
            //position.y == 0 || position.y == VillageDimension - 5 ||

            if (roadMap[posX, posY] > 0)
                continue;

            //int posX = Position(position.x);
            //int posY = Position(position.y);

            //Mark large building location
            if (rand.Next(0, 100) / 100f <= largeBuildingDensity)
            {
                originPoints[posX, posY] = 5;

                var blockOffset = minCellSize / 2;

                //Positive and negative block offset positions
                var pX = posX + blockOffset;
                var nX = posX - blockOffset;

                var pY = posY + blockOffset;
                var nY = posY - blockOffset;

                //Stop other buildings spawning too close
                if(GenericHelper.InBounds(pX, pY, originPoints)) 
                    originPoints[pX, pY] = 3;
                if(GenericHelper.InBounds(nX, pY, originPoints))
                    originPoints[nX, pY] = 3;
                if(GenericHelper.InBounds(pX, nY, originPoints)) 
                    originPoints[pX, nY] = 3;
                if(GenericHelper.InBounds(nX, nY, originPoints)) 
                    originPoints[nX, nY] = 3;
            }
        }

        //Place buildings
        for (int y = minCellSize/2; y < VillageDimension; y += minCellSize) 
        {
            for (int x = minCellSize/2; x < VillageDimension; x += minCellSize)
            {
                if (CheckForRoads(Position(x), Position(y)))
                {
                    if (rand.Next(0, 100) / 100f <= buildingDensity)
                    {
                        if (originPoints[Position(x), Position(y)] == 3)
                            continue;
                        originPoints[Position(x), Position(y)] = 4;
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
            for (int y = minCellSize / 2; y < VillageDimension; y += minCellSize / 2)
            {
                for (int x = minCellSize / 2; x < VillageDimension; x += minCellSize / 2)
                {
                    int maxBuildSize;
                    Vector2Int doorPosition;

                    if (originPoints[Position(x), Position(y)] == 4) //Standard buildings
                    {
                        maxBuildSize = minCellSize - 3;

                        doorPosition = BuildingGenerator.GenerateDoorPosition(maxBuildSize, rand);
                        BuildingGenerator.GenerateBuilding(ref buildingMap, maxBuildSize, doorPosition, new Vector2Int(Position(x), Position(y)));
                    }

                    else if (originPoints[Position(x), Position(y)] == 5) //Large buildings
                    {
                        if (useBuildingPrefabs)
                        {
                            var build = Instantiate(controller, transform.position + new Vector3(Position(x) - VillageDimension/2, Position(y)-VillageDimension/2), Quaternion.identity);
                            build.prefab = largeBuildings[rand.Next(0, largeBuildings.Length)];
                            largeBuildingsToGenerate.Add(build);
                            
                        }
                        else
                        {
                            int sizeVar = 5 + (rand.Next(0, 3) * 2);
                            maxBuildSize = minCellSize * 2 - sizeVar;

                            doorPosition = BuildingGenerator.GenerateDoorPosition(maxBuildSize, rand);
                            BuildingGenerator.GenerateBuilding(ref buildingMap, maxBuildSize, doorPosition, new Vector2Int(Position(x), Position(y)));

                        }
                    }
                }
            }
        }
    }

    int Position(int coord) => coord + border;
}