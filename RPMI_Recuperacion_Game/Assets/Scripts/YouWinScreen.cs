using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class YouWinScreen : MonoBehaviour
{
    public CanvasGroup canvasGroup;  // Requiere CanvasGroup en el Canvas
    public TextMeshProUGUI youWinText;       // El texto "YOU WIN!"
    public Button restartButton;     // El botón Restart

    public float fadeInDuration = 0.8f;
    public float scaleUpDuration = 0.6f;
    public float buttonDelay = 1.2f;  // Cuándo aparece el botón
    public float buttonBounceDuration = 0.5f;

    void Start()
    {
        // Asegúrate de que todo empieza invisible/pequeño
        canvasGroup.alpha = 0f;
        youWinText.transform.localScale = Vector3.zero;
        restartButton.transform.localScale = Vector3.zero;
        restartButton.interactable = false;  // Desactiva interacción hasta que aparezca

        // Inicia la secuencia de animación
        StartCoroutine(AnimateWinScreen());
    }

    IEnumerator AnimateWinScreen()
    {
        // ✅ FASE 1: Fade in del fondo
        yield return StartCoroutine(FadeInBackground());

        // ✅ FASE 2: Scale up del texto "YOU WIN!"
        yield return StartCoroutine(ScaleUpText());

        // ✅ FASE 3: Bounce del botón Restart
        yield return new WaitForSeconds(buttonDelay);
        yield return StartCoroutine(BounceButton());
    }

    IEnumerator FadeInBackground()
    {
        float elapsed = 0f;
        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }
        canvasGroup.alpha = 1f;
    }

    IEnumerator ScaleUpText()
    {
        float elapsed = 0f;
        while (elapsed < scaleUpDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / scaleUpDuration;
            // Easing: ease-out para un efecto suave
            float easeProgress = 1f - Mathf.Pow(1f - progress, 3f);
            youWinText.transform.localScale = Vector3.one * easeProgress;
            yield return null;
        }
        youWinText.transform.localScale = Vector3.one;
    }

    IEnumerator BounceButton()
    {
        // Primero escala desde 0 a 1.1 (overshot)
        float elapsed = 0f;
        while (elapsed < buttonBounceDuration)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / buttonBounceDuration;
            // Bounce easing
            float scale = BounceEasing(progress);
            restartButton.transform.localScale = Vector3.one * scale;
            yield return null;
        }
        restartButton.transform.localScale = Vector3.one;

        // Ya es interactivo
        restartButton.interactable = true;
    }

    // ✅ Función easing para efecto de rebote
    private float BounceEasing(float t)
    {
        if (t < 0.5f)
        {
            // Primera mitad: sube con un poco de overshoot
            return Mathf.Lerp(0f, 1.15f, t * 2f);
        }
        else
        {
            // Segunda mitad: vuelve a 1
            return Mathf.Lerp(1.15f, 1f, (t - 0.5f) * 2f);
        }
    }

    // Llamar esto cuando presiones el botón Restart
    public void OnRestartClicked()
    {
        SceneManager.LoadScene("GameScene");  // Cambia "GameScene" por tu escena
    }
}
