using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class PaintManager : MonoBehaviour
{
    [Header("Tilemap")]
    [SerializeField] private Tilemap baseTilemap;
    [SerializeField] private Tilemap numberTilemap;
    [SerializeField] private Tilemap backTilemap;
    [SerializeField] private Tilemap testTilemap;

    [Header("Tiles")]
    [SerializeField] private TileBase baseTile;
    [SerializeField] private TileBase backTile;
    [SerializeField] private List<TileBase> numberTiles;
    [SerializeField] private Sprite completeSprite;

    [Header("References")]
    [SerializeField] private GameObject colorButtonsHolder;
    [SerializeField] private GameObject colorButtonPrefab;
    [SerializeField] private HistoryManager historyManager;
    [SerializeField] private CameraController cameraController;

    [Header("Other")]
    [SerializeField] private Sprite sprite;

    private bool finished = false;
    private Color32 selectedColor;
    private Camera mainCamera;
    private CustomColorList colorList = new CustomColorList();
    private Slider selectedSlider;

    private readonly Color32 darkColor = new(53, 55, 56, 255);
    private readonly Color32 lightColor = new(220, 224, 210, 255);

    private void Awake()
    {
        mainCamera = Camera.main;

        if (DataHolder.sprite != null)
        {
            sprite = DataHolder.sprite;
        }
    }

    private void Start()
    {
        MatchingGrayTones();
        GenerateImage();
        CenterCameraOnImage();
        SpawnButtons();
        FillCount();
    }

    private void Update()
    {
        if (finished) { return; } 

        if (Input.GetKeyDown(KeyCode.F))
        {
            DownloadPNG();
        }

        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3Int pos = baseTilemap.WorldToCell(mainCamera.ScreenToWorldPoint(Input.mousePosition));

            if (numberTilemap.GetTile(pos) != null)
            {
                int index = colorList.GetTile(selectedColor).id;
                if (numberTilemap.GetTile(pos).name == $"numbers_{index}")
                {
                    baseTilemap.SetTile(pos, baseTile);
                    baseTilemap.SetColor(pos, selectedColor);
                    numberTilemap.SetTile(pos, null);
                    backTilemap.SetTile(pos, backTile);

                    colorList.GetTile(index).count--;

                    if (colorList.GetTile(index).count <= 0)
                    {
                        colorButtonsHolder.transform.GetChild(index).GetChild(2).GetComponent<Image>().sprite = completeSprite;
                    }

                    historyManager.Push(pos, selectedColor);
                }
                else
                {
                    backTilemap.SetTile(pos, baseTile);
                    backTilemap.SetColor(pos, selectedColor);
                    numberTilemap.SetColor(pos, IsColorDark(selectedColor) ? darkColor : lightColor);
                }
            }
        }

        selectedSlider.value = colorList.GetTile(selectedColor).maxCount - colorList.GetTile(selectedColor).count;

        if (IsTilemapEmpty(numberTilemap))
        {
            finished = true;
            DataHolder.colored[DataHolder.index] = 1;
            baseTilemap.ClearAllTiles();

            StartTimelapse();
        }
    }

    private void MatchingGrayTones()
    {
        int val = 255;
        int length = GetColorsCount();
        int step = val / (int)(length * 1.5f);
        for (int i = 0; i < length; i++)
        {
            colorList.AddTile(new CustomColor(i));
            colorList.GetTile(i).grayColor = new Color32((byte)val, (byte)val, (byte)val, 255);
            val -= step;
        }
    }

    private int GetColorsCount()
    {
        Texture2D texture = sprite.texture;
        Color[] pixels = texture.GetPixels();

        HashSet<Color> uniqueColors = new();

        foreach (Color pixelColor in pixels)
        {
            if (pixelColor.a > 0f)
            {
                uniqueColors.Add(pixelColor);
            }
        }

        return uniqueColors.Count;
    }

    private void GenerateImage()
    {
        Texture2D texture = sprite.texture;
        Rect rect = sprite.rect;

        int width = (int)rect.width;
        int height = (int)rect.height;

        for (int y = -height; y < height * 2; y++)
        {
            for (int x = -width * 2; x < width * 3; x++)
            {
                Vector3Int pos = new(x, y, 0);
                backTilemap.SetTile(pos, backTile);
            }
        }

        for (int y = 0; y < rect.height; y++)
        {
            for (int x = 0; x < rect.width; x++)
            {
                int texX = (int)rect.x + x;
                int texY = (int)rect.y + y;

                Color32 newColor = texture.GetPixel(texX, texY);

                if (newColor.a < 255) { continue; }

                bool exist = colorList.ContainsColor(newColor);
                if (!exist)
                {
                    colorList.FirstWithoutColor().color = newColor;
                }
                int index = colorList.GetTile(newColor).id;

                Color32 grayColor = colorList.GetTile(index).grayColor;
                Vector3Int pos = new(x, y, 0);

                backTilemap.SetTile(pos, baseTile); 
                backTilemap.SetColor(pos, grayColor);

                numberTilemap.SetTile(pos, numberTiles[index]);
                numberTilemap.SetColor(pos, IsColorDark(grayColor) ? darkColor : lightColor);
            }
        }

        float maxZoom = width - width * .25f;
        cameraController.SetBorders(0, width, maxZoom);
        cameraController.SetZoom();
    }

    private void SpawnButtons()
    {
        RectTransform rectTransform = colorButtonsHolder.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(colorList.Count * 160, rectTransform.sizeDelta.y); 

        for (int i = 0; i < colorList.Count; i++)
        {
            GameObject button = Instantiate(colorButtonPrefab, colorButtonsHolder.transform);
            Image colorComponent = button.transform.GetChild(0).GetComponent<Image>();
            colorComponent.color = colorList.GetTile(i).color;
            Image countComponent = button.transform.GetChild(2).GetComponent<Image>();
            TileBase foundTile = numberTiles.Find(x => x.name.Contains(i.ToString()));

            if (foundTile is Tile tile)
            {
                countComponent.sprite = tile.sprite;
            }
            countComponent.color = IsColorDark(colorList.GetTile(i).color) ? darkColor : lightColor;

            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (colorList.GetTile(colorComponent.color).count > 0)
                { 
                    selectedColor = colorComponent.color;
                    HighlightSelectedNumber();
                }
            });
        }

        selectedColor = colorList.GetTile(0).color;
        HighlightSelectedNumber();
    }

    private void HighlightSelectedNumber()
    {
        for (int i = 0; i < colorButtonsHolder.transform.childCount; i++)
        {
            Transform child = colorButtonsHolder.transform.GetChild(i);
            Slider slider = child.GetChild(1).GetComponent<Slider>();
            RectTransform rt = child.GetChild(2).GetComponent<Image>().rectTransform;

            if (i != colorList.GetTile(selectedColor).id)
            {
                rt.offsetMin = new Vector2(16, 16);
                rt.offsetMax = new Vector2(-16, -16);
                slider.gameObject.SetActive(false);
            }
            else
            {
                selectedSlider = slider;
                rt.offsetMin = new Vector2(4, 4);
                rt.offsetMax = new Vector2(-4, -4);
                slider.gameObject.SetActive(true);
            }
        }

        int number = colorList.GetTile(selectedColor).id;

        BoundsInt bounds = numberTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = numberTilemap.GetTile(pos);

                if (tile != null)
                {
                    string[] parts = tile.name.Split('_');

                    if (int.TryParse(parts[1], out int tileNumber))
                    {
                        Color32 newColor = tileNumber == number ? Color.black : colorList.GetTile(tileNumber).grayColor;
                        backTilemap.SetColor(pos, newColor);
                        numberTilemap.SetColor(pos, IsColorDark(newColor) ? darkColor : lightColor);

                        selectedSlider.maxValue = colorList.GetTile(selectedColor).maxCount;
                        selectedSlider.gameObject.transform.GetChild(0).GetComponent<Image>().color = IsColorDark(selectedColor) ? darkColor : lightColor;
                        selectedSlider.gameObject.transform.GetChild(1).GetComponentInChildren<Image>().color = selectedColor;
                    }    
                }
            }
        }
    }

    private void FillCount()
    {
        BoundsInt bounds = numberTilemap.cellBounds;

        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tilePos = new Vector3Int(x, y, 0);
                TileBase tile = numberTilemap.GetTile(tilePos);
                if (tile != null)
                {
                    string[] parts = tile.name.Split('_');

                    if (int.TryParse(parts[1], out int tileNumber))
                    {
                        colorList.GetTile(tileNumber).AddCount();
                    }
                }
            }
        }
    }

    private void StartTimelapse()
    {
        colorButtonsHolder.gameObject.SetActive(false);
        CenterCameraOnImage();
        StartCoroutine(TimelapseCoroutine());
    }

    IEnumerator TimelapseCoroutine()
    {
        float totalTime = historyManager.Count / 110f;
        float startTime = Time.time;

        while (historyManager.Count > 0)
        {
            float elapsedTime = Time.time - startTime;
            float remainingTime = totalTime - elapsedTime;

            float interval = remainingTime / Mathf.Max(1, historyManager.Count);
            yield return new WaitForSeconds(interval);

            var kvp = historyManager.Pop();
            baseTilemap.SetTile(kvp.Key, baseTile);
            baseTilemap.SetColor(kvp.Key, kvp.Value);
        }
    }

    void CenterCameraOnImage()
    {
        Vector2 spriteSize = sprite.rect.size;
        mainCamera.transform.position = new Vector3(spriteSize.x / 2, spriteSize.y / 2, mainCamera.transform.position.z);
        cameraController.SetZoom();
    }

    bool IsColorDark(Color32 color)
    {
        if (color.r + color.g + color.b >= 382.5f)
        {
            return true;
        }

        return false;
    }

    bool IsTilemapEmpty(Tilemap tilemap)
    {
        tilemap.CompressBounds();
        TileBase[] tiles = tilemap.GetTilesBlock(tilemap.cellBounds);
        return tiles.All(t => t == null);
    }

    private void DownloadPNG()
    {
        BoundsInt bounds = testTilemap.cellBounds;
        int width = bounds.size.x;
        int height = bounds.size.y;

        Texture2D texture = new(width, height, TextureFormat.RGBA32, false);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                Vector3Int tilePos = new Vector3Int(bounds.x + x, bounds.y + y, 0);
                TileBase tile = testTilemap.GetTile(tilePos);

                if (tile == null || tile == backTile)
                {
                    texture.SetPixel(x, y, new Color(0, 0, 0, 0));
                    continue;
                }

                Color color = testTilemap.GetColor(tilePos);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        string path = Path.Combine(Application.dataPath+"/Images", $"{sprite.name}_gray.png");
        File.WriteAllBytes(path, bytes);
    }
}
