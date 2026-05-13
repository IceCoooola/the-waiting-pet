using UnityEngine;
using System.Collections.Generic;

public class ExperimentCounterInteraction : MonoBehaviour
{
    public float interactionDistance = 2.5f;
    public RuntimeAnimatorController catAnimator;
    public GameObject ladderObject;

    private Transform player;
    private static bool hasInvestigatedOnce = false;
    
    private enum State { Inactive, Thirsty, Intro, Choices, Feedback }
    private State currentState = State.Inactive;

    private List<int> currentSequence = new List<int>();
    private readonly int[] targetSequence = { 3, 1, 2, 5 }; // Blue, Green, Red, Yellow

    private void Start()
    {
        FindPlayer();
    }

    private void FindPlayer()
    {
        GameObject p = GameObject.Find("GoldenRetrieverPlayer");
        if (p == null) p = GameObject.Find("CatPlayer");
        if (p == null) p = GameObject.FindWithTag("Player");
        if (p != null) player = p.transform;
    }

    private bool sequenceCompleted = false;

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        bool inRange = distance <= interactionDistance;

        if (!inRange)
        {
            if (currentState != State.Inactive)
            {
                currentState = State.Inactive;
                sequenceCompleted = false;
                if (DialogueManager.Instance != null)
                    DialogueManager.Instance.HideDialogue();
            }
            return;
        }

        switch (currentState)
        {
            case State.Inactive:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (!hasInvestigatedOnce)
                    {
                        hasInvestigatedOnce = true;
                        currentState = State.Thirsty;
                        if (DialogueManager.Instance != null)
                            DialogueManager.Instance.ShowDialogue("I'm thirsty... But what is it? A potion?\n(Press Space to continue)", false);
                    }
                    else
                    {
                        currentState = State.Intro;
                        if (DialogueManager.Instance != null)
                            DialogueManager.Instance.ShowDialogue("There are some potions on the table... Should I drink one?\n(Press Space to continue)", false);
                    }
                }
                break;

            case State.Thirsty:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentState = State.Intro;
                    if (DialogueManager.Instance != null)
                        DialogueManager.Instance.ShowDialogue("There are some potions on the table... Should I drink one?\n(Press Space to continue)", false);
                }
                break;

            case State.Intro:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    currentState = State.Choices;
                    string choices = "Drink the green potion (Press 1)\n" +
                                     "Drink the red potion (Press 2)\n" +
                                     "Drink the blue potion (Press 3)\n" +
                                     "Drink the orange potion (Press 4)\n" +
                                     "Drink the yellow potion (Press 5)\n" +
                                     "Drink the brown potion (Press 6)\n" +
                                     "Don't drink it (Press 7)";
                    if (DialogueManager.Instance != null)
                        DialogueManager.Instance.ShowDialogue(choices, false, 18);
                }
                break;

            case State.Choices:
                if (Input.GetKeyDown(KeyCode.Alpha1)) DrinkPotion(1, "It tastes like herbs...");
                else if (Input.GetKeyDown(KeyCode.Alpha2)) DrinkPotion(2, "It tastes so spicy!");
                else if (Input.GetKeyDown(KeyCode.Alpha3)) DrinkPotion(3, "It tastes like freezing water.");
                else if (Input.GetKeyDown(KeyCode.Alpha4)) DrinkPotion(4, "It tastes like orange juice! I like it.");
                else if (Input.GetKeyDown(KeyCode.Alpha5)) DrinkPotion(5, "It tastes so bitter.");
                else if (Input.GetKeyDown(KeyCode.Alpha6)) DrinkPotion(6, "It tastes so bitter... Eww!");
                else if (Input.GetKeyDown(KeyCode.Alpha7))
                {
                    currentState = State.Inactive;
                    if (DialogueManager.Instance != null)
                        DialogueManager.Instance.HideDialogue();
                }
                break;

            case State.Feedback:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (sequenceCompleted)
                    {
                        sequenceCompleted = false;
                        TransformToCat();
                    }
                    else
                    {
                        currentState = State.Inactive;
                        if (DialogueManager.Instance != null)
                            DialogueManager.Instance.HideDialogue();
                    }
                }
                break;
        }
    }

    private void DrinkPotion(int id, string message)
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowDialogue(message + "\n(Press Space to continue)", false);
        
        currentState = State.Feedback;

        // Sequence logic: Blue(3), Green(1), Red(2), Yellow(5)
        if (id == targetSequence[currentSequence.Count])
        {
            currentSequence.Add(id);
            if (currentSequence.Count == targetSequence.Length)
            {
                sequenceCompleted = true;
                currentSequence.Clear();
            }
        }
        else
        {
            currentSequence.Clear();
            if (id == targetSequence[0])
            {
                currentSequence.Add(id);
            }
        }
    }

    private void TransformToCat()
    {
        if (player == null || catAnimator == null)
        {
            Debug.LogWarning("[ExperimentCounter] Missing player or catAnimator!");
            return;
        }

        Animator anim = player.GetComponent<Animator>();
        if (anim != null)
        {
            anim.runtimeAnimatorController = catAnimator;
            anim.Update(0);
        }

        player.name = "CatPlayer";

        if (ladderObject != null)
            ladderObject.SetActive(true);

        if (DialogueManager.Instance != null)
            DialogueManager.Instance.ShowDialogue("Poof! You turned into a cat!\n(Press Space to continue)", false);
        
        Debug.Log("[ExperimentCounter] Player transformed into CatPlayer.");
    }
}
