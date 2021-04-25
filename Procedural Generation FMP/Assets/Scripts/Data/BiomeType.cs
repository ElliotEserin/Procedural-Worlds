using UnityEngine;

[CreateAssetMenu(menuName ="Data/Biome Type")]
public class BiomeType : TileType
{
    public BiomeDecoration[] biomeDecorations;
    [Range(0,1)]
    public float chanceOfNoDecorativeTile;

    float max;

    public UnityEngine.Tilemaps.TileBase GetDecorTile(System.Random rand, bool canBeNone, float distance)
    {
        if(max == 0)
            max = GetMaxChanceValue();
        int value = rand.Next(0, (int)max);

        float current = 0;

        if (canBeNone)
        {
            const float minAllowedDistance = 15;
            const float falloff = 3;

            if (distance == 0)
                distance = 0.0001f;

            float chance = rand.Next(0, 100) / 100f + (minAllowedDistance / distance) * falloff;

            if(distance < minAllowedDistance)
            {
                return null;
            }

            if (chance < chanceOfNoDecorativeTile)
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
