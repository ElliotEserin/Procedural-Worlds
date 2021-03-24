using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    [Range(0.0001f, 1f)]
    public float scale;

    public Tilemap terrainMap;
    public Tilemap villageMap;
    public Tilemap detailMap;

    public Color maxTintVariation;

    private void Awake()
    {
        ResetTilemaps();
    }

    public void ResetTilemaps()
    {
        if(terrainMap != null)
        {
            terrainMap.ClearAllTiles();
            terrainMap.ClearAllEditorPreviewTiles();
        }
        if(villageMap != null)
        {
            villageMap.ClearAllTiles();
            villageMap.ClearAllEditorPreviewTiles();
        }
    }

    //Draws texture to a plane
    public void DrawTexture(Texture2D texture)
    {
        textureRenderer.sharedMaterial.mainTexture = texture;
        textureRenderer.transform.localScale = new Vector3(texture.width * scale, 1, texture.height * scale);
    }  

    //Draws the tiles to a tilemap
    public void DrawWorldMap(WorldData worldData)
    {
        terrainMap.SetTiles(worldData.tilePositions, worldData.tiles);

        //var rand = new System.Random();

        //for (int x = 0; x < worldData.tilePositions.Length; x++)
        //{
        //    terrainMap.SetColor(worldData.tilePositions[x], Color.Lerp(Color.white, maxTintVariation, rand.Next(0, 100)));
        //}
    }   
    
    //Draws the tiles to a tilemap
    public void DrawVillage(TilemapData villageData)
    {
        villageMap.SetTiles(villageData.tilePositions, villageData.tiles);
    }

    public void DrawVillage(TilemapPrefab villageData)
    {
        villageMap.SetTiles(villageData.tilePositions, villageData.tiles);
    }

    public void DrawDetail(TilemapData detailData)
    {
        detailMap.SetTiles(detailData.tilePositions, detailData.tiles);
    }
}
