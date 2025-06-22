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

    private void Awake()
    {
        audioSource = FindAnyObjectByType<AudioSource>();

        button = GetComponent<Button>();
        button.onClick.AddListener(() => Animate(null));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, hoverScale, .1f).setEaseOutQuad();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LeanTween.cancel(gameObject);
        LeanTween.scale(gameObject, Vector3.one, .1f).setEaseOutQuad();
    }

    public void Animate(System.Action onComplete)
    {
        PlaySound();

        LeanTween.cancel(gameObject);

        LeanTween.scale(gameObject, clickScale, 0.1f).setEaseOutQuad().setOnComplete(() =>
        {
            LeanTween.scale(gameObject, hoverScale, 0.1f).setEaseOutQuad().setOnComplete(() =>
            {
                onComplete?.Invoke();
            });
        });
    }

    private void PlaySound()
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound, 0.05f);
        }
    }
}
