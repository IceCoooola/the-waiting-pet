using UnityEngine;
using System.Collections.Generic;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;
    private static bool isPuzzleSolved = false;

    public List<PuzzleSlot> slots = new List<PuzzleSlot>();
    public GameObject starDecal;
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
            foreach (var prop in puzzleProps)
            {
                if (prop != null) prop.SetActive(false);
            }
        }
        else
        {
            if (starDecal != null) starDecal.SetActive(false);
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

        bool allSatisfied = true;
        foreach (var slot in slots)
        {
            if (!slot.IsSatisfied)
            {
                allSatisfied = false;
                break;
            }
        }

        if (allSatisfied)
        {
            isPuzzleSolved = true;
            ApplyState();
            Debug.Log("Puzzle Solved!");
        }
    }
}
