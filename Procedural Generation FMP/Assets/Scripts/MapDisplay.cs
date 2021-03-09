﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    [Range(0.0001f, 1f)]
    public float scale;

    //Draws texture to a plane
    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width * scale, 1, texture.height * scale);
    }

    public Tilemap terrainMap;

    //Draws the tiles to a tilemap
    public void DrawWorldMap(WorldData worldData)
    {
        terrainMap.SetTiles(worldData.tilePositions, worldData.tiles);
    }

    public Tilemap villageMap;

    //Draws the tiles to a tilemap
    public void DrawVillage(Vector3Int[] tilePositions, TileBase[] tiles)
    {
        villageMap.SetTiles(tilePositions, tiles);
    }
}
