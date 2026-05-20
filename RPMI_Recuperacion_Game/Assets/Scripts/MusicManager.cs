using UnityEngine;
using UnityEngine.UI;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Música por escena")]
    public AudioClip menuMusic;      // Música del menú principal
    public AudioClip gameMusic;      // Música del juego
    public AudioClip bossMusic;      // Música de jefe (opcional)

    [Header("Volumen inicial")]
    [Range(0f, 1f)] public float musicVolume = 0.5f;
    [Range(0f, 1f)] public float sfxVolume = 1f;

    void Awake()
    {
        // ✅ Singleton: solo existe un MusicManager en toda la partida
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);  // ✅ No se destruye al cambiar de escena
        }
        else
        {
            Destroy(gameObject);  // Si ya existe uno, destruye el duplicado
            return;
        }

        // Cargar volumen guardado (si existe)
        musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.5f);
        sfxVolume = PlayerPrefs.GetFloat("SfxVolume", 1f);

        // Aplicar volumen
        if (musicSource != null) musicSource.volume = musicVolume;
        if (sfxSource != null) sfxSource.volume = sfxVolume;
    }

    void Start()
    {
        // Reproducir música según la escena actual
        PlayMusicForCurrentScene();
    }

    // ==================== MÚSICA POR ESCENA ====================

    public void PlayMusicForCurrentScene()
    {
        string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        switch (sceneName)
        {
            case "MainMenu":
                PlayMusic(menuMusic);
                break;
            case "GameScene":
                PlayMusic(gameMusic);
                break;
            default:
                PlayMusic(gameMusic);  // Por defecto usa la música del juego
                break;
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null || musicSource == null) return;

        // Evita reiniciar si ya está sonando la misma canción
        if (musicSource.clip == clip && musicSource.isPlaying) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    // ==================== EFECTOS DE SONIDO ====================

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip);
    }

    // ==================== VOLUMEN ====================

    // ✅ Llama esto desde el slider de música del PauseMenu
    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        if (musicSource != null)
            musicSource.volume = value;

        // Guardar preferencia
        PlayerPrefs.SetFloat("MusicVolume", value);
        PlayerPrefs.Save();
    }

    // ✅ Llama esto desde el slider de sfx del PauseMenu
    public void SetSfxVolume(float value)
    {
        sfxVolume = value;
        if (sfxSource != null)
            sfxSource.volume = value;

        // Guardar preferencia
        PlayerPrefs.SetFloat("SfxVolume", value);
        PlayerPrefs.Save();
    }

    // ==================== UTILIDADES ====================

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    public void PauseMusic()
    {
        if (musicSource != null)
            musicSource.Pause();
    }

    public void ResumeMusic()
    {
        if (musicSource != null)
            musicSource.UnPause();
    }
    void OnEnable()
    {
        // ✅ Suscribirse al evento de cambio de escena
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // ✅ Desuscribirse para evitar errores
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ✅ Se llama automáticamente cada vez que carga una escena nueva
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        PlayMusicForCurrentScene();
    }
}
