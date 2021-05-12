using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    [System.Serializable]
    public class NPCType
    {
        public string poolTag;
        public BiomeType[] allowedBiomes;
        [Range(0, 100)]
        public int spawnChance;
    }

    public NPCType[] characterTypes;

    Vector3 lastSpawnLocation;
    public int spawnFrequency;
    public int numberToSpawn;
    public int variation;

    public Vector2Int outerSpawn;
    public Vector2Int innerSpawn;

    System.Random rand;

    int totalSpawnChance;

    private void Start()
    {
        rand = new System.Random();
        totalSpawnChance = TotalSpawnChance();
    }

    public void SpawnNewCreatures(Vector3 currentPosition)
    {
        if (Vector3.Distance(lastSpawnLocation, currentPosition) < spawnFrequency)
            return;

        lastSpawnLocation = currentPosition;

        var spawnCount = rand.Next(numberToSpawn - variation, numberToSpawn + variation);

        for (int i = 0; i < spawnCount; i++)
        {
            var p = rand.Next(0, totalSpawnChance);

            var charToSpawn = GetCharacterFromChance(p);

            float x = rand.Next(-outerSpawn.x, outerSpawn.x);
            float y = rand.Next(-outerSpawn.y, outerSpawn.y);

            if (x < innerSpawn.x && x > -innerSpawn.x)
            {
                if (x < innerSpawn.x) x = innerSpawn.x;
                else if (x > -innerSpawn.x) x = innerSpawn.x;
            }
            if (y < innerSpawn.y && y > -innerSpawn.y)
            {
                if (y < innerSpawn.y) y = innerSpawn.y;
                else if (y > -innerSpawn.y) y = innerSpawn.y;
            }

            var pos = new Vector3(x, y, 0);

            Debug.Log(ObjectStore.instance.worldManager.worldData.GetTile(Vector3Int.RoundToInt(pos + currentPosition)));

            if (!ObjectStore.instance.worldManager.worldData.CheckBiomes(Vector3Int.RoundToInt(pos + currentPosition), charToSpawn.allowedBiomes))
                continue;

            pos += currentPosition;

            ObjectPooler.instance.SpawnFromPool(charToSpawn.poolTag, pos, Quaternion.identity, true);
        }
    }

    int TotalSpawnChance()
    {
        int chance = 0;

        foreach (var type in characterTypes)
        {
            chance += type.spawnChance;
        }

        return chance;
    }

    NPCType GetCharacterFromChance(int chance)
    {
        int tally = 0;

        foreach (var type in characterTypes)
        {
            tally += type.spawnChance;

            if (chance <= tally)
                return type;
        }

        return null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(innerSpawn.x, innerSpawn.y));
        Gizmos.DrawWireCube(transform.position, new Vector3(outerSpawn.x, outerSpawn.y));
    }
}
