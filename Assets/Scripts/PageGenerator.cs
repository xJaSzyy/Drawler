using System.Collections.Generic;
using UnityEngine;

public class PageGenerator : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int countPerPage;
    [SerializeField] private List<Sprite> sprites = new();

    [Header("References")]
    [SerializeField] private GameObject pageHolder;
    [SerializeField] private GameObject pagePrefab;
    [SerializeField] private SwipeController swipeController;

    private List<LevelButton> levelButtons = new();

    private void Awake()
    {
        int spritesCount = sprites.Count;
        int pageCount = spritesCount / countPerPage;

        //other settings
        DataHolder.colored = new int[spritesCount];
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
            x.Setup(sprites[index], sprites[index]);
            index++;
        });

        levelButtons.ForEach(x =>
        {
            x.Load();
        });
    }
}
