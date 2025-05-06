using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlot : MonoBehaviour, IDropHandler
{
    public EquipmentType slotType;

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.childCount == 0)
        {
            GameObject droppedObject = eventData.pointerDrag;
            if (droppedObject == null) return;

            InventoryItem inventoryItem = droppedObject.GetComponent<InventoryItem>();
            if (inventoryItem.itemData == null || inventoryItem == null) return;

            if (inventoryItem.itemData.equipmentType == slotType)
            {
                EquipmentManager.Instance.EquipItem(inventoryItem.itemData);

                inventoryItem.parentAfterDrag = transform;

            }
            else
            {
                Debug.Log("Item type does not match slot type.");
            }
        }  
    }

}
