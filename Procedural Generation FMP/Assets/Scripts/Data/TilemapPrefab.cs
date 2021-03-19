using UnityEngine;

[CreateAssetMenu(menuName = "Data/Tilemap Data")]
public class TilemapPrefab : ScriptableObject
{
    public Vector3Int[] tilePositions;
    public UnityEngine.Tilemaps.TileBase[] tiles;
    public bool isSet = false;
}
