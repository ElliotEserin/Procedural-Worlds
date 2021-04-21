using UnityEngine;
using System;
using System.Collections;

public abstract class Generator : MonoBehaviour
{
    public static int worldSeed;
    protected int seed;

    [HideInInspector] public Generator nextGeneration;

    public delegate void GeneratingHandler();

    public event GeneratingHandler OnFinishedGenerating;

    public virtual void Initialise(WorldManager worldManager)
    {
        StartCoroutine(Generate(worldManager));
    }

    protected virtual IEnumerator Generate(WorldManager worldManager)
    {
        FinishGenerating(worldManager);
        yield return null;
    }

    protected void FinishGenerating(WorldManager manager)
    {
        if (nextGeneration != null)
            nextGeneration.Initialise(manager);
        else
            UIManager.StopLoading();

        if (OnFinishedGenerating != null)
            OnFinishedGenerating.Invoke();
    }
}
