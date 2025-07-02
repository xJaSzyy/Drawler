using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PageGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int countPerPage;
    [SerializeField] private List<Sprite> sprites = new();
    [SerializeField] private List<Sprite> graySprites = new();

    [Header("References")]
    [SerializeField] private GameObject pageHolder;
    [SerializeField] private GameObject pagePrefab;
    [SerializeField] private GameObject tabHolder;
    [SerializeField] private GameObject tabPrefab;
    [SerializeField] private SwipeController swipeController;
    [SerializeField] private PopupController popup;

    private List<LevelButton> levelButtons = new();
    private List<GameObject> tabButtons = new();
    private HashSet<string> tabNames = new();

    private void Start()
    {
        int spritesCount = sprites.Count;
        int pageCount = spritesCount / countPerPage;

        if (DataHolder.colored == null || DataHolder.colored.Length == 0)
        {
            DataHolder.colored = new int[spritesCount];
        }
        swipeController.maxPage = pageCount;
 
        for (int i = 0; i < pageCount; i++)
        {
            var page = Instantiate(pagePrefab, pageHolder.transform);
            page.name = $"{i + 1}";

            foreach (Transform child in page.transform)
            {
                LevelButton levelButton = child.gameObject.GetComponent<LevelButton>();
                levelButtons.Add(levelButton);
            }
        }

        int index = 0;
        levelButtons.ForEach(x =>
        {
            x.Setup(sprites[index], graySprites[index], popup);
            tabNames.Add(sprites[index].name.Split('_')[0]);
            index++;
        });

        levelButtons.ForEach(x =>
        {
            x.Load();
        });

        foreach (string tabName in tabNames)
        {
            Button tabButton = Instantiate(tabPrefab, tabHolder.transform).GetComponent<Button>();
            tabButtons.Add(tabButton.gameObject);
            tabButton.gameObject.GetComponentInChildren<TMP_Text>().text = tabName;
            var childText = tabButton.gameObject.transform.GetChild(1).gameObject;
            childText.AddComponent<Localizator>();
            childText.GetComponent<Localizator>().SetId(LocalizationManager.Instance.GetId(tabName));
            tabButton.onClick.AddListener(() => Filter(tabName));
            tabButton.gameObject.GetComponent<DynamicButton>().UpdateSize();
        }

        UpdateTab();

        Filter(tabNames.First());
    }

    public void Filter(string filter)
    {
        swipeController.maxPage = 0;

        levelButtons.ForEach(x =>
        {
            if (x.GetSprite().name.Contains(filter.ToLower()))
            {
                x.gameObject.transform.parent.gameObject.SetActive(true);
                swipeController.maxPage++;
            }
            else
            {
                x.gameObject.transform.parent.gameObject.SetActive(false);
            }
        });

        swipeController.maxPage /= countPerPage;

        swipeController.UpdateUI();

        foreach (GameObject tabButton in tabButtons)
        {
            if (tabButton.GetComponentInChildren<TMP_Text>().text.Contains(LocalizationManager.Instance.GetLocalization(filter.ToLower())))
            {
                tabButton.GetComponentInChildren<Image>().color = Color.yellow;
            }
            else
            {
                tabButton.GetComponentInChildren<Image>().color = Color.white;
            }
        }
    }

    public void UpdateTab()
    {
        RectTransform tabHolderRect = tabHolder.GetComponent<RectTransform>();
        RectTransform prefabRect = tabPrefab.GetComponent<RectTransform>();

        float width = (prefabRect.rect.width + 16) * tabNames.Count;
        tabHolderRect.sizeDelta = new Vector2(width, tabHolderRect.sizeDelta.y);

        LayoutRebuilder.ForceRebuildLayoutImmediate(tabHolderRect);
    }
 }
