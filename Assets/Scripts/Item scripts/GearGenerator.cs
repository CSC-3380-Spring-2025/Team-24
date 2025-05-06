using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum EquipmentType
{
    Rod,
    Reel,
    Line,
    Lure,
    Hat,
    Shirt,
    Pants,
    Boots
}


// Attribute focus for equipment templates
public enum AttributeFocus
{
    None,           // No attribute focus
    Strength,       // Pure strength
    Intelligence,   // Pure intelligence
    Agility,        // Pure agility
    StrInt,         // Strength + intelligence hybrid
    StrAgi,         // Strength + agility hybrid
    IntAgi          // Intelligence + agility hybrid
}

[Serializable]
public class ModTier
{
    public int tier;                  // Tier number (1-10)
    public int minItemLevel;          // Minimum item level required for this tier
    public float minValue;            // Minimum mod value for this tier
    public float maxValue;            // Maximum mod value for this tier
    public float weight = 1f;         // Weight for random selection (higher = more common)
}

[Serializable]
public class EquipmentMod
{
    public string modName;                // Name of the modification
    public List<ModTier> tiers;           // Available tiers for this mod
}

[Serializable]
public class EquipmentTemplate
{
    public string name;
    public Sprite icon;
    public AttributeFocus attributeFocus; // The primary attribute focus
    public int baseStrength;              // Base strength value (if applicable)
    public int baseAgility;               // Base agility value (if applicable)
    public int baseIntelligence;          // Base intelligence value (if applicable)
}

public class GearGenerator : MonoBehaviour
{
    // Equipment templates
    [Header("Equipment Templates")]
    [SerializeField] public List<EquipmentTemplate> rodTemplates;
    [SerializeField] public List<EquipmentTemplate> reelTemplates;
    [SerializeField] public List<EquipmentTemplate> lineTemplates;
    [SerializeField] public List<EquipmentTemplate> lureTemplates;
    [SerializeField] public List<EquipmentTemplate> hatTemplates;
    [SerializeField] public List<EquipmentTemplate> shirtTemplates;
    [SerializeField] public List<EquipmentTemplate> pantsTemplates;
    [SerializeField] public List<EquipmentTemplate> bootsTemplates;

    // Mod definitions for each equipment type
    [Header("Equipment Mods")]
    [SerializeField] public List<EquipmentMod> rodMods;
    [SerializeField] public List<EquipmentMod> reelMods;
    [SerializeField] public List<EquipmentMod> lineMods;
    [SerializeField] public List<EquipmentMod> lureMods;
    [SerializeField] public List<EquipmentMod> hatMods;
    [SerializeField] public List<EquipmentMod> shirtMods;
    [SerializeField] public List<EquipmentMod> pantsMods;
    [SerializeField] public List<EquipmentMod> bootsMods;

    // Attribute mods that can be applied to any equipment
    [Header("Attribute Mods")]
    [SerializeField] public EquipmentMod strengthMod;
    [SerializeField] public EquipmentMod agilityMod;
    [SerializeField] public EquipmentMod intelligenceMod;

    // Rarity weights (higher value = higher chance)
    [Header("Rarity Generation Settings")]
    [SerializeField] public float commonWeight = 100f;
    [SerializeField] public float uncommonWeight = 50f;
    [SerializeField] public float rareWeight = 20f;
    [SerializeField] public float epicWeight = 5f;
    [SerializeField] public float legendaryWeight = 1f;

    // Mod count settings based on rarity
    [Header("Mod Count Settings")]
    [SerializeField] public int commonModCount = 1;
    [SerializeField] public int uncommonModCount = 2;
    [SerializeField] public int rareModCount = 3;
    [SerializeField] public int epicModCount = 4;
    [SerializeField] public int legendaryModCount = 5;

    // Attribute mod chance (chance of an attribute mod being selected)
    [Header("Attribute Mod Settings")]
    [Range(0f, 1f)]
    [SerializeField] private float attributeModChance = 0.3f;

    private void Start()
    {
        // Setup default attribute mods if none are defined
        SetupDefaultAttributeMods();

        // Setup default equipment mods if none are defined
        SetupDefaultEquipmentMods();
    }

    // Generation methods
    public EquipmentItem GenerateRandomEquipment(int fishLevel, Rarity minRarity = Rarity.Common)
    {
        // Select a random equipment type
        EquipmentType type = (EquipmentType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(EquipmentType)).Length);

