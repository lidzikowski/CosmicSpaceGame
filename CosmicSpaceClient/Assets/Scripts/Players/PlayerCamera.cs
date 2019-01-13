using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public static GameObject TargetGameObject;
    
    public static Vector3 CameraDistance;
    public float CameraSpeed;

    private static Vector3 CameraPosition;
    private static int index;
    public static int Index
    {
        get => index;
        set
        {
            index = value;
            CameraDistance = new Vector3(CameraPosition.x, 
                CameraPosition.y - Index, 
                CameraPosition.z - (Index * 5));
        }
    }

    private void Start()
    {
        CameraPosition = new Vector3(0, -10, -50);
        Index = 5;
        CameraSpeed = 1;
    }

    private void LateUpdate()
    {
        if (TargetGameObject == null)
            return;

        transform.position = Vector3.Lerp(transform.position, TargetGameObject.transform.position + CameraDistance, CameraSpeed);

        ScrollController();
    }

    private void ScrollController()
    {
        float axis = Input.GetAxis("Mouse ScrollWheel");
        if (axis != 0f)
        {
            int x = axis > 0 ? -1 : 1;
            if (Index + x >= 0 && Index + x <= 10)
                Index += x;
        }
    }
}