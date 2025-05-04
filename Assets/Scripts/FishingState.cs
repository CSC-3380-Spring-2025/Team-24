using UnityEngine;

/// <summary>  
/// Represents the fishing game state when player is actively fishing  
/// </summary>  
public class FishingState : StateInterface
{
    private FishingController fishingController;
    private PlayerStatsController playerStats;
    private EquipmentManager equipmentManager;

    private GameObject fishingUI;
    private GameObject gameUI;

    public FishingState(
        FishingController fishingController,
        PlayerStatsController playerStats,
        EquipmentManager equipmentManager,
        GameObject fishingUI,
        GameObject gameUI)
    {
        this.fishingController = fishingController;
        this.playerStats = playerStats;
        this.equipmentManager = equipmentManager;
        this.fishingUI = fishingUI;
        this.gameUI = gameUI;
    }

    public void Enter()
    {
        Debug.Log("Entering Fishing State");

        // Check if player has necessary equipment  
        if (!equipmentManager.HasMinimumFishingGear())
        {
            Debug.LogWarning("Player doesn't have minimum fishing gear!");
            // Show notification UI  
            return;
        }

        // Enable fishing controller  
        if (fishingController != null)
        {
            fishingController.enabled = true;
        }

        // Show fishing UI, hide regular game UI  
        if (fishingUI != null) fishingUI.SetActive(true);
        if (gameUI != null) gameUI.SetActive(false);

        // Subscribe to events  
        if (fishingController != null)
        {
            fishingController.OnFishCaught += HandleFishCaught;
            fishingController.OnFishingFailed += HandleFishingFailed;
        }
    }

    public void Update()
    {
        // Most logic is handled by FishingController, this is just for state transitions  

        // Handle escape key to exit fishing  
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
        }
    }

    public void Exit()
    {
        Debug.Log("Exiting Fishing State");

        // Disable fishing controller  
        if (fishingController != null)
        {
            fishingController.enabled = false;
        }

        // Hide fishing UI, show regular game UI  
        if (fishingUI != null) fishingUI.SetActive(false);
        if (gameUI != null) gameUI.SetActive(true);

        // Unsubscribe from events  
        if (fishingController != null)
        {
            fishingController.OnFishCaught -= HandleFishCaught;
            fishingController.OnFishingFailed -= HandleFishingFailed;
        }
    }

    private void HandleFishCaught(FishAI fish)
    {
        Debug.Log($"Fish caught: {fish.fishType.speciesID}");

        // Add fish to inventory  
        InventoryManager inventory = Object.FindObjectOfType<InventoryManager>();

        if (inventory != null && fish.fishData != null)
        {
            inventory.AddFish(fish.fishData);
            Debug.Log("Fish added to inventory");
        }
    }

    private void HandleFishingFailed()
    {
        Debug.Log("Fishing failed");

        // Maybe play a sound or show a feedback message  
    }
}
