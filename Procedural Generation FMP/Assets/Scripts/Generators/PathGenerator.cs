using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    public Vector3Int startPoint, endPoint;

    public PathGenerator(Vector3Int start, Vector3Int end)
    {
        startPoint = start;
        endPoint = end;
    }

    public PathGenerator(Transform start, Transform end)
    {
        startPoint = Vector3Int.RoundToInt(start.position);
        endPoint = Vector3Int.RoundToInt(end.position);
    }

    public void GeneratePath()
    {
        FindObjectOfType<MapDisplay>().DrawDetail(FindObjectOfType<Pathfinding>().GeneratePath(startPoint, endPoint));
    }
}
