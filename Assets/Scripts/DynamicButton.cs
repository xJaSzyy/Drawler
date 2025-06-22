using TMPro;
using UnityEngine;

public class DynamicButton : MonoBehaviour
{
    public TMP_Text tabText;
    public RectTransform buttonRectTransform; 
    public float padding = 8f; 

    public void UpdateSize()
    {
        if (tabText != null && buttonRectTransform != null)
        {
            float textWidth = tabText.preferredWidth;
            float textHeight = tabText.preferredHeight;

            buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, textWidth + padding * 2);
            //buttonRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, textHeight + padding * 2);
        }
    }
}
