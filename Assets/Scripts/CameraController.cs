using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
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
                if (scroll > 0)
                {
                    PlusZoom();
                }
                else
                {
                    MinusZoom();
                }
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

    public void SetZoom(float newSize = -1f)
    {
        float size = (newSize == -1f) ? maxZoom : newSize;
        size = Mathf.Clamp(size, minZoom, maxZoom);
        mainCamera.orthographicSize = size;
    }

    public void PlusZoom()
    {
        float step = (maxZoom - minZoom) / 5;
        SetZoom(mainCamera.orthographicSize - step);
    }

    public void MinusZoom()
    {
        float step = (maxZoom - minZoom) / 5;
        SetZoom(mainCamera.orthographicSize + step);
    }

    public void Lock() => locked = true;
}
