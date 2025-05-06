using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class TownPassiveState : StateInterface
{ 
    private  GameObject inventoryPanel;
    private  GameObject mapPanel;
    private  GameObject settingsPanel;

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

    public void Start() 
    {
        inventoryPanel = GameObject.FindGameObjectWithTag("Inventory");
        mapPanel = GameObject.FindGameObjectWithTag("Map");
        settingsPanel = GameObject.FindGameObjectWithTag("Settings");
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
            if (inventoryPanel == null)
            {
                inventoryPanel = GameObject.FindGameObjectWithTag("Inventory");
                inventoryPanel.transform.GetChild(0).gameObject.SetActive(true);
                inventoryPanel = inventoryPanel.transform.GetChild(0).gameObject;
            }
            TogglePanel(inventoryPanel);
        }
        if (Input.GetKeyDown(mapKey))
        {
            if (mapPanel == null)
            {
                mapPanel = GameObject.FindGameObjectWithTag("Map");
                mapPanel.transform.GetChild(0).gameObject.SetActive(true);
                mapPanel = mapPanel.transform.GetChild(0).gameObject;
            }
            TogglePanel(mapPanel);
        }    
        if (Input.GetKeyDown(settingsKey))
        {
            if (settingsPanel == null) 
            {
                settingsPanel = GameObject.FindGameObjectWithTag("Settings");
                settingsPanel.transform.GetChild(0).gameObject.SetActive(true);
                settingsPanel = settingsPanel.transform.GetChild(0).gameObject;
            }
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
