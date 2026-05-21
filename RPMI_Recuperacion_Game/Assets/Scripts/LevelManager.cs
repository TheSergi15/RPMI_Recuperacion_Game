using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    [Header("Nodos del mapa (en orden)")]
    public LevelNode[] levelNodes;   // Arrastra los 5 nodos aquí en orden

    [Header("Botón de volver")]
    public Button backButton;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Conectar botón de volver
        if (backButton != null)
            backButton.onClick.AddListener(GoToMainMenu);

        // Actualizar estado de todos los nodos
        RefreshNodes();
    }

    public void RefreshNodes()
    {
        for (int i = 0; i < levelNodes.Length; i++)
        {
            bool unlocked = IsLevelUnlocked(i + 1);
            levelNodes[i].SetState(unlocked, i + 1);
        }
    }

    public bool IsLevelUnlocked(int levelNumber)
    {
        if (levelNumber == 1) return true;  // El primer nivel siempre desbloqueado
        return PlayerPrefs.GetInt("Level" + (levelNumber - 1) + "Completed", 0) == 1;
    }

    // ✅ Llama esto cuando el jugador completa un nivel
    public void CompleteLevel(int levelNumber)
    {
        PlayerPrefs.SetInt("Level" + levelNumber + "Completed", 1);
        PlayerPrefs.Save();
        Debug.Log("✅ Nivel " + levelNumber + " completado");
    }

    public void LoadLevel(int levelNumber)
    {
        SceneManager.LoadScene("Level" + levelNumber);  // Ej: "Level1", "Level2"...
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    // ✅ Llama esto desde tu script de victoria para desbloquear el siguiente nivel
    public static void FinishLevel(int levelNumber)
    {
        if (instance != null)
            instance.CompleteLevel(levelNumber);
        else
        {
            // Si no hay instancia (llamado desde GameScene directamente)
            PlayerPrefs.SetInt("Level" + levelNumber + "Completed", 1);
            PlayerPrefs.Save();
        }
    }
    [ContextMenu("Resetear el Progreso")]
    public void ResetProgress()
    {
        PlayerPrefs.DeleteAll();
        RefreshNodes();
        Debug.Log("Progreso reseteado");
    }
}