using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Fusion;

public class SceneTransitionManager : MonoBehaviour
{
    // Singleton instance for easy access
    public static SceneTransitionManager Instance { get; private set; }

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 1.0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep this object between scene loads
    }

    /// <summary>
    /// Loads a scene with optional fade transition and Photon Fusion cleanup.
    /// </summary>
    public void LoadScene(string sceneName, bool isNetworked = false)
    {
        StartCoroutine(TransitionScene(sceneName, isNetworked));
    }

    private IEnumerator TransitionScene(string sceneName, bool isNetworked)
    {
        // Fade out
        yield return StartCoroutine(Fade(1));

        // Clean up networked sessions if required
        if (isNetworked)
        {
            var networkRunner = FindObjectOfType<NetworkRunner>();
            if (networkRunner != null && networkRunner.IsRunning)
            {
                yield return networkRunner.Shutdown();
            }
        }

        // Use UnityEngine.SceneManagement.SceneManager explicitly
        yield return UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);

        // Fade in
        yield return StartCoroutine(Fade(0));
    }

    /// <summary>
    /// Fades the screen in or out.
    /// </summary>
    private IEnumerator Fade(float targetAlpha)
    {
        if (fadeCanvasGroup == null)
        {
            Debug.LogWarning("Fade CanvasGroup is not assigned!");
            yield break;
        }

        float startAlpha = fadeCanvasGroup.alpha;
        float elapsedTime = 0;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha;
    }
}
