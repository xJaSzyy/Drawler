using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private Sprite coloredSprite;
    [SerializeField] private Sprite graySprite;

    private PopupController popup;
    private Button button;
    private ButtonAnimation buttonAnimation;
    private bool colored = false;

    public void Setup(Sprite coloredSprite, Sprite graySprite, PopupController popup)
    {
        this.coloredSprite = coloredSprite;
        this.graySprite = graySprite;
        this.popup = popup;
    }

    public void Load()
    {
        int page = Convert.ToInt32(transform.parent.name);
        index = ((page - 1) * (transform.parent.childCount - 1) + transform.GetSiblingIndex() + page) - 1;

        if (DataHolder.colored[index] == 0)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = graySprite;
            colored = false;
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().sprite = coloredSprite;
            transform.GetChild(1).gameObject.SetActive(true);
            colored = true;
        }

        button = GetComponent<Button>();
        buttonAnimation = GetComponent<ButtonAnimation>();
        button.onClick.AddListener(() =>
        {
            if (colored)
            {
                ShowPopup();
            }
            else
            {
                ButtonClick();
            }
        });
    }

    public Sprite GetSprite()
    {
        return coloredSprite;
    }

    private void ShowPopup()
    {
        buttonAnimation.Animate();
        popup.Show(ButtonClick);
    }

    private void ButtonClick()
    {
        buttonAnimation.Animate();
        DataHolder.index = index;
        DataHolder.sprite = coloredSprite;
        SceneFader.Instance.FadeOutAndLoadScene(1);
    }
}
