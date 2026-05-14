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

    [Header("Normal Cat Jump")]
    public float normalJumpHeight = 0.35f;
    public float normalJumpDuration = 0.25f;

    private Vector2 movement;
    private bool isRunning;
    private float idleTimer;

    private bool isNormalJumping = false;
    private bool isSpecialMoving = false;

    private CatJumpSpot currentCatJumpSpot;
    private FishtankTeleport currentFishtankTeleport;
    private Coroutine normalJumpCoroutine;

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
        if (isSpecialMoving) return;

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
        if (isSpecialMoving)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        float currentSpeed = isRunning ? runSpeed : walkSpeed;
        rb.linearVelocity = movement.normalized * currentSpeed;
    }

    private void HandleJKey()
    {
        // Check for fishtank teleport first
        if (currentFishtankTeleport != null)
        {
            currentFishtankTeleport.Teleport(this);
            return;
        }

        if (!IsCat())
        {
            Debug.Log("[PlayerMovement] Dog cannot use J jump.");
            return;
        }

        // If the player is on a ladder, let LadderClimber handle the J key

        // special movement near the ladder
        if (currentCatJumpSpot != null)
        {
            Debug.Log("[PlayerMovement] Move to bookshelf point.");
            StopNormalJumpIfRunning();
            StartCoroutine(currentCatJumpSpot.MovePlayerToBookshelf(transform, rb, this));
            return;
        }

        NormalCatJump();
    }

    private void NormalCatJump()
    {
        if (isNormalJumping) return;

        Debug.Log("[PlayerMovement] Normal cat jump.");

        if (animator != null && HasParameter("Jump"))
            animator.SetTrigger("Jump");

        normalJumpCoroutine = StartCoroutine(NormalJumpVisual());
    }

    private IEnumerator NormalJumpVisual()
    {
        isNormalJumping = true;

        Vector3 start = transform.position;
        float elapsed = 0f;

        while (elapsed < normalJumpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / normalJumpDuration;

            float yOffset = Mathf.Sin(t * Mathf.PI) * normalJumpHeight;

            // normal jump motion
            transform.position = new Vector3(start.x, start.y + yOffset, start.z);

            yield return null;
        }

        transform.position = start;
        isNormalJumping = false;
        normalJumpCoroutine = null;
    }

    private void StopNormalJumpIfRunning()
    {
        if (normalJumpCoroutine != null)
        {
            StopCoroutine(normalJumpCoroutine);
            normalJumpCoroutine = null;
        }

        isNormalJumping = false;
    }

    public void SetSpecialMoving(bool value)
    {
        isSpecialMoving = value;

        if (value)
        {
            StopNormalJumpIfRunning();
            movement = Vector2.zero;
            rb.linearVelocity = Vector2.zero;
        }
    }

    private bool IsCat()
    {
        return gameObject.name == catPlayerName;
    }

    public void EnterCatJumpSpot(CatJumpSpot spot)
    {
        currentCatJumpSpot = spot;
    }

    public void ExitCatJumpSpot(CatJumpSpot spot)
    {
        if (currentCatJumpSpot == spot)
            currentCatJumpSpot = null;
    }

    public void EnterFishtankTeleport(FishtankTeleport spot)
    {
        currentFishtankTeleport = spot;
    }

    public void ExitFishtankTeleport(FishtankTeleport spot)
    {
        if (currentFishtankTeleport == spot)
            currentFishtankTeleport = null;
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