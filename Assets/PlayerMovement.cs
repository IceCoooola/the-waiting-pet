using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(SpriteRenderer))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 2.5f;
    public float runSpeed = 5f;
    public Rigidbody2D rb;

    [Header("Animation")]
    public Animator animator;             // auto-found if empty
    public SpriteRenderer spriteRenderer; // auto-found if empty
    public float idleActionThreshold = 5f;

    Vector2 movement;
    bool isRunning;
    float idleTimer;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        // Top-down: no gravity, don't let physics rotate the dog
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        // WASD / Arrow keys
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        // Hold Shift to run
        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Flip the sprite based on horizontal direction
        if (movement.x > 0.01f) spriteRenderer.flipX = false;
        else if (movement.x < -0.01f) spriteRenderer.flipX = true;

        // Animation and Idle Timer
        if (animator != null)
        {
            float speedValue = 0f;
            if (movement.sqrMagnitude > 0.001f)
            {
                speedValue = isRunning ? 2f : 1f;
                idleTimer = 0f; // Reset timer when moving
            }
            else
            {
                idleTimer += Time.deltaTime;
                if (idleTimer >= idleActionThreshold)
                {
                    animator.SetTrigger("DoRandom");
                    idleTimer = 0f;
                }
            }
            animator.SetFloat("Speed", speedValue);
        }
    }

    void FixedUpdate()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector2 moveVelocity = movement.normalized * currentSpeed;
        rb.linearVelocity = moveVelocity;
    }
}
