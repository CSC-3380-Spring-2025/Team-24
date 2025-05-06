using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

[System.Serializable]
public class SerializableFishItem
{
    // Basic properties
    public string fishName;
    public string description;
    public Sprite icon;
    public FishType baseFishType;
    public Rarity rarity;

    // Modifier properties
    public float speedMultiplier = 1f;
    public float sizeMultiplier = 1f;
    public float forceMultiplier = 1f;
    public float expValue = 100f;
    public float gearDropChance = 0.5f;
    public int gearRarityBonus = 0;
    public string glowEffect = "";
    public int explicitModCount = 0;
    public float value;
}
