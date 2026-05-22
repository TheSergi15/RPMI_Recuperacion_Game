using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int health = 100;
    public int coins;
    public float moveSpeed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Image healthImage;

    [Header("Coyote Time")]
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    [Header("Sonidos")]
    public AudioClip jumpSound;
    public AudioClip hurtSound;
    public AudioClip runSound;
    public AudioClip fallSound;
    public AudioClip powerupSound; // ⬅️ Nuevo

    private AudioSource audioSource;
    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    public int extraJumpsValue = 1;
    private int extraJumps;

    private bool isPlayingRunSound = false;
    private bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        extraJumps = extraJumpsValue;
    }

    void Update()
    {
        float moveInput = 0f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveInput = 1f;
        else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveInput = -1f;

        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            extraJumps = extraJumpsValue;
            isFalling = false;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (coyoteTimeCounter > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                PlaySound(jumpSound);
                coyoteTimeCounter = 0f;
            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                PlaySound(jumpSound);
                extraJumps--;
            }
        }

        HandleRunSound(moveInput);
        SetAnimation(moveInput);
        healthImage.fillAmount = health / 100f;
    }

    private void HandleRunSound(float moveInput)
    {
        bool shouldRun = isGrounded && moveInput != 0f;

        if (shouldRun && !isPlayingRunSound)
        {
            if (runSound != null)
            {
                audioSource.clip = runSound;
                audioSource.loop = true;
                audioSource.Play();
                isPlayingRunSound = true;
            }
        }
        else if (!shouldRun && isPlayingRunSound)
        {
            audioSource.Stop();
            audioSource.loop = false;
            isPlayingRunSound = false;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
            audioSource.PlayOneShot(clip);
    }

    private void FixedUpdate()
    {
        Collider2D groundCollider = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = groundCollider != null;
    }

    private void SetAnimation(float moveInput)
    {
        if (moveInput > 0)
            spriteRenderer.flipX = false;
        else if (moveInput < 0)
            spriteRenderer.flipX = true;

        if (isGrounded)
        {
            isFalling = false;
            if (moveInput == 0)
                animator.Play("Player_Idle");
            else
                animator.Play("Player_Walk");
        }
        else
        {
            if (rb.linearVelocity.y > 0)
            {
                isFalling = false;
                animator.Play("Player_Jump");
            }
            else
            {
                if (!isFalling)
                {
                    PlaySound(fallSound);
                    isFalling = true;
                }
                animator.Play("Player_Fall");
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            health -= 25;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            PlaySound(hurtSound);
            StartCoroutine(BlinkRed());
            Debug.Log("💔 Daño recibido. Salud: " + health);

            if (health <= 0)
                Die();
        }
        else if (collision.gameObject.tag == "BouncePad")
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 2);
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
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Strawberry")
        {
            extraJumps = 2;
            PlaySound(powerupSound); // 🔊 Sonido powerup
            Destroy(collision.gameObject);
        }
    }
}