using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectStore : MonoBehaviour
{
    public ObjectStore instance;

    private void Awake()
    {
        if (instance != null)
            Destroy(gameObject);

        instance = this;
    }
}
