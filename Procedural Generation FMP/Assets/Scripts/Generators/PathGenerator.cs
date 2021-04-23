using UnityEngine;
using System.Collections.Generic;
using System.Collections;
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
        UIManager.UpdateLoadScreenText("Generating paths...");
        base.Initialise(worldManager);
    }

    protected override IEnumerator Generate(WorldManager worldManager)
    {
        paths = new List<TilemapData>();

        FindObjectOfType<Grid>().Initialise();

        MapDisplay display = FindObjectOfType<MapDisplay>();
        Pathfinding pathfinder = FindObjectOfType<Pathfinding>();

        PathGoal currentGoal;

        for (int i = 0; i < goals.Length; i++)
        {
            currentGoal = goals[i];
            UIManager.UpdateLoadScreenText($"Making path {i}.");
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

                        yield return null;
                    }
                }
            }
        }

        UIManager.UpdateLoadScreenText("Drawing paths.");

        foreach (var path in paths)
        {
            display.DrawNonCollidableDetail(path);
            yield return null;
        }

        FinishGenerating(worldManager);
    }
}
