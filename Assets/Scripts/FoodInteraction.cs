using UnityEngine;

public class FoodInteraction : MonoBehaviour
{
    [Header("Boxes")]
    public GameObject emptyBox;
    
    [Header("Heart Visual")]
    public GameObject heartPrefab;
    public float heartDisplayDuration = 2f;
    public Vector3 heartOffset = new Vector3(0, 1.2f, 0);

    [Header("Settings")]
    public float interactionDistance = 2.0f;
    public KeyCode interactKey = KeyCode.Space;

    private GameObject player;

    void Start()
    {
        // Find player by tag or name
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) player = GameObject.Find("GoldenRetrieverPlayer");

        // Ensure empty box is hidden at start
        if (emptyBox != null)
        {
            emptyBox.SetActive(false);
        }
    }

    void Update()
    {
        if (player == null) return;

        if (Input.GetKeyDown(interactKey))
        {
            float distance = Vector2.Distance(transform.position, player.transform.position);
            if (distance <= interactionDistance)
            {
                EatFood();
            }
        }
    }

    private void EatFood()
    {
        // 1. Swap boxes
        gameObject.SetActive(false);
        if (emptyBox != null)
        {
            emptyBox.SetActive(true);
        }

        // 2. Show dialogue
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue("yummy!");
        }
        else
        {
            Debug.LogWarning("DialogueManager instance not found!");
        }

        // 3. Show heart
        if (heartPrefab != null)
        {
            GameObject heart = Instantiate(heartPrefab, player.transform.position + heartOffset, Quaternion.identity);
            heart.transform.SetParent(player.transform);
            Destroy(heart, heartDisplayDuration);
        }
    }
}
