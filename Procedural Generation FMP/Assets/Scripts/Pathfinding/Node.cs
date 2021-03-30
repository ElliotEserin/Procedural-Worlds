﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;

    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;

    public Node parent;

    public Node(bool _walkable, Vector3 _position, int _gridX, int _gridY)
    {
        gridX = _gridX;
        gridY = _gridY;

        walkable = _walkable;
        worldPosition = _position;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }
}
