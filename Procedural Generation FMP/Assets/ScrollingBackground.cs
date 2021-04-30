using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GenerationHelpers;

public class ScrollingBackground : MonoBehaviour
{
    public int size;
    public int seed;
    public float ratio = 0.01f;

    public NoiseData terrainData;
    public TerrainType[] regions;

    public Renderer textureRenderer;
    public Texture2D mapTexture;

    public float[,] falloffMap;

    public Vector2 offsetPerSecond;
    Vector2 offset = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(size, size);

        textureRenderer.sharedMaterial.mainTexture = mapTexture;
        textureRenderer.transform.localScale = new Vector3(mapTexture.width * ratio, 1, mapTexture.height * ratio);

        seed = new System.Random().Next(0, 100);

        ChangeMap();
    }

    // Update is called once per frame
    void ChangeMap()
    {
        offset += offsetPerSecond * Time.deltaTime;

        var terrainMap = Noise.GenerateNoiseMap(size, seed, terrainData.noiseScale, terrainData.octaves, terrainData.persistance, terrainData.lacunarity, terrainData.offset + offset);

        Color[] colorMap = new Color[size * size];

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                //Get values of a tile
                float currentHeight = terrainMap[x, y] - falloffMap[x,y];

                //Assign color and tile based on values
                for (int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colorMap[y * size + x] = regions[i].colour;
                        Debug.Log(regions[i].height);
                        break;
                    }
                }
            }
        }

        DisplayMap(colorMap, size);

        void DisplayMap(Color[] colourMap, int size)
        {
            TextureGenerator.TextureFromColourMap(colourMap, size, size, mapTexture);
        }
    }
}
