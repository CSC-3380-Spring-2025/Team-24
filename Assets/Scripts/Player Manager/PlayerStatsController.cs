using System;
using UnityEngine;

/// <summary>
/// Manages player stats based on equipped items and provides fishing-related calculations
/// </summary>
public class PlayerStatsController : MonoBehaviour
{
    [Header("Base Stats")]
    public float baseStrength = 5f;
    public float baseAgility = 5f;
    public float baseIntelligence = 5f;

    [Header("Current Stats")]
    [SerializeField] private float currentStrength;
    [SerializeField] private float currentAgility;
    [SerializeField] private float currentIntelligence;

    [Header("Derived Fishing Stats")]
    [SerializeField] private float castDistance = 10f;
    [SerializeField] private float castAccuracy = 1f;
    [SerializeField] private float reelSpeed = 1f;
    [SerializeField] private float tensionResistance = 1f;
    [SerializeField] private float lureAttraction = 1f;
    [SerializeField] private float fishPerceptionRange = 5f;

    // Public getters for derived stats
    public float CastDistance => castDistance;
    public float CastAccuracy => castAccuracy;
    public float ReelSpeed => reelSpeed;
    public float TensionResistance => tensionResistance;
    public float LureAttraction => lureAttraction;
    public float FishPerceptionRange => fishPerceptionRange;

    // Reference to the equipment manager
    private EquipmentManager equipmentManager;

