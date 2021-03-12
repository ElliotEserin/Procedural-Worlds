using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed;
    // Update is called once per frame
    void Update()
    {
        Camera.main.orthographicSize -= Input.GetAxisRaw("Mouse ScrollWheel") * speed;
    }
}
