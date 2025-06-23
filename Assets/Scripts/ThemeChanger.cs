using UnityEngine;
using UnityEngine.UI;

public class ThemeChanger : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite darkSprite;
    [SerializeField] private Sprite lightSprite;

    public void UpdateUI()
    {
        if (DataHolder.theme == "dark")
        {
            image.sprite = lightSprite;
        }
        else
        {
            image.sprite = darkSprite;
        }
    }
}
