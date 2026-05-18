using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int health = 100;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Image healthImage;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public int extraJumpsValue = 1;
    private int extraJumps;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        extraJumps = extraJumpsValue;
    }

    void Update()
    {
        // Movimiento horizontal
        float moveInput = 0f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveInput = 1f;
        else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveInput = -1f;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        // Resetear saltos extra al tocar el suelo
        if (isGrounded)
            extraJumps = extraJumpsValue;

        // Salto corregido
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (isGrounded || extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                if (!isGrounded)
                    extraJumps--;
            }
        }

        SetAnimation(moveInput);

        healthImage.fillAmount = health / 100f;
    }

    private void FixedUpdate()
    {
        // ✅ CORREGIDO: OverlapCircle devuelve Collider2D, no bool
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = groundCollider != null;
    }

    private void SetAnimation(float moveInput)
    {
        // ✅ FLIPEAR el sprite según la dirección
        if (moveInput > 0)
            spriteRenderer.flipX = false;  // Mirando a la derecha
        else if (moveInput < 0)
            spriteRenderer.flipX = true;   // Mirando a la izquierda

        if (isGrounded)
        {
            if (moveInput == 0)
                animator.Play("Player_Idle");
            else
                animator.Play("Player_Walk");
        }
        else
        {
            // ✅ CORREGIDO: linearVelocity.y (no linearVelocityY)
            if (rb.linearVelocity.y > 0)
                animator.Play("Player_Jump");
            else
                animator.Play("Player_Fall");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // ✅ CORREGIDO: Usar CompareTag
        if (collision.gameObject.CompareTag("Damage"))
        {
            health -= 25;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BlinkRed());

            Debug.Log("💔 Daño recibido. Salud: " + health);

            if (health <= 0)
            {
                Die();
            }
        }
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
    }

    private void Die()
    {
        Debug.Log("💀 ¡Muerto!");
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
