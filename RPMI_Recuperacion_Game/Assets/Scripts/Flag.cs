using UnityEngine;
public class Flag : MonoBehaviour
{
    public GameObject YouWinScreen;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Time.timeScale = 0;
            YouWinScreen.SetActive(true); // ✅ Cambiado winUI → YouWinScreen
        }
    }
}