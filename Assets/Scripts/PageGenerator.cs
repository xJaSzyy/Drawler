using System.Collections.Generic;
using UnityEngine;

public class PageGenerator : MonoBehaviour
{
    [SerializeField] private int countPerPage;
    [SerializeField] private GameObject pageHolder;
    [SerializeField] private GameObject pagePrefab;
    [SerializeField] private List<Sprite> sprites = new();

    [SerializeField] private List<LevelButton> levelButtons = new();

    private void Awake()
    {
        DataHolder.colored = new int[sprites.Count];

        int pageCount = sprites.Count / countPerPage;

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
