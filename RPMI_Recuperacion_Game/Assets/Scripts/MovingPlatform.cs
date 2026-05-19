using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speed = 2f;
    public Transform[] points;
    private int i;
    private Transform playerOnPlatform;
    private Vector3 previousPosition;

    void Start()
    {
        transform.position = points[0].position;
        previousPosition = transform.position;
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.01f)
        {
            i++;
            if (i == points.Length) i = 0;
        }

        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);

        // Si hay jugador encima, lo arrastramos con la plataforma
        if (playerOnPlatform != null)
        {
            Vector3 delta = transform.position - previousPosition;
            playerOnPlatform.position += delta;
        }

        previousPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = collision.transform;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerOnPlatform = null;
        }
    }
}