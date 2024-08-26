using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;
    public float smoothSpeed = 0.125f;
    public float tiltAngle = 35f;

    private void LateUpdate()
    {
        if (player == null)
        {
            Debug.LogWarning("Player transform not assigned.");
            return;
        }

        Vector3 desiredPosition = player.position + offset;

        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

        transform.position = smoothedPosition;

        Quaternion targetRotation = Quaternion.Euler(tiltAngle, transform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed);

    }
}