using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pixel : MonoBehaviour
{
    [SerializeField] private PaintManager paintManager;
    [SerializeField] private Camera mainCamera;

    private void Update()
    {
        if (IsMouseOver())
        {
            if (Input.GetMouseButton(0))
            {
                paintManager.Paint(gameObject);
            }
        }
    }

    private bool IsMouseOver()
    {
        Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D collider = Physics2D.OverlapPoint(mouseWorldPos);

        if (collider != null && collider.gameObject == gameObject)
        {
            return true;
        }

        return false;
    }
}
