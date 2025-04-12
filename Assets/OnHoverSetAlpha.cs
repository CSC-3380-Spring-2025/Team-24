using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OnHoverSetAlpha : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image image; // Assign the UI Image in the Inspector

    public void SetOpacity(float alpha)
    {
        Color newColor = image.color;
        newColor.a = alpha;
        image.color = newColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetOpacity(1.0f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        SetOpacity(0f); 
            

        
    }
}
