using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    public static LoadingManager Instance;

    [Header("UI da Tela de Carregamento")]
    public GameObject loadingScreen;
    public Slider progressBar;

    void Start()
    {
        if (loadingScreen != null) 
        {
            loadingScreen.SetActive(false);
        }
    }

    void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadAsynchronously(sceneName));
    }

    IEnumerator LoadAsynchronously(string sceneName)
    {
        loadingScreen.SetActive(true);

        yield return null;

        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null) progressBar.value = progress;
            yield return null;
        }

        yield return new WaitForSeconds(0.5f); 
        loadingScreen.SetActive(false);
    }
}