using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Responsible for initializing and configuring FishGenerator component with fish templates and mods.
/// </summary>
/// <remarks> 
/// This component acts as a bridge between Unity's serializable assets in the inspector and the 
/// runtime data structures needed by the FishGenerator. It converts FishTemplateAsset   EquipmentTemplateAsset and
/// FishModAsset EquipmentModAsset instances into their runtime counterparts during initialization.
/// </remarks>
public class FishGeneratorSetup : MonoBehaviour
{
    [Header("FishTemplates")]
    public List<FishType> fishTypes;

    /// <summary>
    /// Initializes the attached GearGenerator with equipment templates and mods.
    /// </summary>
    /// <remarks>
    /// 1. Retrieves the FishGenerator component from the GameObject.
    /// 2. Converts all template assets into runtime EquipmentTemplate instances
    /// 3. Converts all mods assets into runtime EquipmentMod instances.
    /// 4. Assigns the converted templates and mdos to the GearGenerator.
    /// </remarks>
    public void Start()
    {
        GearGenerator gearGenerator = GetComponent<GearGenerator>();
        if (gearGenerator == null) return;

        // Set up templates
        //SetupTemplates(rodTemplateAssets, ref gearGenerator.rodTemplates);

        // Set up mods
        //SetupMods(rodModAssets, ref gearGenerator.rodMods);
    }

    /// <summary>
    /// Converts a list of template assets to runtime template objects.
    /// </summary>
    /// <param name="assets">The source list of template assets</param>
    /// <param name="templates">The destination list of runtime templates</param>
    private void SetupTemplates(List<FishType> fishTypes, ref List<EquipmentTemplate> templates)
    {   
    }

    /// <summary>
    /// Converts a list of mod assets to runtime mod objects.
    /// </summary>
    /// <param name="assets">The source list of mod assets.</param>
    /// <param name="mods">The destination list of runtime mods.</param>
    private void SetupMods(List<EquipmentModAsset> assets, ref List<EquipmentMod> mods)
    {
    }

}