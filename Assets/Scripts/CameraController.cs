using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    [SerializeField] private GameObject grid;
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float maxZoom = 4f;
    [SerializeField] private float minZoom = 1f;

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
        if (locked) return;

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
                Vector3 difference = mainCamera.ScreenToWorldPoint(Input.mousePosition) - mainCamera.ScreenToWorldPoint(dragOrigin);
                Vector3 newPosition = grid.transform.position + difference * moveSpeed;

                grid.transform.position = newPosition;
                dragOrigin = Input.mousePosition;
            }
            else
            {
                isDragging = false;
            }
        }
    }

    public void SetZoom(float newScale = -1f)
    {
        float scale = (newScale == -1f) ? minZoom : newScale;
        scale = Mathf.Clamp(scale, minZoom, maxZoom);
        grid.transform.localScale = new Vector3(scale, scale, scale);
    }

    public void PlusZoom()
    {
        float step = (maxZoom - minZoom) / 5f;
        float currentScale = grid.transform.localScale.x;
        SetZoom(currentScale + step);
    }

    public void MinusZoom()
    {
        float step = (maxZoom - minZoom) / 5f;
        float currentScale = grid.transform.localScale.x;
        SetZoom(currentScale - step);
    }

    public void ResetImage()
    {
        SetZoom();
        LeanTween.moveLocal(grid, Vector3.zero, 0.5f).setEase(LeanTweenType.easeOutQuad);
    }

    public void Lock() => locked = true;
}
