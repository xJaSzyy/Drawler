using TMPro;
using UnityEngine;

public class DynamicButton : MonoBehaviour
{
    [SerializeField] private float padding = 8f;
    
    private RectTransform rectTransform; 
    private TMP_Text text;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        text = GetComponentInChildren<TMP_Text>();
    }

    public void UpdateSize()
    {
        if (text != null && rectTransform != null)
        {
            float textWidth = text.preferredWidth;
            float textHeight = text.preferredHeight;

            rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textWidth + padding * 2);
        }
    }
}
