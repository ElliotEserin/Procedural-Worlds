using UnityEngine;
using System;

public abstract class Generator : MonoBehaviour
{
    public static int worldSeed;
    protected int seed;

    [HideInInspector] public Generator nextGeneration;

    public abstract void Initialise(WorldManager worldManager);

    protected abstract void Generate(WorldManager worldManager);

    protected void FinishGenerating(WorldManager manager)
    {
        if (nextGeneration != null)
            nextGeneration.Initialise(manager);
    }
}
