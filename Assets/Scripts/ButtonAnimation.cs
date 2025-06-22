using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private AudioClip clickSound;
    [SerializeField] private HighlightElement highlightElement;
    [SerializeField] private Image image;
    [SerializeField] private TMP_Text text;
    public Color color;

    private Color imageColor;
    private Color textColor;
    private AudioSource audioSource;
    private Button button;
    private Vector3 hoverScale = new(1.1f, 1.1f, 1f);
    private Vector3 clickScale = new(1.2f, 1.2f, 1f);

    private void Awake()
    {
        audioSource = FindAnyObjectByType<AudioSource>();

        button = GetComponent<Button>();
        button.onClick.AddListener(Animate);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, hoverScale, .1f).setEaseOutQuad();
        if (highlightElement == HighlightElement.Image)
        {
            imageColor = image.color;
            image.color = color;
        }
        if (highlightElement == HighlightElement.Text)
        {
            textColor = text.color;
            text.color = color;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.one, .1f).setEaseOutQuad();
        if (highlightElement == HighlightElement.Image)
        {
            image.color = imageColor;
        }
        if (highlightElement == HighlightElement.Text)
        {
            text.color = textColor;
        }
    }

    public void Animate()
    {
        PlaySound();

        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, clickScale, .1f).setEaseOutQuad();
        LeanTween.scale(gameObject, hoverScale, .1f).setEaseOutQuad().setDelay(.1f);
    }

    private void PlaySound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound, .05f);
        }
    }
}

public enum HighlightElement
{
    None = 0,
    Image = 1,
    Text = 2,
}
