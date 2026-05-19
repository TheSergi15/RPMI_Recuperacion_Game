using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Sonido")]
    public AudioClip coinSound;

    [Range(0.1f, 5f)]
    public float soundDuration = 0.3f; // ⬅️ Ajusta cuántos segundos suena

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.coins += 1;

            // 🔊 Reproducir sonido cortado
            PlayClipTrimmed(coinSound, transform.position, soundDuration);

            Destroy(gameObject);
        }
    }

    private void PlayClipTrimmed(AudioClip clip, Vector3 position, float duration)
    {
        if (clip == null) return;

        // Crear objeto temporal
        GameObject tempAudio = new GameObject("CoinSound");
        tempAudio.transform.position = position;

        AudioSource source = tempAudio.AddComponent<AudioSource>();
        source.clip = clip;
        source.Play();

        // Destruirlo después de 'duration' segundos
        Destroy(tempAudio, duration);
    }
}