using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VillageGeneratorMK2 : Generator
{
    public int debugSeed;
    public VillageRadius villageRadius;
    public TileBase roadTile;
    public int cellSize = 14;
    public int maxIterations = 20;
    public bool useThickRoads = true;
    [Range(1,3)]
    public int maxNumberOfRoadsPerPoint = 1;
    [Header("Major Roads")]
    [Range(0, 0.9f)]
    public float majorRoadChance = 0.25f;
    [Range(0, 0.9f)]
    public float majorRoadPersistence = 0.25f;
    [Range(0, 4)]
    public int initialMainRoadCount = 2;
    [Header("Minor Roads")]
    [Range(0, 0.9f)]
    public float minorRoadChance = 0.25f;
    [Range(0f, 0.9f)]
    public float minorRoadPersistence = 0.1f;

    System.Random rand;

    public enum VillageRadius
    {
        Small = 42,
        Medium = 56,
        Large = 70
    }

    public override void Initialise(int seed)
    {
        this.seed = seed;
        rand = new System.Random(seed);
        Generate();
    }

    protected override void Generate()
    {
        TilemapData roadData = GenerateRoadGrid();
        TilemapData buildingData = new TilemapData();

        //Display
        MapDisplay display = FindObjectOfType<MapDisplay>();
        display.DrawVillage(roadData);
        display.DrawVillage(buildingData);
    }

    private TilemapData GenerateRoadGrid()
    {
        //Initialise Lists
        List<Vector3Int> positions = new List<Vector3Int>();
        List<TileBase> tiles = new List<TileBase>();

        Queue<RoadPoint> roadPoints = new Queue<RoadPoint>();

        float currentMajorChance = majorRoadChance;
        float currentMinorChance = minorRoadChance;
        
        //Define origin
        RoadPoint origin = new RoadPoint(Vector3Int.RoundToInt(transform.position), RoadType.Major, Direction.None);
        roadPoints.Enqueue(origin);

        PlacePoint(origin.position, roadTile);

        int currentIteration = 0;
        //Main loop
        while (roadPoints.Count > 0 && currentIteration <= maxIterations)
        {
            //Get next road point
            var current = roadPoints.Dequeue();
            //Evaluate the next road point
            Evaluate(current);

            currentIteration++;
        }

        return new TilemapData
        {
            tilePositions = positions.ToArray(),
            tiles = tiles.ToArray(),
        };

        // FUNCTIONS
        //Places new road points
        void Evaluate(RoadPoint current)
        {
            int roadsLeft = maxNumberOfRoadsPerPoint;

            List<Direction> directionsToTry = GetAllowedDirections(current.cameFrom);

            //Points with 4 directions (Origin)
            if(current.cameFrom == Direction.None)
            {
                int majorLeft = initialMainRoadCount;

                for (int i = 0; i < 4; i++)
                {
                    int nextRoadDir = rand.Next(0, directionsToTry.Count);

                    Direction dir = directionsToTry[nextRoadDir];

                    CalculateNewRoad(0, 1, dir, (majorLeft > 0) ? true : false);

                    majorLeft--;

                    directionsToTry.Remove(dir);
                }
            }

            //Try and place a new major road
            else if (current.roadType == RoadType.Major)
            {
                float chance = rand.Next(0, 100) / 100f;

                Direction oppositeDir = GetOppositeDirection(current.cameFrom);

                CalculateNewRoad(chance, currentMajorChance, oppositeDir, true);

                currentMajorChance *= majorRoadPersistence;

                directionsToTry.Remove(oppositeDir);              
            }

            //Try and place new minor roads
            while (roadsLeft > 0 && directionsToTry.Count > 0)
            {
                int nextRoadDir = rand.Next(0, directionsToTry.Count);

                Direction dir = directionsToTry[nextRoadDir];

                float chance = rand.Next(0, 100) / 100f;

                CalculateNewRoad(chance, currentMinorChance, dir, false);

                //currentMinorChance *= minorRoadPersistence;

                directionsToTry.Remove(dir);
            }

            // FUNCTIONS
            void CalculateNewRoad(float chance, float probability, Direction dir, bool isMajor)
            {
                if (chance < probability)
                {
                    RoadPoint newPoint = new RoadPoint()
                    {
                        position = GetNewPointPosition(current, dir, out Direction direction, isMajor),
                        cameFrom = direction,
                        roadType = isMajor ? RoadType.Major : RoadType.Minor,
                    };

                    if (Vector3.Distance(newPoint.position, transform.position) > (int)villageRadius)
                        return;

                    roadPoints.Enqueue(newPoint);

                    PlaceRoad(current, newPoint, isMajor);

                    roadsLeft--;
                }
            }
        }

        //Adds road to tilemap data
        void PlaceRoad(RoadPoint current, RoadPoint newPoint, bool major)
        {
            Vector3 dir = newPoint.position - current.position;
            dir = dir.normalized;
            Vector3Int trueDir = Vector3Int.RoundToInt(dir);

            Vector3Int currentPoint = current.position;

            while (currentPoint != newPoint.position)
            {
                currentPoint += trueDir;
                positions.Add(currentPoint);
                tiles.Add(roadTile);               
            }      
        }

        void PlacePoint(Vector3Int position, TileBase tile)
        {
            positions.Add(position);
            tiles.Add(tile);
        }

        //Calculates the new road point
        Vector3Int GetNewPointPosition(RoadPoint current, Direction goingTo, out Direction cameFrom, bool major)
        {
            int cell = cellSize;

            Vector3Int offset = Vector3Int.zero;

            cameFrom = Direction.None;

            if (major)
                cell *= 2;

            switch (goingTo)
            {
                default:
                case Direction.Up:
                    offset = Vector3Int.up;
                    cameFrom = Direction.Down;
                    break;
                case Direction.Down:
                    offset = Vector3Int.down;
                    cameFrom = Direction.Up;
                    break;
                case Direction.Left:
                    offset = Vector3Int.left;
                    cameFrom = Direction.Right;
                    break;
                case Direction.Right:
                    offset = Vector3Int.right;
                    cameFrom = Direction.Left;
                    break;
            }

            return current.position + (offset * cell);
        }
    }

    //Gets all directions except its own
    List<Direction> GetAllowedDirections(Direction cameFrom)
    {
        switch (cameFrom)
        {
            default:
            case Direction.None:
                return new List<Direction>
            {
                Direction.Up,
                Direction.Down,
                Direction.Left,
                Direction.Right
            };
            case Direction.Up:
                return new List<Direction>
            {
                Direction.Down,
                Direction.Left,
                Direction.Right
            };
            case Direction.Down:
                return new List<Direction>
            {
                Direction.Up,
                Direction.Left,
                Direction.Right
            };
            case Direction.Left:
                return new List<Direction>
            {
                Direction.Up,
                Direction.Down,
                Direction.Right
            };
            case Direction.Right:
                return new List<Direction>
            {
                Direction.Up,
                Direction.Down,
                Direction.Left
            };
        }
    }

    //Inverts the given direction
    Direction GetOppositeDirection(Direction direction)
    {
        switch (direction)
        {
            default:
            case Direction.Up:
                return Direction.Down;
            case Direction.Down:
                return Direction.Up;
            case Direction.Left:
                return Direction.Right;
            case Direction.Right:
                return Direction.Left;
        }
    }

    private void GenerateBuildings()
    {

    }

    public enum RoadType
    {
        Major,
        Minor
    }

    public enum Direction
    {
        None,
        Up,
        Down,
        Left,
        Right
    }

    struct RoadPoint
    {
        public Vector3Int position;
        public RoadType roadType;
        public Direction cameFrom;

        public RoadPoint(Vector3Int position, RoadType roadType, Direction cameFrom)
        {
            this.position = position;
            this.roadType = roadType;
            this.cameFrom = cameFrom;
        }
    }
}