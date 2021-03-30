using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    int gridWorldSize;

    Node[,] grid;

    int gridSizeX, gridSizeY;

    private void Start()
    {
        gridWorldSize = (int)FindObjectOfType<WorldGenerator>().worldSize;
        gridSizeX = Mathf.RoundToInt(gridWorldSize);
        gridSizeY = Mathf.RoundToInt(gridWorldSize);
        CreateGrid();
    }

    private void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3Int worldPoint = new Vector3Int(x, y, 0);
                bool walkable = !Physics2D.OverlapCircle(new Vector2(x, y), 1, unwalkableMask);

                grid[x, y] = new Node(walkable, worldPoint, x, y);
                
            }
        }
    }

    public Node NodeFromWorldPoint(Vector3Int worldPos)
    {
        //float percentX = (worldPos.x + gridWorldSize / 2) / gridWorldSize;
        //float percentY = (worldPos.y + gridWorldSize / 2) / gridWorldSize;
        //percentX = Mathf.Clamp(percentX, 0, 1);
        //percentY = Mathf.Clamp(percentY, 0, 1);

        //int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        //int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[worldPos.x, worldPos.y];
    }

    public List<Node> GetNeighbours(Node node)
    {
        List<Node> neighbours = new List<Node>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0)
                {
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }
}
