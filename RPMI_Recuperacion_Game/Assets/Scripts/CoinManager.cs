using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;

    [Header("HUD")]
    public TextMeshProUGUI coinText;

    
    

    private int totalCoins = 0;

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
       
        UpdateHUD();
    }

    // ✅ Llamado por cada moneda al recogerla
    public void AddCoins(int value)
    {
        totalCoins += value;
        UpdateHUD();

    

        Debug.Log(" Monedas: " + totalCoins);
    }

    void UpdateHUD()
    {
        if (coinText != null)
            coinText.text = totalCoins.ToString();
    }

    public int GetTotalCoins()
    {
        return totalCoins;
    }
}
