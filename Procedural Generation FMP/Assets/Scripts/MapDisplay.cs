using UnityEngine;
using UnityEngine.Tilemaps;

public class MapDisplay : MonoBehaviour
{
    public Renderer textureRenderer;
    [Range(0.0001f, 1f)]
    public float scale;

    public Color maxTintVariation;

    private void Awake()
    {
        ResetTilemaps();
    }

    public void ResetTilemaps()
    {
        if(ObjectStore.instance.terrainMap != null)
        {
            ObjectStore.instance.terrainMap.ClearAllTiles();
            ObjectStore.instance.terrainMap.ClearAllEditorPreviewTiles();
        }
        if(ObjectStore.instance.villageMap != null)
        {
            ObjectStore.instance.villageMap.ClearAllTiles();
            ObjectStore.instance.villageMap.ClearAllEditorPreviewTiles();
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
        ObjectStore.instance.terrainMap.SetTiles(worldData.tilePositions, worldData.tiles);

        //var rand = new System.Random();

        //for (int x = 0; x < worldData.tilePositions.Length; x++)
        //{
        //    terrainMap.SetColor(worldData.tilePositions[x], Color.Lerp(Color.white, maxTintVariation, rand.Next(0, 100)));
        //}
    }   
    
    //Draws the tiles to a tilemap
    public void DrawVillage(TilemapData villageData)
    {
        ObjectStore.instance.villageMap.SetTiles(villageData.tilePositions, villageData.tiles);
    }

    public void DrawVillage(TilemapPrefab villageData)
    {
        ObjectStore.instance.villageMap.SetTiles(villageData.tilePositions, villageData.tiles);
    }

    public void DrawDetail(TilemapData detailData)
    {
        ObjectStore.instance.detailMap.SetTiles(detailData.tilePositions, detailData.tiles);
    }
}
