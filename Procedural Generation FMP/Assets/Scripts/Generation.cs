using UnityEngine;

public abstract class Generator : MonoBehaviour
{
    protected int seed;

    public abstract void Initialise(int seed);

    public abstract void Generate();
}
