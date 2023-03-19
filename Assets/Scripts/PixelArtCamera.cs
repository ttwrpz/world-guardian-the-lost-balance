using UnityEngine;

public class PixelArtCamera : MonoBehaviour
{
    private Camera mainCamera;
    Camera cam;
    public float Z;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        cam.fieldOfView = mainCamera.fieldOfView;
        cam.transform.position = new Vector3 (mainCamera.transform.position.x, mainCamera.transform.position.y, mainCamera.transform.position.z - Z);
    }
}