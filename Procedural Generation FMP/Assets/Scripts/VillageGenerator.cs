﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VillageGenerator : MonoBehaviour
{
    public enum VillageSize //Dimensions of the village
    {
        Small = 25,
        Medium = 50,
        Large = 75
    }

    public enum VillageType //Type of generation
    {
        Grid,
    }

    [Header("Village Roads")]
    //Tiles
    public Tile majorRoadTile;
    public Tile minorRoadTile;

    //Falloff
    public bool useFalloff;
    public Texture2D falloffMap;

    public int seed;

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
    public Tile wall;
    public Tile floor;
    [Range(0,1)]
    public float buildingDensity;

    [Range(0, 1)]
    public float chanceOfJoining;

    [Range(0, 1)]
    public float largeBuildingDensity;
    public bool buildHouses;

    List<Vector2Int> potentialLargeBuildingLocations;

    private void Start()
    {
        GenerateVillage(seed);
    }

    public VillageData GenerateVillage(int seed)
    {
        //Generate the maps
        var roadMap = GenerateRoadPoints(seed + (int)transform.position.x + (int)transform.position.y);
        var buildingMap = GenerateBuildingPoints(seed + (int)transform.position.x + (int)transform.position.y, roadMap);

        //Initiate the tilemap data
        VillageData village = new VillageData()
        {
            tilePositions = new Vector3Int[(int)villageSize * (int)villageSize],
            tiles = new TileBase[(int)villageSize * (int)villageSize]
        };

        //Implement the tilemap data
        for (int y = 0; y < (int)villageSize; y++)
        {
            for (int x = 0; x < (int)villageSize; x++)
            {
                village.tilePositions[y * (int)villageSize + x] = new Vector3Int((int)transform.position.x + x - (int)villageSize / 2, (int)transform.position.y + y - (int)villageSize / 2, 0);
                if (roadMap[x, y] == 1)
                    village.tiles[y * (int)villageSize + x] = majorRoadTile;
                else if(roadMap[x,y] == 2)
                    village.tiles[y * (int)villageSize + x] = minorRoadTile;

                else if(buildingMap[x,y] == 1 || buildingMap[x, y] == 4)
                    village.tiles[y * (int)villageSize + x] = wall;
                else if (buildingMap[x, y] == 2 || buildingMap[x, y] == 5)
                    village.tiles[y * (int)villageSize + x] = floor;
            }
        }

        FindObjectOfType<MapDisplay>().DrawVillage(village.tilePositions, village.tiles);

        return village;
    }
   
    int[,] GenerateRoadPoints(int seed)
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

                    try
                    {
                        var other = pointMap[x, y - minCellSize * multiplier];
                        if (other <= roadType && other > 0) //Vertical
                        {
                            for (int i = y; i >= y - minCellSize * multiplier; i--)
                            {
                                if(roadMap[x, i] != 1)
                                    roadMap[x, i] = roadType;

                                if(roadType == 1 && thickRoads)
                                {
                                    PlaceAdjacentRoads(x, i, 1);
                                }
                            }
                        }
                    }
                    catch { }

                    try
                    {
                        var other = pointMap[x - minCellSize * multiplier, y];

                        if (other <= roadType && other > 0) //Horizontal
                        {
                            for (int i = x; i >= x - minCellSize * multiplier; i--)
                            {
                                if (roadMap[i, y] != 1)
                                    roadMap[i, y] = roadType;

                                if (roadType == 1 && thickRoads)
                                {
                                    PlaceAdjacentRoads(i, y, 1);
                                }
                            }
                        }
                    }
                    catch { }

                    void PlaceAdjacentRoads(int _x, int _y, int _roadType)
                    {
                        roadMap[_x + 1, _y + 1] = _roadType;
                        roadMap[_x - 1, _y + 1] = _roadType;
                        roadMap[_x + 1, _y - 1] = _roadType;
                        roadMap[_x - 1, _y - 1] = _roadType;
                        roadMap[_x + 1, _y ] = _roadType;
                        roadMap[_x, _y + 1] = _roadType;
                        roadMap[_x - 1, _y ] = _roadType;
                        roadMap[_x, _y - 1] = _roadType;
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

                try { originPoints[position.x + blockOffset, position.y + blockOffset] = 3; } catch { }
                try { originPoints[position.x - blockOffset, position.y + blockOffset] = 3; } catch { }
                try { originPoints[position.x + blockOffset, position.y - blockOffset] = 3; } catch { }
                try { originPoints[position.x - blockOffset, position.y - blockOffset] = 3; } catch { }
            }
        }

        for (int y = minCellSize/2; y < villageDimension; y += minCellSize) //Place buildings
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

        bool CheckForRoads(int x, int y)
        {
            if (roadMap[x + minCellSize / 2, y] > 0 ||
                roadMap[x - minCellSize / 2, y] > 0 ||
                roadMap[x, y + minCellSize / 2] > 0 ||
                roadMap[x, y - minCellSize / 2] > 0) return true;

            return false;        
        }

        if (buildHouses)
            ConstructBuildings();

        return (buildHouses) ? buildingMap : originPoints;

        //Generating building shapes
        void ConstructBuildings()
        {
            for (int y = minCellSize / 2; y < villageDimension; y += minCellSize / 2)
            {
                for (int x = minCellSize / 2; x < villageDimension; x += minCellSize / 2)
                {
                    if(originPoints[x,y] == 4) //Standard buildings
                    {
                        int maxBuildSize = minCellSize - 3;

                        var doorPositions = GenerateDoorPositions(maxBuildSize);
                        var doorPos = doorPositions[rand.Next(0, doorPositions.Length)];

                        for (int xx = 0; xx < maxBuildSize; xx++)
                        {
                            for (int yy = 0; yy < maxBuildSize; yy++)
                            {
                                if((xx == 0 || xx == maxBuildSize - 1 || yy == 0 || yy == maxBuildSize - 1) && new Vector2Int(xx, yy) != doorPos)
                                {
                                    buildingMap[x + xx - maxBuildSize / 2, y + yy - maxBuildSize / 2] = 1;
                                }
                                else
                                {
                                    buildingMap[x + xx - maxBuildSize / 2, y + yy - maxBuildSize / 2] = 2;
                                }
                            }
                        }
                    }

                    if (originPoints[x, y] == 5) //Large buildings
                    {
                        int sizeVar = 5 + (rand.Next(0, 3) * 2);

                        int maxBuildSize = minCellSize * 2 - sizeVar;

                        var doorPositions = GenerateDoorPositions(maxBuildSize);
                        var doorPos = doorPositions[rand.Next(0, doorPositions.Length)];

                        for (int xx = 0; xx < maxBuildSize; xx++)
                        {
                            for (int yy = 0; yy < maxBuildSize; yy++)
                            {
                                if ((xx == 0 || xx == maxBuildSize - 1 || yy == 0 || yy == maxBuildSize - 1) && new Vector2Int(xx, yy) != doorPos)
                                {
                                    buildingMap[x + xx - maxBuildSize / 2, y + yy - maxBuildSize / 2] = 1;
                                }
                                else
                                {
                                    buildingMap[x + xx - maxBuildSize / 2, y + yy - maxBuildSize / 2] = 2;
                                }
                            }
                        }
                    }

                    Vector2Int[] GenerateDoorPositions(int maxBuildSize)
                    {
                        return new Vector2Int[]
                        {
                            new Vector2Int(maxBuildSize/2, 0),
                            new Vector2Int(maxBuildSize/2, maxBuildSize-1),
                            new Vector2Int(0, maxBuildSize/2),
                            new Vector2Int(maxBuildSize-1, maxBuildSize/2)
                        };
                    }
                }
            }
        }
    }
}

public struct VillageData
{
    public Vector3Int[] tilePositions;
    public TileBase[] tiles;
}