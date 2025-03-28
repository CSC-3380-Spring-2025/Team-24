using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    public List<SerializableEquipmentItem> inventoryItems = new List<SerializableEquipmentItem>();
    public InventorySlot[] inventorySlots;


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

    void SpawnNewItem(SerializableEquipmentItem item, InventorySlot slot)
    {
        Debug.Log($"Spawning new item {item} at slot {slot}");

        GameObject newItemObject = new GameObject("InventoryItem");

        Image imageComponent = newItemObject.AddComponent<Image>();

        InventoryItem newItem = newItemObject.AddComponent<InventoryItem>();

        newItemObject.transform.SetParent(slot.transform);

        newItem.InitialiseItem(item);
    }
}
