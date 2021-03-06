﻿using UnityEngine;

[CreateAssetMenu(menuName ="Data/Terrain Type")]
public class TerrainType : ScriptableObject
{
    new public string name;
    public float height;
    public Color colour;
    public UnityEngine.Tilemaps.Tile tile;
    public bool allowBiomes;
}
