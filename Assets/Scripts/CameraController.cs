using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float zoomSpeed = 5f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private float minZoom = 2f;
    [SerializeField] private float maxSize = 16f;
    [SerializeField] private float minSize = 0f;

    private Camera mainCamera;
    private Vector3 dragOrigin;
    private bool isDragging = false;
    private bool locked = false;


    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (locked) { return; }

        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            if (Input.GetKey(KeyCode.LeftControl))
            {
                float newSize = mainCamera.orthographicSize - scroll * zoomSpeed;
                newSize = Mathf.Clamp(newSize, minZoom, maxZoom);
                SetZoom(newSize);
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
        }

        if (isDragging)
        {
            if (Input.GetMouseButton(2))
            {
                Vector3 difference = mainCamera.ScreenToWorldPoint(dragOrigin) - mainCamera.ScreenToWorldPoint(Input.mousePosition);
                Vector3 newPosition = mainCamera.transform.position + difference;

                newPosition.x = Mathf.Clamp(newPosition.x, minSize, maxSize);
                newPosition.y = Mathf.Clamp(newPosition.y, minSize, maxSize);

                mainCamera.transform.position = newPosition;
                dragOrigin = Input.mousePosition;
            }
            else
            {
                isDragging = false;
            }
        }

        Vector3 clampedPosition = mainCamera.transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, minSize, maxSize);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, minSize, maxSize);
        mainCamera.transform.position = clampedPosition;
    }

    public void SetBorders(float minSize, float maxSize, float maxZoom)
    {
        this.minSize = minSize;
        this.maxSize = maxSize;
        this.maxZoom = maxZoom;
    }

    public void SetZoom(float newSize = -1f) => mainCamera.orthographicSize = (newSize < 0) ? maxZoom : newSize;

    public void Lock() => locked = true;
}
