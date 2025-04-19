using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PassiveState : StateInterface
{ 
    private readonly GameObject inventoryPanel;
    private readonly GameObject mapPanel;
    private readonly GameObject settingsPanel;
    private readonly GameObject directionIndicator;

    public KeyCode inventoryKey;
    public KeyCode mapKey;
    public KeyCode settingsKey;
    public KeyCode sceneswitchKey;

    public PassiveState(
        GameObject inventoryPanel, 
        GameObject mapPanel, 
        GameObject settingsPanel, 
        KeyCode inventoryKey,
        KeyCode mapKey, 
        KeyCode settingsKey,
        KeyCode sceneswitchKey,
        GameObject directionIndicator = null)
    {
        this.inventoryPanel = inventoryPanel;
        this.mapPanel = mapPanel;
        this.settingsPanel = settingsPanel;
        this.inventoryKey = inventoryKey;
        this.mapKey = mapKey;
        this.settingsKey = settingsKey;
        this.directionIndicator = directionIndicator;
    }

    public void Enter()
    {
        //Change camera to track player
        CameraController cameraController = Camera.main.GetComponent<CameraController>();
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        cameraController.target = playerObject.transform;

        if (directionIndicator != null)
        {
            directionIndicator.SetActive(true);
        }


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
        if (Input.GetKeyDown(sceneswitchKey)){
            SceneManager.LoadScene("Town Shop Change");
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
