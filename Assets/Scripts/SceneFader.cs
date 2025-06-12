using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public static SceneFader Instance { get; private set; }

    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1f;

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
        FadeIn();
    }

    public void FadeOutAndLoadScene(int sceneIndex)
    {
        LeanTween.alpha(fadeImage.rectTransform, 1f, fadeDuration).setOnComplete(() =>
        {
            StartCoroutine(LoadSceneAsync(sceneIndex));
        });
    }

    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
        asyncLoad.allowSceneActivation = false;

        while (!asyncLoad.isDone)
        {
            if (asyncLoad.progress >= 0.9f)
            {
                asyncLoad.allowSceneActivation = true;
            }
            yield return null;
        }

        FadeIn();
    }

    public void FadeIn()
    {
        LeanTween.alpha(fadeImage.rectTransform, 0f, fadeDuration);
    }
}
