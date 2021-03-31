using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;

    public int gridX;
    public int gridY;
    public int movementPenalty;

    public int gCost;
    public int hCost;

    public Node parent;

    int heapIndex;

    public Node(bool _walkable, Vector3 _position, int _gridX, int _gridY, int _penalty)
    {
        gridX = _gridX;
        gridY = _gridY;

        walkable = _walkable;
        worldPosition = _position;

        movementPenalty = _penalty;
    }

    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    public int HeapIndex {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node other)
    {
        int compare = fCost.CompareTo(other.fCost);
        if(compare == 0)
        {
            compare = hCost.CompareTo(other.hCost);
        }
        return -compare;
    }
}
