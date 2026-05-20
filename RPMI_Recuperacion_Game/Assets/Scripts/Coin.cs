using UnityEngine;

public class Coin : MonoBehaviour
{
    public enum CoinType { Bronze, Silver, Gold }

    [Header("Tipo de moneda")]
    public CoinType coinType = CoinType.Bronze;

    [Header("Animación")]
    public float rotateSpeed = 90f;
    public float bobSpeed = 2f;
    public float bobHeight = 0.2f;

    [Header("Sonido")]
    public AudioClip coinSound;          // ✅ Arrastra tu audio aquí
    [Range(0.1f, 5f)]
    public float soundDuration = 0.3f;

    private int coinValue;
    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        switch (coinType)
        {
            case CoinType.Bronze: coinValue = 1; break;
            case CoinType.Silver: coinValue = 5; break;
            case CoinType.Gold: coinValue = 10; break;
        }
    }

    void Update()
    {
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f);
        float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (CoinManager.instance != null)
                CoinManager.instance.AddCoins(coinValue);

            // ✅ Reproducir sonido antes de destruir
            PlayClipTrimmed(coinSound, transform.position, soundDuration);

            Destroy(gameObject);
        }
    }

    private void PlayClipTrimmed(AudioClip clip, Vector3 position, float duration)
    {
        if (clip == null) return;
        GameObject tempAudio = new GameObject("CoinSound");
        tempAudio.transform.position = position;
        AudioSource source = tempAudio.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();
        Destroy(tempAudio, duration);  // ✅ Se destruye tras 'duration' segundos
    }
}