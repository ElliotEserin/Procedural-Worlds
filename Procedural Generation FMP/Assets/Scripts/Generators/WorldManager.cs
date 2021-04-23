using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldManager : MonoBehaviour
{
    public enum WorldSize
    {
        Island = 500,
        Small = 1000,
        Medium = 1500,
        Large = 2500,
    }

    public WorldSize worldSize;

    public bool useCustomSize;
    public int customSize;

    public Texture2D worldMap;

    //Seed for the world
    public int seed;

    public Grid pathfindingGrid;
    public Generator mapGen, riverGen, villageGen, buildingGen, detailGen, pathGen;

    public GameObject playerPrefab;
    public CameraController cameraController;

    public WorldData worldData;

    public TilemapPrefab[] buildingPrefabs;

    private void Start()
    {
        if (WorldInfo.useWorldInfo)
        {
            seed = WorldInfo.worldSeed;
            worldSize = WorldInfo.worldSize;
        }

        if (seed == 0)
            seed = RandomString(8).GetHashCode();

        Generator.worldSeed = seed;

        Generation();
    }

    void Generation()
    {
        UIManager.UpdateLoadScreenText("Initialising...");

        //Setting up generating chain
        mapGen.nextGeneration = riverGen;
        riverGen.nextGeneration = villageGen;
        villageGen.nextGeneration = buildingGen;
        buildingGen.nextGeneration = detailGen;
        detailGen.nextGeneration = pathGen;
        pathGen.OnFinishedGenerating += InitialisePlayer;
        pathGen.OnFinishedGenerating += InitialiseWorldMap;

        //Generates the world
        mapGen.Initialise(this);
    }

    public void InitialisePlayer()
    {
        UIManager.UpdateLoadScreenText("Moulding a player...");

        var middle = (int)worldSize / 2;
        var target = Instantiate(playerPrefab, new Vector3(middle, middle, 0), Quaternion.identity);

        cameraController.target = target.transform;
        cameraController.transform.position = target.transform.position;

        UIManager.UpdateLoadScreenText("Starting weather.");

        var weather = FindObjectOfType<DayNightCycle>();

        weather.transform.parent = target.transform;
        weather.transform.localPosition = Vector3.zero;
        weather.spotLights.Add(target.GetComponentInChildren<UnityEngine.Experimental.Rendering.Universal.Light2D>());
    }

    public void InitialiseWorldMap()
    {
        TextureGenerator.TextureFromColourMap(worldData.worldMap, MapGenerator.mapDimension, MapGenerator.mapDimension, worldMap);
    }

    public string RandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];
        var random = new System.Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        return new string(stringChars);
    }
}

//Stores the world data
public class WorldData : TilemapData
{
    public Color[] worldMap;
    public float[,] heightMap;

    public TileType[,] tileTypeMap;

    public void SetWorldMapPoint(int x, int y, Color colour)
    {
        worldMap[y * MapGenerator.mapDimension + x] = colour;
    }

    public void SetWorldMapIcon(int x, int y, Texture2D icon)
    {
        if (icon == null)
            return;

        x /= MapGenerator.mapRatio;
        y /= MapGenerator.mapRatio;

        Color[] iconColours = icon.GetPixels();

        int sideLength = (int)Mathf.Sqrt(iconColours.Length);

        for (int yy = 0; yy < sideLength; yy++)
        {
            for (int xx = 0; xx < sideLength; xx++)
            {
                Debug.Log(iconColours[yy * sideLength + xx]);
                SetWorldMapPoint(x + xx, y + yy, iconColours[yy * sideLength + xx]);
            }
        }
    }
}
