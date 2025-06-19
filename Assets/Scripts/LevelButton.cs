using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private int index;
    [SerializeField] private Sprite coloredSprite;
    [SerializeField] private Sprite graySprite;

    private Button button;
    private ButtonAnimation buttonAnimation;

    public void Setup(Sprite coloredSprite, Sprite graySprite)
    {
        this.coloredSprite = coloredSprite;
        this.graySprite = graySprite;    
    }

    public void Load()
    {
        button = GetComponent<Button>();
        buttonAnimation = GetComponent<ButtonAnimation>();
        button.onClick.AddListener(ButtonClick);

        int page = Convert.ToInt32(transform.parent.name);
        index = ((page - 1) * (transform.parent.childCount - 1) + transform.GetSiblingIndex() + page) - 1;

        if (DataHolder.colored[index] == 0)
        {
            transform.GetChild(0).GetComponent<Image>().sprite = graySprite;
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().sprite = coloredSprite;
        }
    }

    public Sprite GetSprite()
    {
        return coloredSprite;
    }

    private void ButtonClick()
    {
        buttonAnimation.Animate(() =>
        {
            DataHolder.index = index;
            DataHolder.sprite = coloredSprite;
            SceneFader.Instance.FadeOutAndLoadScene(1);
        });
    }
}
