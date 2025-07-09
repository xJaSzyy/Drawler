using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.WSA;
using static UnityEditor.Progress;

public class PaintManager : MonoBehaviour
{
    [Header("Tiles and Sprites")]
    [SerializeField] private Tilemap backTilemap;
    [SerializeField] private TileBase backDarkTile;
    [SerializeField] private TileBase backLightTile;
    [SerializeField] private List<Sprite> numberSprites;
    [SerializeField] private Sprite completeSprite;

    [Header("References")]
    [SerializeField] private GameObject colorButtonsHolder;
    [SerializeField] private GameObject colorButtonPrefab;
    [SerializeField] private HistoryManager historyManager;
    [SerializeField] private CameraController cameraController;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip coloringSound;
    [SerializeField] private AudioClip finishSound;
    [SerializeField] private GameObject checkMark;
    [SerializeField] private GameObject plusButton;
    [SerializeField] private GameObject minusButton;
    [SerializeField] private List<GameObject> pixels;

    [Header("Other")]
    [SerializeField] private Sprite sprite;
    [SerializeField] private float volumeScale;
    [SerializeField] private float audioInterval = 0.04f;

    public bool finished = false;
    private Color32 selectedColor;
    private Camera mainCamera;
    private CustomColorList colorList = new();
    private Slider selectedSlider;
    private float lastPlayTime = 0f;
    private TileBase backTile;

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
        GenerateBackground();
        GenerateImage();
        SpawnButtons();
        FillCount();
        SelectFirstColor();
    }

    private void MatchingGrayTones()
    {
        int val = 255;
        int length = GetColorsCount();
        int step = val / (int)(length * 1.75f);
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
        Color[] pixels = texture.GetPixels(
            (int)sprite.rect.x,
            (int)sprite.rect.y,
            (int)sprite.rect.width,
            (int)sprite.rect.height
        );

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

    private void GenerateBackground()
    {
        Texture2D texture = sprite.texture;
        Rect rect = sprite.rect;

        List<bool> darks = new List<bool>();
        for (int y = 0; y < rect.height; y++)
        {
            for (int x = 0; x < rect.width; x++)
            {
                int texX = (int)rect.x + x;
                int texY = (int)rect.y + y;

                Color32 color = texture.GetPixel(texX, texY);

                if (color.a < 255) { continue; }

                darks.Add(IsColorDark(color));
            }
        }

        int darkCount = darks.Count(b => b == true);
        int lightCount = darks.Count(b => b == false);

        if (darkCount > lightCount)
        {
            backTile = backDarkTile;
            DataHolder.theme = "dark";
        }
        else
        {
            backTile = backLightTile;
            DataHolder.theme = "light";
        }

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

        var items = FindObjectsByType<ThemeChanger>(FindObjectsSortMode.None);
        foreach (var item in items)
        {
            item.UpdateUI();
        }
    }

    private void GenerateImage()
    {
        Texture2D texture = sprite.texture;
        Rect rect = sprite.rect;
        int width = (int)rect.width;

        List<GameObject> pixelsToHide = new();

        for (int y = 0; y < rect.height; y++)
        {
            int flippedY = (int)rect.height - 1 - y;
            for (int x = 0; x < rect.width; x++)
            {
                int texX = (int)rect.x + x;
                int texY = (int)rect.y + flippedY;

                Color32 newColor = texture.GetPixel(texX, texY);

                int indexInPixels = y * width + x;

                if (newColor.a < 255)
                {
                    pixelsToHide.Add(pixels[indexInPixels]);

                    continue;
                }

                bool exist = colorList.ContainsColor(newColor);
                if (!exist)
                {
                    colorList.FirstWithoutColor().color = newColor;
                }
                int index = colorList.GetTile(newColor).id;

                Color32 grayColor = colorList.GetTile(index).grayColor;

                pixels[indexInPixels].GetComponent<Image>().color = grayColor;
                pixels[indexInPixels].transform.GetChild(0).GetComponent<Image>().sprite = numberSprites[index];
                pixels[indexInPixels].transform.GetChild(0).GetComponent<Image>().color = IsColorDark(grayColor) ? darkColor : lightColor;
                colorList.GetTile(index).pixel = pixels[indexInPixels];
            }
        }

        foreach (var item in pixelsToHide)
        {
            item.GetComponent<Image>().color = new Color(0, 0, 0, 0);
            item.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
            item.GetComponent<Pixel>().enabled = false;
        }
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

            countComponent.sprite = numberSprites.Find(x => x.name.Contains(i.ToString()));
            countComponent.color = IsColorDark(colorList.GetTile(i).color) ? darkColor : lightColor;

            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (colorList.GetTile(colorComponent.color).count > 0)
                {
                    selectedColor = colorComponent.color;
                    HighlightSelectedNumber();
                }
            });
            button.GetComponent<ButtonAnimation>().color = IsColorDark(colorList.GetTile(i).color) ? Color.black : Color.white;
        }
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

        foreach (var item in pixels)
        {
            if (item.GetComponent<Pixel>().enabled == false) { continue; }

            var number = GetNumber(item.transform.GetChild(0).GetComponent<Image>().sprite.name);
            if (number != -1)
            {
                Color32 newColor = colorList.GetTile(selectedColor).id == number ? Color.black : colorList.GetTile(number).grayColor;
                item.GetComponent<Image>().color = newColor;
                item.transform.GetChild(0).GetComponent<Image>().color = IsColorDark(newColor) ? darkColor : lightColor;

                selectedSlider.maxValue = colorList.GetTile(selectedColor).maxCount;
                selectedSlider.gameObject.transform.GetChild(0).GetComponent<Image>().color = IsColorDark(selectedColor) ? darkColor : lightColor;
                selectedSlider.gameObject.transform.GetChild(1).GetComponentInChildren<Image>().color = selectedColor;
            }
        }
    }

    private void FillCount()
    {
        foreach (var item in pixels)
        {
            if (item.GetComponent<Image>().color == new Color(0, 0, 0, 0)) { continue; }

            var number = GetNumber(item.transform.GetChild(0).GetComponent<Image>().sprite.name);
            if (number != -1)
            {
                colorList.GetTile(number).AddCount();
            }
        }
    }

    public void Paint(GameObject pixel)
    {
        int index = colorList.GetTile(selectedColor).id;

        var image = pixel.GetComponent<Image>();
        var countImage = pixel.transform.GetChild(0).GetComponent<Image>();
        var number = GetNumber(countImage.sprite.name);

        if (number == index)
        {
            image.color = selectedColor;
            countImage.enabled = false;

            PlaySound(coloringSound, volumeScale);
            colorList.GetTile(index).count--;
            pixel.GetComponent<Pixel>().enabled = false;

            if (colorList.GetTile(index).count <= 0)
            {
                Transform target = colorButtonsHolder.transform.GetChild(index).GetChild(2);
                Image img = target.GetComponent<Image>();
                img.sprite = completeSprite;
                img.transform.localScale = Vector3.one;

                LeanTween.cancel(img.gameObject);
                LeanTween.scale(colorButtonsHolder.transform.GetChild(index).gameObject, Vector3.one * .8f, 1.2f).setEasePunch();
            }

            historyManager.Push(pixel, selectedColor);
        }
        else
        {
            image.color = selectedColor;
            countImage.color = IsColorDark(selectedColor) ? darkColor : lightColor;
        }

        selectedSlider.value = colorList.GetTile(selectedColor).maxCount - colorList.GetTile(selectedColor).count;
        if (selectedSlider.value == selectedSlider.maxValue)
        {
            selectedSlider.gameObject.SetActive(false);
        }

        FinishCheck();
    }

    private int GetNumber(string name)
    {
        string[] parts = name.Split('_');

        if (int.TryParse(parts[1], out int number))
        {
            return number;
        }

        return -1;
    }

    private void FinishCheck()
    {
        bool isFinished = true;
        for (int i = 0; i < colorList.Count; i++)
        {
            if (colorList.GetTile(i).count > 0)
            {
                isFinished = false;
            }
        }

        if (isFinished)
        {
            finished = true;
            DataHolder.colored[DataHolder.index] = 1;
            cameraController.Lock();

            pixels.ForEach(pixel =>
            {
                pixel.GetComponent<Image>().color = new Color(0, 0, 0, 0);
                pixel.transform.GetChild(0).GetComponent<Image>().color = new Color(0, 0, 0, 0);
            });

            StartTimelapse();
        }
    }

    private void SelectFirstColor()
    {
        selectedColor = colorList.GetTile(0).color;
        HighlightSelectedNumber();
    }

    private void StartTimelapse()
    {
        colorButtonsHolder.SetActive(false);
        plusButton.SetActive(false);
        minusButton.SetActive(false);
        cameraController.ResetImage();
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
            kvp.Key.GetComponent<Image>().color = kvp.Value;
        }

        PlaySound(finishSound, volumeScale);
        checkMark.transform.localScale = Vector3.zero;
        checkMark.SetActive(true);
        LeanTween.scale(checkMark, Vector3.one, 0.5f).setEase(LeanTweenType.easeOutBack);
    }

    bool IsColorDark(Color32 color)
    {
        if (color.r + color.g + color.b >= 382.5f)
        {
            return true;
        }

        return false;
    }

    void PlaySound(AudioClip clip, float volume)
    {
        if (Time.time - lastPlayTime > audioInterval)
        {
            audioSource.PlayOneShot(clip, volume);
            lastPlayTime = Time.time;
        }
    }
}
