using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class BuyItemSlot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemImage;
    [SerializeField] private Image backgroundImage;
    [SerializeField] private TextMeshProUGUI priceText;

    private SerializableEquipmentItem item;
    private BuyPanelManager buyPanelManager;
    private Color defaultBackgroundColor;
    private Color selectedColor = new Color(1f, 0.92f, 0.016f, 1f); // Bright yellow for selected items

    public SerializableEquipmentItem Item => item;

    public void Initialize(SerializableEquipmentItem newItem, BuyPanelManager manager)
    {
        item = newItem;
        buyPanelManager = manager;

        // If not assigned in inspector, try to find components
        if (itemImage == null)
            itemImage = transform.Find("ItemImage")?.GetComponent<Image>();

        if (backgroundImage == null)
            backgroundImage = GetComponent<Image>();

        if (priceText == null)
            priceText = transform.Find("PriceText")?.GetComponent<TextMeshProUGUI>();

        // Save original background color
        if (backgroundImage != null)
            defaultBackgroundColor = backgroundImage.color;

        // Set up the item icon
        if (itemImage != null && item.icon != null)
        {
            itemImage.sprite = item.icon;
            itemImage.preserveAspect = true;
        }

        // Set price text if available
        if (priceText != null)
        {
            priceText.text = item.value.ToString();
        }

        // Set initial selected state
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        if (backgroundImage != null)
        {
            // Change background color based on selection
            backgroundImage.color = selected ? selectedColor : defaultBackgroundColor;
        }

        // Optional: add scaling or outline effect when selected
        transform.localScale = selected ? new Vector3(1.1f, 1.1f, 1f) : Vector3.one;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // When clicked, notify the manager that this item was selected
        if (buyPanelManager != null)
        {
            buyPanelManager.SelectBuyItem(this);
        }
    }
}