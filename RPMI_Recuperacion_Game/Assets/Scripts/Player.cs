using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int health = 100;
    public float moveSpeed = 5f;
    public float jumpForce = 12f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;
    public Image healthImage;

    [Header("Coyote Time")]
    public float coyoteTime = 0.15f;
    private float coyoteTimeCounter;

    [Header("Gravedad")]
    public float fallGravityScale = 4f;
    public float riseGravityScale = 2f;

    [Header("Variable Jump")]
    public float variableJumpMultiplier = 0.5f;

    [Header("Suelos Especiales")]
    public float slipperyFriction = 0.3f; // Multiplicador de velocidad en suelo resbaladizo
    public float slipperyDeceleration = 0.85f; // 0-1, qué tan rápido se frena (0.85 = lento)
    private bool isOnSlipperyGround = false;
    private float currentHorizontalVelocity = 0f; // Guarda la velocidad para deslizamiento

    [Header("Sonidos")]
    public AudioClip jumpSound;
    public AudioClip hurtSound;
    public AudioClip runSound;
    public AudioClip slipperyRunSound; // Sonido especial para suelo resbaladizo
    public AudioClip fallSound;
    public AudioClip powerupSound;

    [Header("Muerte")]
    public float deathY = -20f;

    private AudioSource sfxSource;
    private AudioSource loopSource;

    private Rigidbody2D rb;
    private bool isGrounded;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public int extraJumpsValue = 1;
    private int extraJumps;

    private bool isPlayingRunSound = false;
    private bool isFalling = false;
    private bool isJumping = false;

    private float moveInput = 0f;
    private bool jumpRequested = false;

    private Coroutine blinkCoroutine;
    private bool isDead = false;

    // ─────────────────────────────────────────────
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        loopSource = gameObject.AddComponent<AudioSource>();
        loopSource.playOnAwake = false;
        loopSource.loop = true;

        extraJumps = extraJumpsValue;
    }

    // ─────────────────────────────────────────────
    void Update()
    {
        if (Keyboard.current == null) return;

        // Lee el input PRIMERO
        moveInput = 0f;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
            moveInput = 1f;
        else if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
            moveInput = -1f;

        // Variable jump: solo si está saltando y suelta la tecla
        if (Keyboard.current.spaceKey.wasReleasedThisFrame && rb.linearVelocity.y > 0 && isJumping)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * variableJumpMultiplier);
            isJumping = false;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
            jumpRequested = true;

        rb.gravityScale = rb.linearVelocity.y < 0 ? fallGravityScale : riseGravityScale;

        SetAnimation(moveInput);

        if (healthImage != null)
            healthImage.fillAmount = Mathf.Clamp(health / 100f, 0f, 1f);

        if (transform.position.y < deathY)
            Die();
    }

    // ─────────────────────────────────────────────
    private void FixedUpdate()
    {
        Collider2D groundCollider = Physics2D.OverlapCircle(
            groundCheck.position, groundCheckRadius, groundLayer);
        isGrounded = groundCollider != null;

        // Detecta si el suelo es resbaladizo
        isOnSlipperyGround = false;
        if (isGrounded && groundCollider != null)
        {
            isOnSlipperyGround = groundCollider.CompareTag("SlipperyGround");
        }

        // 🔧 Manejo de velocidad horizontal con fricción y deslizamiento
        if (isGrounded)
        {
            if (moveInput != 0f)
            {
                float currentSpeed = isOnSlipperyGround ? moveSpeed * slipperyFriction : moveSpeed;
                currentHorizontalVelocity = moveInput * currentSpeed;
            }
            else
            {
                if (isOnSlipperyGround)
                {
                    // En suelo resbaladizo: deslizamiento lento
                    currentHorizontalVelocity *= slipperyDeceleration;
                }
                else
                {
                    // En suelo normal: frena inmediatamente
                    currentHorizontalVelocity = 0f;
                }
            }
        }
        else
        {
            // 🔧 NUEVO: En el aire, control total (sin inercia del deslizamiento)
            currentHorizontalVelocity = moveInput * moveSpeed;
        }

        rb.linearVelocity = new Vector2(currentHorizontalVelocity, rb.linearVelocity.y);

        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            extraJumps = extraJumpsValue;
            isFalling = false;
            isJumping = false;
        }
        else
        {
            coyoteTimeCounter -= Time.fixedDeltaTime;
        }

        if (jumpRequested)
        {
            if (coyoteTimeCounter > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                PlaySound(jumpSound);
                coyoteTimeCounter = 0f;
                isJumping = true;
            }
            else if (extraJumps > 0)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                PlaySound(jumpSound);
                extraJumps--;
                isJumping = true;
            }
            jumpRequested = false;
        }

        HandleRunSound(moveInput);
    }


    // ─────────────────────────────────────────────
    private void HandleRunSound(float input)
    {
        bool shouldRun = isGrounded && input != 0f;

        if (shouldRun && !isPlayingRunSound)
        {
            if (runSound != null)
            {
                // Si está en suelo resbaladizo, usa otro sonido
                AudioClip soundToPlay = isOnSlipperyGround && slipperyRunSound != null
                    ? slipperyRunSound
                    : runSound;

                loopSource.clip = soundToPlay;
                loopSource.Play();
                isPlayingRunSound = true;
            }
        }
        else if (!shouldRun && isPlayingRunSound)
        {
            loopSource.Stop();
            isPlayingRunSound = false;
        }
    }

    // ─────────────────────────────────────────────
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip);
    }

    // ─────────────────────────────────────────────
    private void SetAnimation(float input)
    {
        if (input > 0)
            spriteRenderer.flipX = false;
        else if (input < 0)
            spriteRenderer.flipX = true;

        if (isGrounded)
        {
            isFalling = false;
            if (input == 0)
                PlayAnimation("Player_Idle");
            else
                PlayAnimation("Player_Walk");
        }
        else
        {
            if (rb.linearVelocity.y > 0)
            {
                isFalling = false;
                PlayAnimation("Player_Jump");
            }
            else
            {
                if (!isFalling)
                {
                    PlaySound(fallSound);
                    isFalling = true;
                }
                PlayAnimation("Player_Fall");
            }
        }
    }

    // ─────────────────────────────────────────────
    private void PlayAnimation(string stateName)
    {
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(stateName))
            animator.Play(stateName);
    }

    // ─────────────────────────────────────────────
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            health = Mathf.Max(0, health - 25);
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            PlaySound(hurtSound);

            if (blinkCoroutine != null)
                StopCoroutine(blinkCoroutine);
            blinkCoroutine = StartCoroutine(BlinkRed());

            Debug.Log("💔 Daño recibido. Salud: " + health);

            if (health <= 0)
                Die();
        }
        else if (collision.gameObject.CompareTag("BouncePad"))
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * 2);
        }
    }

    // ─────────────────────────────────────────────
    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = Color.white;
        blinkCoroutine = null;
    }

    // ─────────────────────────────────────────────
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("💀 ¡Muerto!");
        StopAllCoroutines();
        spriteRenderer.color = Color.white;
        loopSource.Stop();

        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // ─────────────────────────────────────────────
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Strawberry"))
        {
            extraJumps = extraJumpsValue;
            PlaySound(powerupSound);
            Destroy(collision.gameObject);
        }
    }
}
