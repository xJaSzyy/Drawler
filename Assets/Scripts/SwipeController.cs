using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SwipeController : MonoBehaviour, IEndDragHandler
{
    [Header("Settings")]
    [SerializeField] private int maxPage;
    [SerializeField] private Vector3 pageStep;
    [SerializeField] private float tweenTime;
    [SerializeField] private LeanTweenType tweenType;
    [SerializeField] private Sprite barDefault;
    [SerializeField] private Sprite barActive;

    [Header("References")]
    [SerializeField] private RectTransform levelPagesRect;
    [SerializeField] private Image[] barImages;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;

    private int currentPage;
    private Vector3 targetPos;
    private float dragThreshould;
    private LTDescr tween;

    private void Awake()
    {
        currentPage = 1;
        targetPos = levelPagesRect.localPosition;
        dragThreshould = Screen.width / 15;
        UpdateBar();
        UpdateArrowButton();
    }

    public void Next()
    {
        if (currentPage < maxPage)
        {
            currentPage++;
            targetPos += pageStep;
            MovePage();
        }
    }

    public void Previous()
    {
        if (currentPage > 1)
        {
            currentPage--;
            targetPos -= pageStep;
            MovePage();
        }
    }

    private void MovePage()
    {
        if (tween != null)
        {
            tween.reset();
        }
        tween = levelPagesRect.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);
        UpdateBar();
        UpdateArrowButton();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.position.x - eventData.pressPosition.x) > dragThreshould)
        {
            if (eventData.position.x > eventData.pressPosition.x)
            {
                Previous();
            }
            else
            {
                Next();
            }
        }
        else
        {
            MovePage();
        }
    }

    private void UpdateBar()
    {
        foreach (var item in barImages)
        {
            item.sprite = barDefault;
            item.rectTransform.localScale = Vector3.one;
            item.color = new Color(item.color.r, item.color.g, item.color.b, 1f);
        }

        var activeImage = barImages[currentPage - 1];
        activeImage.sprite = barActive;

        activeImage.color = new Color(activeImage.color.r, activeImage.color.g, activeImage.color.b, 0f);
        activeImage.rectTransform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        LeanTween.alpha(activeImage.rectTransform, 1f, 0.5f).setEase(LeanTweenType.easeOutQuad);
        LeanTween.scale(activeImage.rectTransform, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);
    }

    private void UpdateArrowButton()
    {
        nextButton.interactable = true;
        previousButton.interactable = true;
        if (currentPage == 1)
        {
            previousButton.interactable = false;
        }
        else if (currentPage == maxPage)
        {
            nextButton.interactable = false;
        }
    }
}
