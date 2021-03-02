using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapGenerator : MonoBehaviour
{
    public Tilemap terrainMap;

    //Draws the tilemap
    public void DrawMap(WorldData worldData)
    {
        terrainMap.SetTiles(worldData.tilePositions, worldData.tiles);
        Debug.Log($"Setting {worldData.tiles.Length} tiles");
    }
}
