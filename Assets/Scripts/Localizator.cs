using TMPro;
using UnityEngine;

public class Localizator : MonoBehaviour
{
    [SerializeField] private int id = -1;

    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();

        if (id != -1) 
        {
            UpdateText();
        }
    }

    public void SetId(int newId) 
    {
        id = newId;
        UpdateText();
    }

    public void UpdateText() 
    {
        string result = string.Empty;
        while (result == string.Empty)
        {
            result = LocalizationManager.Instance.GetLocalization(id);
        }
        text.text = result;
    }
}
