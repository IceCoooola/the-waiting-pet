using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("UI Settings")]
    public Image[] slotIcons; // 5 slot icons in the UI

    private List<string> items = new List<string>();
    private Dictionary<string, Sprite> itemIcons = new Dictionary<string, Sprite>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // Hide all icons initially
        foreach (var icon in slotIcons)
        {
            if (icon != null) icon.gameObject.SetActive(false);
        }
    }

    public void AddItem(string id, Sprite icon)
    {
        if (items.Count >= slotIcons.Length)
        {
            Debug.LogWarning("Inventory full!");
            return;
        }

        items.Add(id);
        itemIcons[id] = icon;
        UpdateUI();
    }

    public bool HasItem(string id)
    {
        return items.Contains(id);
    }

    public void RemoveItem(string id)
    {
        if (items.Contains(id))
        {
            items.Remove(id);
            itemIcons.Remove(id);
            UpdateUI();
        }
    }

    private void UpdateUI()
    {
        // Hide all first
        foreach (var icon in slotIcons)
        {
            if (icon != null) icon.gameObject.SetActive(false);
        }

        // Show icons for current items
        for (int i = 0; i < items.Count; i++)
        {
            if (i < slotIcons.Length && slotIcons[i] != null)
            {
                slotIcons[i].sprite = itemIcons[items[i]];
                slotIcons[i].gameObject.SetActive(true);
            }
        }
    }
}
