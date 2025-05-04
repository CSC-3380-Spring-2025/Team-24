using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StateController : MonoBehaviour
{
    public ParticleSystem ps;
    public AudioSource lureBreak;
    public AudioSource fishCaught;
    public AudioSource fishCast;

    public static event Action<GameObject> OnFishCaught;
    public static event Action OnFishEscaped;

    private GearGenerator gearGenerator;
    private Inventory inventory;
    private StateInterface currentState;

    [Header("Fishing Components")]
    public FishingController fishingController;
    public PlayerStatsController playerStatsController;
    public GameObject fishingUI;
    private FishingState fishingState;

    [Header("Passive State Settings")]
    public GameObject inventoryPanel;
    public GameObject mapPanel;
    public GameObject settingsPanel;
    public KeyCode inventoryKey = KeyCode.I;
    public KeyCode mapKey = KeyCode.M;
    public KeyCode settingsKey = KeyCode.Escape;

    [Header("Cast State Settings")]
    public float castSpeed;
    public float maxCastSpeed;
    public GameObject lurePrefab;
    public Transform castOrigin;
    public DirectionIndicator directionIndicator;
    public KeyCode castKey = KeyCode.Space;

    [Header("Power Minigame Settings")]
    public GameObject powerMinigameObject;

    [Header("Water settings")]
    public float waterLevel = 16f;

    [Header("Hooked state settings")]
    public GameObject tensionBarGameObject;

    private PassiveState passiveState;
    private CastState castState;
    private InAirState inAirState;
    private InWaterState inWaterState;
    private HookedState hookedState;




    private void Start()
    {
        InitializePanels();
        passiveState = new PassiveState(inventoryPanel, mapPanel, settingsPanel, inventoryKey, mapKey, settingsKey, directionIndicator.gameObject);
        castState = new CastState(castSpeed, maxCastSpeed, lurePrefab, castOrigin, directionIndicator, powerMinigameObject.GetComponent<IPowerMinigame>());
        inAirState = new InAirState(waterLevel);
        inWaterState = new InWaterState();
        hookedState = new HookedState(tensionBarGameObject);
        ChangeState(passiveState);
        InitializeFishingState();

    }

    private void InitializeFishingState()
    {
        // Find components if not assigned
        if (fishingController == null)
        {
            fishingController = GetComponent<FishingController>();
        }

        if (playerStatsController == null)
        {
            playerStatsController = GetComponent<PlayerStatsController>();
        }

        // Create fishing state
        fishingState = new FishingState(
            fishingController,
            playerStatsController,
            EquipmentManager.Instance,
            fishingUI,
            gameUI); // Assuming gameUI is already defined in StateController

        // Initially disable fishing controller
        if (fishingController != null)
        {
            fishingController.enabled = false;
        }

        // Hide fishing UI initially
        if (fishingUI != null)
        {
            fishingUI.SetActive(false);
        }
    }

    public void ChangeState(StateInterface newState)
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
  
        currentState = newState;
  
        if (currentState != null)
        {
            currentState.Enter();
        }
    }


    private void Update()
    {
        if (currentState != null)
            currentState.Update();

        HandleStateTransitions();

        HandleKeyInputs();
    }

    private void HandleStateTransitions()
    {
        if (currentState is InAirState)
        {
            InAirState airState = (InAirState)currentState;
            if (airState.IsLureInWater())
            {
                ChangeState(inWaterState);
            }
        }

        if (currentState is InWaterState)
        {
            InWaterState waterState = (InWaterState)currentState;
            if (waterState.IsFishHooked())
            {
                ChangeState(hookedState);
            }
        }
    }

    private void HandleKeyInputs()
    {
        if (currentState is PassiveState && Input.GetKeyDown(castKey))
        {
            ChangeState(castState);
        }
        
        if (currentState is CastState && Input.GetKeyUp(castKey))
        {
            fishCast.Play();
            ChangeState(inAirState);
        }

        // Recast ability - pressing R during any fishing state (InAir, InWater)
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (currentState is HookedState)
            {
                // For hooked state, this means the fish escaped
                // FishEscaped() handles the state change internally after animation
                FishEscaped();
                // Don't immediately change state - let the escape animation play first
                // The FishEscaped method will change to passive state via SeeFishLeave coroutine
            }
            else if (currentState is InAirState || currentState is InWaterState)
            {
                // For other fishing states, allow recasting
                GameObject lure = GameObject.FindWithTag("Lure");
                if (lure != null)
                {
                    Destroy(lure);
                }
                
                // Return to passive state then immediately to cast state
                ChangeState(passiveState);
                ChangeState(castState);
            }
        } 
    }

    public void FishCaught(GameObject fish)
    {
        //Debug.Log($"Fish caught: {fish.name}");

        OnFishCaught?.Invoke(fish);
        
        Debug.Log("Generating loot for caught fish...");

        FishAI fishAI = fish.GetComponent<FishAI>();
        if (fishAI != null && fishAI.fishType != null)
        {
            //Debug.Log($"Fish type: {fishAI.fishType.name}");      
        }

        //Debug.Log($"Fish data: {fishAI.fishData}");
       
        
        //Find and destroy lure 
        GameObject lure = GameObject.FindWithTag("Lure") ?? GameObject.FindGameObjectWithTag("OccupiedLure");
        if ( lure != null)
        {
            // Clean up lure state and associated components
            CleanUpLureAndFish(lure, fish);

            //Destroy Lure object
            Destroy(lure);
        }

        //Destroy fish object
        Destroy(fish);

        //Get fish data for loot generation
        SerializableFishItem fishData = fishAI.fishData;
        Rarity lootRarity = fishData.rarity;
       
        Debug.Log(fishData);
        Debug.Log(lootRarity);

        if (fishData != null)
        {
            //Apply rarity bonus if any
            if (fishData.gearRarityBonus > 0)
            {
                int rarityIndex = (int)lootRarity + fishData.gearRarityBonus;
                rarityIndex = Mathf.Min(rarityIndex, 4);
                lootRarity = (Rarity)rarityIndex;
            }
        }
 
         //Add fish item to inventory
        InventoryManager inventory = FindFirstObjectByType<InventoryManager>();
        //fish. item = gearGenerator.GetSerializableEquipment(type, 1, lootRarity);
        inventory.AddItem(fishData);
        
        fishCaught.Play();

        // Note: Fish is already destroyed above (line 175), no need to destroy it again here
        
        // Return to passive state
        ChangeState(passiveState);
    }

    public void FishEscaped()
    {
        Debug.Log("Fish escaped");

        OnFishEscaped?.Invoke();

        // Find the lure and hooked fish
        GameObject lure = GameObject.FindWithTag("Lure") ?? GameObject.FindGameObjectWithTag("OccupiedLure");
        if (lure != null)
        {
            // Get reference to the hooked fish before modifying lure
            LureStateController lureState = lure.GetComponent<LureStateController>();
            GameObject hookedFish = lureState?.HookedFish;

            if (hookedFish != null)
            {
                // Clean up connections between lure and fish
                CleanUpLureAndFish(lure, hookedFish);
                
                // Make the fish swim away naturally instead of destroying it
                FishAI fishAI = hookedFish.GetComponent<FishAI>();
                if (fishAI != null)
                {
                    // Re-enable fish AI to allow natural swimming
                    fishAI.enabled = true;
                    
                    // Get reference to fish rigidbody to apply escape movement
                    Rigidbody2D fishRb = hookedFish.GetComponent<Rigidbody2D>();
                    if (fishRb != null)
                    {
                        // Find player to determine escape direction
                        GameObject player = GameObject.FindGameObjectWithTag("Player");
                        if (player != null)
                        {
                            // Calculate direction away from player
                            Vector2 escapeDirection = ((Vector2)hookedFish.transform.position - (Vector2)player.transform.position).normalized;
                            
                            // Apply strong escape impulse in that direction and slightly downward
                            escapeDirection.y -= 0.3f; // Add downward component for natural escape
                            escapeDirection = escapeDirection.normalized;
                            
                            // Apply the escape impulse - stronger for bigger fish
                            float escapeForce = 8f + (fishAI.transform.localScale.x * 4f);
                            fishRb.AddForce(escapeDirection * escapeForce, ForceMode2D.Impulse);
                            
                            // Add slight random rotation to make escape look more natural
                            fishRb.AddTorque(UnityEngine.Random.Range(-0.5f, 0.5f), ForceMode2D.Impulse);
                        }
                    }

                    lureBreak.Play();
                    // Destroy the fish after a few seconds (when it's off-screen)
                    // This keeps it from permanently staying in the scene
                    Destroy(hookedFish, 5f);
                }
            }

            // Wait for fish escape animation before returning to passive state and cleaning up
            StartCoroutine(FishAndLureCleanup(lure, 2f));
            
            // Note: We don't destroy the lure immediately - we do it in the coroutine
            // to ensure the fish has time to swim away first
        }
    }

    IEnumerator SeeFishLeave(float time)
    {
        yield return new WaitForSeconds(time);

        // Return to passive state
        ChangeState(passiveState);
    }
    
    IEnumerator FishAndLureCleanup(GameObject lure, float time)
    {
        // First add a small broken line particle effect (for line snap)
        if (ps != null && lure != null)
        {
            // Scale down the particle effect for a more subtle line break
            GameObject particles = Instantiate(ps, lure.transform.position, Quaternion.identity).gameObject;
            ParticleSystem particleSystem = particles.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                // Make particles less dramatic - subtle line snap instead of explosion
                var main = particleSystem.main;
                main.startSpeed = main.startSpeed.constant * 0.5f;
                main.startSize = main.startSize.constant * 0.5f;
                main.maxParticles = main.maxParticles / 2;
            }
        }
        
        // Wait to let the fish swim away animation play
        yield return new WaitForSeconds(time);
        
        // Now destroy the lure object after the delay
        if (lure != null)
        {
            Destroy(lure);
        }
        
        // Return to passive state
        ChangeState(passiveState);
    }

    private void CleanUpLureAndFish(GameObject lure, GameObject fish)
    {
        // Reset lure state if it has a controller
        LureStateController lureState = lure.GetComponent<LureStateController>();
        if (lureState != null)
        {
            lureState.SetFree();
        }

        // Re-enable fish colliders
        Collider2D lureCollider = lureCollider = lure.GetComponent<Collider2D>();
        if (lureCollider != null)
        {
            lureCollider.enabled = true;
        }

        if (fish != null)
        {
            //Re-enable fish AI behavior
            FishAI fishAI = fish.GetComponent<FishAI>();
            if (fishAI != null)
            {
                fishAI.enabled = true;
            }

            //Remove joint connecting fish and lure 
            FixedJoint2D joint = fish.GetComponent<FixedJoint2D>();
            if (joint != null)
            {
                Destroy(joint);
            }
        }
    }

    public void InitializePanels()
    {
        inventoryPanel = GameObject.FindGameObjectWithTag("Inventory");
        inventoryPanel = inventoryPanel.transform.GetChild(0).gameObject;
        mapPanel = GameObject.FindGameObjectWithTag("Map");
        mapPanel = mapPanel.transform.GetChild(0).gameObject;
        settingsPanel = GameObject.FindGameObjectWithTag("Settings");
        settingsPanel = settingsPanel.transform.GetChild(0).gameObject;
    }

    private void HandleFishingKeyInput()
    {
        // Check for fishing rod hotkey (e.g., F key)
        if (Input.GetKeyDown(KeyCode.F))
        {
            // If player is already fishing, exit fishing state
            if (currentState == fishingState)
            {
                ChangeState(passiveState); // Assuming passiveState is available
            }
            // Otherwise enter fishing state
            else
            {
                ChangeState(fishingState);
            }
        }
    }

}
