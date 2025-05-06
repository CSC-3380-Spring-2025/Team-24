using UnityEngine;
[System.Serializable]
public class SerializableEquipmentItem 
{
    /// <summary>
    /// The display name of the item.
    /// </summary>
    public string itemName;

    /// <summary>
    /// A detailed description of the item's properties and effects
    /// </summary>
    public string description;

    /// <summary>
    /// The visual represntation of the item in inventory.
    /// </summary>
    public Sprite icon;

    /// <summary>
    /// The level of the item, which affercts its base stats and requirements
    /// </summary>
    public int itemLevel;

    /// <summary>
    /// Affects that player's casting power and fish fighitng ability
    /// </summary>
    public int strength;

    /// <summary>
    /// Affects the player's movement speed and agility when reeling in fish
    /// </summary>
    public int agility;

    /// <summary>
    /// Affects the player's ability to locate and attract fish, as well as their overall fishing skill
    /// </summary>
    public int intelligence;
    
    public Rarity rarity;

    public EquipmentType equipmentType;

    public float value;
}


