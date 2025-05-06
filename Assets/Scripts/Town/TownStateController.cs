using System;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class TownStateController : MonoBehaviour
{
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
    public GameObject buyPanel;
    public KeyCode BuyKey = KeyCode.B;
    public GameObject sellPanel;
    public KeyCode SellKey = KeyCode.R;

    public GameObject Player;
    

    private TownPassiveState passiveState;
    private TradeState tradeState;

    private void Start()
    {
        passiveState = new TownPassiveState(inventoryPanel,mapPanel,settingsPanel,inventoryKey,mapKey,settingsKey);
        ChangeState(passiveState);

        inventoryPanel = GameObject.FindGameObjectWithTag("Inventory");
        mapPanel = GameObject.FindGameObjectWithTag("Map");
        settingsPanel = GameObject.FindGameObjectWithTag("Settings");
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
        
    }

    private void HandleKeyInputs()
    {
     
    }
    
}
