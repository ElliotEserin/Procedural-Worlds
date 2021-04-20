using UnityEngine;
using System.Collections.Generic;
using GenerationHelpers;

public class PathGenerator : Generator
{
    PathGoal[] goals;
    public int maxPathsPerGoal = 2;
    public float minPathLength = 100;
    public float maxPathLength = 700;

    List<TilemapData> paths;

    public override void Initialise(WorldManager worldManager)
    {
        goals = FindObjectsOfType<PathGoal>();
        Generate(worldManager);
    }

    protected override void Generate(WorldManager worldManager)
    {
        paths = new List<TilemapData>();

        MapDisplay display = FindObjectOfType<MapDisplay>();
        Pathfinding pathfinder = FindObjectOfType<Pathfinding>();

        PathGoal currentGoal;

        Debug.Log(goals.Length);

        for (int i = 0; i < goals.Length; i++)
        {
            currentGoal = goals[i];
            for (int j = i+1; j < goals.Length; j++)
            {
                var dist = Vector3.Distance(currentGoal.transform.position, goals[j].transform.position);
                Debug.Log(dist);
                if (dist <= maxPathLength && dist >= minPathLength)
                {
                    if (currentGoal.ValidPath(goals[j]) && currentGoal.goalCount < maxPathsPerGoal)
                    {
                        currentGoal.goalCount++;

                        var pos1 = GenericHelper.ToVector3Int(currentGoal.transform);
                        var pos2 = GenericHelper.ToVector3Int(goals[j].transform);

                        paths.Add(pathfinder.GeneratePath(pos1, pos2));

                        Debug.Log("Made new path");
                    }
                }
                else
                {
                    Debug.Log("Invalid distance");
                }
            }
        }

        foreach(var path in paths)
        {
            display.DrawNonCollidableDetail(path);
        }

        FinishGenerating(worldManager);
    }
}
