using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileType : ScriptableObject
{
    new public string name;
    public Color colour;
    public UnityEngine.Tilemaps.TileBase tile;

    public int pathfindingWeight;
}
