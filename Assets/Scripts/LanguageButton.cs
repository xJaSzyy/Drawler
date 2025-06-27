using UnityEngine;
using UnityEngine.UI;
using static Unity.VisualScripting.Icons;

public class LanguageButton : MonoBehaviour
{
    [SerializeField] private Sprite enSprite;
    [SerializeField] private Sprite ruSprite;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = LocalizationManager.Instance.GetLanguage() == LocalizationLanguage.en ? enSprite : ruSprite;
    }

    public void ChangeLanguage()
    {
        var language = LocalizationManager.Instance.ChangeLanguage();
        image.sprite = language == LocalizationLanguage.en ? enSprite : ruSprite;
    }
}
