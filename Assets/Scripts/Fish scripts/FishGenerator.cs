using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

[Serializable]
public class FishModTier
{
    public int tier;
    public int minFishLevel;
    public float minValue;
    public float maxValue;
    public float weight = 1f;
}

[Serializable]
public class FishMod
{
    public string modName;
    public List<FishModTier> modTiers;
}


/// <summary>
/// FishGenerator will randomly generate a fish with weighted rarity and mods
/// </summary>
public class FishGenerator : MonoBehaviour
{
    //Fish templates
    [Header("Fish Templates")]
    [SerializeField] public List<FishType> fishTypes;

    //Fish modifiers
    [Header("Fish Modifiers")]
    [SerializeField] public List<FishMod> fishMods; //List of all fish mods

    //Rarity weight
    [Header("Rarity Generation Settings")]
    [SerializeField] private float commonWeight = 100f;
    [SerializeField] private float uncommonWeight = 50f;
    [SerializeField] private float rareWeight = 25f;
    [SerializeField] private float epicWeight = 5f;
    [SerializeField] private float legendaryWeight = 1f;

    [Header("Fish Rarity Templates")]
    [SerializeField] private SerializableFishItem commonFishTemplate;
    [SerializeField] private SerializableFishItem uncommonFishTemplate;
    [SerializeField] private SerializableFishItem rareFishTemplate;
    [SerializeField] private SerializableFishItem epicFishTemplate;
    [SerializeField] private SerializableFishItem legendaryFishTemplate;

    //Generate fish with random rarity
    public SerializableFishItem GenerateSerializableFish(FishType baseFishType, int level = 1)
    {
        Rarity rarity = GenerateRarity();
        return GenerateFish(baseFishType, level, rarity);
    }

    //Generate fish with specific rarity
    public SerializableFishItem GenerateFish(FishType baseFishType, int level, Rarity rarity = Rarity.Common)
    {
        SerializableFishItem fish = new SerializableFishItem();
        fish.baseFishType = baseFishType;
        fish.fishName = baseFishType.speciesID;
        fish.rarity = rarity;
        fish.icon = baseFishType.prefab.GetComponent<SpriteRenderer>().sprite;

        //Apply rarity template
        ApplyRarityTemplate(fish, rarity);

        //Apply name
        fish.fishName = $"{rarity} {fish.fishName}";

        //Apply description
        UpdateFishDescription(fish);

        return fish;
    }
  
    //Helper method to generate rarity
    private Rarity GenerateRarity()
    {
        //Set weights
        float[] weights = new float[5] { commonWeight, uncommonWeight, rareWeight, epicWeight, legendaryWeight };

        //Calculate total weight
        float totalWeight = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            totalWeight += weights[i];
        }

        //Select rarity on weights
        float random = UnityEngine.Random.Range(0, totalWeight);
        float accumulatedWeight = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            accumulatedWeight += weights[i];
            if (random < accumulatedWeight)
            {
                return (Rarity)i;
            }
        }

        return Rarity.Common;
    }

    //Apply template based on rarity
    private void ApplyRarityTemplate(SerializableFishItem fish, Rarity rarity)
    {
        SerializableFishItem template = null;

        switch (rarity)
        {
            case Rarity.Common:
                template = commonFishTemplate;
                break;
            case Rarity.Uncommon:
                template = uncommonFishTemplate;
                break;
            case Rarity.Rare:
                template = rareFishTemplate;
                break;
            case Rarity.Epic:
                template = epicFishTemplate;
                break;
            case Rarity.Legendary:
                template = legendaryFishTemplate;
                break;
        }

        if (template != null)
        {
            fish.speedMultiplier = template.speedMultiplier;
            fish.sizeMultiplier = template.sizeMultiplier;
            fish.forceMultiplier = template.forceMultiplier;
            fish.expValue = template.expValue;
            fish.gearDropChance = template.gearDropChance;
            fish.gearRarityBonus = template.gearRarityBonus;
            fish.glowEffect = template.glowEffect;
            fish.explicitModCount = template.explicitModCount;
        }
    }

    //Update fish description based on modifiers
    private void UpdateFishDescription(SerializableFishItem fish)
    {
        string description = $"{fish.rarity} {fish.baseFishType.speciesID}\n\n";

        if (fish.speedMultiplier != 1f)
        {
            description += $"Speed: {fish.speedMultiplier}x\n";
        }
        if (fish.sizeMultiplier != 1f)
        {
            description += $"Size: {fish.sizeMultiplier}x\n";
        }
        if (fish.forceMultiplier != 1f)
        {
            description += $"Force: {fish.forceMultiplier}x\n";
        }
        description += $"Exp Value: {fish.expValue}\n";
        description += $"Gear Drop Chance: {fish.gearDropChance}\n";

        if (fish.gearRarityBonus != 0)
        {
            description += $"Gear Rarity Bonus: {fish.gearRarityBonus}\n";
        }
        fish.description = description;
    }

    public void DebugGenerateAndPrint(FishType fishType, int level)
    {

        SerializableFishItem fish = GenerateSerializableFish(fishType, level);
        Debug.Log($"Generated Fish: {fish.fishName}\nDescription: {fish.description}");
    }

}
