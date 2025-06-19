using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PageGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int countPerPage;
    [SerializeField] private List<Sprite> sprites = new();
    [SerializeField] private List<Sprite> graySprites = new();

    [Header("References")]
    [SerializeField] private GameObject pageHolder;
    [SerializeField] private GameObject pagePrefab;
    [SerializeField] private SwipeController swipeController;

    private List<LevelButton> levelButtons = new();

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

        int index = 0;
        levelButtons.ForEach(x =>
        {
            x.Setup(sprites[index], graySprites[index]);
            index++;
        });

        levelButtons.ForEach(x =>
        {
            x.Load();
        });

        Filter("Fruits");
    }

    public void Filter(TMP_Text filter)
    {
        Filter(filter.text);
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
    }
}
