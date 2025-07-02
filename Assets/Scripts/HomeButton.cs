using UnityEngine;
using UnityEngine.UI;

public class HomeButton : MonoBehaviour
{
    [SerializeField] private PopupController popup;

    private Button button;
    private ButtonAnimation buttonAnimation;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonAnimation = GetComponent<ButtonAnimation>();
        button.onClick.AddListener(() =>
        {
            PaintManager paintManager = FindAnyObjectByType<PaintManager>();
            if (!paintManager.finished)
            {
                ShowPopup();
            }
            else
            {
                ButtonClick();
            }
        });
    }

    private void ShowPopup()
    {
        buttonAnimation.Animate();
        popup.Show(ButtonClick);
    }

    private void ButtonClick()
    {
        buttonAnimation.Animate();
        SceneFader.Instance.FadeOutAndLoadScene(0);
    }
}
