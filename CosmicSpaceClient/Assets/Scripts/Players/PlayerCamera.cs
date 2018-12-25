using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static GameObject TargetGameObject;

    [Header("Camera Settings")]
    public Vector3 CameraDistance;
    public float CameraSpeed;

    private void LateUpdate()
    {
        if (TargetGameObject == null)
            return;

        transform.position = Vector3.Lerp(transform.position, TargetGameObject.transform.position + CameraDistance, CameraSpeed);
    }
}