using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public int health = 100;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

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

        // ✅ Salto corregido - wasPressedThisFrame en AMBOS casos
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
    }

    private void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void SetAnimation(float moveInput)
    {
        if (isGrounded)
        {
            if (moveInput == 0)
                animator.Play("Player_Idle");
            else
                animator.Play("Player_Walk"); // ← también tenías "Player_Idle" aquí por error
        }
        else
        {
            if (rb.linearVelocityY > 0)
                animator.Play("Player_Jump");
            else
                animator.Play("Player_Fall");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Damage")
        {
            health -= 25;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            StartCoroutine(BlinkRed());

            if(health < 0)
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
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}