using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class StateController : MonoBehaviour
{
    public ParticleSystem ps;

    public static event Action<GameObject> OnFishCaught;
    public static event Action OnFishEscaped;

    private GearGenerator gearGenerator;
    private Inventory inventory;
    private StateInterface currentState;

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
        if (powerMinigameObject == null)
        {
            Debug.LogError("Power minigame GameObject reference is not set");
            return;
        }

        IPowerMinigame powerMinigame = powerMinigameObject.GetComponent<CastingMinigame>();
        if (powerMinigame == null)
        {
            Debug.LogError("Power minigame component not found on the powerMinigameObject");
            return;
        }

        passiveState = new PassiveState(inventoryPanel,mapPanel,settingsPanel,inventoryKey,mapKey,settingsKey, directionIndicator.gameObject);
        castState = new CastState(castSpeed, maxCastSpeed, lurePrefab, castOrigin, directionIndicator, powerMinigame);
        inAirState = new InAirState(waterLevel);
        inWaterState = new InWaterState();
        hookedState = new HookedState(tensionBarGameObject);

        ChangeState(passiveState);
       
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
            ChangeState(inAirState);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            FishEscaped();
            ChangeState(passiveState);
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
        


        Destroy(fish);

        // Return to passive state
        ChangeState(passiveState);
    }

    public void FishEscaped()
    {
        Debug.Log("Fish escaped");

        OnFishEscaped?.Invoke();

        // Find and destoy only the lure 
        GameObject lure = GameObject.FindWithTag("Lure") ?? GameObject.FindGameObjectWithTag("OccupiedLure");
        if (lure != null)
        {
            //Get reference to the hooked fish before destoying lure
            LureStateController lureState = lure.GetComponent<LureStateController>();
            GameObject hookedFish = lureState?.HookedFish;

            if (hookedFish != null)
            {
                CleanUpLureAndFish(lure, hookedFish);
            }

            StartCoroutine(SeeFishLeave(2f));


        }

        //PARTICLE EFFECT
        Instantiate(ps, lure.transform.position, Quaternion.identity);

        //Destroy Lure object
        Destroy(lure);
    }

    IEnumerator SeeFishLeave(float time)
    {
        yield return new WaitForSeconds(time);

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

}
