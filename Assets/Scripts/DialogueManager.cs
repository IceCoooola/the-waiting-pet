using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialoguePanel;
    public Text dialogueText;
    public Image portraitImage;
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

    private int lastShownFrame = -1;
    public bool isMultiPage = false;

    public bool IsDialogueActive()
    {
        return dialoguePanel != null && dialoguePanel.activeSelf;
    }

    private void Update()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // If it's a multi-page dialogue, the interaction script handles Space
                if (isMultiPage) return;

                // Skip hiding if shown in the same frame
                if (Time.frameCount == lastShownFrame) return;

                // We are closing a single-page dialogue. 
                // We should consume the interaction so other scripts don't re-trigger.
                if (InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.ConsumeInteraction();
                }

                HideDialogue();
            }
        }
    }

    public void ShowDialogue(string message, bool autoHide = false, int fontSize = 0, Sprite portrait = null, bool isFullScreen = false, bool isMultiPage = false)
    {
        lastShownFrame = Time.frameCount;
        this.isMultiPage = isMultiPage;

        if (dialoguePanel == null || dialogueText == null) return;

        dialogueText.fontSize = fontSize > 0 ? fontSize : defaultFontSize;
        dialogueText.text = message;

        RectTransform panelRT = dialoguePanel.GetComponent<RectTransform>();
        RectTransform textRT = dialogueText.rectTransform;
        Image panelImage = dialoguePanel.GetComponent<Image>();

        if (isFullScreen)
        {
            // Full screen layout
            panelRT.anchorMin = Vector2.zero;
            panelRT.anchorMax = Vector2.one;
            panelRT.offsetMin = Vector2.zero;
            panelRT.offsetMax = Vector2.zero;
            panelRT.pivot = new Vector2(0.5f, 0.5f);
            panelRT.anchoredPosition = Vector2.zero;
            panelRT.sizeDelta = Vector2.zero;
            
            // Disable layout components that might interfere
            var layout = dialoguePanel.GetComponent<LayoutGroup>();
            if (layout != null) layout.enabled = false;
            var fitter = dialoguePanel.GetComponent<ContentSizeFitter>();
            if (fitter != null) fitter.enabled = false;

            if (panelImage != null)
            {
                panelImage.color = Color.black; 
                panelImage.sprite = null; 
            }

            if (portraitImage != null && portrait != null)
            {
                portraitImage.sprite = portrait;
                portraitImage.gameObject.SetActive(true);
                portraitImage.preserveAspect = false; // Force stretch
                
                RectTransform portraitRT = portraitImage.rectTransform;
                portraitRT.anchorMin = Vector2.zero;
                portraitRT.anchorMax = Vector2.one;
                portraitRT.offsetMin = Vector2.zero;
                portraitRT.offsetMax = Vector2.zero;
                portraitRT.pivot = new Vector2(0.5f, 0.5f);
                portraitRT.anchoredPosition = Vector2.zero;
                portraitRT.sizeDelta = Vector2.zero;
                
                portraitImage.transform.SetAsFirstSibling();
            }

            // Subtitle style text
            textRT.anchorMin = new Vector2(0, 0);
            textRT.anchorMax = new Vector2(1, 0.3f); 
            textRT.pivot = new Vector2(0.5f, 0.5f);
            textRT.anchoredPosition = new Vector2(0, 50);
            textRT.sizeDelta = new Vector2(-100, 0); // Margin
            dialogueText.alignment = TextAnchor.MiddleCenter;
            dialogueText.color = Color.white;
            textRT.SetAsLastSibling();
        }
        else
        {
            // Standard small panel layout
            panelRT.anchorMin = new Vector2(0.5f, 0.1f);
            panelRT.anchorMax = new Vector2(0.5f, 0.1f);
            panelRT.sizeDelta = new Vector2(500, 200);
            panelRT.anchoredPosition = new Vector2(0, 100);
            
            if (panelImage != null)
            {
                panelImage.color = new Color(0, 0, 0, 0.8f); // Translucent black box
                // Restore original sprite if you had one? For now just color.
            }

            if (portraitImage != null)
            {
                if (portrait != null)
                {
                    portraitImage.sprite = portrait;
                    portraitImage.gameObject.SetActive(true);
                    portraitImage.preserveAspect = true;
                    
                    RectTransform portraitRT = portraitImage.rectTransform;
                    portraitRT.anchorMin = new Vector2(0, 0.5f);
                    portraitRT.anchorMax = new Vector2(0, 0.5f);
                    portraitRT.pivot = new Vector2(0, 0.5f);
                    portraitRT.sizeDelta = new Vector2(150, 150);
                    portraitRT.anchoredPosition = new Vector2(20, 0);
                    
                    dialogueText.alignment = TextAnchor.MiddleLeft;
                    textRT.anchorMin = new Vector2(0, 1);
                    textRT.anchorMax = new Vector2(0, 1);
                    textRT.pivot = new Vector2(0.5f, 0.5f);
                    textRT.anchoredPosition = new Vector2(335, -100);
                    textRT.sizeDelta = new Vector2(310, 160);
                }
                else
                {
                    portraitImage.gameObject.SetActive(false);
                    dialogueText.alignment = TextAnchor.MiddleCenter;
                    textRT.anchorMin = new Vector2(0, 1);
                    textRT.anchorMax = new Vector2(0, 1);
                    textRT.pivot = new Vector2(0.5f, 0.5f);
                    textRT.anchoredPosition = new Vector2(250, -100);
                    textRT.sizeDelta = new Vector2(460, 160);
                }
            }
        }

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
        isMultiPage = false;
    }

    public void HideSingleDialogue()
    {
        if (IsDialogueActive() && !isMultiPage)
        {
            HideDialogue();
        }
    }

    private IEnumerator HideAfterDelay()
{
        yield return new WaitForSeconds(displayDuration);
        dialoguePanel.SetActive(false);
        hideCoroutine = null;
        isMultiPage = false;
    }
}
