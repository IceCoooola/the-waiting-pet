using UnityEngine;

public class PuzzleSlot : MonoBehaviour
{
    public GameObject targetProp;
    public bool IsSatisfied => currentPot == targetProp;
    public bool HasAnyPot => currentPot != null;

    private GameObject currentPot;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Pot"))
        {
            currentPot = other.gameObject;
            PuzzleManager.Instance?.CheckPuzzle();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == currentPot)
        {
            currentPot = null;
            PuzzleManager.Instance?.CheckPuzzle();
        }
    }
}
