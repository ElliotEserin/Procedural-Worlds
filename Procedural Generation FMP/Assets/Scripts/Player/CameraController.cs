using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothTime;
    Vector2 velocity;

    private void Update()
    {
        transform.position = Vector2.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
    }
}
