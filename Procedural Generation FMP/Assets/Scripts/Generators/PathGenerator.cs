using UnityEngine;
using System.Collections.Generic;
using GenerationHelpers;

public class PathGenerator : Generator
{
    PathGoal[] goals;
    public int maxPathsPerGoal = 2;
    public float minPathLength = 100;
    public float maxPathLength = 700;

    List<Path> paths;

    public override void Initialise(int seed)
    {
        goals = FindObjectsOfType<PathGoal>();
        Generate();
    }

    protected override void Generate()
    {
        paths = new List<Path>();

        PathGoal currentGoal;
        for (int i = 0; i < goals.Length; i++)
        {
            currentGoal = goals[i];
            for (int j = i+1; j < goals.Length; j++)
            {
                var dist = Vector3.Distance(currentGoal.transform.position, goals[j].transform.position);
                if (dist <= maxPathLength && dist >= minPathLength)
                {
                    if (currentGoal.ValidPath(goals[j]) && currentGoal.goalCount < maxPathsPerGoal)
                    {
                        currentGoal.goalCount++;

                        var pos1 = GenericHelper.ToVector3Int(currentGoal.transform);
                        var pos2 = GenericHelper.ToVector3Int(goals[j].transform);

                        paths.Add(new Path(pos1, pos2));
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        Pathfinding pathfinder = FindObjectOfType<Pathfinding>();

        foreach(var path in paths)
        {
            display.DrawDetail(path.GeneratePath(pathfinder));
        }
    }



    struct Path
    {
        public Vector3Int start, end;
        public Path(Vector3Int start, Vector3Int end)
        {
            this.start = start;
            this.end = end;
        }

        public TilemapData GeneratePath(Pathfinding pathfinder)
        {
            return pathfinder.GeneratePath(start, end);
        }
    }
}
