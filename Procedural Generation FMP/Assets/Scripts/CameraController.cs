using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveSpeed;
    public float zoomSpeed;
    // Update is called once per frame
    void Update()
    {
        Camera.main.orthographicSize -= Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed;
        Vector3 pos = new Vector2(Input.GetAxisRaw("Horizontal") * moveSpeed, Input.GetAxisRaw("Vertical") * moveSpeed);
        transform.Translate(pos * Time.deltaTime);
    }
}
