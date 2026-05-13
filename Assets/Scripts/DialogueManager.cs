using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public Text dialogueText;
    public float displayDuration = 3f;

    private Coroutine hideCoroutine;

    private int defaultFontSize;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (dialogueText != null) defaultFontSize = dialogueText.fontSize;
    }

    public void ShowDialogue(string message, bool autoHide = true, int fontSize = 0)
    {
        if (dialoguePanel == null || dialogueText == null) return;

        dialogueText.fontSize = fontSize > 0 ? fontSize : defaultFontSize;
        dialogueText.text = message;
        dialoguePanel.SetActive(true);

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        if (autoHide)
        {
            hideCoroutine = StartCoroutine(HideAfterDelay());
        }
    }

    public void HideDialogue()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
    }

    private IEnumerator HideAfterDelay()
{
        yield return new WaitForSeconds(displayDuration);
        dialoguePanel.SetActive(false);
        hideCoroutine = null;
    }
}
