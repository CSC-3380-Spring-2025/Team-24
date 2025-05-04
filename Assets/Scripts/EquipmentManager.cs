using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages player equipment slots and provides access to equipped items
/// </summary>
public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }

    [SerializeField] private Transform equipmentSlotsParent;
    private Dictionary<EquipmentType, EquipmentSlot> equipmentSlots = new Dictionary<EquipmentType, EquipmentSlot>();
    private Dictionary<EquipmentType, SerializableEquipmentItem> equippedItems = new Dictionary<EquipmentType, SerializableEquipmentItem>();

    // Add an event for equipment changes
    public event Action OnEquipmentChanged;

    private void Awake()
    {
        // Singleton setup
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Initialize equipment slots
        InitializeEquipmentSlots();
    }

    private void InitializeEquipmentSlots()
    {
        if (equipmentSlotsParent == null)
        {
            Debug.LogError("Equipment slots parent not assigned!");
            return;
        }

        // Find all equipment slot components
        foreach (Transform child in equipmentSlotsParent)
        {
            EquipmentSlot slot = child.GetComponent<EquipmentSlot>();
            if (slot != null)
            {
                equipmentSlots[slot.slotType] = slot;
                Debug.Log($"Found equipment slot: {slot.slotType}");
            }
        }
    }

    /// <summary>
    /// Equips an item to the appropriate slot
    /// </summary>
    /// <param name="itemData">The item to equip</param>
    /// <returns>True if successful, false otherwise</returns>
    public bool EquipItem(SerializableEquipmentItem itemData)
    {
        if (itemData == null)
        {
            Debug.LogError("Attempted to equip null item!");
            return false;
        }

        EquipmentType type = itemData.equipmentType;

        // Check if slot exists
        if (!equipmentSlots.ContainsKey(type))
        {
            Debug.LogError($"No slot found for equipment type: {type}");
            return false;
        }

        // Store reference to the equipped item
        equippedItems[type] = itemData;

        // Trigger equipment changed event
        OnEquipmentChanged?.Invoke();

        Debug.Log($"Equipped item: {itemData.itemName} in slot {type}");
        return true;
    }

    /// <summary>
    /// Unequips an item from the specified slot
    /// </summary>
    /// <param name="type">The slot to unequip</param>
    /// <returns>The unequipped item, or null if none was equipped</returns>
    public SerializableEquipmentItem UnequipItem(EquipmentType type)
    {
        // Check if slot exists and has an item
        if (!equipmentSlots.ContainsKey(type) || !equippedItems.ContainsKey(type))
        {
            return null;
        }

        // Get the item before removing it
        SerializableEquipmentItem unequippedItem = equippedItems[type];

        // Remove from equipped items
        equippedItems.Remove(type);

        // Trigger equipment changed event
        OnEquipmentChanged?.Invoke();

        Debug.Log($"Unequipped item from slot {type}");
        return unequippedItem;
    }

    /// <summary>
    /// Gets the item equipped in the specified slot
    /// </summary>
    /// <param name="type">The slot to check</param>
    /// <returns>The equipped item, or null if none is equipped</returns>
    public SerializableEquipmentItem GetEquippedItem(EquipmentType type)
    {
        if (equippedItems.ContainsKey(type))
        {
            return equippedItems[type];
        }

        return null;
    }

    /// <summary>
    /// Gets the equipment slot for the specified type
    /// </summary>
    /// <param name="type">The equipment type</param>
    /// <returns>The equipment slot, or null if not found</returns>
    public EquipmentSlot GetSlotForType(EquipmentType type)
    {
        if (equipmentSlots.ContainsKey(type))
        {
            return equipmentSlots[type];
        }

        return null;
    }

    /// <summary>
    /// Checks if all required fishing gear is equipped
    /// </summary>
    /// <returns>True if player has all necessary gear to fish</returns>
    public bool HasMinimumFishingGear()
    {
        // At minimum, player needs a rod, reel, and line to fish
        bool hasRod = equippedItems.ContainsKey(EquipmentType.Rod);
        bool hasReel = equippedItems.ContainsKey(EquipmentType.Reel);
        bool hasLine = equippedItems.ContainsKey(EquipmentType.Line);

        return hasRod && hasReel && hasLine;
    }
}