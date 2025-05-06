using System.Collections.Generic;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance;
    private Dictionary<EquipmentType, EquipmentSlot> slotLookup;

    public SerializableEquipmentItem equippedRod;
    public SerializableEquipmentItem equippedReel;
    public SerializableEquipmentItem equippedLine;
    public SerializableEquipmentItem equippedLure;
    public SerializableEquipmentItem equippedHat;
    public SerializableEquipmentItem equippedShirt;
    public SerializableEquipmentItem equippedPants;
    public SerializableEquipmentItem equippedBoots;

    public EquipmentSlot[] equipmentSlots;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        //Build lookup on awake
        slotLookup = new Dictionary<EquipmentType, EquipmentSlot>();
        foreach (var slot in equipmentSlots)
        {
            if (!slotLookup.ContainsKey(slot.slotType))
                slotLookup.Add(slot.slotType, slot);
        }
    }
    public EquipmentSlot GetSlotForType(EquipmentType type)
    {
        return slotLookup.TryGetValue(type, out EquipmentSlot slot) ? slot : null;
    }

    public void EquipItem(SerializableEquipmentItem item)
    {
        if (item == null)
        {
            Debug.LogWarning("Attempted to equip a null item.");
            return;
        }
        switch (item.equipmentType)
        {
            case EquipmentType.Rod: equippedRod = item;
                break;
            case EquipmentType.Reel:
                equippedReel = item;
                break;
            case EquipmentType.Line:
                equippedLine = item;
                break;
            case EquipmentType.Lure:
                equippedLure = item;
                break;
            case EquipmentType.Hat:
                equippedHat = item;
                break;
            case EquipmentType.Shirt:
                equippedShirt = item;
                break;
            case EquipmentType.Pants:
                equippedPants = item;
                break;
            case EquipmentType.Boots:
                equippedBoots = item;
                break;
            default:
                Debug.LogWarning("Unknown equipment type: " + item.equipmentType);
                break;
        }
    }

    public void UnequipItem(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Rod: equippedRod = null; break;
            case EquipmentType.Reel: equippedReel = null; break;
            case EquipmentType.Line: equippedLine = null; break;
            case EquipmentType.Lure: equippedLure = null; break;
            case EquipmentType.Hat: equippedHat = null; break;
            case EquipmentType.Shirt: equippedShirt = null; break;
            case EquipmentType.Pants: equippedPants = null; break;
            case EquipmentType.Boots: equippedBoots = null; break;
        }

    }

}
