using System.Collections;
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
    public Animator animator;
    public SpriteRenderer spriteRenderer;
    public float idleActionThreshold = 5f;

    [Header("Cat Settings")]
    public string catPlayerName = "CatPlayer";

    private Vector2 movement;
    private bool isRunning;
    private float idleTimer;

    private bool isSpecialJumping = false;
    private bool isNormalJumping = false;

    private CatJumpSpot currentCatJumpSpot;

    void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();

        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    void Update()
    {
        if (isSpecialJumping) return;

        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (movement.x > 0.01f) spriteRenderer.flipX = false;
        else if (movement.x < -0.01f) spriteRenderer.flipX = true;

        if (Input.GetKeyDown(KeyCode.J))
        {
            HandleJKey();
        }

        HandleAnimation();
    }

    void FixedUpdate()
    {
        if (isSpecialJumping || isNormalJumping)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        rb.linearVelocity = movement.normalized * currentSpeed;
    }

    private void HandleJKey()
    {
        if (!IsCat())
        {
            Debug.Log("[PlayerMovement] Dog cannot jump with J.");
            return;
        }

        // special jump near the ladder
        if (currentCatJumpSpot != null)
        {
            Debug.Log("[PlayerMovement] Special ladder jump.");
            StartCoroutine(currentCatJumpSpot.PerformSpecialJump(transform, rb, this));
            return;
        }

        NormalCatJump();
    }

    private void NormalCatJump()
    {
        if (isNormalJumping) return;

        Debug.Log("[PlayerMovement] Normal cat jump.");

        if (animator != null && HasParameter("Jump"))
        {
            animator.SetTrigger("Jump");
        }

        StartCoroutine(NormalJumpVisual());
    }

    private IEnumerator NormalJumpVisual()
    {
        isNormalJumping = true;

        Vector3 start = transform.position;
        float duration = 0.22f;
        float height = 0.12f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            float arc = Mathf.Sin(t * Mathf.PI) * height;
            transform.position = new Vector3(start.x, start.y + arc, start.z);

            yield return null;
        }

        transform.position = start;
        isNormalJumping = false;
    }

    private bool IsCat()
    {
        return gameObject.name == catPlayerName;
    }

    public void EnterCatJumpSpot(CatJumpSpot spot)
    {
        currentCatJumpSpot = spot;
        Debug.Log("[PlayerMovement] Current special jump spot set.");
    }

    public void ExitCatJumpSpot(CatJumpSpot spot)
    {
        if (currentCatJumpSpot == spot)
        {
            currentCatJumpSpot = null;
            Debug.Log("[PlayerMovement] Current special jump spot cleared.");
        }
    }

    public void SetSpecialJumping(bool value)
    {
        isSpecialJumping = value;

        if (value)
        {
            isNormalJumping = false;
            movement = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void HandleAnimation()
    {
        if (animator == null) return;

        float speedValue = 0f;

        if (movement.sqrMagnitude > 0.001f)
        {
            speedValue = isRunning ? 2f : 1f;
            idleTimer = 0f;

            if (HasParameter("IsIdle"))
                animator.SetBool("IsIdle", false);
        }
        else
        {
            idleTimer += Time.deltaTime;

            if (HasParameter("IsIdle"))
                animator.SetBool("IsIdle", true);

            if (idleTimer >= idleActionThreshold && idleTimer < idleActionThreshold + 0.1f)
            {
                if (HasParameter("RandomActionIndex"))
                    animator.SetInteger("RandomActionIndex", Random.Range(0, 4));

                if (HasParameter("DoRandom"))
                    animator.SetTrigger("DoRandom");
            }
        }

        if (HasParameter("Speed"))
            animator.SetFloat("Speed", speedValue);

        if (HasParameter("IdleTime"))
            animator.SetFloat("IdleTime", idleTimer);
    }

    private bool HasParameter(string paramName)
    {
        if (animator == null) return false;

        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
                return true;
        }

        return false;
    }
}