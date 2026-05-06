using UnityEngine;

public class InteractableWatch : MonoBehaviour
{
    public string dialogue = "a very old watch";
    public float interactionDistance = 1.5f;
    private Transform player;
    private GameObject prompt;

    private void Start()
    {
        GameObject dog = GameObject.Find("GoldenRetrieverPlayer");
        if (dog != null) player = dog.transform;
        
        prompt = transform.Find("InteractionPrompt")?.gameObject;
        if (prompt != null) prompt.SetActive(false);
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        bool inRange = distance <= interactionDistance;

        if (prompt != null) prompt.SetActive(inRange);

        if (inRange && Input.GetKeyDown(KeyCode.Space))
        {
            DialogueManager.Instance.ShowDialogue(dialogue);
        }
    }
}
