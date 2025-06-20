using UnityEngine;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;

    private AudioSource audioSource;
    private Button button;
    private Vector3 upScale = new(1.1f, 1.1f, 1f);

    private void Awake()
    {
        audioSource = FindAnyObjectByType<AudioSource>();

        button = GetComponent<Button>();
        button.onClick.AddListener(() => Animate(null));
    }

    public void Animate(System.Action onComplete)
    {
        if (clickSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(clickSound, 0.05f);
        }

        LeanTween.scale(gameObject, upScale, .1f);
        LeanTween.scale(gameObject, Vector3.one, .1f).setDelay(.1f).setOnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
}
