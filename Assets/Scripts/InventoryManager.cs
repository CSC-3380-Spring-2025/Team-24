using System.Collections.Generic;
using NUnit.Framework;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class InventoryManager : MonoBehaviour
{
    private static GameObject inventoryManagerInstance;
    public List<SerializableEquipmentItem> inventoryItems = new List<SerializableEquipmentItem>();
    public InventorySlot[] inventorySlots;

    private void Awake()
    {
        DontDestroyOnLoad(this);

        if (inventoryManagerInstance == null)
        {
            inventoryManagerInstance = gameObject;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddItem(SerializableEquipmentItem item)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(item, slot);
                return;
            }
        }
    }

    public void AddItem(SerializableFishItem fishItem)
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot == null)
            {
                SpawnNewItem(fishItem, slot);
                return;
            }
        }
    }

    void SpawnNewItem(SerializableEquipmentItem item, InventorySlot slot)
    {
        Debug.Log($"Spawning new item {item} at slot {slot}");

        GameObject newItemObject = new GameObject("InventoryItem");

        Image imageComponent = newItemObject.AddComponent<Image>();

        InventoryItem newItem = newItemObject.AddComponent<InventoryItem>();

        newItemObject.transform.SetParent(slot.transform);

        newItem.InitializeItem(item);
    }

    void SpawnNewItem(SerializableFishItem itemFish, InventorySlot slot)
    {
        Debug.Log($"Spawning new item {itemFish} at slot {slot}");

        GameObject newItemObject = new GameObject("InventoryItem");

        Image imageComponent = newItemObject.AddComponent<Image>();

        InventoryItem newItem = newItemObject.AddComponent<InventoryItem>();

        newItemObject.transform.SetParent(slot.transform);

        newItem.InitializeItem(itemFish);
    }

    public bool TryAddItemToInventorySlot(GameObject itemObject)
    {
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.transform.childCount == 0)
            {
                itemObject.transform.SetParent(slot.transform);
                itemObject.transform.localPosition = Vector3.zero;
                return true;
            }
        }

        Debug.LogWarning("No open inventory slots to return item");
        return false;
    }

    /// <summary>
    /// Adds a caught fish to the player's inventory
    /// </summary>
    /// <param name="fishData">The serializable fish data to add</param>
    /// <returns>True if successful, false if inventory is full</returns>
    public bool AddFish(SerializableFishItem fishData)
    {
        if (fishData == null)
        {
            Debug.LogError("Attempted to add null fish data!");
            return false;
        }

        // Find an empty inventory slot
        int emptySlot = -1;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetComponentInChildren<InventoryItem>() == null)
            {
                emptySlot = i;
                break;
            }
        }

        // If no empty slot found, inventory is full
        if (emptySlot == -1)
        {
            Debug.Log("Inventory is full!");
            return false;
        }

        // Create new inventory item
        GameObject newItem = Instantiate(itemPrefab, slots[emptySlot].transform);

        // Initialize with fish data
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        if (inventoryItem != null)
        {
            inventoryItem.InitializeItem(fishData);

            // Calculate value based on fish properties
            float baseValue = fishData.value > 0 ? fishData.value : 10f; // Default value if not set
            float sizeModifier = fishData.sizeMultiplier;
            float rarityModifier = 1f;

            // Apply rarity modifier
            switch (fishData.rarity)
            {
                case Rarity.Common:
                    rarityModifier = 1f;
                    break;
                case Rarity.Uncommon:
                    rarityModifier = 2f;
                    break;
                case Rarity.Rare:
                    rarityModifier = 4f;
                    break;
                case Rarity.Epic:
                    rarityModifier = 8f;
                    break;
                case Rarity.Legendary:
                    rarityModifier = 16f;
                    break;
            }

            // Calculate final value
            float finalValue = baseValue * sizeModifier * rarityModifier;
            fishData.value = Mathf.RoundToInt(finalValue);

            Debug.Log($"Added fish to inventory: {fishData.fishName}, Value: {fishData.value}");
            return true;
        }

        return false;
    }

}
