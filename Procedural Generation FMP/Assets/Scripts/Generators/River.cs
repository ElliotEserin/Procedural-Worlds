using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class River : MonoBehaviour
{
    public TileBase waterTile;

    public TerrainType oceanBiome;

    public int maxIterations = 1000;
    public float directionSensitivity = 0.001f;

    public int forceChangeDirection = 50;

    public void Generate()
    {
        List<Vector3Int> positions = new List<Vector3Int>();
        List<TileBase> tiles = new List<TileBase>();

        WorldManager wg = FindObjectOfType<WorldManager>();

        Vector3Int currentPosition = Vector3Int.RoundToInt(transform.position);
        float currentHeight = wg.worldData.heightMap[currentPosition.x, currentPosition.y];

        AddTile(currentPosition);

        int currentIteration = 0;

        Vector3 currentDirection = Vector3.zero;
        while (currentHeight > oceanBiome.height - 0.05f && currentIteration <= maxIterations)
        {
            AdjacentTile[] adjacent = new AdjacentTile[4]
            {
                new AdjacentTile(currentPosition + Vector3Int.up, wg.worldData.heightMap),
                new AdjacentTile(currentPosition + Vector3Int.right, wg.worldData.heightMap),
                new AdjacentTile(currentPosition + Vector3Int.down, wg.worldData.heightMap),
                new AdjacentTile(currentPosition + Vector3Int.left, wg.worldData.heightMap)
            };

            int lowestIndex = 0;

            for (int i = 1; i < adjacent.Length; i++)
            {
                //if (currentPosition - adjacent[i].pos == currentDirection)
                //    adjacent[i].height -= directionSensitivity; 

                if(currentIteration%forceChangeDirection == 0)
                {
                    if (currentPosition - adjacent[i].pos == currentDirection)
                        adjacent[i].height += directionSensitivity * 2;
                }

                if (Vector3.Distance(adjacent[i].pos, transform.position) > Vector3.Distance(currentPosition, transform.position))
                    adjacent[i].height -= directionSensitivity;

                if (adjacent[i].height < adjacent[lowestIndex].height && !positions.Contains(adjacent[i].pos))
                    lowestIndex = i;
            }

            currentDirection = currentPosition - adjacent[lowestIndex].pos;
            currentHeight = adjacent[lowestIndex].height;
            currentPosition = adjacent[lowestIndex].pos;           

            AddTile(currentPosition);

            currentIteration++;

            if (currentIteration >= maxIterations)
                return;
        }

        ObjectStore.instance.mapDisplay.DrawTerrain(new TilemapData(positions.ToArray(), tiles.ToArray()));
  
        //FUNCTIONS
        void AddTile(Vector3Int pos)
        {
            positions.Add(pos);
            tiles.Add(waterTile);
        }
    }

    struct AdjacentTile
    {
        public Vector3Int pos;
        public float height;

        public AdjacentTile(Vector3Int pos, float[,] heightMap)
        {
            this.pos = pos;
            if (pos.x >= heightMap.GetLength(0) || pos.x < 0 || pos.y >= heightMap.GetLength(1) || pos.y < 0)
                height = 0;
            else
                height = heightMap[pos.x, pos.y];
        }
    }
}
