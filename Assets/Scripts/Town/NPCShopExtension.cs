using UnityEngine;

public class NPCShopExtension : MonoBehaviour
{
    [Header("Shop Configuration")]
    public GameObject buyItemSlotPrefab; // Prefab for each buyable item slot
    public Transform buyItemsContainer; // Parent transform where item slots will be spawned
    public GameObject buyDetailsPanel; // Panel showing selected item details

    [Header("Buy UI Elements")]
    public TMPro.TextMeshProUGUI itemNameText;
    public TMPro.TextMeshProUGUI itemDescriptionText;
    public TMPro.TextMeshProUGUI itemPriceText;
    public UnityEngine.UI.Button buyButton;

    private BuyPanelManager buyPanelManager;
    private NPC npcComponent;

    void Start()
    {
        // Get the NPC component
        npcComponent = GetComponent<NPC>();
        if (npcComponent == null)
        {
            Debug.LogError("NPC component not found on this GameObject!");
            return;
        }

        // Only initialize if this is a merchant NPC
        if (npcComponent.type != "B") return;

        // Setup Buy Panel Manager
        if (npcComponent.buyUI != null)
        {
            // Find existing or add BuyPanelManager component
            buyPanelManager = npcComponent.buyUI.GetComponent<BuyPanelManager>();
            if (buyPanelManager == null)
            {
                buyPanelManager = npcComponent.buyUI.AddComponent<BuyPanelManager>();
            }

            // Configure BuyPanelManager with our UI elements
            ConfigureBuyPanelManager();
        }
    }

    private void ConfigureBuyPanelManager()
    {
        if (buyPanelManager == null) return;

        // Set references
        buyPanelManager.buyItemSlotPrefab = buyItemSlotPrefab;
        buyPanelManager.buyItemsContainer = buyItemsContainer;

        // Set UI elements
        buyPanelManager.selectedItemNameText = itemNameText;
        buyPanelManager.selectedItemDescriptionText = itemDescriptionText;
        buyPanelManager.selectedItemPriceText = itemPriceText;
        buyPanelManager.buyButton = buyButton;

        // Set gold text from MoneyPanel
        if (npcComponent.MoneyPanel != null)
        {
            MoneyUI moneyUI = npcComponent.MoneyPanel.GetComponent<MoneyUI>();
            if (moneyUI != null && moneyUI.moneyText != null)
            {
                buyPanelManager.goldText = (TMPro.TextMeshProUGUI)moneyUI.moneyText;
            }
        }

        // Set audio
        AudioSource[] audioSources = GetComponents<AudioSource>();
        if (audioSources.Length >= 2)
        {
            buyPanelManager.yesClip = audioSources[0];
            buyPanelManager.noClip = audioSources[1];
        }
    }

    // Add this to hook into NPC's existing update logic
    void LateUpdate()
    {
        // Check if buy UI just became active this frame
        if (npcComponent.buyUI != null && npcComponent.buyUI.activeSelf)
        {
            // Check if this happened in the current frame
            if (Time.frameCount % 3 == 0 && buyPanelManager != null)
            {
                buyPanelManager.PopulateBuyPanel();
            }
        }
    }
}