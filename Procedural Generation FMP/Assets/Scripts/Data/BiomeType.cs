using UnityEngine;

[CreateAssetMenu(menuName ="Data/Biome Type")]
public class BiomeType : ScriptableObject
{
    new public string name;
    public Color colour;
    public UnityEngine.Tilemaps.TileBase tile;

    public BiomeDecoration[] biomeDecorations;
    [Range(0,1)]
    public float chanceOfNoDecorativeTile;

    float max;

    public UnityEngine.Tilemaps.TileBase GetDecorTile(System.Random rand, bool canBeNone)
    {
        if(max == 0)
            max = GetMaxChanceValue();
        int value = rand.Next(0, (int)max);

        float current = 0;

        if (canBeNone)
        {
            if(rand.Next(0, 100) / 100f < chanceOfNoDecorativeTile)
            {
                return null;
            }
        }

        foreach(BiomeDecoration decoration in biomeDecorations)
        {
            current += decoration.chance;

            if(value <= current)
            {
                return decoration.tile;
            }
        }

        return null;
    }

    float GetMaxChanceValue()
    {
        float chance = 0;

        foreach (var decoration in biomeDecorations)
            chance += decoration.chance;

        return chance;
    }

    [System.Serializable]
    public class BiomeDecoration
    {
        public UnityEngine.Tilemaps.TileBase tile;
        public float chance;
    }
}
