using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuyPanelManager : MonoBehaviour
{
    [Header("References")]
    public Transform buyItemsContainer; // Container for buy item slots
    public GameObject buyItemSlotPrefab; // Prefab for a buy item slot
    public TextMeshProUGUI goldText;
    public TextMeshProUGUI selectedItemNameText;
    public TextMeshProUGUI selectedItemDescriptionText;
    public TextMeshProUGUI selectedItemPriceText;
    public Button buyButton;
    public AudioSource yesClip;
    public AudioSource noClip;

    [Header("Settings")]
    public int minItemsToGenerate = 3;
    public int maxItemsToGenerate = 6;
    public float itemPriceMultiplier = 1.5f; // Makes items more expensive than their base value

    // Private references
    private GearGenerator gearGenerator;
    private InventoryManager inventoryManager;
    private List<BuyItemSlot> buyItemSlots = new List<BuyItemSlot>();
    private BuyItemSlot selectedSlot;

    private void Awake()
    {
        
        if (gearGenerator == null)
        {
            Debug.LogError("GearGenerator not found in scene!");
        }

        
        if (inventoryManager == null)
        {
            Debug.LogError("InventoryManager not found in scene!");
        }

        // Configure buy button
        if (buyButton != null)
        {
            buyButton.onClick.AddListener(BuySelectedItem);
            buyButton.interactable = false;
        }
    }

    private void OnEnable()
    {
        // Populate shop when panel becomes active
        PopulateBuyPanel();
    }

    private void ClearBuyPanel()
    {
        // Clear selection
        selectedSlot = null;
        if (buyButton != null) buyButton.interactable = false;

        // Clear description texts
        if (selectedItemNameText != null) selectedItemNameText.text = "";
        if (selectedItemDescriptionText != null) selectedItemDescriptionText.text = "";
        if (selectedItemPriceText != null) selectedItemPriceText.text = "";

        // Destroy all existing item slots
        if (buyItemsContainer != null)
        {
            foreach (Transform child in buyItemsContainer)
            {
                Destroy(child.gameObject);
            }
        }

        buyItemSlots.Clear();
    }

    public void PopulateBuyPanel()
    {
        ClearBuyPanel();

        if (gearGenerator == null || buyItemsContainer == null || buyItemSlotPrefab == null)
        {
            Debug.LogError("Missing required components for buy panel!");
            return;
        }

        // Generate a random number of items
        int itemCount = Random.Range(minItemsToGenerate, maxItemsToGenerate + 1);

        for (int i = 0; i < itemCount; i++)
        {
            // Randomly pick an equipment type
            EquipmentType type = (EquipmentType)Random.Range(0, System.Enum.GetValues(typeof(EquipmentType)).Length);

            // Random item level between 1-10
            int itemLevel = Random.Range(1, 11);

            // Weighted rarity (more common items should appear more frequently)
            float rarityRoll = Random.value;
            Rarity rarity;

            if (rarityRoll < 0.6f) rarity = Rarity.Common;
            else if (rarityRoll < 0.85f) rarity = Rarity.Uncommon;
            else if (rarityRoll < 0.95f) rarity = Rarity.Rare;
            else if (rarityRoll < 0.99f) rarity = Rarity.Epic;
            else rarity = Rarity.Legendary;

            // Generate a serializable equipment item
            SerializableEquipmentItem item = gearGenerator.GetSerializableEquipment(type, itemLevel, rarity);

            // Set a price based on item quality and rarity
            float basePrice = 50 * (itemLevel) * ((int)rarity + 1) * itemPriceMultiplier;
            item.value = Mathf.Round(basePrice);

            // Create a buy item slot
            CreateBuyItemSlot(item);
        }

        // Update gold display
        UpdateGoldDisplay();
    }

    private void CreateBuyItemSlot(SerializableEquipmentItem item)
    {
        // Instantiate the buy item slot prefab
        GameObject slotObject = Instantiate(buyItemSlotPrefab, buyItemsContainer);

        // Get or add the BuyItemSlot component
        BuyItemSlot buyItemSlot = slotObject.GetComponent<BuyItemSlot>();
        if (buyItemSlot == null)
        {
            buyItemSlot = slotObject.AddComponent<BuyItemSlot>();
        }

        // Initialize the buy item slot
        buyItemSlot.Initialize(item, this);

        // Add to our list
        buyItemSlots.Add(buyItemSlot);
    }

    public void SelectBuyItem(BuyItemSlot slot)
    {
        // Clear previous selection
        if (selectedSlot != null)
        {
            selectedSlot.SetSelected(false);
        }

        // Set new selection
        selectedSlot = slot;
        slot.SetSelected(true);

        // Update UI with item details
        if (selectedItemNameText != null) selectedItemNameText.text = slot.Item.itemName;
        if (selectedItemDescriptionText != null) selectedItemDescriptionText.text = slot.Item.description;
        if (selectedItemPriceText != null) selectedItemPriceText.text = slot.Item.value.ToString() + " gold";

        // Enable buy button if player has enough gold
        if (buyButton != null)
        {
            buyButton.interactable = inventoryManager.gold >= slot.Item.value;

            // Update button color based on affordability
            ColorBlock colors = buyButton.colors;
            if (buyButton.interactable)
            {
                colors.normalColor = new Color(0.2f, 0.8f, 0.2f); // Green if affordable
            }
            else
            {
                colors.normalColor = new Color(0.8f, 0.2f, 0.2f); // Red if not affordable
            }
            buyButton.colors = colors;
        }
    }

    public void BuySelectedItem()
    {
        if (selectedSlot == null) return;

        SerializableEquipmentItem itemToBuy = selectedSlot.Item;

        // Check if player has enough gold
        if (inventoryManager.gold < itemToBuy.value)
        {
            if (noClip != null) noClip.Play();
            Debug.Log("Not enough gold to buy this item.");
            return;
        }

        // Deduct gold
        inventoryManager.gold -= (float)itemToBuy.value;

        // Add item to inventory
        inventoryManager.AddItem(itemToBuy);

        // Play success sound
        if (yesClip != null) yesClip.Play();

        // Update gold display
        UpdateGoldDisplay();

        // Remove the item from the shop
        buyItemSlots.Remove(selectedSlot);
        Destroy(selectedSlot.gameObject);

        // Clear selection
        selectedSlot = null;
        if (buyButton != null) buyButton.interactable = false;
        if (selectedItemNameText != null) selectedItemNameText.text = "";
        if (selectedItemDescriptionText != null) selectedItemDescriptionText.text = "";
        if (selectedItemPriceText != null) selectedItemPriceText.text = "";
    }

    private void UpdateGoldDisplay()
    {
        if (goldText != null)
        {
            goldText.text = inventoryManager.gold.ToString() + " gold";
        }
    }
}