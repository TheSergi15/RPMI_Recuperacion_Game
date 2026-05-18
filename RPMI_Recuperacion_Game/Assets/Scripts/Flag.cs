using UnityEngine;

public class Flag : MonoBehaviour
{
    public GameObject YouWinScreen;

    void Start()
    {
        YouWinScreen.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("¡Tocaste la meta! " + collision.name);

        if (collision.CompareTag("Player"))
        {
            YouWinScreen.SetActive(true);
            Time.timeScale = 0;
        }
    }
}