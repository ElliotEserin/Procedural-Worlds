﻿using UnityEngine;
using System.Collections.Generic;

public class TilemapBuilder : MonoBehaviour
{
    public TilemapPrefab tileData;
    public UnityEngine.Tilemaps.Tilemap tilemap;

    public void Generate()
    {
        if (tileData.locked)
        {
            Debug.Log("This data is locked! Unlock to override it...");
            return;
        }

        int size = Mathf.Max(tilemap.size.x, tilemap.size.y);

        List<Vector3Int> positions = new List<Vector3Int>();
        List<UnityEngine.Tilemaps.TileBase> tiles = new List<UnityEngine.Tilemaps.TileBase>();

        tileData.isSet = true;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                var tile = tilemap.GetTile(new Vector3Int(x, y, 0));
                if(tile != null)
                {
                    tiles.Add(tilemap.GetTile(new Vector3Int(x, y, 0)));
                    positions.Add(new Vector3Int(x, y, 0));
                }
            }
        }

        tileData.tiles = tiles.ToArray();
        tileData.tilePositions = positions.ToArray();

        Debug.Log($"Generated {tileData.name}: {size * size} tiles.");
    }
}