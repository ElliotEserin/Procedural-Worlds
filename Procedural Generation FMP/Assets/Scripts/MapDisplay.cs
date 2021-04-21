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
        }
        if(ObjectStore.instance.villageMap != null)
        {
            ObjectStore.instance.villageMap.ClearAllTiles();
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
    }   

    public void DrawTerrain(TilemapData terrain)
    {
        ObjectStore.instance.terrainMap.SetTiles(terrain.tilePositions, terrain.tiles);
    }
    
    //Draws the tiles to a tilemap
    public void DrawVillage(TilemapData villageData)
    {
        ObjectStore.instance.villageMap.SetTiles(villageData.tilePositions, villageData.tiles);
    }

    public void DrawBuilding(TilemapPrefab villageData)
    {
        ObjectStore.instance.villageMap.SetTiles(villageData.tilePositions, villageData.tiles);
    }

    public void DrawNonCollidableDetail(TilemapData detailData)
    {
        ObjectStore.instance.detailMap.SetTiles(detailData.tilePositions, detailData.tiles);
    }

    public void DrawCollidableDetail(TilemapData data)
    {
        ObjectStore.instance.detailMap2.SetTiles(data.tilePositions, data.tiles);
    }
}
