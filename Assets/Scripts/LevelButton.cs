using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
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
            SpriteHolder.sprite = transform.GetChild(0).GetComponent<Image>().sprite;
            SceneFader.Instance.FadeOutAndLoadScene(1);
        });
    }
}
