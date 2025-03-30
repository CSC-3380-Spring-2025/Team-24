using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RawImage))]
public class RegionHoverController : MonoBehaviour, IPointerMoveHandler, IPointerExitHandler
{
    [Header("Textures")]
    public Texture2D colorMapTexture;
    public Texture2D grayscaleMaskTexture;

    [Header("Region Settings")]
    [Tooltip("List of valid grayscale values for regions (e.g., 0.3=forest, 0.6=water)")]
    public float[] validRegionValues = { 0.1f, 0.3f, 0.5f, 0.7f, 0.9f };
    public Color highlightColor = new Color(1, 1, 1, 0.5f);

    private Material regionMaterial;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        var image = GetComponent<RawImage>();
        image.texture = colorMapTexture;

        regionMaterial = new Material(Shader.Find("Custom/RegionHover"));
        image.material = regionMaterial;

        regionMaterial.SetTexture("_MainTex", colorMapTexture);
        regionMaterial.SetTexture("_MaskTex", grayscaleMaskTexture);
        regionMaterial.SetColor("_HighlightColor", highlightColor);
        regionMaterial.SetFloat("_HoverRegion", -1);
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint)) return;

        Vector2 uv = new Vector2(
            Mathf.Clamp01((localPoint.x + rectTransform.rect.width * 0.5f) / rectTransform.rect.width),
            Mathf.Clamp01((localPoint.y + rectTransform.rect.height * 0.5f) / rectTransform.rect.height)
        );

        float rawValue = grayscaleMaskTexture.GetPixelBilinear(uv.x, uv.y).r;
        float nearestValidRegion = GetNearestValidRegion(rawValue);

        Debug.Log($"UV: {uv} | Raw: {rawValue:F3} | " +
                $"Nearest Region: {nearestValidRegion} | " +
                $"Should Highlight: {nearestValidRegion >= 0}");

        regionMaterial.SetFloat("_HoverRegion", nearestValidRegion);
    }

    private float GetNearestValidRegion(float sampledValue)
    {
        float closestValue = -1f;
        float smallestDifference = float.MaxValue;

        foreach (float regionValue in validRegionValues)
        {
            float difference = Mathf.Abs(sampledValue - regionValue);
            if (difference < 0.05f && difference < smallestDifference) // 0.05 tolerance
            {
                smallestDifference = difference;
                closestValue = regionValue;
            }
        }
        return closestValue;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        regionMaterial.SetFloat("_HoverRegion", -1);
    }

    void OnDestroy()
    {
        if (regionMaterial != null) Destroy(regionMaterial);
    }
}