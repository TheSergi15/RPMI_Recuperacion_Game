using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Canvas youWinCanvas;
    private bool hasWon = false;

    void Awake()
    {
        // Singleton
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        // Asegúrate de que el Canvas esté desactivado al inicio
        if (youWinCanvas != null)
            youWinCanvas.gameObject.SetActive(false);
    }

    // Llamar esto cuando el jugador gane
    public void PlayerWins()
    {
        if (hasWon) return;  // Evita llamarlo múltiples veces

        hasWon = true;
        Debug.Log("¡GANASTE!");

        // Desactiva controles del jugador
        Time.timeScale = 0f;  // Pausa el juego

        // Activa la pantalla de victoria
        if (youWinCanvas != null)
            youWinCanvas.gameObject.SetActive(true);
    }

    // El botón Restart llama esto
    public void RestartGame()
    {
        Time.timeScale = 1f;  // Reanuda el tiempo
        SceneManager.LoadScene("GameScene");
    }
}