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

    private List<LevelButton> levelButtons = new();
    private List<GameObject> tabButtons = new();

    private void Awake()
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

        HashSet<string> tabNames = new();
        int index = 0;
        levelButtons.ForEach(x =>
        {
            x.Setup(sprites[index], graySprites[index]);
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
            tabButton.onClick.AddListener(() => Filter(tabName));
            tabButton.gameObject.GetComponent<DynamicButton>().UpdateSize();
        }

        RectTransform tabHolderRect = tabHolder.GetComponent<RectTransform>();
        RectTransform prefabRect = tabPrefab.GetComponent<RectTransform>();

        float width = (prefabRect.rect.width + 16) * tabNames.Count;
        tabHolderRect.sizeDelta = new Vector2(width, tabHolderRect.sizeDelta.y);

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
            if (tabButton.GetComponentInChildren<TMP_Text>().text.Contains(filter.ToLower()))
            {
                tabButton.GetComponentInChildren<Image>().color = Color.yellow;
            }
            else
            {
                tabButton.GetComponentInChildren<Image>().color = Color.white;
            }
        }
    }
}
