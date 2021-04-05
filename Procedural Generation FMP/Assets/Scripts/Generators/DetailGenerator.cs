using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DetailGenerator : MonoBehaviour
{
    public int maxNumberOfDecorations;

    public BiomeType[] allowedBiomes;

    public GenerationMethod generationMethod;
    [Range(1, 100)]
    public int minDetailDistanceInGrid;

    public enum GenerationMethod
    {
        Random,
        Grid
    }

    public TilemapData Generate(int seed, int worldDimension)
    {
        TilemapData data = new TilemapData();

        System.Random rand = new System.Random(seed);

        switch (generationMethod)
        {
            case GenerationMethod.Random:
                RandomPlacement();
                break;
            case GenerationMethod.Grid:
                GridPlacement();
                break;
        }

        return data;

        void RandomPlacement()
        {
            List<Vector3Int> positions = new List<Vector3Int>();
            List<TileBase> tiles = new List<TileBase>();

            int number = 0;
            int attempts = 0;

            while (number < maxNumberOfDecorations && attempts < 50)
            {
                bool canPlace = false;

                Vector3Int potentialPosition = new Vector3Int(rand.Next(0, worldDimension - 1), rand.Next(0, worldDimension - 1), 0);

                if (ObjectStore.instance.villageMap.GetTile(potentialPosition) != null)
                {
                    continue;
                }

                BiomeType currentBiome;

                foreach (var biome in allowedBiomes)
                {
                    if (ObjectStore.instance.terrainMap.GetTile(potentialPosition) == biome.tile)
                    {
                        currentBiome = biome;

                        canPlace = true;

                        if (!positions.Contains(potentialPosition))
                        {
                            positions.Add(potentialPosition);

                            TileBase tile = currentBiome.GetDecorTile(rand, false);
                            tiles.Add(tile);

                            attempts = 0;
                            number++;
                        }

                        break;
                    }
                }

                if (!canPlace)
                {
                    attempts++;
                }
            }

            data.tilePositions = positions.ToArray();
            data.tiles = tiles.ToArray();
        }

        void GridPlacement()
        {
            List<Vector3Int> positions = new List<Vector3Int>();
            List<TileBase> tiles = new List<TileBase>();

            for (int x = 0; x < worldDimension-minDetailDistanceInGrid; x += minDetailDistanceInGrid)
            {
                for (int y = 0; y < worldDimension-minDetailDistanceInGrid; y += minDetailDistanceInGrid)
                {
                    var dist = (minDetailDistanceInGrid - 1) / 2;

                    Vector3Int randomOffset = new Vector3Int(rand.Next(-dist, dist), rand.Next(-dist, dist), 0);

                    Vector3Int potentialPosition = new Vector3Int(x, y, 0) + randomOffset;

                    if (ObjectStore.instance.villageMap.GetTile(potentialPosition) != null)
                    {
                        continue;
                    }

                    BiomeType currentBiome;

                    foreach (var biome in allowedBiomes)
                    {
                        if (ObjectStore.instance.terrainMap.GetTile(potentialPosition) == biome.tile)
                        {
                            currentBiome = biome;

                            positions.Add(potentialPosition);

                            TileBase tile = currentBiome.GetDecorTile(rand, true);
                            tiles.Add(tile);

                            break;
                        }
                    }
                }
            }

            data.tilePositions = positions.ToArray();
            data.tiles = tiles.ToArray();
        }
    }
}
