using UnityEngine;

namespace GenerationHelpers
{
    /// <summary>
    /// Contains functions to generate buildings - rectangles with an outline.
    /// </summary>
    public static class BuildingGenerator
    {
        public static void GenerateBuilding(ref int[,] map, int maxBuildSize, Vector2Int doorPos, Vector2Int originPoint)
        {
            for (int xx = 0; xx < maxBuildSize; xx++)
            {
                for (int yy = 0; yy < maxBuildSize; yy++)
                {
                    try
                    {
                        if ((xx == 0 || xx == maxBuildSize - 1 || yy == 0 || yy == maxBuildSize - 1) && new Vector2Int(xx, yy) != doorPos)
                        {
                            map[originPoint.x + xx - maxBuildSize / 2, originPoint.y + yy - maxBuildSize / 2] = 1;
                        }
                        else
                        {
                            map[originPoint.x + xx - maxBuildSize / 2, originPoint.y + yy - maxBuildSize / 2] = 2;
                        }
                    }
                    catch { }
                }
            }
        }

        public static void GenerateOutline(ref int[,] map, int diameter, Vector2Int doorPos, Vector2Int originPoint)
        {
            for (int xx = 0; xx < diameter; xx++)
            {
                for (int yy = 0; yy < diameter; yy++)
                {
                    try
                    {
                        if ((xx == 0 || xx == diameter - 1 || yy == 0 || yy == diameter - 1) && new Vector2Int(xx, yy) != doorPos)
                        {
                            map[originPoint.x + xx - diameter / 2, originPoint.y + yy - diameter / 2] = 1;
                        }
                    }
                    catch { }
                }
            }
        }

        static Vector2Int[] GenerateDoorPositions(int maxBuildSize)
        {
            return new Vector2Int[]
            {
                new Vector2Int(maxBuildSize/2, maxBuildSize-1), // Up
                new Vector2Int(maxBuildSize/2, 0), // Down
                new Vector2Int(0, maxBuildSize/2), // Left
                new Vector2Int(maxBuildSize-1, maxBuildSize/2) // Right
            };
        }

        public static Vector2Int GenerateDoorPosition(int maxBuildSize, System.Random rand)
        {
            var doorPositions = GenerateDoorPositions(maxBuildSize);

            return doorPositions[rand.Next(0, doorPositions.Length)];
        }

        public static Vector2Int GenerateDoorPosition(int maxBuildSize, Direction direction)
        {
            var doorPositions = GenerateDoorPositions(maxBuildSize);

            return doorPositions[(int)direction];
        }
    }

    /// <summary>
    /// Contains functions for generating a road between two points.
    /// </summary>
    public static class RoadGenerator
    {
        public static void GenerateRoad(ref int[,] map, Vector2Int startPos, Vector2Int endPos, int roadType, bool thickRoads, Axis direction)
        {
            Vector2Int start = Vector2Int.Min(startPos, endPos);
            Vector2Int end = Vector2Int.Max(startPos, endPos);

            if (direction == Axis.Vertical)
            {
                for (int i = start.y; i <= end.y; i++)
                {
                    map[start.x, i] = roadType;

                    if (thickRoads)
                    {
                        PlaceAdjacentRoads(start.x, i, 1, ref map);
                    }
                }
            }

            else if (direction == Axis.Horizontal)
            {
                for (int i = start.x; i <= end.x; i++)
                {
                    map[i, start.y] = roadType;

                    if (thickRoads)
                    {
                        PlaceAdjacentRoads(i, start.y, 1, ref map);
                    }
                }
            }
        }

        public static void GenerateRoad(ref int[,] map, int x, int y, int minCellSize, int multiplier, int roadType, bool thickRoads, Axis direction)
        {
            if (direction == Axis.Vertical)
            {
                for (int i = y; i >= y - minCellSize * multiplier; i--)
                {
                    if (map[x, i] != 1)
                        map[x, i] = roadType;

                    if (roadType == 1 && thickRoads)
                    {
                        PlaceAdjacentRoads(x, i, 1, ref map);
                    }
                }
            }

            else if(direction == Axis.Horizontal)
            {
                for (int i = x; i >= x - minCellSize * multiplier; i--)
                {
                    if (map[i, y] != 1)
                        map[i, y] = roadType;

                    if (roadType == 1 && thickRoads)
                    {
                        PlaceAdjacentRoads(i, y, 1, ref map);
                    }
                }
            }
        }

        static void PlaceAdjacentRoads(int _x, int _y, int _roadType, ref int[,] _map)
        {
            _map[_x + 1, _y + 1] = _roadType;
            _map[_x - 1, _y + 1] = _roadType;
            _map[_x + 1, _y - 1] = _roadType;
            _map[_x - 1, _y - 1] = _roadType;
            _map[_x + 1, _y] = _roadType;
            _map[_x, _y + 1] = _roadType;
            _map[_x - 1, _y] = _roadType;
            _map[_x, _y - 1] = _roadType;
        }
    }

    /// <summary>
    /// Contains functions for generating Perlin noise.
    /// </summary>
    public static class Noise
    {
        //Generates noise map from given parameters
        public static float[,] GenerateNoiseMap(int size, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
        {
            float[,] noiseMap = new float[size, size];

            System.Random prng = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];

            for (int i = 0; i < octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + offset.x;
                float offsety = prng.Next(-100000, 100000) + offset.y;

                octaveOffsets[i] = new Vector2(offsetX, offsety);
            }

            if (scale <= 0) scale = 0.0001f;

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            float halfWidth = size / 2f;
            float halfHeight = size / 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < octaves; i++)
                    {
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                        float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if (noiseHeight > maxNoiseHeight)
                        maxNoiseHeight = noiseHeight;
                    else if (noiseHeight < minNoiseHeight)
                        minNoiseHeight = noiseHeight;

                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }
    }

    /// <summary>
    /// Basic helper functions for common generation features.
    /// </summary>
    public static class GenericHelper
    {
        public static Vector3Int[] Vector2IntMap(int length, Vector2Int origin)
        {
            Vector3Int[] positions = new Vector3Int[length * length];

            for (int x = 0; x < length; x++)
            {
                for (int y = 0; y < length; y++)
                {
                    positions[y * length + x] = new Vector3Int(origin.x + x - length / 2, origin.y + y - length / 2, 0);
                }
            }

            return positions;
        }

        public static Vector3Int[] Vector2IntMap(Vector3Int dimensions, Vector2Int origin)
        {
            Vector3Int[] positions = new Vector3Int[dimensions.x * dimensions.y];

            for (int x = 0; x < dimensions.x; x++)
            {
                for (int y = 0; y < dimensions.y; y++)
                {
                    positions[y * dimensions.y + x] = new Vector3Int(origin.x + x - dimensions.x / 2, origin.y + y - dimensions.y / 2, 0);
                }
            }

            return positions;
        }

        public static bool InBounds<T>(int x, int y, T[,] array)
        {
            return (x >= 0 && y >= 0) && (x < array.GetLength(0) && y < array.GetLength(1));
        }
    }
}

public class TilemapData
{
    public Vector3Int[] tilePositions;
    public UnityEngine.Tilemaps.TileBase[] tiles;
}

public enum Axis
{
    Vertical,
    Horizontal,
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}