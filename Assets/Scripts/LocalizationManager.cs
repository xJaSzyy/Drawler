using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }

    private List<LocalizationElement> elements = new();
    private LocalizationLanguage language = LocalizationLanguage.ru;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        elements.Add(new LocalizationElement(0, "clothes", "одежда"));
        elements.Add(new LocalizationElement(1, "fruits", "фрукты"));
        elements.Add(new LocalizationElement(2, "minecraft", "майнкрафт"));
        elements.Add(new LocalizationElement(3, "space", "космос"));
        elements.Add(new LocalizationElement(4, "vegetables", "овощи"));
        elements.Add(new LocalizationElement(5, "weapons", "оружие"));
    }

    public string GetLocalization(int id)
    {
        LocalizationElement element = elements.First(x => x.id == id);

        if (language == LocalizationLanguage.en)
        {
            return element.en_name;
        }
        else if (language == LocalizationLanguage.ru)
        {
            return element.ru_name;
        }

        return string.Empty;
    }

    public int GetId(string en_name)
    {
        return elements.First(x => x.en_name == en_name).id;
    }

    public void SetLanguage(LocalizationLanguage newLanguage) 
    {
        language = newLanguage;
        var items = FindObjectsByType<Localizator>(FindObjectsSortMode.None);
        foreach (var item in items)
        {
            item.UpdateText();
        }
    }
}

public struct LocalizationElement
{
    public int id;
    public string en_name;
    public string ru_name;

    public LocalizationElement(int id, string en_name, string ru_name)
    {
        this.id = id;
        this.en_name = en_name;
        this.ru_name = ru_name;
    }
}

public enum LocalizationLanguage
{
    en = 0,
    ru = 1,
}
