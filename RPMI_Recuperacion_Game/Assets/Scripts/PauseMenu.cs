using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    public GameObject pauseMenuPanel;
    public CanvasGroup canvasGroup;      // Para el fade
    public RectTransform panelRect;      // Para el scale

    [Header("Animación")]
    public float animationDuration = 0.3f;

    private bool isPaused = false;
    private bool isAnimating = false;

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && !isAnimating)
        {
            if (isPaused)
                StartCoroutine(AnimateOut());
            else
                StartCoroutine(AnimateIn());
        }
    }

    private IEnumerator AnimateIn()
    {
        isPaused = true;
        isAnimating = true;
        Time.timeScale = 0f;

        pauseMenuPanel.SetActive(true);
        canvasGroup.alpha = 0f;
        panelRect.localScale = Vector3.one * 0.8f;

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            // Usar unscaledDeltaTime porque el tiempo está pausado
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animationDuration;
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            canvasGroup.alpha = smooth;
            panelRect.localScale = Vector3.Lerp(Vector3.one * 0.8f, Vector3.one, smooth);

            yield return null;
        }

        canvasGroup.alpha = 1f;
        panelRect.localScale = Vector3.one;
        isAnimating = false;
    }

    private IEnumerator AnimateOut()
    {
        isAnimating = true;

        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animationDuration;
            float smooth = Mathf.SmoothStep(0f, 1f, t);

            canvasGroup.alpha = 1f - smooth;
            panelRect.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 0.8f, smooth);

            yield return null;
        }

        pauseMenuPanel.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        isAnimating = false;
    }

    public void Resume()
    {
        if (!isAnimating)
            StartCoroutine(AnimateOut());
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }
}