using UnityEngine;
using UnityEngine.UI;

public class TreasureChestInteraction : MonoBehaviour
{
    [Header("Required Key")]
    public string requiredKeyId = "QuestKey";

    [Header("Potion Choices")]
    public PotionChoice[] potions;

    [Header("Potion UI")]
    public GameObject potionPanel;
    public Image[] potionSlots;
    public RectTransform selectionMarker;

    [Header("UI Layout")]
    public float slotSize = 140f;
    public float spacingX = 180f;
    public float spacingY = 180f;

    [Header("Dialogue")]
    public string lockedDialogue = "It is locked.";
    public string foundDialogue = "There are potions inside...";
    public string instructionDialogue = "Choose one potion. Use arrow keys to move, and press Space to select.";
    public string fullInventoryDialogue = "My pockets are full...";

    private bool isPlayerInRange = false;
    private bool chestOpened = false;
    private bool showingFoundDialogue = false;
    private bool showingInstructionDialogue = false;
    private bool choosingPotion = false;

    private int selectedIndex = 0;

    private void Start()
    {
        ClosePotionUI();
    }

    private void Update()
    {
        if (!isPlayerInRange) return;

        if (!chestOpened && Input.GetKeyDown(KeyCode.Space))
        {
            TryOpenChest();
            return;
        }

        if (showingFoundDialogue && Input.GetKeyDown(KeyCode.Space))
        {
            showingFoundDialogue = false;
            showingInstructionDialogue = true;
            ShowText(instructionDialogue);
            return;
        }

        if (showingInstructionDialogue && Input.GetKeyDown(KeyCode.Space))
        {
            showingInstructionDialogue = false;
            choosingPotion = true;

            HideDialogue();
            OpenPotionUI();
            return;
        }

        if (choosingPotion)
        {
            HandlePotionSelection();
        }
    }

    private void TryOpenChest()
    {
        if (InventoryManager.Instance == null) return;

        if (!InventoryManager.Instance.HasItem(requiredKeyId))
        {
            ShowText(lockedDialogue);
            return;
        }

        InventoryManager.Instance.RemoveItem(requiredKeyId);

        chestOpened = true;
        showingFoundDialogue = true;

        ShowText(foundDialogue);
    }

    private void OpenPotionUI()
    {
        PlayerMovement.movementLocked = true;

        if (potionPanel != null)
        {
            potionPanel.SetActive(true);
        }

        selectedIndex = 0;

        for (int i = 0; i < potionSlots.Length; i++)
        {
            if (potionSlots[i] == null) continue;

            RectTransform slotRect = potionSlots[i].GetComponent<RectTransform>();

            slotRect.sizeDelta = new Vector2(slotSize, slotSize);
            potionSlots[i].preserveAspect = true;

            int col = i % 4;
            int row = i / 4;

            float x = (col - 1.5f) * spacingX;
            float y = row == 0 ? spacingY / 2f : -spacingY / 2f;

            slotRect.anchoredPosition = new Vector2(x, y);

            if (i < potions.Length && potions[i] != null)
            {
                potionSlots[i].sprite = potions[i].potionIcon;
                potionSlots[i].color = Color.white;
                potionSlots[i].gameObject.SetActive(true);
            }
            else
            {
                potionSlots[i].gameObject.SetActive(false);
            }
        }

        UpdateSelectionMarker();
    }

    private void ClosePotionUI()
    {
        PlayerMovement.movementLocked = false;

        if (potionPanel != null)
        {
            potionPanel.SetActive(false);
        }

        if (selectionMarker != null)
        {
            selectionMarker.gameObject.SetActive(false);
        }
    }

    private void HandlePotionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSelection(1);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSelection(-1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelection(4);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelection(-4);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            ChoosePotion();
        }
    }

    private void MoveSelection(int amount)
    {
        if (potions == null || potions.Length == 0) return;

        selectedIndex += amount;

        if (selectedIndex < 0)
        {
            selectedIndex = potions.Length - 1;
        }
        else if (selectedIndex >= potions.Length)
        {
            selectedIndex = 0;
        }

        UpdateSelectionMarker();
    }

    private void UpdateSelectionMarker()
    {
        if (selectionMarker == null) return;
        if (potionSlots == null || potionSlots.Length == 0) return;
        if (selectedIndex < 0 || selectedIndex >= potionSlots.Length) return;
        if (potionSlots[selectedIndex] == null) return;

        RectTransform selectedSlot = potionSlots[selectedIndex].GetComponent<RectTransform>();

        selectionMarker.gameObject.SetActive(true);
        selectionMarker.SetParent(selectedSlot.parent);
        selectionMarker.anchoredPosition = selectedSlot.anchoredPosition;
        selectionMarker.sizeDelta = selectedSlot.sizeDelta + new Vector2(25f, 25f);
        selectionMarker.SetAsLastSibling();
    }

    private void ChoosePotion()
    {
        if (potions == null || potions.Length == 0) return;
        if (selectedIndex < 0 || selectedIndex >= potions.Length) return;

        PotionChoice chosenPotion = potions[selectedIndex];
        if (chosenPotion == null) return;

        if (InventoryManager.Instance == null) return;

        if (InventoryManager.Instance.IsFull())
        {
            ShowText(fullInventoryDialogue);
            return;
        }

        bool added = InventoryManager.Instance.AddItem(
            chosenPotion.potionId,
            chosenPotion.potionIcon
        );

        if (added)
        {
            choosingPotion = false;
            ClosePotionUI();
        }
    }

    private void ShowText(string text)
    {
        if (!string.IsNullOrEmpty(text) && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(text, false);
        }
    }

    private void HideDialogue()
    {
        if (DialogueManager.Instance != null)
        {
            DialogueManager.Instance.HideDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;

            showingFoundDialogue = false;
            showingInstructionDialogue = false;
            choosingPotion = false;

            ClosePotionUI();
            HideDialogue();
        }
    }
}