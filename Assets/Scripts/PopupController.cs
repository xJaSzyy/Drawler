using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private void Awake()
    {
        cancelButton.onClick.AddListener(() =>
        {
            cancelButton.gameObject.GetComponent<ButtonAnimation>().Animate();
            Close();
        });
    }

    public void Show(UnityAction ConfirmClick)
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() =>
        {
            Close();
            ConfirmClick();
        });

        gameObject.SetActive(true);
        gameObject.transform.localScale = Vector3.zero;
        LeanTween.scale(gameObject, Vector3.one, 0.3f).setEase(LeanTweenType.easeOutBack);
    }

    public void Close()
    {
        LeanTween.scale(gameObject, Vector3.zero, 0.3f).setEase(LeanTweenType.easeInBack).setOnComplete(() =>
        {
            gameObject.SetActive(false);
            gameObject.transform.localScale = Vector3.one;
        });
    }
}
