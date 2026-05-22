using UnityEngine;

public class Key : MonoBehaviour
{
    public GameObject door;

    [Header("Flotación")]
    public float floatHeight = 0.3f;
    public float floatSpeed = 2f;

    [Header("Rotación")]
    public float rotationSpeed = 90f;

    [Header("Sonido")]
    public AudioClip keySound;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (keySound != null)
                AudioSource.PlayClipAtPoint(keySound, transform.position);

            Destroy(door);
            Destroy(gameObject);
        }
    }
}