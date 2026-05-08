using UnityEngine;

public class PuzzleSlot : MonoBehaviour
{
    public GameObject targetProp;
    public bool IsSatisfied { get; private set; }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == targetProp)
        {
            IsSatisfied = true;
            PuzzleManager.Instance?.CheckPuzzle();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject == targetProp)
        {
            IsSatisfied = false;
            PuzzleManager.Instance?.CheckPuzzle();
        }
    }
}
