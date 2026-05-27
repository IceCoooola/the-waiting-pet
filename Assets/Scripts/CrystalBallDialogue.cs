using UnityEngine;

public class CrystalBallDialogue : MonoBehaviour
{
    public string itemId;
    public bool isStoryCompleted { get; private set; }
    
    public CrystalBallDialogue[] dependencies;
    
    [TextArea]
    public string prePickupDialogue = "I saw something in the crystal ball.... I can't see it clearly.... seems like a ...";

    [TextArea]
    public string lockedDialogue = "The mist inside won't clear... I feel like I'm missing something important.";
    
    [TextArea]
    public string[] postPickupDialogues;

    [Header("Witch Transformation")]
    public Transform witchSpawnPoint;
    public bool triggerTransformation = false;
    public GameObject roomToDisable;
    public GameObject roomToEnable;

    [Header("Lighting Animation")]
    public Color glowColor = new Color(0.2f, 0.8f, 1f, 1f); 
    public float pulseSpeed = 4f;
    public float scalePulseAmount = 0.15f;
    
    private bool isGlowing = false;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private Vector3 originalScale;
    private Light pointLight;

    private const string SPACE_PROMPT = "\n(Press space to continue.)";
    private int currentPageIndex = -1;
    private bool isPlayerInRange;
    private bool hasTransformed = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null) originalColor = spriteRenderer.color;
        originalScale = transform.localScale;
        
        pointLight = GetComponent<Light>();
        if (pointLight != null) pointLight.enabled = false;
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.Space))
        {
            if (InventoryManager.Instance != null && !InventoryManager.Instance.CanInteract()) return;

            if (InventoryManager.Instance != null && InventoryManager.Instance.HasItem(itemId))
            {
                if (AreDependenciesMet())
                {
                    ShowPostPickupDialogue();
                }
                else
                {
                    ShowLockedDialogue();
                }
            }
            else
            {
                ShowPrePickupDialogue();
            }
        }

        if (isGlowing)
        {
            float lerp = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            
            if (spriteRenderer != null)
            {
                spriteRenderer.color = Color.Lerp(originalColor, glowColor, lerp);
            }

            transform.localScale = originalScale * (1f + lerp * scalePulseAmount);

            if (pointLight != null)
            {
                pointLight.enabled = true;
                pointLight.intensity = 1f + lerp * 2f;
            }
        }
    }

    private bool AreDependenciesMet()
    {
        if (dependencies == null || dependencies.Length == 0) return true;
        foreach (var dep in dependencies)
        {
            if (dep != null && !dep.isStoryCompleted) return false;
        }
        return true;
    }

    private void ShowLockedDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(lockedDialogue, false);
            StartGlow();
        }
    }

    private void ShowPrePickupDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(prePickupDialogue + SPACE_PROMPT, false);
            // Optional: StartGlow(); // If you want it to glow on pre-pickup too
        }
    }

    public void StartGlow()
    {
        isGlowing = true;
    }

    private void ShowPostPickupDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            currentPageIndex++;
            if (currentPageIndex < postPickupDialogues.Length)
            {
                DialogueManager.Instance.ShowDialogue(postPickupDialogues[currentPageIndex] + SPACE_PROMPT, false, 0, null, false, true);
                
                // Mark as completed when the last page is reached
                if (currentPageIndex == postPickupDialogues.Length - 1)
                {
                    isStoryCompleted = true;
                }
            }
            else
            {
                ResetDialogue();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            ResetDialogue();
        }
    }

    private void ResetDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            if (currentPageIndex != -1)
            {
                DialogueManager.Instance.HideDialogue();
            }
            else
            {
                DialogueManager.Instance.HideSingleDialogue();
            }
        }

        currentPageIndex = -1;
        
        if (isStoryCompleted && triggerTransformation && !hasTransformed)
{
            TransformToWitch();
        }
    }

    private void TransformToWitch()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player == null) player = GameObject.Find("GoldenRetrieverPlayer");

        if (player != null)
        {
            if (witchSpawnPoint != null)
            {
                player.transform.position = witchSpawnPoint.position;
            }

            PlayerAppearanceSwitcher switcher = player.GetComponent<PlayerAppearanceSwitcher>();
            if (switcher != null)
            {
                switcher.SwitchToWitch();
            }

            player.name = "WitchPlayer";
            hasTransformed = true;

            // Trigger the flashback dialogue
            player.AddComponent<WitchTransformationDialogue>();

            if (roomToDisable != null)
            {
                roomToDisable.SetActive(false);
            }
            if (roomToEnable != null)
            {
                roomToEnable.SetActive(true);
            }
            
            Debug.Log("[CrystalBall] Player transformed into Witch and teleported.");
        }
    }
    }
