using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TownPassiveState : StateInterface
{ 
    private readonly GameObject inventoryPanel;
    private readonly GameObject mapPanel;
    private readonly GameObject settingsPanel;

    public KeyCode inventoryKey;
    public KeyCode mapKey;
    public KeyCode settingsKey;

    public TownPassiveState(
        GameObject inventoryPanel, 
        GameObject mapPanel, 
        GameObject settingsPanel, 
        KeyCode inventoryKey,
        KeyCode mapKey, 
        KeyCode settingsKey
        )
    {
        this.inventoryPanel = inventoryPanel;
        this.mapPanel = mapPanel;
        this.settingsPanel = settingsPanel;
        this.inventoryKey = inventoryKey;
        this.mapKey = mapKey;
        this.settingsKey = settingsKey;
    }

    public void Enter()
    {
        //Change camera to track player
        //CameraController cameraController = Camera.main.GetComponent<CameraController>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        //cameraController.target = playerObject.transform
    }
    public void Update()
    {
        if (Input.GetKeyDown(inventoryKey))
        {
            TogglePanel(inventoryPanel);
        }
        if (Input.GetKeyDown(mapKey))
        {
            TogglePanel(mapPanel);
        }    
        if (Input.GetKeyDown(settingsKey))
        {
            TogglePanel(settingsPanel);
        }
    }

    public void Exit()
    {
    }

    void TogglePanel(GameObject panel)
    {
       //Simple toggle, activeSelf returns current state
       //Then SetActive will set it to negation of the return
       panel.SetActive(!panel.activeSelf);
    }
}
