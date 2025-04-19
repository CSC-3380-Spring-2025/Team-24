using UnityEngine;

public class VendorSystem : MonoBehaviour
{
    public static VendorSystem Instance;

    [Header("UI Reference")]
    public VendorUI vendorUI;

    [Header("Audio")]
    public AudioClip purchaseSound;
    public AudioClip errorSound;

    private VendorData currentVendor;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OpenVendorShop(VendorData vendor)
    {
        currentVendor = vendor;
        vendorUI.ShowVendorItems(vendor.GetAvailableItems());
        Player.Instance.SetControl(false); // Disable player movement
    }

    public void CloseVendorShop()
    {
        vendorUI.Hide();
        Player.Instance.SetControl(true); // Re-enable player movement
    }

    public bool TryPurchaseItem(EquipmentItem item)
    {
        if (!currentVendor.GetAvailableItems().Contains(item))
        {
            PlaySound(errorSound);
            Debug.Log("Item not available");
            return false;
        }

        if (InventoryManager.Instance.playerGold < item.GetVendorPrice())
        {
            PlaySound(errorSound);
            Debug.Log("Not enough gold");
            return false;
        }

        // Successful purchase
        InventoryManager.Instance.playerGold -= item.GetVendorPrice();
        InventoryManager.Instance.AddItem(Instantiate(item));
        PlaySound(purchaseSound);

        // Notify NPC to play effects
        FindCurrentNPCVendor()?.PlayPurchaseEffect();

        // Update UI
        vendorUI.UpdatePlayerGold();

        return true;
    }

    private NPCVendor FindCurrentNPCVendor()
    {
        // Find the NPC vendor in the scene that matches our current vendor data
        foreach (var vendor in FindObjectsOfType<NPCVendor>())
        {
            if (vendor.vendorData == currentVendor)
                return vendor;
        }
        return null;
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }
}