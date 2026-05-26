using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;
    private static bool isPuzzleSolved = false;

    public List<PuzzleSlot> slots = new List<PuzzleSlot>();
    public GameObject starDecal;
    public GameObject livingSpotDecal;
    public List<GameObject> puzzleProps = new List<GameObject>(); // Prop_1 to Prop_4

    private struct PropData
    {
        public GameObject go;
        public Vector3 initialPos;
    }
    private List<PropData> allPropsData = new List<PropData>();

    private void Awake()
    {
        Instance = this;
        
        // Find all objects starting with Prop_ in children
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Prop_"))
            {
                allPropsData.Add(new PropData { go = child.gameObject, initialPos = child.position });
            }
        }
    }

    private void OnEnable()
    {
        ApplyState();
    }

    private void ApplyState()
    {
        if (isPuzzleSolved)
        {
            if (starDecal != null) starDecal.SetActive(true);
            if (livingSpotDecal != null) livingSpotDecal.SetActive(true);
            foreach (var prop in puzzleProps)
            {
                if (prop != null) prop.SetActive(false);
            }
        }
        else
        {
            if (starDecal != null) starDecal.SetActive(false);
            if (livingSpotDecal != null) livingSpotDecal.SetActive(false);
            // Reset all props to initial positions
            foreach (var data in allPropsData)
            {
                if (data.go != null)
                {
                    data.go.transform.position = data.initialPos;
                    // Reset velocity if they have Rigidbody2D
                    var rb = data.go.GetComponent<Rigidbody2D>();
                    if (rb != null)
                    {
                        rb.linearVelocity = Vector2.zero;
                        rb.angularVelocity = 0f;
                    }
                }
            }
        }
    }

    public void CheckPuzzle()
    {
        if (isPuzzleSolved) return;

        bool allSlotsFilled = true;
        bool allSatisfied = true;
        foreach (var slot in slots)
        {
            if (!slot.HasAnyPot)
            {
                allSlotsFilled = false;
            }
            if (!slot.IsSatisfied)
            {
                allSatisfied = false;
            }
        }

        if (allSatisfied)
        {
            isPuzzleSolved = true;
            if (isDisplayingDialogue)
            {
                StopAllCoroutines();
                isDisplayingDialogue = false;
                DialogueManager.Instance?.HideDialogue();
            }
            ApplyState();
            Debug.Log("Puzzle Solved!");
        }
        else if (allSlotsFilled && !isDisplayingDialogue)
        {
            if (Time.time - lastDialogueTime > dialogueCooldown)
            {
                StartCoroutine(PlayFailureDialogue());
            }
        }
    }

    private string[] failureLines = new string[]
    {
        "Hmm... Why did nothing happen?\n(Press space to continue.)",
        "That's weird, maybe I did it wrong... \n(Press space to continue.)",
        "Maybe it's the wrong pot... \n(Press space to continue.)",
        "But which pot should I use to cover each eye? \n(Press space to continue.)"
    };

    private bool isDisplayingDialogue = false;
    private float lastDialogueTime = -10f;
    private const float dialogueCooldown = 10f;

    private bool AreAllSlotsFilled()
    {
        foreach (var slot in slots)
        {
            if (!slot.HasAnyPot) return false;
        }
        return true;
    }

    private IEnumerator PlayFailureDialogue()
    {
        isDisplayingDialogue = true;
        lastDialogueTime = Time.time;

        foreach (var line in failureLines)
        {
            if (isPuzzleSolved || !AreAllSlotsFilled()) break;
            
            DialogueManager.Instance?.ShowDialogue(line, false);
            
            yield return null; 
            
            while (!Input.GetKeyDown(KeyCode.Space))
            {
                if (isPuzzleSolved || !AreAllSlotsFilled()) 
                {
                    DialogueManager.Instance?.HideDialogue();
                    isDisplayingDialogue = false;
                    yield break;
                }
                yield return null;
            }
            
            while (Input.GetKey(KeyCode.Space)) yield return null;
        }

        DialogueManager.Instance?.HideDialogue();
        isDisplayingDialogue = false;
    }
    }
