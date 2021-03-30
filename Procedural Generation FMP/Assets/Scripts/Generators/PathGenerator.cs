using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGenerator : MonoBehaviour
{
    public Vector3Int startPoint, endPoint;

    private void Start()
    {
        FindObjectOfType<MapDisplay>().DrawDetail(FindObjectOfType<Pathfinding>().GeneratePath(startPoint, endPoint));
    }
}
