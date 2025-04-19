using UnityEngine;

public class NPCVendor : MonoBehaviour, IInteractable
{
    [Header("Vendor Configuration")]
    public VendorData vendorData; // Assign in inspector
    public float interactionRadius = 2f;

    [Header("Visual Feedback")]
    public GameObject interactionPrompt;
    public ParticleSystem purchaseEffect;

    private void Update()
    {
        // Show/hide interaction prompt based on player distance
        bool playerInRange = Vector3.Distance(transform.position, Player.Instance.transform.position) <= interactionRadius;
        interactionPrompt.SetActive(playerInRange);
    }

    public void OnInteract()
    {
        VendorSystem.Instance.OpenVendorShop(vendorData);
    }

    // Visual feedback when player buys something
    public void PlayPurchaseEffect()
    {
        if (purchaseEffect != null)
            purchaseEffect.Play();
    }
}