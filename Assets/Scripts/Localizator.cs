using TMPro;
using UnityEngine;

public class Localizator : MonoBehaviour
{
    [SerializeField] private int id;

    private TMP_Text text;

    private void Awake()
    {
        text = GetComponent<TMP_Text>();

        string result = string.Empty;
        while (result == "not found" || result == string.Empty)
        {
            result = LocalizationManager.Instance.GetLocalization(id);
        }
        text.text = result;
    }
}
