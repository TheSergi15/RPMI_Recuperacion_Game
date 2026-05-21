using UnityEngine;

public class NextLevel : MonoBehaviour
{
    public string nextLevelName;
    public int thisLevelNumber; // ⬅️ Añade esto en el Inspector

    public void LoadCurrentScene()
    {
        LevelManager.FinishLevel(thisLevelNumber); // ⬅️ Guarda el nivel como completado
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevelName);
    }
}