using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class PopupController : MonoBehaviour
{
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button cancelButton;

    private void Awake()
    {
        cancelButton.onClick.AddListener(Close);
    }

    public void Show(UnityAction ConfirmClick)
    {
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(ConfirmClick);

        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
