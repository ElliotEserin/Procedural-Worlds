using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class VillageGeneratorMK2 : Generator
{
    public int debugSeed;
    public bool usePaths;
    public VillageRadius villageRadius;
    public TileBase roadTile;
    public int cellSize = 14;
    public int maxIterations = 20;
    public bool useThickRoads = true;
    public int roadThickness = 3;
    [Range(1,3)]
    public int maxDivisions = 1;
    [Header("Major Roads")]
    [Range(0, 1f)]
    public float majorRoadChance = 0.25f;
    [Range(0, 1f)]
    public float majorRoadPersistence = 0.25f;
    [Range(0, 4)]
    public int initialMainRoadCount = 2;
    [Header("Minor Roads")]
    [Range(0, 1f)]
    public float minorRoadChance = 0.25f;
    [Range(0f, 1f)]
    public float minorRoadPersistence = 0.1f;
    [Header("Buildings")]
    [Range(0, 1)]
    public float buildingChance = 0.5f;
    [Range(0, 1)]
    public float buildingPersistence = 0.5f;
    public int maxNumberOfBuildings = 10;
    public int maxNumberOfMajorBuildings = 3;

    [Space(5)]
    public TilemapPrefab[] minorBuildings;
    public TilemapPrefab[] majorBuildings;

    public BuildingGenerator BuildingGenerator;
    public PathGoal pathTarget;

    System.Random rand;

    readonly Vector3Int upLeft = new Vector3Int(-1, 1, 0);
    readonly Vector3Int upRight = new Vector3Int(1, 1, 0);
    readonly Vector3Int downLeft = new Vector3Int(-1, -1, 0);
    readonly Vector3Int downRight = new Vector3Int(1, -1, 0);

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
        TilemapData roadData = GenerateRoadGrid(out List<BuildingPoint> points);
        GenerateBuildings(points);

        //Display
        ObjectStore.instance.mapDisplay.DrawVillage(roadData);
    }

    private TilemapData GenerateRoadGrid(out List<BuildingPoint> buildings)
    {
        //Initialise Lists
        List<Vector3Int> positions = new List<Vector3Int>();
        List<TileBase> tiles = new List<TileBase>();

        var potentialBuildings = new List<BuildingPoint>();

        var occupiedSpots = new List<Vector3Int>();

        Queue<RoadPoint> roadPoints = new Queue<RoadPoint>();

        float currentMajorChance = majorRoadChance;
        float currentMinorChance = minorRoadChance;
        int buildingsLeft = maxNumberOfBuildings;
        int majorBuildingsLeft = maxNumberOfMajorBuildings;
        float currentBuildingChance = buildingChance;
        
        //Define origin
        RoadPoint origin = new RoadPoint(Vector3Int.RoundToInt(transform.position), Type.Major, Direction.None);
        roadPoints.Enqueue(origin);

        DrawSquareRoad(origin.position, 5);

        occupiedSpots.Add(origin.position);

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

        buildings = potentialBuildings;

        return new TilemapData
        {
            tilePositions = positions.ToArray(),
            tiles = tiles.ToArray(),
        };

        // FUNCTIONS
        //Places new road points
        void Evaluate(RoadPoint current)
        {
            int roadsLeft = maxDivisions;

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
            else if (current.roadType == Type.Major)
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

                currentMinorChance *= minorRoadPersistence;

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
                        roadType = isMajor ? Type.Major : Type.Minor,
                    };

                    if (Vector3.Distance(newPoint.position, transform.position) > (int)villageRadius)
                        return;

                    if (CheckForOccupiedSpot(newPoint.position))
                        return;

                    roadPoints.Enqueue(newPoint);

                    PlaceRoad(current, newPoint, isMajor);

                    occupiedSpots.Add(newPoint.position);

                    roadsLeft--;
                }

                else if(isMajor && usePaths)
                {
                    var position = current.position;
                    var target = Instantiate(pathTarget, position, Quaternion.identity);
                    target.facingDirection = (PathGoal.Direction)(int)dir;
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
                if (!major || !useThickRoads)
                {
                    positions.Add(currentPoint);
                    tiles.Add(roadTile);
                }
                else
                {
                    DrawSquareRoad(currentPoint, roadThickness);
                }
            }

            CheckForBuildingSpots(newPoint);
        }

        void DrawSquareRoad(Vector3Int currentPoint, int length)
        {
            int half = (length - 1) / 2;

            for (int y = -half; y <= half; y++)
            {
                for (int x = -half; x <= half; x++)
                {
                    positions.Add(currentPoint + new Vector3Int(x, y, 0));
                    tiles.Add(roadTile);
                }
            }
        }

        void CheckForBuildingSpots(RoadPoint currentPoint)
        {
            if (buildingsLeft <= 0)
                return;

            int half = (currentPoint.roadType == Type.Major) ? cellSize : cellSize / 2;
            Type buildingType = currentPoint.roadType;

            if (majorBuildingsLeft <= 0 && buildingType == Type.Major)
            {
                buildingType = Type.Minor;
                half = cellSize / 2;
            }

            //TODO validate spots
            switch (currentPoint.cameFrom)
            {
                case Direction.Up:
                    ValidateSpot(upLeft * half, buildingType, Direction.Right);
                    ValidateSpot(upRight * half, buildingType, Direction.Left);
                    break;
                case Direction.Down:
                    ValidateSpot(downLeft * half, buildingType, Direction.Right);
                    ValidateSpot(downRight * half, buildingType, Direction.Left);
                    break;
                case Direction.Left:
                    ValidateSpot(upLeft * half, buildingType, Direction.Down);
                    ValidateSpot(downLeft * half, buildingType, Direction.Up);
                    break;
                case Direction.Right:
                    ValidateSpot(upRight * half, buildingType, Direction.Down);
                    ValidateSpot(downRight * half, buildingType, Direction.Up);
                    break;
                case Direction.None:
                    ValidateSpot(upLeft * half, buildingType, Direction.Right);
                    ValidateSpot(upRight * half, buildingType, Direction.Down);
                    ValidateSpot(downLeft * half, buildingType, Direction.Up);
                    ValidateSpot(downRight * half, buildingType, Direction.Left);
                    break;
            }

            void ValidateSpot(Vector3Int offset, Type type, Direction facing)
            {
                var position = currentPoint.position + offset;

                if (CheckForOccupiedSpot(position))
                    return;

                var chance = rand.Next(0, 100) / 100f;

                if (chance <= buildingChance)
                {
                    var build = new BuildingPoint(position, type, facing);

                    potentialBuildings.Add(build);
                    if (type == Type.Major) majorBuildingsLeft--;
                    buildingsLeft--;
                    currentBuildingChance *= buildingPersistence;
                    occupiedSpots.Add(position);

                    if (type == Type.Major)
                    {
                        var halfMajor = cellSize / 2;
                        occupiedSpots.Add(position + upLeft * halfMajor);
                        occupiedSpots.Add(position + upRight * halfMajor);
                        occupiedSpots.Add(position + downLeft * halfMajor);
                        occupiedSpots.Add(position + downRight * halfMajor);
                    }
                }
            }
        }

        bool CheckForOccupiedSpot(Vector3Int position)
        {
            if (occupiedSpots.Contains(position)) return true;
            return false;
        }

        //Calculates the new road point
        Vector3Int GetNewPointPosition(RoadPoint current, Direction goingTo, out Direction cameFrom, bool major)
        {
            int cell = cellSize;

            if (major)
                cell *= 2;
            Vector3Int offset;
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

    private void GenerateBuildings(List<BuildingPoint> buildingPoints)
    {
        foreach(var building in buildingPoints)
        {
            var gen = Instantiate(BuildingGenerator, building.originPoint, Quaternion.identity);
            gen.Initialise(seed, 
                building.buildingType == Type.Major ? majorBuildings : minorBuildings, 
                building.facing);

            if (!Application.isPlaying)
                DestroyImmediate(gen.gameObject);

            else
                Destroy(gen.gameObject, 1);
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

    public enum Type
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
        public Type roadType;
        public Direction cameFrom;

        public RoadPoint(Vector3Int position, Type roadType, Direction cameFrom)
        {
            this.position = position;
            this.roadType = roadType;
            this.cameFrom = cameFrom;
        }
    }

    struct BuildingPoint
    {
        public Vector3Int originPoint;
        public Type buildingType;
        public Direction facing;

        public BuildingPoint(Vector3Int originPoint, Type buildingType, Direction facing)
        {
            this.originPoint = originPoint;
            this.buildingType = buildingType;
            this.facing = facing;
        }
    }
}