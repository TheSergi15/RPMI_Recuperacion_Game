using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject pauseMenuPanel;
    public GameObject settingsPanel;

    [Header("Animación")]
    public CanvasGroup canvasGroup;
    public RectTransform panelRect;
    public float animationDuration = 0.3f;

    [Header("Ajustes - Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Sonido (arrastra el archivo directamente)")]
    public AudioClip sonidoBoton;      // ✅ Solo arrastra el .mp3/.wav aquí
    private AudioSource audioSource;   // ✅ Se crea automáticamente

    private bool isPaused = false;
    private bool isAnimating = false;
    private bool settingsOpen = false;

    void Start()
    {
        // ✅ Crea el AudioSource automáticamente, sin necesidad de GameObjects extra
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Paneles cerrados al inicio
        if (pauseMenuPanel != null) pauseMenuPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(false);

        // Sliders
        if (musicSlider != null)
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        if (sfxSlider != null)
            sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame && !isAnimating)
        {
            if (settingsOpen)
                CloseSettings();
            else if (isPaused)
                StartCoroutine(AnimateOut());
            else
                StartCoroutine(AnimateIn());
        }
    }

    // ==================== ANIMACIONES ====================

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
        if (settingsOpen) CloseSettings();

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

    // ==================== BOTONES ====================

    public void Resume()
    {
        PlaySFX();
        if (!isAnimating)
            StartCoroutine(AnimateOut());
    }

    public void OpenSettings()
    {
        PlaySFX();
        settingsOpen = true;
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        PlaySFX();
        settingsOpen = false;
        if (settingsPanel != null) settingsPanel.SetActive(false);
    }

    public void GoToMainMenu()
    {
        PlaySFX();
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartGame()
    {
        PlaySFX();
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        PlaySFX();
        Time.timeScale = 1f;
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }

    // ==================== AJUSTES ====================

    public void OnMusicVolumeChanged(float value)
    {
        if (MusicManager.instance != null)
            MusicManager.instance.SetMusicVolume(value);
    }

    public void OnSfxVolumeChanged(float value)
    {
        if (MusicManager.instance != null)
            MusicManager.instance.SetSfxVolume(value);
    }

    // ==================== AUDIO ====================

    void PlaySFX()
    {
        if (sonidoBoton != null)
            audioSource.PlayOneShot(sonidoBoton);
    }
}