using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class Pixel : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
{
    [SerializeField] private PaintManager paintManager;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            paintManager.Paint(gameObject);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            paintManager.Paint(gameObject);
        }
    }
}
