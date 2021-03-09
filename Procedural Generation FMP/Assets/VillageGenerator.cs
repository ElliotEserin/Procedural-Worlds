using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VillageGenerator : MonoBehaviour
{
    public Tile majorRoadTile;
    public Tile minorRoadTile;

    public bool useFalloff;
    public Texture2D falloffMap;

    public int seed;

    public enum VillageSize
    {
        Small = 25,
        Medium = 50,
        Large = 100
    }

    public enum VillageType
    {
        Grid,
    }

    public VillageSize villageSize;

    public VillageType villageType;

    public int minCellSize = 10;

    public bool useMajorRoads = true;
    public float majorRoadDensity = 0.5f;
    public int majorRoadFrequency = 2;

    public float minorRoadDensity = 0.5f;

    public bool linkRoads = true;

    private void Start()
    {
        GenerateVillage(seed);
    }

    public VillageData GenerateVillage(int seed)
    {
        var roadMap = GenerateRoadPoints(seed + (int)transform.position.x + (int)transform.position.y);

        VillageData village = new VillageData()
        {
            tilePositions = new Vector3Int[(int)villageSize * (int)villageSize],
            tiles = new TileBase[(int)villageSize * (int)villageSize]
        };

        for (int y = 0; y < (int)villageSize; y++)
        {
            for (int x = 0; x < (int)villageSize; x++)
            {
                village.tilePositions[y * (int)villageSize + x] = new Vector3Int((int)transform.position.x + x - (int)villageSize / 2, (int)transform.position.y + y - (int)villageSize / 2, 0);
                if (roadMap[x, y] == 1)
                    village.tiles[y * (int)villageSize + x] = majorRoadTile;
                else if(roadMap[x,y] == 2)
                    village.tiles[y * (int)villageSize + x] = minorRoadTile;

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
            for (int y = 0; y < villageDimension; y += minCellSize)
            {
                for (int x = 0; x < villageDimension; x += minCellSize)
                {
                    int roadType = pointMap[x, y];
                    if (roadType < 1)
                    {
                        continue;
                    }

                    int multiplier = (roadType == 1) ? majorRoadFrequency : 1;

                    try
                    {
                        if (pointMap[x, y - minCellSize * multiplier] == roadType) //up
                        {
                            for (int i = y; i > y - minCellSize * multiplier; i--)
                            {
                                roadMap[x, i] = roadType;
                            }
                            Debug.Log("1");
                        }
                    }
                    catch { }

                    //try
                    //{
                    //    if (pointMap[x, y + minCellSize * multiplier] == roadType) //down
                    //    {
                    //        for (int i = y; i < y + minCellSize * multiplier; i++)
                    //        {
                    //            roadMap[x, i] = roadType;
                    //        }
                    //    }
                    //}
                    //catch { }

                    try
                    {
                        if (pointMap[x - minCellSize * multiplier, y] == roadType) //left
                        {
                            for (int i = x; i > x - minCellSize * multiplier; i--)
                            {
                                roadMap[i, y] = roadType;
                            }
                        }
                    }
                    catch { }

                    //try
                    //{
                    //    if (pointMap[x + minCellSize * multiplier, y] == roadType) //right
                    //    {
                    //        for (int i = x; i < x + minCellSize * multiplier; i++)
                    //        {
                    //            roadMap[i, y] = roadType;
                    //        }
                    //    }
                    //}
                    //catch { }
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
