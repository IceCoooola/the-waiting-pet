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
                if (HasParameter("IsIdle")) animator.SetBool("IsIdle", false);
            }
            else
            {
                idleTimer += Time.deltaTime;
                if (HasParameter("IsIdle")) animator.SetBool("IsIdle", true);
            
                // Logic for random actions after 5 seconds
                if (idleTimer >= 5f && idleTimer < 5.1f) 
                {
                     if (HasParameter("RandomActionIndex")) animator.SetInteger("RandomActionIndex", Random.Range(0, 4));
                     animator.SetTrigger("DoRandom");
                }
            }
            animator.SetFloat("Speed", speedValue);
            if (HasParameter("IdleTime")) animator.SetFloat("IdleTime", idleTimer);
        }
        }

        private bool HasParameter(string paramName)
        {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName) return true;
        }
        return false;
        }

    void FixedUpdate()
    {
        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        Vector2 moveVelocity = movement.normalized * currentSpeed;
        rb.linearVelocity = moveVelocity;
    }
}
