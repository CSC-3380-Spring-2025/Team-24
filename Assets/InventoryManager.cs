using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public List<SerializableEquipmentItem> inventoryItems = new List<SerializableEquipmentItem>();

    public void addItem(SerializableEquipmentItem item)
    {
    Debug.Log("Item Added");
    inventoryItems.Add(item);
    }

}
