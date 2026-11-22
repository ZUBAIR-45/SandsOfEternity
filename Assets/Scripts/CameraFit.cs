using UnityEngine;

// Camera ko screen ke hisaab se adjust karta hai taake poora maze + upar UI bar fit ho.
[RequireComponent(typeof(Camera))]
public class CameraFit : MonoBehaviour
{
    public float mazeWidth = 19f;
    public float mazeHeight = 11f;
    public float topSpace = 2.3f;

    Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = true;
    }

    void LateUpdate()
    {
        float sizeH = (mazeHeight + topSpace + 0.4f) / 2f;
        float sizeW = (mazeWidth + 1f) / (2f * cam.aspect);
        float size = Mathf.Max(sizeH, sizeW);
        cam.orthographicSize = size;
        float y = size - (mazeHeight * 0.5f + 0.2f);
        transform.position = new Vector3(0f, y, -10f);
    }
}
