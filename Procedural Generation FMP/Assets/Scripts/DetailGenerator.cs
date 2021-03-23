using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DetailGenerator : MonoBehaviour
{
    public int maxNumberOfDecorations;

    public Tilemap terrainMap;
    public Tilemap buildingMap;

    public TileBase[] allowedTiles;

    public Tile tree;

    public GenerationMethod generationMethod;

    public enum GenerationMethod
    {
        Random,
        Grid,
        Perlin
    }

    public TilemapData Generate(int seed, int worldDimension)
    {
        TilemapData data = new TilemapData();

        switch (generationMethod)
        {
            case GenerationMethod.Random:
                RandomPlacement();
                break;
        }

        return data;

        void RandomPlacement()
        {
            System.Random rand = new System.Random(seed);

            List<Vector3Int> positions = new List<Vector3Int>();
            List<TileBase> tiles = new List<TileBase>();

            int number = 0;
            int attempts = 0;

            while (number < maxNumberOfDecorations && attempts < 50)
            {
                bool canPlace = false;

                Vector3Int potentialPosition = new Vector3Int(rand.Next(0, worldDimension - 1), rand.Next(0, worldDimension - 1), 0);

                foreach (TileBase tile in allowedTiles)
                {
                    if (terrainMap.GetTile(potentialPosition) == tile)
                    {
                        canPlace = true;
                    }
                }

                if (buildingMap.GetTile(potentialPosition) != null)
                {
                    canPlace = false;
                }

                if (canPlace)
                {
                    if (!positions.Contains(potentialPosition))
                    {
                        positions.Add(potentialPosition);
                        tiles.Add(tree);
                    }

                    attempts = 0;
                    number++;
                }
                else
                {
                    attempts++;
                }
            }

            data.tilePositions = positions.ToArray();
            data.tiles = tiles.ToArray();
        }
    }
}
