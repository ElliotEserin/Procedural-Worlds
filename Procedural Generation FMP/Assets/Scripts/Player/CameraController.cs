using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float smoothSpeed;
    Vector3 velocity;

    public Vector3 offset = new Vector3(0, 0, -10);

    private void FixedUpdate()
    {
        if (target != null)
        {
            Vector3 desiredPosition = target.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.smoothDeltaTime);
            transform.position = smoothedPosition;
        }
    }
}
