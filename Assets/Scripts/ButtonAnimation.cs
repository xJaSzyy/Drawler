using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class ButtonAnimation : MonoBehaviour
{
    private Button button;
    private Vector3 upScale = new Vector3(1.1f, 1.1f, 1f);

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => Animate(null));
    }

    public void Animate(System.Action onComplete)
    {
        LeanTween.scale(gameObject, upScale, .1f);
        LeanTween.scale(gameObject, Vector3.one, .1f).setDelay(.1f).setOnComplete(() =>
        {
            onComplete?.Invoke();
        });
    }
}
