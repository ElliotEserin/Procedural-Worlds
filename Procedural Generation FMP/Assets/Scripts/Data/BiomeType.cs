using UnityEngine;

[CreateAssetMenu(menuName ="Data/Biome Type")]
public class BiomeType : ScriptableObject
{
    new public string name;
    public Color colour;
    public UnityEngine.Tilemaps.TileBase tile;
}
