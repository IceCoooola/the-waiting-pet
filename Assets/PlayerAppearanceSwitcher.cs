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
    }

    public AppearanceData fishAppearance;
    private AppearanceData originalAppearance;

    private Animator animator;
    private BoxCollider2D boxCollider;
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

        originalAppearance.controller = animator != null ? animator.runtimeAnimatorController : null;
        originalAppearance.scale = transform.localScale;
        if (boxCollider != null)
        {
            originalAppearance.colliderSize = boxCollider.size;
            originalAppearance.colliderOffset = boxCollider.offset;
        }
        isInitialized = true;
    }

    public void SwitchToFish()
    {
        CaptureOriginal();

        if (fishAppearance.controller != null)
        {
            animator.runtimeAnimatorController = fishAppearance.controller;
            transform.localScale = fishAppearance.scale;
            
            if (boxCollider != null && fishAppearance.colliderSize != Vector2.zero)
            {
                boxCollider.size = fishAppearance.colliderSize;
                boxCollider.offset = fishAppearance.colliderOffset;
            }
            
            Debug.Log("[AppearanceSwitcher] Switched to Fish appearance.");
        }
    }

    public void RestoreOriginal()
    {
        if (animator == null) animator = GetComponent<Animator>();
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();

        animator.runtimeAnimatorController = originalAppearance.controller;
        transform.localScale = originalAppearance.scale;
        
        if (boxCollider != null)
        {
            boxCollider.size = originalAppearance.colliderSize;
            boxCollider.offset = originalAppearance.colliderOffset;
        }
        
        Debug.Log("[AppearanceSwitcher] Restored original appearance.");
    }
}