using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothTime;
    Vector3 velocity;

    public Vector3 offset = new Vector3(0, 0, -10);

    private void Update()
    {
        if(target != null)
            transform.position = Vector3.SmoothDamp(transform.position, target.position + offset, ref velocity, smoothTime);
    }
}
