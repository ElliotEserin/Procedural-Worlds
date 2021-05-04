using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ObjectStore : MonoBehaviour
{
    #region singleton
    public static ObjectStore instance;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(gameObject);

        instance = this;
    }
    #endregion

    private void OnValidate()
    {
        if (instance == null)
            instance = this;
    }

    public Tilemap terrainMap;
    public Tilemap villageMap;
    public Tilemap detailMap;
    public Tilemap detailMap2;

    public MapDisplay mapDisplay;
    public WorldManager worldManager;
}
