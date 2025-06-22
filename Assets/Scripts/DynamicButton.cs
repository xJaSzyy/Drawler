using TMPro;
using UnityEngine;

public class DynamicButton : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private float padding = 8f;
    
    private RectTransform rectTransform; 

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
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
