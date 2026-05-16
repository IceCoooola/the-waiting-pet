using UnityEngine;
using UnityEngine.UI;

public class TreasureChestInteraction : MonoBehaviour
{
    [Header("Required Key")]
    public string requiredKeyId = "Room1Key";

    [Header("Potion Choices")]
    public PotionChoice[] potions; // 8 potions

    [Header("Potion UI")]
    public GameObject potionPanel;
    public Image[] potionSlots; // 8 UI Images, arranged 4 x 2
    public GameObject selectionMarker;

    [Header("Dialogue")]
    public string lockedDialogue = "It is locked.";
    public string chooseDialogue = "Choose one potion.";

    private bool isPlayerInRange = false;
    private bool choosingPotion = false;
    private bool chestOpened = false;

    private int selectedIndex = 0;

    private void Start()
    {
        if (potionPanel != null)
        {
            potionPanel.SetActive(false);
        }

        if (selectionMarker != null)
        {
            selectionMarker.SetActive(false);
        }
    }

    private void Update()
    {
        if (!isPlayerInRange) return;

        if (choosingPotion)
        {
            HandlePotionSelection();
            return;
        }

        if (!chestOpened && Input.GetKeyDown(KeyCode.Space))
        {
            TryOpenChest();
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

        chestOpened = true;
        choosingPotion = true;
        selectedIndex = 0;

        OpenPotionUI();
        ShowText(chooseDialogue);
    }

    private void OpenPotionUI()
    {
        if (potionPanel != null)
        {
            potionPanel.SetActive(true);
        }

        for (int i = 0; i < potionSlots.Length; i++)
        {
            if (potionSlots[i] == null) continue;

            if (i < potions.Length && potions[i] != null)
            {
                potionSlots[i].sprite = potions[i].potionIcon;
                potionSlots[i].gameObject.SetActive(true);
            }
            else
            {
                potionSlots[i].gameObject.SetActive(false);
            }
        }

        UpdateSelectionMarker();
    }

    private void HandlePotionSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            MoveSelection(1);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MoveSelection(-1);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            MoveSelection(4);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            MoveSelection(-4);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ChoosePotion();
        }
    }

    private void MoveSelection(int amount)
    {
        selectedIndex += amount;

        if (selectedIndex < 0)
        {
            selectedIndex = potions.Length - 1;
        }

        if (selectedIndex >= potions.Length)
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

        selectionMarker.SetActive(true);
        selectionMarker.transform.position = potionSlots[selectedIndex].transform.position;
    }

    private void ChoosePotion()
    {
        if (InventoryManager.Instance == null) return;
        if (selectedIndex < 0 || selectedIndex >= potions.Length) return;

        PotionChoice chosenPotion = potions[selectedIndex];

        if (InventoryManager.Instance.IsFull())
        {
            ShowText("My pockets are full...");
            return;
        }

        bool added = InventoryManager.Instance.AddItem(
            chosenPotion.potionId,
            chosenPotion.potionIcon
        );

        if (added)
        {
            choosingPotion = false;

            if (potionPanel != null)
            {
                potionPanel.SetActive(false);
            }

            if (selectionMarker != null)
            {
                selectionMarker.SetActive(false);
            }

            ShowText("I chose a potion.");
        }
    }

    private void ShowText(string text)
    {
        if (!string.IsNullOrEmpty(text) && DialogueManager.Instance != null)
        {
            DialogueManager.Instance.ShowDialogue(text, false);
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

            if (!choosingPotion && DialogueManager.Instance != null)
            {
                DialogueManager.Instance.HideDialogue();
            }
        }
    }
}