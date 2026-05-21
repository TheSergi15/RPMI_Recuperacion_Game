using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    [Header("Título")]
    public TextMeshProUGUI titleText;

    [Header("Botones principales")]
    public Button playButton;
    public Button optionsButton;
    public Button creditsButton;
    public Button exitButton;

    [Header("Paneles")]
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    [Header("Botones de retorno")]
    public Button backOptionsButton;
    public Button backCreditsButton;

    [Header("Opciones - Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    [Header("Audio")]
    
    public AudioClip sonidoBoton;
    public AudioClip sonidoPlay;
    
   

    [Header("Colores")]
    public Color colorTextoNormal = Color.white;
    public Color colorTextoHover = Color.yellow;
    public Color[] coloresTitulo = new Color[]
    {
        new Color(1f, 0.2f, 0.4f),
        new Color(1f, 0.6f, 0f),
        new Color(1f, 0.9f, 0f),
        new Color(0.2f, 0.9f, 0.4f),
        new Color(0.2f, 0.6f, 1f),
        new Color(0.8f, 0.3f, 1f),
    };

    [Header("Configuración")]
    public string gameSceneName = "LevelSelector";
    public float duracionEntrada = 1f;
    public float delayEntrebotones = 0.15f;
    public float velocidadColorTitulo = 1.5f;

    private bool optionsOpen = false;
    private bool creditsOpen = false;

    // ✅ Guardamos los scales originales de cada elemento
    private Vector3 titleScale;
    private Vector3 playScale;
    private Vector3 optionsScale;
    private Vector3 creditsScale;
    private Vector3 exitScale;

    void Start()
    {
       

        // ✅ Guardar scales originales ANTES de ponerlos a cero
        titleScale = titleText.transform.localScale;
        playScale = playButton.transform.localScale;
        optionsScale = optionsButton.transform.localScale;
        creditsScale = creditsButton.transform.localScale;
        exitScale = exitButton.transform.localScale;

        // Iniciar todo invisible
        titleText.transform.localScale = Vector3.zero;
        playButton.transform.localScale = Vector3.zero;
        optionsButton.transform.localScale = Vector3.zero;
        creditsButton.transform.localScale = Vector3.zero;
        exitButton.transform.localScale = Vector3.zero;

        // Resetear colores
        SetButtonTextColor(playButton, colorTextoNormal);
        SetButtonTextColor(optionsButton, colorTextoNormal);
        SetButtonTextColor(creditsButton, colorTextoNormal);
        SetButtonTextColor(exitButton, colorTextoNormal);

        // Paneles cerrados al inicio
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (creditsPanel != null) creditsPanel.SetActive(false);

        // Conectar botones de retorno
        if (backOptionsButton != null)
            backOptionsButton.onClick.AddListener(OnBackOptionsClicked);
        if (backCreditsButton != null)
            backCreditsButton.onClick.AddListener(OnBackCreditsClicked);

        // Sliders
        if (musicSlider != null)
        {
            musicSlider.value = 0.5f;
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }
        if (sfxSlider != null)
        {
            sfxSlider.value = 1f;
            sfxSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
        }

        // Animaciones
        StartCoroutine(AnimarEntrada());
        StartCoroutine(AnimarColorTitulo());
    }

    // ==================== ANIMACIONES ====================

    IEnumerator AnimarEntrada()
    {
        yield return StartCoroutine(ScaleUp(titleText.transform, duracionEntrada, titleScale));
        yield return StartCoroutine(ScaleUpConBounce(playButton.transform, playScale));
        yield return new WaitForSeconds(delayEntrebotones);
        yield return StartCoroutine(ScaleUpConBounce(optionsButton.transform, optionsScale));
        yield return new WaitForSeconds(delayEntrebotones);
        yield return StartCoroutine(ScaleUpConBounce(creditsButton.transform, creditsScale));
        yield return new WaitForSeconds(delayEntrebotones);
        yield return StartCoroutine(ScaleUpConBounce(exitButton.transform, exitScale));
    }

    IEnumerator ScaleUp(Transform target, float duracion, Vector3 scaleDestino)
    {
        float elapsed = 0f;
        while (elapsed < duracion)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duracion);
            float eased = 1f - Mathf.Pow(1f - progress, 3f);
            target.localScale = scaleDestino * eased;  // ✅ Anima hasta el scale original
            yield return null;
        }
        target.localScale = scaleDestino;  // ✅ Termina en el scale correcto
    }

    IEnumerator ScaleUpConBounce(Transform target, Vector3 scaleDestino)
    {
        float duracion = 0.4f;
        float elapsed = 0f;
        while (elapsed < duracion)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duracion);
            float scale = progress < 0.7f
                ? Mathf.Lerp(0f, 1.1f, progress / 0.7f)
                : Mathf.Lerp(1.1f, 1f, (progress - 0.7f) / 0.3f);
            target.localScale = scaleDestino * scale;  // ✅ Anima hasta el scale original
            yield return null;
        }
        target.localScale = scaleDestino;  // ✅ Termina en el scale correcto
    }

    IEnumerator AnimarColorTitulo()
    {
        int colorIndex = 0;
        while (true)
        {
            Color colorActual = coloresTitulo[colorIndex];
            Color colorSiguiente = coloresTitulo[(colorIndex + 1) % coloresTitulo.Length];
            float elapsed = 0f;
            while (elapsed < velocidadColorTitulo)
            {
                elapsed += Time.deltaTime;
                titleText.color = Color.Lerp(colorActual, colorSiguiente, elapsed / velocidadColorTitulo);
                yield return null;
            }
            colorIndex = (colorIndex + 1) % coloresTitulo.Length;
        }
    }

    // ==================== HOVER ====================

    public void OnButtonHoverEnter(Button boton)
    {
        PlaySFX(sonidoBoton);
        StartCoroutine(ScaleBoton(boton.transform, 1.15f));
        SetButtonTextColor(boton, colorTextoHover);
    }

    public void OnButtonHoverExit(Button boton)
    {
        StartCoroutine(ScaleBoton(boton.transform, 1f));
        SetButtonTextColor(boton, colorTextoNormal);
    }

    IEnumerator ScaleBoton(Transform boton, float targetScale)
    {
        float duracion = 0.1f;
        float elapsed = 0f;
        Vector3 inicio = boton.localScale;
        Vector3 final = Vector3.one * targetScale;
        while (elapsed < duracion)
        {
            elapsed += Time.deltaTime;
            boton.localScale = Vector3.Lerp(inicio, final, elapsed / duracion);
            yield return null;
        }
        boton.localScale = final;
    }

    void SetButtonTextColor(Button boton, Color color)
    {
        if (boton == null) return;
        TextMeshProUGUI texto = boton.GetComponentInChildren<TextMeshProUGUI>();
        if (texto != null)
            texto.color = color;
    }

    // ==================== BOTONES PRINCIPALES ====================

    public void OnPlayClicked()
    {
        PlaySFX(sonidoPlay);
        StartCoroutine(CargarEscena());
    }

    IEnumerator CargarEscena()
    {
        yield return new WaitForSeconds(0.3f);
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnOptionsClicked()
    {
        PlaySFX(sonidoBoton);
        optionsOpen = !optionsOpen;
        if (optionsPanel != null) optionsPanel.SetActive(optionsOpen);
        if (creditsOpen)
        {
            creditsOpen = false;
            if (creditsPanel != null) creditsPanel.SetActive(false);
        }
    }

    public void OnCreditsClicked()
    {
        PlaySFX(sonidoBoton);
        creditsOpen = !creditsOpen;
        if (creditsPanel != null) creditsPanel.SetActive(creditsOpen);
        if (optionsOpen)
        {
            optionsOpen = false;
            if (optionsPanel != null) optionsPanel.SetActive(false);
        }
    }

    public void OnExitClicked()
    {
        PlaySFX(sonidoBoton);
        StartCoroutine(Salir());
    }

    IEnumerator Salir()
    {
        yield return new WaitForSeconds(0.3f);
        Application.Quit();
        Debug.Log("Saliendo del juego...");
    }

    // ==================== BOTONES DE RETORNO ====================

    public void OnBackOptionsClicked()
    {
        PlaySFX(sonidoBoton);
        optionsOpen = false;
        if (optionsPanel != null) optionsPanel.SetActive(false);
    }

    public void OnBackCreditsClicked()
    {
        PlaySFX(sonidoBoton);
        creditsOpen = false;
        if (creditsPanel != null) creditsPanel.SetActive(false);
    }

    // ==================== AUDIO ====================

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

    void PlaySFX(AudioClip clip)
    {
        if (MusicManager.instance != null)
            MusicManager.instance.PlaySFX(clip);
    }
}
