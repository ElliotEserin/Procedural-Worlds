using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    static int currentCollected;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            currentCollected++;
            Debug.Log(currentCollected);
            Destroy(gameObject);
        }
    }
}