    private void Start()
    {
        // Get reference to equipment manager
        equipmentManager = EquipmentManager.Instance;

        if (equipmentManager == null)
        {
            Debug.LogError("EquipmentManager not found in scene!");
        }
        else
        {
            // Subscribe to equipment changed event
            equipmentManager.OnEquipmentChanged += RecalculateStats;

            // Initial calculation of stats
            RecalculateStats();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from event when this component is destroyed
        if (equipmentManager != null)
        {
            equipmentManager.OnEquipmentChanged -= RecalculateStats;
        }
    }

    /// <summary>
    /// Recalculates all player stats based on current equipment
    /// </summary>
    public void RecalculateStats()
    {
        // Reset stats to base values
        currentStrength = baseStrength;
        currentAgility = baseAgility;
        currentIntelligence = baseIntelligence;

        // Default values for fishing stats
        castDistance = 10f;
        castAccuracy = 1f;
        reelSpeed = 1f;
        tensionResistance = 1f;
        lureAttraction = 1f;
        fishPerceptionRange = 5f;

        // If equipment manager is not available, use default values
        if (equipmentManager == null) return;

        // Add stats from equipped rod
        SerializableEquipmentItem rodItem = equipmentManager.GetEquippedItem(EquipmentType.Rod);
        if (rodItem != null)
        {
            // Add attribute bonuses
            currentStrength += rodItem.strength;
            currentAgility += rodItem.agility;
            currentIntelligence += rodItem.intelligence;

            // Extract rod-specific data using reflection or a custom method
            float rodCastDistanceMultiplier = GetEquipmentStatValue(rodItem, "CastDistanceMultiplier", 1f);
            float rodAccuracy = GetEquipmentStatValue(rodItem, "Accuracy", 1f);

            // Apply rod stats
            castDistance *= rodCastDistanceMultiplier;
            castAccuracy *= rodAccuracy;
        }

        // Add stats from equipped reel
        SerializableEquipmentItem reelItem = equipmentManager.GetEquippedItem(EquipmentType.Reel);
        if (reelItem != null)
        {
            // Add attribute bonuses
            currentStrength += reelItem.strength;
            currentAgility += reelItem.agility;
            currentIntelligence += reelItem.intelligence;

            // Extract reel-specific data
            float reelSpeedMultiplier = GetEquipmentStatValue(reelItem, "ReelSpeed", 1f);
            float reelTensionResistance = GetEquipmentStatValue(reelItem, "TensionResistance", 1f);

            // Apply reel stats
            reelSpeed *= reelSpeedMultiplier;
            tensionResistance *= reelTensionResistance;
        }

        // Add stats from equipped line
        SerializableEquipmentItem lineItem = equipmentManager.GetEquippedItem(EquipmentType.Line);
        if (lineItem != null)
        {
            // Add attribute bonuses
            currentStrength += lineItem.strength;
            currentAgility += lineItem.agility;
            currentIntelligence += lineItem.intelligence;

            // Extract line-specific data
            float lineTensionLimit = GetEquipmentStatValue(lineItem, "TensionLimit", 10f);
            float lineLength = GetEquipmentStatValue(lineItem, "LineLength", 10f);

            // Apply line stats
            tensionResistance = Mathf.Min(tensionResistance * lineTensionLimit / 10f, lineTensionLimit);
            castDistance = Mathf.Min(castDistance, lineLength);
        }

        // Add stats from equipped lure
        SerializableEquipmentItem lureItem = equipmentManager.GetEquippedItem(EquipmentType.Lure);
        if (lureItem != null)
        {
            // Add attribute bonuses
            currentStrength += lureItem.strength;
            currentAgility += lureItem.agility;
            currentIntelligence += lureItem.intelligence;

            // Extract lure-specific data
            float lureAttractionValue = GetEquipmentStatValue(lureItem, "Attraction", 1f);

            // Apply lure stats
            lureAttraction *= lureAttractionValue;
        }

        // Add stats from equipped hat
        SerializableEquipmentItem hatItem = equipmentManager.GetEquippedItem(EquipmentType.Hat);
        if (hatItem != null)
        {
            // Add attribute bonuses
            currentStrength += hatItem.strength;
            currentAgility += hatItem.agility;
            currentIntelligence += hatItem.intelligence;

            // Extract hat-specific data
            float hatPerceptionRange = GetEquipmentStatValue(hatItem, "fishPerceptionRange", 5f);

            // Apply hat stats
            fishPerceptionRange = hatPerceptionRange;
        }

        // Add stats from other equipped apparel (shirt, pants, boots)
        AddApparelStats(EquipmentType.Shirt);
        AddApparelStats(EquipmentType.Pants);
        AddApparelStats(EquipmentType.Boots);

        // Apply intelligence bonus to fishing ability
        // Intelligence improves lure attraction and fish perception
        lureAttraction *= 1f + (currentIntelligence - baseIntelligence) * 0.05f;
        fishPerceptionRange *= 1f + (currentIntelligence - baseIntelligence) * 0.03f;

        // Apply strength bonus to casting and reeling
        // Strength improves cast distance and tension resistance
        castDistance *= 1f + (currentStrength - baseStrength) * 0.05f;
        tensionResistance *= 1f + (currentStrength - baseStrength) * 0.04f;

        // Apply agility bonus to casting accuracy and reel speed
        // Agility improves cast accuracy and reel speed
        castAccuracy *= 1f + (currentAgility - baseAgility) * 0.04f;
        reelSpeed *= 1f + (currentAgility - baseAgility) * 0.05f;

        // Debug log all calculated stats
        Debug.Log($"Player Stats - STR: {currentStrength}, AGI: {currentAgility}, INT: {currentIntelligence}");
        Debug.Log($"Fishing Stats - Cast Distance: {castDistance}m, Accuracy: {castAccuracy}, " +
                 $"Reel Speed: {reelSpeed}, Tension Resistance: {tensionResistance}, " +
                 $"Lure Attraction: {lureAttraction}, Fish Perception: {fishPerceptionRange}m");
    }

    /// <summary>
    /// Helper method to add stats from apparel items
    /// </summary>
    private void AddApparelStats(EquipmentType type)
    {
        SerializableEquipmentItem item = equipmentManager.GetEquippedItem(type);
        if (item != null)
        {
            // Add attribute bonuses
            currentStrength += item.strength;
            currentAgility += item.agility;
            currentIntelligence += item.intelligence;
        }
    }

    /// <summary>
    /// Helper method to get a specific stat value from an equipment item
    /// </summary>
    private float GetEquipmentStatValue(SerializableEquipmentItem item, string statName, float defaultValue)
    {
        // This is a simplified approach - in practice, you would need a more robust system
        // to extract specific equipment stats since SerializableEquipmentItem doesn't have all gear-specific fields

        // For now, we'll use the item's value field as a placeholder for specific stats
        // In a real implementation, you might use reflection or a dictionary approach
        return item.value > 0 ? item.value : defaultValue;
    }
}