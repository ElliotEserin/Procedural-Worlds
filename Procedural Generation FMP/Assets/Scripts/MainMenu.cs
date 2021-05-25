using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField worldName;
    public TMP_InputField seed;
    public TMP_Dropdown size;
    public Toggle generateBuildings;
    public Toggle generateRivers;
    public Toggle spawnAnimals;

    public void Generate()
    {
        WorldInfo.worldName = worldName.text;
        WorldInfo.worldSeed = seed.text.GetHashCode();

        WorldInfo.generateBuildings = generateBuildings.isOn;
        WorldInfo.generateRivers = generateRivers.isOn;
        WorldInfo.spawnAnimals = spawnAnimals.isOn;

        WorldInfo.useWorldInfo = true;

        switch (size.value)
        {
            case 0:
                WorldInfo.worldSize = WorldManager.WorldSize.Large;
                break;
            case 1:
                WorldInfo.worldSize = WorldManager.WorldSize.Medium;
                break;
            case 2:
                WorldInfo.worldSize = WorldManager.WorldSize.Small;
                break;
            case 3:
                WorldInfo.worldSize = WorldManager.WorldSize.Island;
                break;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}

public static class WorldInfo
{
    public static bool useWorldInfo = false;

    public static string worldName;
    public static int worldSeed;
    public static WorldManager.WorldSize worldSize;
    public static bool generateBuildings;
    public static bool generateRivers;
    public static bool spawnAnimals;
}
