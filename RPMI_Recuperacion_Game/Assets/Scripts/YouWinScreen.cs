using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class YouWinScreen : MonoBehaviour
{
    [Header("Elementos del Canvas")]
    public Image panelFondo;               // Panel negro de fondo
    public TextMeshProUGUI youWinText;     // Texto "YOU WIN!"
    public Button restartButton;           // Botón Restart

    [Header("Duración de animaciones")]
    public float duracion = 0.8f;          // Duración de todas las animaciones

    void OnEnable()
    {
        // Resetear todo al estado inicial cuando se activa el Canvas
        panelFondo.color = new Color(0, 0, 0, 0);
        youWinText.transform.localScale = Vector3.zero;
        restartButton.transform.localScale = Vector3.zero;
        restartButton.interactable = false;

        // Lanzar todas las animaciones a la vez
        StartCoroutine(AnimarTodo());
    }

    IEnumerator AnimarTodo()
    {
        float elapsed = 0f;

        while (elapsed < duracion)
        {
            elapsed += Time.unscaledDeltaTime; // ✅ unscaledDeltaTime para que funcione con TimeScale = 0
            float progress = Mathf.Clamp01(elapsed / duracion);
            float eased = 1f - Mathf.Pow(1f - progress, 3f); // Ease out cúbico

            // Fondo negro: fade in
            panelFondo.color = new Color(0, 0, 0, eased);

            // Texto YOU WIN: scale up
            youWinText.transform.localScale = Vector3.one * eased;

            // Botón Restart: scale up con pequeño rebote
            float bounceScale = progress < 0.8f
               ? Mathf.Lerp(0f, 1.1f, progress / 0.8f)
               : Mathf.Lerp(1.1f, 1f, (progress - 0.8f) / 0.2f);
            restartButton.transform.localScale = Vector3.one * bounceScale;

            yield return null;
        }

        // Asegurarse de que todo queda en estado final
        panelFondo.color = new Color(0, 0, 0, 1);
        youWinText.transform.localScale = Vector3.one;
        restartButton.transform.localScale = Vector3.one;
        restartButton.interactable = true; // Ya se puede pulsar el botón
    }

    // Asigna esto al onClick del botón Restart
    public void OnRestartClicked()
    {
        Time.timeScale = 1f;          // Reanuda el tiempo
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reinicia la escena actual
    }
}