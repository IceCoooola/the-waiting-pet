using UnityEngine;

public class PlayerAppearanceSwitcher : MonoBehaviour
{
    [System.Serializable]
    public struct AppearanceData
    {
        public RuntimeAnimatorController controller;
        public Vector3 scale;
        public Vector2 colliderSize;
        public Vector2 colliderOffset;
        public bool reverseSpriteFlip;
        public GameObject prefab;
        public float cameraSize;
    }

    public AppearanceData fishAppearance;
    public AppearanceData witchAppearance;
    private AppearanceData originalAppearance;

    private Animator animator;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private GameObject currentPrefab;
    private bool isInitialized = false;

    void Awake()
    {
        CaptureOriginal();
    }

    public void ResetOriginal()
    {
        isInitialized = false;
        CaptureOriginal();
    }

    private void CaptureOriginal()
    {
        if (isInitialized) return;

        if (animator == null) animator = GetComponent<Animator>();
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        PlayerMovement movement = GetComponent<PlayerMovement>();

        originalAppearance.controller = animator != null ? animator.runtimeAnimatorController : null;
        originalAppearance.scale = transform.localScale;
        originalAppearance.reverseSpriteFlip = movement != null ? movement.reverseSpriteFlip : false;
        
        if (boxCollider != null)
        {
            originalAppearance.colliderSize = boxCollider.size;
            originalAppearance.colliderOffset = boxCollider.offset;
        }

        if (Camera.main != null)
        {
            originalAppearance.cameraSize = Camera.main.orthographicSize;
        }
        else
        {
            originalAppearance.cameraSize = 5f;
        }

        isInitialized = true;
    }

    public void SwitchToFish()
    {
        ApplyAppearance(fishAppearance, "Fish");
    }

    public void SwitchToWitch()
    {
        ApplyAppearance(witchAppearance, "Witch");
    }

    private void ApplyAppearance(AppearanceData data, string name)
    {
        CaptureOriginal();

        if (currentPrefab != null)
        {
            Destroy(currentPrefab);
            currentPrefab = null;
        }

        PlayerMovement movement = GetComponent<PlayerMovement>();

        if (data.prefab != null)
        {
            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (animator != null) animator.enabled = false;

            currentPrefab = Instantiate(data.prefab, transform);
            currentPrefab.transform.localPosition = Vector3.zero;
            currentPrefab.transform.localRotation = Quaternion.identity;

            Animator childAnimator = currentPrefab.GetComponentInChildren<Animator>();
            if (movement != null && childAnimator != null)
            {
                movement.animator = childAnimator;
            }
        }
        else if (data.controller != null)
        {
            if (spriteRenderer != null) spriteRenderer.enabled = true;
            if (animator != null)
            {
                animator.enabled = true;
                animator.runtimeAnimatorController = data.controller;
            }
            if (movement != null) movement.animator = animator;
        }

        transform.localScale = data.scale;
        if (movement != null) movement.reverseSpriteFlip = data.reverseSpriteFlip;

        if (boxCollider != null && data.colliderSize != Vector2.zero)
        {
            boxCollider.size = data.colliderSize;
            boxCollider.offset = data.colliderOffset;
        }

        if (Camera.main != null && data.cameraSize > 0.1f)
        {
            Camera.main.orthographicSize = data.cameraSize;
        }
        
        Debug.Log($"[AppearanceSwitcher] Switched to {name} appearance.");
    }

    public void RestoreOriginal()
    {
        ApplyAppearance(originalAppearance, "Original");
    }
}