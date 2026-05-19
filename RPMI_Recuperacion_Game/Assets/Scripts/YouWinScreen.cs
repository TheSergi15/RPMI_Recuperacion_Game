using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class YouWinScreen : MonoBehaviour
{
    [Header("Elementos del Canvas")]
    public Image panelFondo;
    public TextMeshProUGUI youWinText;
    public Button restartButton;

    [Header("Sonido")]
    public AudioClip victoriaSound;        // Arrastra tu archivo de audio aquí
    private AudioSource audioSource;

    [Header("Colores")]
    public Color colorFinal = new Color(0.18f, 0.8f, 0.44f, 1f);  // Verde por defecto
    public Color colorInicial = Color.white;

    [Header("Duración")]
    public float duracion = 0.8f;

    void OnEnable()
    {
        // Resetear estado inicial
        panelFondo.color = new Color(0, 0, 0, 0);
        youWinText.transform.localScale = Vector3.zero;
        youWinText.color = colorInicial;
        restartButton.transform.localScale = Vector3.zero;
        restartButton.interactable = false;

        // Configurar AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        // Reproducir sonido de victoria
        if (victoriaSound != null)
            audioSource.PlayOneShot(victoriaSound);
        else
            Debug.LogWarning("⚠️ No hay AudioClip asignado en victoriaSound");

        StartCoroutine(AnimarTodo());
    }

    IEnumerator AnimarTodo()
    {
        float elapsed = 0f;

        while (elapsed < duracion)
        {
            elapsed += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(elapsed / duracion);
            float eased = 1f - Mathf.Pow(1f - progress, 3f); // Ease out cúbico

            // Fondo negro: fade in
            panelFondo.color = new Color(0, 0, 0, eased);

            // Texto YOU WIN: scale up + cambio de color (blanco → verde)
            youWinText.transform.localScale = Vector3.one * eased;
            youWinText.color = Color.Lerp(colorInicial, colorFinal, eased);

            // Botón Restart: scale up con rebote
            float bounceScale = progress < 0.8f
                ? Mathf.Lerp(0f, 1.1f, progress / 0.8f)
                : Mathf.Lerp(1.1f, 1f, (progress - 0.8f) / 0.2f);
            restartButton.transform.localScale = Vector3.one * bounceScale;

            yield return null;
        }

        // Estado final
        panelFondo.color = new Color(0, 0, 0, 1f);
        youWinText.transform.localScale = Vector3.one;
        youWinText.color = colorFinal;
        restartButton.transform.localScale = Vector3.one;
        restartButton.interactable = true;
    }

    public void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}