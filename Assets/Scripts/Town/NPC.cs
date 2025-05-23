using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class NPC : MonoBehaviour
{
    public string npcName = "NPC";
    [TextArea(3, 10)]
    public string dialogueText = "Hello, traveler!";
    public bool isPlayerInRange = false;
    public GameObject dialogueUI;
    public TMP_Text dialogueTextComponent;
    public GameObject sellUI;
    public GameObject MoneyPanel;
    public GameObject buyUI;
    public GameObject inventoryUI;
    public string type = "B"; 
    
    [SerializeField] GameObject Player;
    [SerializeField] GameObject CharacterNP;

    private Rigidbody2D RB;
    private Rigidbody2D RBN;

    void Start()
    {
        inventoryUI = GameObject.FindGameObjectWithTag("Inventory");
        inventoryUI = inventoryUI.transform.GetChild(0).gameObject;
        //sellUI = GameObject.FindGameObjectWithTag("SellPanel");
        if (sellUI == null)
        {
            Debug.LogError("Sell UI not found!");
        }
        //sellUI = sellUI.transform.GetChild(0).gameObject;
        //sellUI.SetActive(false);
        if (inventoryUI == null)
        {
            Debug.LogError("Inventory UI not found!");
        }

        RB = Player.GetComponent<Rigidbody2D>();
        RBN = CharacterNP.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (inventoryUI == null) 
        {
            inventoryUI = GameObject.FindGameObjectWithTag("InventoryPanel");
        }
        
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E))
        {
            if (dialogueUI.activeSelf)
            {
                dialogueUI.SetActive(false);
            }
            else
            {
                dialogueTextComponent.text = npcName + ": " + dialogueText;
                dialogueUI.SetActive(true);
            }
        }
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.B) && type=="B")
        {
            if (buyUI.activeSelf)
            {
                buyUI.SetActive(false);
                MoneyPanel.SetActive(false);
                inventoryUI.SetActive(false);
            }
            else
            {
                buyUI.SetActive(true);
                MoneyPanel.SetActive(true);
                inventoryUI.SetActive(true);

            }
        }
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.R) && type=="S")
        {
            if (sellUI.activeSelf)
            {
                sellUI.SetActive(false);
                MoneyPanel.SetActive(false);
                inventoryUI.SetActive(false);
            }
            else
            {
                sellUI.SetActive(true);
                MoneyPanel.SetActive(true);
                inventoryUI.SetActive(true);

            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            dialogueUI.SetActive(false);
            sellUI.SetActive(false);
            buyUI.SetActive(false);
            inventoryUI.SetActive(false);
            MoneyPanel.SetActive(false);
        }
    }
}