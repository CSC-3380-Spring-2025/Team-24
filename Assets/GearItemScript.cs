using UnityEngine;

public class GearItemScript : MonoBehaviour
{
    public EquipmentItem gearItem; // Reference to the EquipmentItem data

    // Initialize the GameObject with the EquipmentItem data
    public void Initialize(EquipmentItem item)
    {
        gearItem = item;
        UpdateVisuals();
    }

    // Update the visual representation of the gear item
    private void UpdateVisuals()
    {
        // Example: Update the sprite and name based on the gear item
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null && gearItem.icon != null)
        {
            spriteRenderer.sprite = gearItem.icon;
        }

        // Example: Update the name (optional)
        gameObject.name = gearItem.itemName;
    }
}