using UnityEngine;

public class PotBoundaryDialogue : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Pot"))
        {
            // // Replace with your dialogue system call
            // DialogueManager.Instance.ShowDialogue(
            //     "Hmm... maybe I don't want to push the pots too far away. I need to find what else to do."
            // );
        }
    }
}