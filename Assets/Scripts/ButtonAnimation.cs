using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private AudioClip clickSound;

    private AudioSource audioSource;
    private Button button;
    private Vector3 hoverScale = new(1.1f, 1.1f, 1f);
    private Vector3 clickScale = new(1.2f, 1.2f, 1f);

    private Image image;
    private TMP_Text text;

    private void Awake()
    {
        if (TryGetComponent(out Image image))
        {
            this.image = image;
        }
        else
        {
            text = GetComponentInChildren<TMP_Text>();
        }

        audioSource = FindAnyObjectByType<AudioSource>();

        button = GetComponent<Button>();
        button.onClick.AddListener(Animate);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, hoverScale, .1f).setEaseOutQuad();
        if (image != null)
        {
            image.color = Color.yellow;
        }
        else
        {
            text.color = Color.yellow;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.one, .1f).setEaseOutQuad();
        if (image != null)
        {
            image.color = Color.white;
        }
        else
        {
            text.color = Color.white;
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
