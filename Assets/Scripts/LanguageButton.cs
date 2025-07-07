using UnityEngine;
using UnityEngine.UI;

public class LanguageButton : MonoBehaviour
{
    [SerializeField] private Sprite enSprite;
    [SerializeField] private Sprite ruSprite;
    [SerializeField] private float animationSpeed = .2f;

    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
        image.sprite = LocalizationManager.Instance.GetLanguage() == LocalizationLanguage.en ? enSprite : ruSprite;
    }

    public void ChangeLanguage()
    {
        var language = LocalizationManager.Instance.ChangeLanguage();
        Sprite newSprite = language == LocalizationLanguage.en ? enSprite : ruSprite;

        LeanTween.scale(image.rectTransform, Vector3.zero, animationSpeed).setOnComplete(() =>
        {
            image.sprite = newSprite;
            LeanTween.scale(image.rectTransform, Vector3.one, animationSpeed).setEase(LeanTweenType.easeOutBack);
        });
    }
}