        // Generate the equipment based on the selected type
        return GenerateEquipment(type, fishLevel, minRarity);
    }

    public EquipmentItem GenerateEquipment(EquipmentType type, int fishLevel, Rarity minRarity = Rarity.Common)
    {
        // Generate rarity with minimum constraint
        Rarity rarity = GenerateRarity(minRarity);

        // Item level is directly based on fish level
        int itemLevel = fishLevel;

        // Create the equipment based on type
        EquipmentItem item = null;

        switch (type)
        {
            case EquipmentType.Rod:
                item = GenerateRod(itemLevel, rarity);
                break;
            case EquipmentType.Reel:
                item = GenerateReel(itemLevel, rarity);
                break;
            case EquipmentType.Line:
                item = GenerateLine(itemLevel, rarity);
                break;
            case EquipmentType.Lure:
                item = GenerateLure(itemLevel, rarity);
                break;
            case EquipmentType.Hat:
                item = GenerateHat(itemLevel, rarity);
                break;
            case EquipmentType.Shirt:
                item = GenerateShirt(itemLevel, rarity);
                break;
            case EquipmentType.Pants:
                item = GeneratePants(itemLevel, rarity);
                break;
            case EquipmentType.Boots:
                item = GenerateBoots(itemLevel, rarity);
                break;
        }

        // Apply base stats to all equipment
        if (item != null)
        {
            ApplyRarityName(item, rarity);
            UpdateItemDescription(item);
        }

        return item;
    }
    public SerializableEquipmentItem GetSerializableEquipment(EquipmentType type, int itemLevel, Rarity minRarity = Rarity.Common)
    {
        EquipmentItem item = GenerateEquipment(type, itemLevel, minRarity);
        SerializableEquipmentItem serializableEquipmentItem = new SerializableEquipmentItem();
        serializableEquipmentItem.itemName = item.itemName;
        serializableEquipmentItem.description = item.description;
        serializableEquipmentItem.icon = item.icon;
        serializableEquipmentItem.itemLevel = item.itemLevel;
        serializableEquipmentItem.rarity = minRarity;
        serializableEquipmentItem.equipmentType = type;
        serializableEquipmentItem.strength = item.strength;
        serializableEquipmentItem.agility = item.agility;
        serializableEquipmentItem.intelligence = item.intelligence;

        return serializableEquipmentItem;
    }
    // Helper method to generate rarity
    private Rarity GenerateRarity(Rarity minRarity)
    {
        // Set up weights based on minimum rarity
        float[] weights = new float[5] { commonWeight, uncommonWeight, rareWeight, epicWeight, legendaryWeight };

        // Zero out weights below minimum rarity
        for (int i = 0; i < (int)minRarity; i++)
        {
            weights[i] = 0;
        }

        // Calculate total weight
        float totalWeight = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            totalWeight += weights[i];
        }

        // Select rarity based on weights
        float random = UnityEngine.Random.Range(0, totalWeight);
        float accumulatedWeight = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            accumulatedWeight += weights[i];
            if (random <= accumulatedWeight)
            {
                return (Rarity)i;
            }
        }

        // Fallback (should never get here)
        return minRarity;
    }

    // Apply rarity to item name
    private void ApplyRarityName(EquipmentItem item, Rarity rarity)
    {
        // Set item name with rarity prefix
        item.itemName = $"{rarity} {item.itemName}";
    }

    private void UpdateItemDescription(EquipmentItem item)
    {
        // Build basic description
        string description = $"\nLevel {item.itemLevel}\n";

        // Add main stats
        if (item.strength > 0) description += $"Strength: +{item.strength}\n";
        if (item.agility > 0) description += $"Agility: +{item.agility}\n";
        if (item.intelligence > 0) description += $"Intelligence: +{item.intelligence}\n";

        // Add specific equipment properties based on type
        if (item is FishingRod rod)
        {
            description += $"Cast Distance: +{rod.CastDistanceMultiplier}\n";
            description += $"Accuracy: +{rod.Accuracy}\n";
            description += $"Power Rating: +{rod.PowerRating}\n";
        }
        else if (item is FishingReel reel)
        {
            description += $"Reel Speed: {reel.ReelSpeed}\n";
            description += $"Tension Resist: {reel.TensionResistance}\n";
        }
        else if (item is FishingLine line)
        {
            description += $"Tension Limit: {line.TensionLimit}\n";
            description += $"Line length: {line.LineLength}\n";
        }
        else if (item is FishingLure lure)
        {
            description += $"Attraction: {lure.Attraction}\n";
            description += $"Visibility: {lure.Visibility}\n";
            description += $"Size: {lure.Size}\n";
        }
        else if (item is Hat hat)
        {
            description += $"Fish Perception: +{hat.fishPerceptionRange:F1}m\n";
        }
        else if (item is Shirt shirt)
        {
            description += $"Stamina Regen: +{shirt.staminaRegenRate:F1}\n";
            description += $"Extra Slots: +{shirt.extraInventorySlots}\n";
        }
        else if (item is Pants pants)
        {
            description += $"Struggle Resist: +{pants.fishStruggleResistance:P0}\n";
        }
        else if (item is Boots boots)
        {
            description += $"Move Speed: +{boots.moveSpeedBonus:P0}\n";
        }

        item.description = description;
    }

    // Apply base attributes from a template's attribute focus
    private void ApplyAttributesFromTemplate(EquipmentItem item, EquipmentTemplate template, int itemLevel)
    {
        // Reset all attributes
        item.strength = 0;
        item.agility = 0;
        item.intelligence = 0;

        // Scale factor based on item level
        float levelScaleFactor = 1f + (itemLevel * 0.1f);

        // Apply based on focus type
        switch (template.attributeFocus)
        {
            case AttributeFocus.None:
                // No attributes
                break;

            case AttributeFocus.Strength:
                item.strength = Mathf.RoundToInt(template.baseStrength * levelScaleFactor);
                break;

            case AttributeFocus.Intelligence:
                item.intelligence = Mathf.RoundToInt(template.baseIntelligence * levelScaleFactor);
                break;

            case AttributeFocus.Agility:
                item.agility = Mathf.RoundToInt(template.baseAgility * levelScaleFactor);
                break;

            case AttributeFocus.StrInt:
                item.strength = Mathf.RoundToInt(template.baseStrength * levelScaleFactor);
                item.intelligence = Mathf.RoundToInt(template.baseIntelligence * levelScaleFactor);
                break;

            case AttributeFocus.StrAgi:
                item.strength = Mathf.RoundToInt(template.baseStrength * levelScaleFactor);
                item.agility = Mathf.RoundToInt(template.baseAgility * levelScaleFactor);
                break;

            case AttributeFocus.IntAgi:
                item.intelligence = Mathf.RoundToInt(template.baseIntelligence * levelScaleFactor);
                item.agility = Mathf.RoundToInt(template.baseAgility * levelScaleFactor);
                break;
        }
    }

    // Get the appropriate list of mods for an equipment type
    private List<EquipmentMod> GetModsForEquipmentType(EquipmentType type)
    {
        switch (type)
        {
            case EquipmentType.Rod: return rodMods;
            case EquipmentType.Reel: return reelMods;
            case EquipmentType.Line: return lineMods;
            case EquipmentType.Lure: return lureMods;
            case EquipmentType.Hat: return hatMods;
            case EquipmentType.Shirt: return shirtMods;
            case EquipmentType.Pants: return pantsMods;
            case EquipmentType.Boots: return bootsMods;
            default: return new List<EquipmentMod>();
        }
    }

    // Select mods for an item based on its rarity and item level
    private List<(string modName, float modValue)> SelectMods(EquipmentType equipType, int itemLevel, Rarity rarity)
    {
        // Get mods for this equipment type
        List<EquipmentMod> availableMods = GetModsForEquipmentType(equipType);

        // Determine how many mods to apply based on rarity
        int modCount = 0;
        switch (rarity)
        {
            case Rarity.Common:
                modCount = commonModCount;
                break;
            case Rarity.Uncommon:
                modCount = uncommonModCount;
                break;
            case Rarity.Rare:
                modCount = rareModCount;
                break;
            case Rarity.Epic:
                modCount = epicModCount;
                break;
            case Rarity.Legendary:
                modCount = legendaryModCount;
                break;
        }

        // Make sure we don't try to add more mods than available
        modCount = Mathf.Min(modCount, availableMods.Count);

        // Shuffle the mods for random selection
        List<EquipmentMod> shuffledMods = new List<EquipmentMod>(availableMods);
        for (int i = 0; i < shuffledMods.Count; i++)
        {
            int j = UnityEngine.Random.Range(i, shuffledMods.Count);
            EquipmentMod temp = shuffledMods[i];
            shuffledMods[i] = shuffledMods[j];
            shuffledMods[j] = temp;
        }

        // Select mods and their values
        List<(string, float)> selectedMods = new List<(string, float)>();

        for (int i = 0; i < modCount; i++)
        {
            /*
            // Chance to select an attribute mod instead of equipment mod
            if (UnityEngine.Random.value < attributeModChance)
            {
                // Select a random attribute mod
                EquipmentMod attrMod = null;
                float rand = UnityEngine.Random.value;

                if (rand < 0.33f)
                    attrMod = strengthMod;
                else if (rand < 0.66f)
                    attrMod = agilityMod;
                else
                    attrMod = intelligenceMod;

                if (attrMod != null && attrMod.tiers.Count > 0)
                {
                    // Select a tier and value for the attribute mod
                    (string modName, float value) attributeResult = SelectModValueFromTiers(attrMod, itemLevel);
                    selectedMods.Add(attributeResult);
                    continue;
                }
            }
            */

            // If we didn't select an attribute mod or it failed, get an equipment mod
            if (i < shuffledMods.Count)
            {
                EquipmentMod mod = shuffledMods[i];

                // Select a tier and value for this mod
                (string modName, float value) result = SelectModValueFromTiers(mod, itemLevel);

                // Only add if we got a valid result
                if (!string.IsNullOrEmpty(result.modName))
                {
                    selectedMods.Add(result);
                }
            }
        }
        return selectedMods;
    }

    // Select a value from the appropriate tier for a mod
    private (string modName, float value) SelectModValueFromTiers(EquipmentMod mod, int itemLevel)
    {
        // Filter tiers that are available at this item level
        List<ModTier> availableTiers = mod.tiers.FindAll(tier => tier.minItemLevel <= itemLevel);

        if (availableTiers.Count == 0)
            return (string.Empty, 0);

        // Select a tier based on weights
        float totalWeight = 0;
        foreach (ModTier tier in availableTiers)
        {
            totalWeight += tier.weight;
        }

        float random = UnityEngine.Random.Range(0, totalWeight);
        float accumulatedWeight = 0;
        ModTier selectedTier = availableTiers[0]; // Default to first tier

        foreach (ModTier tier in availableTiers)
        {
            accumulatedWeight += tier.weight;
            if (random <= accumulatedWeight)
            {
                selectedTier = tier;
                break;
            }
        }

        // Generate a random value within the tier's range
        float modValue = UnityEngine.Random.Range(selectedTier.minValue, selectedTier.maxValue);

        // Return the mod name and value
        return (mod.modName, modValue);
    }

    // Apply selected mods to equipment attributes
    private void ApplyModsToAttributes(EquipmentItem item, List<(string modName, float modValue)> mods)
    {
        foreach (var mod in mods)
        {
            // Check for attribute mods
            if (mod.modName == "Strength")
            {
                item.strength += Mathf.RoundToInt(mod.modValue);
            }
            else if (mod.modName == "Agility")
            {
                item.agility += Mathf.RoundToInt(mod.modValue);
            }
            else if (mod.modName == "Intelligence")
            {
                item.intelligence += Mathf.RoundToInt(mod.modValue);
            }
        }
    }

    // Generation methods for each equipment type
    private FishingRod GenerateRod(int itemLevel, Rarity rarity)
    {
        // Select a random template
        EquipmentTemplate template = rodTemplates.Count > 0
            ? rodTemplates[UnityEngine.Random.Range(0, rodTemplates.Count)]
            : CreateDefaultTemplate("Fishing Rod");

        // Create a new instance
        FishingRod rod = ScriptableObject.CreateInstance<FishingRod>();

        // Copy base properties
        rod.itemName = template.name;
        rod.icon = template.icon;
        rod.itemLevel = itemLevel;

        // Apply base attributes from template
        ApplyAttributesFromTemplate(rod, template, itemLevel);

        // Select mods for this rod
        List<(string modName, float modValue)> selectedMods = SelectMods(EquipmentType.Rod, itemLevel, rarity);

        // Apply attribute mods if any
        ApplyModsToAttributes(rod, selectedMods);

        // Set default values
        rod.CastDistanceMultiplier = 1.0f;

        // Apply equipment-specific mods
        foreach (var mod in selectedMods)
        {
            if (mod.modName == "Cast Distance")
            {
                rod.CastDistanceMultiplier = mod.modValue;
            }
            if (mod.modName == "Accuracy")
            {
                rod.Accuracy = mod.modValue;
            }
            if (mod.modName == "Power Rating")
            {
                rod.PowerRating = mod.modValue;
            }
        }

        return rod;
    }

    private FishingReel GenerateReel(int itemLevel, Rarity rarity)
    {
        // Select a random template
        EquipmentTemplate template = reelTemplates.Count > 0
            ? reelTemplates[UnityEngine.Random.Range(0, reelTemplates.Count)]
            : CreateDefaultTemplate("Fishing Reel");

        // Create a new instance
        FishingReel reel = ScriptableObject.CreateInstance<FishingReel>();

        // Copy base properties
        reel.itemName = template.name;
        reel.icon = template.icon;
        reel.itemLevel = itemLevel;

        // Apply base attributes from template
        ApplyAttributesFromTemplate(reel, template, itemLevel);

        // Select mods for this reel
        List<(string modName, float modValue)> selectedMods = SelectMods(EquipmentType.Reel, itemLevel, rarity);

        // Apply attribute mods if any
        ApplyModsToAttributes(reel, selectedMods);

        // Set default values
        reel.ReelSpeed = 1.0f;
        reel.TensionResistance = 1.0f;

        // Apply equipment-specific mods
        foreach (var mod in selectedMods)
        {
            if (mod.modName == "Reel Speed")
            {
                reel.ReelSpeed = mod.modValue;
            }
            else if (mod.modName == "Tension Resistance")
            {
                reel.TensionResistance = mod.modValue;
            }
        }

        return reel;
    }

    private FishingLine GenerateLine(int itemLevel, Rarity rarity)
    {
        // Select a random template
        EquipmentTemplate template = lineTemplates.Count > 0
            ? lineTemplates[UnityEngine.Random.Range(0, lineTemplates.Count)]
            : CreateDefaultTemplate("Fishing Line");

        // Create a new instance
        FishingLine line = ScriptableObject.CreateInstance<FishingLine>();

        // Copy base properties
        line.itemName = template.name;
        line.icon = template.icon;
        line.itemLevel = itemLevel;

        // Apply base attributes from template
        ApplyAttributesFromTemplate(line, template, itemLevel);

        // Select mods for this line
        List<(string modName, float modValue)> selectedMods = SelectMods(EquipmentType.Line, itemLevel, rarity);

        // Apply attribute mods if any
        ApplyModsToAttributes(line, selectedMods);

        // Set default values
        line.TensionLimit = 10.0f;
        line.LineLength = 10.0f;

        // Apply equipment-specific mods
        foreach (var mod in selectedMods)
        {
            if (mod.modName == "Line Length")
            {
                line.LineLength = mod.modValue;
            }
            else if (mod.modName == "Tension Limit")
            {
                line.TensionLimit = mod.modValue;
            }
        }

        return line;
    }

    private FishingLure GenerateLure(int itemLevel, Rarity rarity)
    {
        // Select a random template
        EquipmentTemplate template = lureTemplates.Count > 0
            ? lureTemplates[UnityEngine.Random.Range(0, lureTemplates.Count)]
            : CreateDefaultTemplate("Fishing Lure");

        // Create a new instance
        FishingLure lure = ScriptableObject.CreateInstance<FishingLure>();

        // Copy base properties
        lure.itemName = template.name;
        lure.icon = template.icon;
        lure.itemLevel = itemLevel;

        // Apply base attributes from template
        ApplyAttributesFromTemplate(lure, template, itemLevel);

        // Select mods for this lure
        List<(string modName, float modValue)> selectedMods = SelectMods(EquipmentType.Lure, itemLevel, rarity);

        // Apply attribute mods if any
        ApplyModsToAttributes(lure, selectedMods);

        // Set default values
        lure.Attraction = 1.0f;
        lure.Visibility = 1.0f;
        lure.Size = 1.0f;

        // Apply equipment-specific mods
        foreach (var mod in selectedMods)
        {
            if (mod.modName == "Attraction")
            {
                lure.Attraction = mod.modValue;
            }
            else if (mod.modName == "Visibility")
            {
                lure.Visibility = mod.modValue;
            }
            else if (mod.modName == "Size")
            {
                lure.Size = mod.modValue;
            }
        }

        return lure;
    }

    private Hat GenerateHat(int itemLevel, Rarity rarity)
    {
        // Select a random template
        EquipmentTemplate template = hatTemplates.Count > 0
            ? hatTemplates[UnityEngine.Random.Range(0, hatTemplates.Count)]
            : CreateDefaultTemplate("Fishing Hat");

        // Create a new instance
        Hat hat = ScriptableObject.CreateInstance<Hat>();

        // Copy base properties
        hat.itemName = template.name;
        hat.icon = template.icon;
        hat.itemLevel = itemLevel;

        // Apply base attributes from template
        ApplyAttributesFromTemplate(hat, template, itemLevel);

        // Select mods for this hat
        List<(string modName, float modValue)> selectedMods = SelectMods(EquipmentType.Hat, itemLevel, rarity);

        // Apply attribute mods if any
        ApplyModsToAttributes(hat, selectedMods);

        // Set default values
        hat.fishPerceptionRange = 5.0f;

        // Apply equipment-specific mods
        foreach (var mod in selectedMods)
        {
            if (mod.modName == "Fish Perception Range")
            {
                hat.fishPerceptionRange = mod.modValue;
            }
        }

        return hat;
    }

    private Shirt GenerateShirt(int itemLevel, Rarity rarity)
    {
        // Select a random template
        EquipmentTemplate template = shirtTemplates.Count > 0
            ? shirtTemplates[UnityEngine.Random.Range(0, shirtTemplates.Count)]
            : CreateDefaultTemplate("Fishing Shirt");

        // Create a new instance
        Shirt shirt = ScriptableObject.CreateInstance<Shirt>();

        // Copy base properties
        shirt.itemName = template.name;
        shirt.icon = template.icon;
        shirt.itemLevel = itemLevel;

        // Apply base attributes from template
        ApplyAttributesFromTemplate(shirt, template, itemLevel);

        // Select mods for this shirt
        List<(string modName, float modValue)> selectedMods = SelectMods(EquipmentType.Shirt, itemLevel, rarity);

        // Apply attribute mods if any
        ApplyModsToAttributes(shirt, selectedMods);

        // Set default values
        shirt.staminaRegenRate = 1.0f;
        shirt.extraInventorySlots = 0;

        // Apply equipment-specific mods
        foreach (var mod in selectedMods)
        {
            if (mod.modName == "Stamina Regen Rate")
            {
                shirt.staminaRegenRate = mod.modValue;
            }
            else if (mod.modName == "Extra Inventory Slots")
            {
                shirt.extraInventorySlots = Mathf.RoundToInt(mod.modValue);
            }
        }

        return shirt;
    }

    private Pants GeneratePants(int itemLevel, Rarity rarity)
    {
        // Select a random template
        EquipmentTemplate template = pantsTemplates.Count > 0
            ? pantsTemplates[UnityEngine.Random.Range(0, pantsTemplates.Count)]
            : CreateDefaultTemplate("Fishing Pants");

        // Create a new instance
        Pants pants = ScriptableObject.CreateInstance<Pants>();

        // Copy base properties
        pants.itemName = template.name;
        pants.icon = template.icon;
        pants.itemLevel = itemLevel;

        // Apply base attributes from template
        ApplyAttributesFromTemplate(pants, template, itemLevel);

        // Select mods for this pants
        List<(string modName, float modValue)> selectedMods = SelectMods(EquipmentType.Pants, itemLevel, rarity);

        // Apply attribute mods if any
        ApplyModsToAttributes(pants, selectedMods);

        // Set default values
        pants.fishStruggleResistance = 0.1f;

        // Apply equipment-specific mods
        foreach (var mod in selectedMods)
        {
            if (mod.modName == "Fish Struggle Resistance")
            {
                pants.fishStruggleResistance = mod.modValue;
            }
        }

        return pants;
    }

    private Boots GenerateBoots(int itemLevel, Rarity rarity)
    {
        // Select a random template
        EquipmentTemplate template = bootsTemplates.Count > 0
            ? bootsTemplates[UnityEngine.Random.Range(0, bootsTemplates.Count)]
            : CreateDefaultTemplate("Fishing Boots");

        // Create a new instance
        Boots boots = ScriptableObject.CreateInstance<Boots>();

        // Copy base properties
        boots.itemName = template.name;
        boots.icon = template.icon;
        boots.itemLevel = itemLevel;

        // Apply base attributes from template
        ApplyAttributesFromTemplate(boots, template, itemLevel);

        // Select mods for this boots
        List<(string modName, float modValue)> selectedMods = SelectMods(EquipmentType.Boots, itemLevel, rarity);

        // Apply attribute mods if any
        ApplyModsToAttributes(boots, selectedMods);

        // Set default values
        boots.moveSpeedBonus = 0.1f;

        // Apply equipment-specific mods
        foreach (var mod in selectedMods)
        {
            if (mod.modName == "Move Speed Bonus")
            {
                boots.moveSpeedBonus = mod.modValue;
            }
        }

        return boots;
    }

    // Utility method to generate equipment rewards based on fish type
    public GameObject gearItemPrefab;
    public List<EquipmentItem> GenerateFishingRewards(FishType fishType, int rewardCount = 1)
    {

        List<EquipmentItem> rewards = new List<EquipmentItem>();

        // Determine minimum rarity based on fish properties
        Rarity minRarity = CalculateMinRarityFromFish(fishType);

        // Determine item level based on fish type
        int itemLevel = CalculateItemLevelFromFish(fishType);

        // Generate the requested number of rewards
        for (int i = 0; i < rewardCount; i++)
        {
            // Random chance for each equipment type, but make fishing gear more common
            float randomValue = UnityEngine.Random.value;
            EquipmentType type;

            if (randomValue < 0.6f)
            {
                // 60% chance for fishing gear
                type = (EquipmentType)UnityEngine.Random.Range(0, 4); // Rod, Reel, Line, Lure
            }
            else
            {
                // 40% chance for apparel
                type = (EquipmentType)UnityEngine.Random.Range(4, 8); // Hat, Shirt, Pants, Boots
            }

            // Generate the equipment
            EquipmentItem item = GenerateEquipment(type, itemLevel, minRarity);
            
        
            rewards.Add(item);
        }

        return rewards;
    }

    // Calculate minimum rarity based on fish properties
    private Rarity CalculateMinRarityFromFish(FishType fishType)
    {
        // Base rarity on the inverse of spawn weight (rarer fish give better loot)
        float rarityValue = 1f / Mathf.Max(0.1f, fishType.spawnWeight);

        // Scale rarityValue to determine minimum rarity
        if (rarityValue > 10f)
            return Rarity.Epic;
        else if (rarityValue > 5f)
            return Rarity.Rare;
        else if (rarityValue > 2f)
            return Rarity.Uncommon;
        else
            return Rarity.Common;
    }

    // Calculate item level based on fish type
    private int CalculateItemLevelFromFish(FishType fishType)
    {
        // Base level calculation on depth and other factors
        float baseLevel = Mathf.Max(1f, fishType.spawnWeight / 5f);

        // Add random variance
        int variance = UnityEngine.Random.Range(-2, 3);

        // Return final item level, with minimum of 1
        return Mathf.Max(1, Mathf.RoundToInt(baseLevel) + variance);
    }

    // Create a default equipment template with minimal settings
    private EquipmentTemplate CreateDefaultTemplate(string name)
    {
        EquipmentTemplate template = new EquipmentTemplate();
        template.name = name;
        template.attributeFocus = AttributeFocus.None;
        template.baseStrength = 0;
        template.baseAgility = 0;
        template.baseIntelligence = 0;
        return template;
    }

    // Setup default attribute mods if none are defined
    private void SetupDefaultAttributeMods()
    {
        if (strengthMod == null || strengthMod.tiers == null || strengthMod.tiers.Count == 0)
        {
            strengthMod = new EquipmentMod();
            strengthMod.modName = "Strength";
            strengthMod.tiers = CreateDefaultTiers(1, 5);
        }

        if (agilityMod == null || agilityMod.tiers == null || agilityMod.tiers.Count == 0)
        {
            agilityMod = new EquipmentMod();
            agilityMod.modName = "Agility";
            agilityMod.tiers = CreateDefaultTiers(1, 5);
        }

        if (intelligenceMod == null || intelligenceMod.tiers == null || intelligenceMod.tiers.Count == 0)
        {
            intelligenceMod = new EquipmentMod();
            intelligenceMod.modName = "Intelligence";
            intelligenceMod.tiers = CreateDefaultTiers(1, 5);
        }
    }

    // Setup default equipment mods if none are defined
    private void SetupDefaultEquipmentMods()
    {
        // Rod mods
        if (rodMods == null || rodMods.Count == 0)
        {
            rodMods = new List<EquipmentMod>
            {
                CreateDefaultMod("Cast Distance", 1.0f, 2.0f, 5),
                CreateDefaultMod("Reel Speed", 1.0f, 2.0f, 5)
            };
        }

        // Reel mods
        if (reelMods == null || reelMods.Count == 0)
        {
            reelMods = new List<EquipmentMod>
            {
                CreateDefaultMod("Reel Speed", 1.0f, 3.0f, 5),
                CreateDefaultMod("Tension Resistance", 1.0f, 3.0f, 5)
            };
        }

        // Line mods
        if (lineMods == null || lineMods.Count == 0)
        {
            lineMods = new List<EquipmentMod>
            {
                CreateDefaultMod("Tension Limit", 10.0f, 30.0f, 5),
                CreateDefaultMod("Visibility", 0.5f, 1.5f, 5)
            };
        }

        // Lure mods
        if (lureMods == null || lureMods.Count == 0)
        {
            lureMods = new List<EquipmentMod>
            {
                CreateDefaultMod("Attraction", 1.0f, 3.0f, 5),
                CreateDefaultMod("Visibility", 1.0f, 3.0f, 5)
            };
        }

        // Hat mods
        if (hatMods == null || hatMods.Count == 0)
        {
            hatMods = new List<EquipmentMod>
            {
                CreateDefaultMod("Fish Perception Range", 5.0f, 15.0f, 5)
            };
        }

        // Shirt mods
        if (shirtMods == null || shirtMods.Count == 0)
        {
            shirtMods = new List<EquipmentMod>
            {
                CreateDefaultMod("Stamina Regen Rate", 1.0f, 3.0f, 5),
                CreateDefaultMod("Extra Inventory Slots", 0.0f, 5.0f, 5)
            };
        }

        // Pants mods
        if (pantsMods == null || pantsMods.Count == 0)
        {
            pantsMods = new List<EquipmentMod>
            {
                CreateDefaultMod("Fish Struggle Resistance", 0.1f, 0.5f, 5)
            };
        }

        // Boots mods
        if (bootsMods == null || bootsMods.Count == 0)
        {
            bootsMods = new List<EquipmentMod>
            {
                CreateDefaultMod("Move Speed Bonus", 0.1f, 0.5f, 5)
            };
        }
    }

    // Helper to create a default mod with tiers
    private EquipmentMod CreateDefaultMod(string name, float minValue, float maxValue, int tierCount)
    {
        EquipmentMod mod = new EquipmentMod();
        mod.modName = name;
        mod.tiers = CreateDefaultTiers(minValue, maxValue, tierCount);
        return mod;
    }

    // Helper to create default tiers with a range of values
    private List<ModTier> CreateDefaultTiers(float minValue, float maxValue, int tierCount = 5)
    {
        List<ModTier> tiers = new List<ModTier>();
        float valueStep = (maxValue - minValue) / tierCount;

        for (int i = 0; i < tierCount; i++)
        {
            ModTier tier = new ModTier();
            tier.tier = i + 1;
            tier.minItemLevel = i * 5; // Every 5 levels unlocks a new tier
            tier.minValue = minValue + (valueStep * i);
            tier.maxValue = minValue + (valueStep * (i + 1));
            tier.weight = 10f / (i + 1); // Higher tiers are less common
            tiers.Add(tier);
        }

        return tiers;
    }



    // Debugging method to print all generated equipment
    public void DubugGenerateAndPrint(EquipmentType type, int level, Rarity rarity)
    {
        EquipmentItem item = GenerateEquipment(type, level, rarity);
        InventoryManager inventoryManager = FindFirstObjectByType<InventoryManager>();
        SerializableEquipmentItem serializableEquipmentItem = GetSerializableEquipment(type, level, rarity);
        inventoryManager.AddItem(serializableEquipmentItem); 
        Debug.Log($"Generated Item: {item.itemName} (Level {item.itemLevel})");
        Debug.Log($"Description: {item.description}");
    }

}