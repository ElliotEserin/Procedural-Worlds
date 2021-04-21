using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public TMP_InputField seed;
    public TMP_Dropdown size;
    public Toggle useVillages;

    public void Generate()
    {
        WorldInfo.worldSeed = seed.text.GetHashCode();

        WorldInfo.useVillages = useVillages.isOn;

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

    public static int worldSeed;
    public static WorldManager.WorldSize worldSize;
    public static bool useVillages;
}
