using UnityEngine;
public class Flag : MonoBehaviour
{
    public GameObject YouWinScreen;
    public int thisLevelNumber;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LevelManager.FinishLevel(thisLevelNumber);
            Time.timeScale = 0;
            YouWinScreen.SetActive(true); // ✅ Cambiado winUI → YouWinScreen
        }
    }
}