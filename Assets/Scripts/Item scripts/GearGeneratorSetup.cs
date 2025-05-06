using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Responsible for initializing and configuring a GearGenerator component with equipment templates and mods.
/// </summary>
/// <remarks> 
/// This component acts as a bridge between Unity's serializable assets in the inspector and the 
/// runtime data structures needed by the GearGenerator. It converts EquipmentTemplateAsset and
/// EquipmentModAsset instances into their runtime counterparts during initialization.
/// </remarks>
public class GearGeneratorSetup : MonoBehaviour
{
    [Header("Rod Templates")]
    public List<EquipmentTemplateAsset> rodTemplateAssets;

    [Header("Reel Templates")]
    public List<EquipmentTemplateAsset> reelTemplateAssets;

    [Header("Line Templates")]
    public List<EquipmentTemplateAsset> lineTemplateAssets;

    [Header("Lure Templates")]
    public List<EquipmentTemplateAsset> lureTemplateAssets;

    [Header("Hat Templates")]
    public List<EquipmentTemplateAsset> hatTemplateAssets;

    [Header("Shirt Templates")]
    public List<EquipmentTemplateAsset> shirtTemplateAssets;

    [Header("Pants Templates")]
    public List<EquipmentTemplateAsset> pantsTemplateAssets;

    [Header("Boots Templates")]
    public List<EquipmentTemplateAsset> bootsTemplateAssets;

    [Header("Rod Mods")]
    public List<EquipmentModAsset> rodModAssets;

    [Header("Reel Mods")]
    public List<EquipmentModAsset> reelModAssets;

    [Header("Line Mods")]
    public List<EquipmentModAsset> lineModAssets;

    [Header("Lure Mods")]
    public List<EquipmentModAsset> lureModAssets;

    [Header("Hat Mods")]
    public List<EquipmentModAsset> hatModAssets;

    [Header("Shirt Mods")]
    public List<EquipmentModAsset> shirtModAssets;

    [Header("Pants Mods")]
    public List<EquipmentModAsset> pantsModAssets;

    [Header("Boots Mods")]
    public List<EquipmentModAsset> bootsModAssets;

    [Header("Attribute Mods")]
    public EquipmentModAsset strengthModAsset;
    public EquipmentModAsset agilityModAsset;
    public EquipmentModAsset intelligenceModAsset;

    /// <summary>
    /// Initializes the attached GearGenerator with equipment tempaltes and mods.
    /// </summary>
    /// <remarks>
    /// 1. Retrieves the GearGenreator component from the GameObject.
    /// 2. Converts all template assets into runtime EquipmentTemplate instances
    /// 3. Converts all mods assets into runtime EquipmentMod instances.
    /// 4. Assigns the converted templates and mdos to the GearGenerator.
    /// </remarks>
    public void Start()
    {
        GearGenerator gearGenerator = GetComponent<GearGenerator>();
        if (gearGenerator == null) return;

        // Set up templates
        SetupTemplates(rodTemplateAssets, ref gearGenerator.rodTemplates);
        SetupTemplates(reelTemplateAssets, ref gearGenerator.reelTemplates);
        SetupTemplates(lineTemplateAssets, ref gearGenerator.lineTemplates);
        SetupTemplates(lureTemplateAssets, ref gearGenerator.lureTemplates);
        SetupTemplates(hatTemplateAssets, ref gearGenerator.hatTemplates);
        SetupTemplates(shirtTemplateAssets, ref gearGenerator.shirtTemplates);
        SetupTemplates(pantsTemplateAssets, ref gearGenerator.pantsTemplates);
        SetupTemplates(bootsTemplateAssets, ref gearGenerator.bootsTemplates);

        // Set up mods
        SetupMods(rodModAssets, ref gearGenerator.rodMods);
        SetupMods(reelModAssets, ref gearGenerator.reelMods);
        SetupMods(lineModAssets, ref gearGenerator.lineMods);
        SetupMods(lureModAssets, ref gearGenerator.lureMods);
        SetupMods(hatModAssets, ref gearGenerator.hatMods);
        SetupMods(shirtModAssets, ref gearGenerator.shirtMods);
        SetupMods(pantsModAssets, ref gearGenerator.pantsMods);
        SetupMods(bootsModAssets, ref gearGenerator.bootsMods);

        // Set up attribute mods
        if (strengthModAsset != null)
            gearGenerator.strengthMod = strengthModAsset.GetEquipmentMod();

        if (agilityModAsset != null)
            gearGenerator.agilityMod = agilityModAsset.GetEquipmentMod();

        if (intelligenceModAsset != null)
            gearGenerator.intelligenceMod = intelligenceModAsset.GetEquipmentMod();
    }

    /// <summary>
    /// Converts a list of template assets to runtime template objects.
    /// </summary>
    /// <param name="assets">The source list of template assets</param>
    /// <param name="templates">The destination list of runtime templates</param>
    private void SetupTemplates(List<EquipmentTemplateAsset> assets, ref List<EquipmentTemplate> templates)
    {
        templates = new List<EquipmentTemplate>();
        foreach (EquipmentTemplateAsset asset in assets)
        {
            if (asset != null)
                templates.Add(asset.GetTemplate());
        }
    }

    /// <summary>
    /// Converts a list of mod assets to runtime mod objects.
    /// </summary>
    /// <param name="assets">The source list of mod assets.</param>
    /// <param name="mods">The destination list of runtime mods.</param>
    private void SetupMods(List<EquipmentModAsset> assets, ref List<EquipmentMod> mods)
    {
        mods = new List<EquipmentMod>();
        foreach (EquipmentModAsset asset in assets)
        {
            if (asset != null)
                mods.Add(asset.GetEquipmentMod());
        }
    }
}