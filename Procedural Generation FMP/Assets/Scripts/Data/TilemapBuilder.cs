using UnityEngine;

public class TilemapBuilder : MonoBehaviour
{
    public TilemapPrefab tileData;
    public UnityEngine.Tilemaps.Tilemap tilemap;

    public void Generate()
    {
        int size = Mathf.Max(tilemap.size.x, tilemap.size.y);

        tileData.tilePositions = new Vector3Int[size * size];
        tileData.tiles = new UnityEngine.Tilemaps.TileBase[size * size];

        tileData.isSet = true;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                tileData.tilePositions[y * size + x] = new Vector3Int(x, y, 0);
                tileData.tiles[y * size + x] = tilemap.GetTile(new Vector3Int(x, y, 0));
            }
        }

        Debug.Log($"Generated {tileData.name}: {size * size} tiles.");
    }
}
