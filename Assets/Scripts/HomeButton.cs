using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeButton : MonoBehaviour
{
    private Button button;
    private ButtonAnimation buttonAnimation;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonAnimation = GetComponent<ButtonAnimation>();
        button.onClick.AddListener(ButtonClick);
    }

    private void ButtonClick()
    {
        buttonAnimation.Animate(() =>
        {
            SceneManager.LoadScene(0);
        });
    }
}
