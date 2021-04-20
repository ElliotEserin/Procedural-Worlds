using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class TilemapBuilder : MonoBehaviour
{
    public TilemapPrefab tilemapPrefabData;
    public Tilemap tilemap;

    public int cellSize = 14;

    public OverrideTile[] overrides;

    public void Generate()
    {
        if (tilemapPrefabData != null)
        {
            if (tilemapPrefabData.locked)
            {
                Debug.Log("This data is locked! Unlock to override it...");
                return;
            }
        }

        var start = tilemap.cellBounds.min;
        var end = tilemap.cellBounds.max;

        List<Vector3Int> positions = new List<Vector3Int>();
        List<TileBase> tiles = new List<TileBase>();

        tilemapPrefabData.isSet = true;

        for (int y = start.y; y < end.y; y++)
        {
            for (int x = start.x; x < end.x; x++)
            {
                var pos = new Vector3Int(x, y, 0);
                Debug.Log(pos);
                var tile = tilemap.GetTile(pos);
                if(tile != null)
                {
                    tiles.Add(tile);
                    positions.Add(pos);
                }
            }
        }

        if (tilemapPrefabData != null)
        {
            tilemapPrefabData.tiles = tiles.ToArray();
            tilemapPrefabData.tilePositions = positions.ToArray();

            Debug.Log($"Generated {tilemapPrefabData.name}: {tilemapPrefabData.tilePositions.Length} tiles.");
        }
    }

    public void Override()
    {
        foreach (var _override in overrides)
        {
            for(int i = 0; i < tilemapPrefabData.tiles.Length; i++)
            {
                if(tilemapPrefabData.tiles[i] == _override.oldTile)
                {
                    if (_override.newTile != null && _override.oldTile != null)
                    {
                        tilemapPrefabData.tiles[i] = _override.newTile;
                    }
                }
            }
        }
    }

    [System.Serializable]
    public struct OverrideTile
    {
        public TileBase newTile;
        public TileBase oldTile;
    }

    private void OnDrawGizmos()
    {
        var offset = (cellSize % 2 == 0) ? Vector3.zero : Vector3.one / 2;
        var origin = transform.position;
        offset += origin;

        Gizmos.color = Color.white;
        Gizmos.DrawLine(origin - Vector3.right * cellSize, origin + Vector3.right * cellSize);
        Gizmos.DrawLine(origin - Vector3.up * cellSize, origin + Vector3.up * cellSize);

        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(offset, Vector3.one * cellSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(origin, Vector3.one * cellSize * 2);
    }
}
